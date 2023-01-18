using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;
using Warhead = Exiled.Events.Handlers.Warhead;
using MEC;

namespace FacilityHorror
{
    public class Main : Plugin<Config>
    {

        public override string Name { get; } = "FacilityHorror";
        public override string Author { get; } = "ThijsNameIsTaken";
        public override Version Version { get; } = new Version(2, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);

        private static readonly Main Singleton = new();
        
        internal bool eventActive;
        private bool warheadActive;
        

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
            Warhead.Starting += OnWarheadStarting;
            Warhead.Stopping += OnWarheadStopping;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.RestartingRound -= OnRestartingRound;
            Server.RoundStarted -= OnStartingRound;
            Player.TriggeringTesla -= OnTriggeringTesla;
            Warhead.Starting -= OnWarheadStarting;
            Warhead.Stopping -= OnWarheadStopping;

            base.OnDisabled();
        }

        private void OnWarheadStarting(StartingEventArgs ev)
        {
            warheadActive = true;
        }

        private void OnWarheadStopping(StoppingEventArgs ev)
        {
            warheadActive = false;
        }

        internal CoroutineHandle EventCoroutine;

        private void OnRestartingRound()
        {
            if (EventCoroutine.IsRunning) Timing.KillCoroutines(EventCoroutine);
        }

        private void OnStartingRound()
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < Config.ActivationChance;
            Log.Debug($"Event active this round: {activeTrigger}");
            if (activeTrigger == true) EventCoroutine = Timing.RunCoroutine(Lights());
        }

        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (Config.KeepTeslaEnabled || eventActive == false)
            {
                ev.IsAllowed = true;
                return;
            }
            ev.IsInIdleRange = false;
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

                switch (warheadActive)
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

                        if (Config.CassieWaitForToggle)
                        {
                            while (Cassie.IsSpeaking)
                            {
                                
                            }
                            yield return Timing.WaitForSeconds(waitTime);
                        }
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
