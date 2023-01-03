using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using Server = Exiled.Events.Handlers.Server;
using MEC;

namespace LightsOut
{
    public class Main : Plugin<Config>
    {

        public override string Name { get; } = "LightsOut";
        public override string Author { get; } = "ThijsNameIsTaken";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);

        private static readonly Main Singleton = new();

        private Main()
        {
        }

        public static Main Instance => Singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Last;

        public override void OnEnabled()
        {
            Server.RestartingRound += OnRestartingRound;
            Server.RoundStarted += OnStartingRound;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.RestartingRound -= OnRestartingRound;
            Server.RoundStarted -= OnStartingRound;

            base.OnDisabled();
        }

        private CoroutineHandle _coroutine;

        private void OnRestartingRound()
        {
            if (_coroutine.IsRunning)
                Timing.KillCoroutines(_coroutine);
        }

        private void OnStartingRound()
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < Config.ActivationChance;
            Log.Debug($"LightsOut event active this round: {activeTrigger}");
            if (activeTrigger == true) _coroutine = Timing.RunCoroutine(Lights());
        }

        private IEnumerator<float> Lights()
        {
            yield return Timing.WaitForSeconds(Config.MinStartOffset);
            while (true)
            {
                int RandomInterval = UnityEngine.Random.Range(Config.MinRandomInterval, Config.MaxRandomInterval);
                Log.Debug($"Next event: {RandomInterval} seconds");
                yield return Timing.WaitForSeconds(RandomInterval);

                int ActiveTime = UnityEngine.Random.Range(Config.MinRandomBlackoutTime, Config.MaxRandomBlackoutTime);
                float WaitTime = Cassie.CalculateDuration(Config.CassieMessage, true);

                switch (Warhead.IsInProgress)
                {
                    case true:
                        Log.Debug($"Event skipped due to warhead sequence");
                        break;
                    case false:
                        if (Config.CassieDisplaySubtitles == true)
                        {
                            Cassie.MessageTranslated(Config.CassieMessage, Config.CassieSubtitles, false, Config.CassieSoundAlarm, true);
                        }
                        else
                        {
                            Cassie.Message(Config.CassieMessage, false, Config.CassieSoundAlarm, true);
                        }
                        if (Config.CassieWaitForToggle == true) yield return Timing.WaitForSeconds(WaitTime);
                        Map.TurnOffAllLights(ActiveTime);
                        Log.Debug($"Event active for {ActiveTime} seconds");
                        yield return Timing.WaitForSeconds(ActiveTime);
                        break;
                }
            }
        }
    }
}
