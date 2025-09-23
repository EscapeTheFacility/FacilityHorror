using System;
using System.Collections.Generic;
using System.Linq;
using FacilityHorror_LabAPI.ConfigObjects;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using MEC;

namespace FacilityHorror_LabAPI
{
    public class FacilityHorror : Plugin<Config>
    {
        public override string Name => "FacilityHorror";
        public override string Description =>
            "Event plugin for LabAPI that introduces randomized world events to SCP:SL";
        public override string Author => "ThijsNameIsTaken";
        public override Version Version => new Version(3, 0, 0);
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
        
        public static FacilityHorror Instance { get; private set; }

        internal bool EventActive;
        internal bool WarheadActive;

        internal CoroutineHandle EventCoroutine;
        internal ConfigEventObject CurrentEvent;
        
        public override void Enable()
        {
            if (!Properties.IsEnabled) return;
            Instance = this;
            EventHandlers.Register();
        }

        public override void Disable()
        {
            throw new NotImplementedException();
        }

        internal IEnumerator<float> EventTimer()
        {
            List<string> configList = new List<string>(Config.EventList.Keys);
            yield return Timing.WaitForSeconds(Config.MinStartOffset);
            while (true)
            {
                string randomEvent = configList[UnityEngine.Random.Range(0, configList.Count)];
                CurrentEvent = Config.EventList[randomEvent];
                int randomInterval = UnityEngine.Random.Range(Config.MinRandomInterval, Config.MaxRandomInterval);
                Logger.Debug($"Next event will start in {randomInterval} seconds. Event type: {randomEvent}", Config.Debug);
                yield return Timing.WaitForSeconds(randomInterval);

                int activeTime = UnityEngine.Random.Range(CurrentEvent.MinRandomEventDuration,
                    CurrentEvent.MaxRandomEventDuration);
                float waitTime = Cassie.CalculateDuration(CurrentEvent.CassieText, true);

                if (!CurrentEvent.RunWhenWarhead && WarheadActive) continue;

                switch (WarheadActive)
                {
                    case true:
                        Logger.Debug($"Event skipped due to warhead sequence", Config.Debug);
                        break;
                    case false:
                        if (CurrentEvent.CassieTextEnabled)
                        {
                            if (CurrentEvent.CassieShowSubtitles)
                            {
                                Cassie.Message(CurrentEvent.CassieText, isHeld: false,
                                    isNoisy: CurrentEvent.CassieRunJingle, isSubtitles: true);
                            }
                            else
                            {
                                Cassie.Message(CurrentEvent.CassieText, isHeld: false,
                                    isNoisy: CurrentEvent.CassieRunJingle, isSubtitles: false);
                            }

                            if (CurrentEvent.CassieWaitForMessage)
                            {
                                while (Cassie.IsSpeaking)
                                {

                                }
                                yield return Timing.WaitForSeconds(waitTime);
                            }
                        }

                        EventActive = true;
                        if (CurrentEvent.LightsOut) Map.TurnOffLights(activeTime);
                        if (CurrentEvent.DoorsLocked)
                        {
                            foreach (var door in DoorVariant.AllDoors.Where(door => !CheckDisallowedDoor(door)))
                            {
                                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                            }
                        }
                        Logger.Debug($"Event active for {activeTime} seconds", Config.Debug);
                        yield return Timing.WaitForSeconds(activeTime);
                        if (CurrentEvent.DoorsLocked)
                        {
                            foreach (var door in DoorVariant.AllDoors.Where(door => !CheckDisallowedDoor(door)))
                            {
                                door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, false);
                            }
                        }
                        EventActive = false;
                        break;
                }
            }
        }
        
        private bool CheckDisallowedDoor(DoorVariant door)
        {
            var nameTag = door.DoorName;
            if (nameTag == null) return false;
            var bracketStart = nameTag.IndexOf('(') - 1;
            if (bracketStart > 0)
                nameTag = nameTag.Remove(bracketStart, nameTag.Length - bracketStart);
            return nameTag is "SURFACE_NUKE" or "079_FIRST" or "079_SECOND";
        }
        
    }
}