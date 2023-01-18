using System;
using System.Collections.Generic;

using MEC;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;

namespace FacilityHorror
{
    public class Main
    {

        [PluginConfig] public Config Config;

        [PluginEntryPoint("FacilityHorror", "2.0.0", "Random scary facility events", "ThijsNameIsTaken")]
        void LoadPlugin()
        {
            Instance = this;
            PluginAPI.Events.EventManager.RegisterEvents(this);
        }
        
        internal bool eventActive;
        private bool warheadActive;
        
        public Main Instance { get; set; }

        [PluginEvent(ServerEventType.WarheadStart)]
        void OnWarheadStarting(bool isAutomatic, IPlayer player, bool isResumed)
        {
            warheadActive = true;
        }

        void OnWarheadStopping(IPlayer player)
        {
            warheadActive = false;
        }

        internal CoroutineHandle LightsCoroutine;

        [PluginEvent(ServerEventType.RoundRestart)]
        private void OnRestartingRound()
        {
            if (LightsCoroutine.IsRunning) Timing.KillCoroutines(LightsCoroutine);
        }

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnStartingRound()
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < Config.ActivationChance;
            Log.Debug($"LightsOut event active this round: {activeTrigger}", Config.Debug);
            if (activeTrigger == true) LightsCoroutine = Timing.RunCoroutine(Lights());
        }

        /*
        [PluginEvent(ServerEventType.player)]
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
        */
        

        private IEnumerator<float> Lights()
        {
            yield return Timing.WaitForSeconds(Config.MinStartOffset);
            while (true)
            {
                int randomInterval = UnityEngine.Random.Range(Config.MinRandomInterval, Config.MaxRandomInterval);
                Log.Debug($"Next event will start in {randomInterval} seconds", Config.Debug);
                yield return Timing.WaitForSeconds(randomInterval);

                int activeTime = UnityEngine.Random.Range(Config.MinRandomBlackoutTime, Config.MaxRandomBlackoutTime);
                float waitTime = Cassie.CalculateDuration(Config.CassieMessage, true);

                switch (warheadActive)
                {
                    case true:
                        Log.Debug($"Event skipped due to warhead sequence", Config.Debug);
                        break;
                    case false:
                        Cassie.Message(Config.CassieMessage, false, Config.CassieSoundAlarm, true);
                        if (Config.CassieWaitForToggle)
                        {
                            while (Cassie.IsSpeaking)
                            {
                                
                            }
                            yield return Timing.WaitForSeconds(waitTime);
                        }
                        eventActive = true;
                        Facility.TurnOffAllLights(activeTime);
                        Log.Debug($"Event active for {activeTime} seconds", Config.Debug);
                        yield return Timing.WaitForSeconds(activeTime);
                        eventActive = false;
                        break;
                }
            }
        }
    }
}
