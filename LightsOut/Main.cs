using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using MEC;

namespace LightsOut
{
    public class Main : Plugin<Config>
    {

        public override string Name { get; } = "LightsOut";
        public override string Author { get; } = "ThijsNameIsTaken";
        public override Version Version { get; } = new Version(1, 0, 1);
        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);

        private static readonly Main Singleton = new();
        
        public bool eventActive = false;

        private Main()
        {
        }

        public static Main Instance => Singleton;

        public override PluginPriority Priority { get; } = PluginPriority.Last;

        public override void OnEnabled()
        {
            Server.RestartingRound += OnRestartingRound;
            Server.RoundStarted += OnStartingRound;
            Player.TriggeringTesla += OnTriggeringTesla;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.RestartingRound -= OnRestartingRound;
            Server.RoundStarted -= OnStartingRound;
            Player.TriggeringTesla -= OnTriggeringTesla;

            base.OnDisabled();
        }

        private CoroutineHandle _coroutine;

        private void OnRestartingRound()
        {
            if (_coroutine.IsRunning) Timing.KillCoroutines(_coroutine);
        }

        private void OnStartingRound()
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < Config.ActivationChance;
            Log.Debug($"LightsOut event active this round: {activeTrigger}");
            if (activeTrigger == true) _coroutine = Timing.RunCoroutine(Lights());
        }

        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (eventActive == false) return;
            if (Config.KeepTeslaEnabled == true) return;
            ev.IsAllowed = false;
        }

        private IEnumerator<float> Lights()
        {
            yield return Timing.WaitForSeconds(Config.MinStartOffset);
            while (true)
            {
                int randomInterval = UnityEngine.Random.Range(Config.MinRandomInterval, Config.MaxRandomInterval);
                Log.Debug($"Next event will start in {randomInterval} seconds");
                yield return Timing.WaitForSeconds(randomInterval);

                int activeTime = UnityEngine.Random.Range(Config.MinRandomBlackoutTime, Config.MaxRandomBlackoutTime);
                float waitTime = Cassie.CalculateDuration(Config.CassieMessage, true);

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
                        if (Config.CassieWaitForToggle == true) yield return Timing.WaitForSeconds(waitTime);
                        eventActive = true;
                        Map.TurnOffAllLights(activeTime);
                        Log.Debug($"Event active for {activeTime} seconds");
                        yield return Timing.WaitForSeconds(activeTime);
                        eventActive = false;
                        break;
                }
            }
        }
    }
}
