using System;
using System.Collections.Generic;
using System.Linq;
using FacilityHorror.ConfigObjects;
using Interactables.Interobjects.DoorUtils;
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
        
        private bool _eventActive;
        private bool _warheadActive;
        
        public Main Instance { get; set; }

        [PluginEvent(ServerEventType.WarheadStart)]
        void OnWarheadStarting(bool isAutomatic, IPlayer player, bool isResumed)
        {
            _warheadActive = true;
        }

        [PluginEvent(ServerEventType.WarheadStop)]
        void OnWarheadStopping(IPlayer player)
        {
            _warheadActive = false;
        }

        private CoroutineHandle _eventCoroutine;
        private ConfigEventObject _currentEvent;

        [PluginEvent(ServerEventType.RoundRestart)]
        private void OnRestartingRound()
        {
            if (_eventCoroutine.IsRunning) Timing.KillCoroutines(_eventCoroutine);
        }

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnStartingRound()
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < Config.ActivationChance;
            Log.Debug($"Event active this round: {activeTrigger}");
            if (activeTrigger) _eventCoroutine = Timing.RunCoroutine(EventTimer());
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
        
        private IEnumerator<float> EventTimer()
        {
            List<string> configList = new List<string>(Config.EventList.Keys);
            yield return Timing.WaitForSeconds(Config.MinStartOffset);
            while (true)
            {
                string randomEvent = configList[UnityEngine.Random.Range(0, configList.Count)];
                _currentEvent = Config.EventList[randomEvent];
                int randomInterval = UnityEngine.Random.Range(Config.MinRandomInterval, Config.MaxRandomInterval);
                Log.Debug($"Next event will start in {randomInterval} seconds");
                yield return Timing.WaitForSeconds(randomInterval);

                int activeTime = UnityEngine.Random.Range(_currentEvent.MinRandomEventDuration,
                    _currentEvent.MaxRandomEventDuration);
                float waitTime = Cassie.CalculateDuration(_currentEvent.CassieText, true);

                if (!_currentEvent.RunWhenWarhead && _warheadActive) continue;

                switch (_warheadActive)
                {
                    case true:
                        Log.Debug($"Event skipped due to warhead sequence");
                        break;
                    case false:
                        if (_currentEvent.CassieTextEnabled)
                        {
                            if (_currentEvent.CassieShowSubtitles)
                            {
                                Cassie.Message(_currentEvent.CassieText, isHeld: false,
                                    isNoisy: _currentEvent.CassieRunJingle, isSubtitles: true);
                            }
                            else
                            {
                                Cassie.Message(_currentEvent.CassieText, isHeld: false,
                                    isNoisy: _currentEvent.CassieRunJingle, isSubtitles: false);
                            }

                            if (_currentEvent.CassieWaitForMessage)
                            {
                                while (Cassie.IsSpeaking)
                                {

                                }
                                yield return Timing.WaitForSeconds(waitTime);
                            }
                        }

                        _eventActive = true;
                        if (_currentEvent.LightsOut) Facility.TurnOffAllLights(activeTime);
                        if (_currentEvent.DoorsLocked)
                        {
                            foreach (var door in DoorVariant.AllDoors.Where(door => !CheckDisallowedDoor(door)))
                            {
                                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                            }
                        }
                        Log.Debug($"Event active for {activeTime} seconds");
                        yield return Timing.WaitForSeconds(activeTime);
                        if (_currentEvent.DoorsLocked)
                        {
                            foreach (var door in DoorVariant.AllDoors.Where(door => !CheckDisallowedDoor(door)))
                            {
                                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                            }
                        }
                        _eventActive = false;
                        break;
                }
            }
        }

        private bool CheckDisallowedDoor(DoorVariant door)
        {
            var nameTag = door.TryGetComponent(out DoorNametagExtension name) ? name.GetName : null;
            if (nameTag == null) return false;
            var bracketStart = nameTag.IndexOf('(') - 1;
            if (bracketStart > 0)
                nameTag = nameTag.Remove(bracketStart, nameTag.Length - bracketStart);
            return nameTag is "SURFACE_NUKE" or "079_FIRST" or "079_SECOND";
        }
    }
}
