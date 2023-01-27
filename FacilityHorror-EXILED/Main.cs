using System;
using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using FacilityHorror.ConfigObjects;
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

        private bool _eventActive;
        private bool _warheadActive;

        public static Main Instance { get; set; }

        public override void OnEnabled()
        {
            Instance = this;

            Server.RestartingRound += OnRestartingRound;
            Server.RoundStarted += OnStartingRound;
            Player.TriggeringTesla += OnTriggeringTesla;
            Warhead.Starting += OnWarheadStarting;
            Warhead.Stopping += OnWarheadStopping;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            if (_eventCoroutine.IsRunning) Timing.KillCoroutines(_eventCoroutine);

            Server.RestartingRound -= OnRestartingRound;
            Server.RoundStarted -= OnStartingRound;
            Player.TriggeringTesla -= OnTriggeringTesla;
            Warhead.Starting -= OnWarheadStarting;
            Warhead.Stopping -= OnWarheadStopping;

            base.OnDisabled();
        }

        private void OnWarheadStarting(StartingEventArgs ev)
        {
            _warheadActive = true;
        }

        private void OnWarheadStopping(StoppingEventArgs ev)
        {
            _warheadActive = false;
        }

        private CoroutineHandle _eventCoroutine;
        private ConfigEventObject _currentEvent;

        private void OnRestartingRound()
        {
            if (_eventCoroutine.IsRunning) Timing.KillCoroutines(_eventCoroutine);
        }

        private void OnStartingRound()
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < Config.ActivationChance;
            Log.Debug($"Event active this round: {activeTrigger}");
            if (activeTrigger) _eventCoroutine = Timing.RunCoroutine(EventTimer());
        }

        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (_currentEvent.TeslaEnabled || _eventActive == false)
            {
                return;
            }

            ev.IsInIdleRange = false;
            ev.IsAllowed = false;
        }

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
                        if (_currentEvent.LightsOut) Map.TurnOffAllLights(activeTime);
                        if (_currentEvent.DoorsLocked)
                        {
                            foreach (var door in Door.List)
                            {
                                if (door.Type == DoorType.NukeSurface) continue;
                                if (door.Type == DoorType.Scp079First || door.Type == DoorType.Scp079Second) continue;
                                door.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                        }
                        Log.Debug($"Event active for {activeTime} seconds");
                        yield return Timing.WaitForSeconds(activeTime);
                        if (_currentEvent.DoorsLocked)
                        {
                            foreach (var door in Door.List)
                            {
                                if(door.Type == DoorType.NukeSurface) continue;
                                door.ChangeLock(DoorLockType.SpecialDoorFeature);
                            }
                        }
                        _eventActive = false;
                        break;
                }
            }
        }
    }
}
