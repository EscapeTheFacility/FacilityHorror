using System.Collections.Generic;
using System.Linq;
using GameCore;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using MEC;

namespace FacilityHorror_LabAPI
{
    internal static class EventHandlers
    {
        internal static void Register()
        {
            ServerEvents.RoundRestarted += OnRoundRestarted;
            ServerEvents.RoundStarting += OnRoundStarting;
            
            PlayerEvents.TriggeringTesla += OnTriggeringTesla;
            
            WarheadEvents.Starting += OnWarheadStarting;
            WarheadEvents.Stopping += OnWarheadStopping;
        }

        internal static void Unregister()
        {
            WarheadEvents.Starting -= OnWarheadStarting;
            WarheadEvents.Stopping -= OnWarheadStopping;
        }


        private static void OnRoundRestarted()
        {
            if (FacilityHorror.Instance.EventCoroutine.IsRunning)
                Timing.KillCoroutines(FacilityHorror.Instance.EventCoroutine);
        }

        private static void OnRoundStarting(RoundStartingEventArgs ev)
        {
            bool activeTrigger = UnityEngine.Random.Range(0, 100) < FacilityHorror.Instance.Config.ActivationChance;
            Logger.Debug($"Event active this round: {activeTrigger}", FacilityHorror.Instance.Config.Debug);
            if (activeTrigger) FacilityHorror.Instance.EventCoroutine = Timing.RunCoroutine(FacilityHorror.Instance.EventTimer());
        }

        private static void OnTriggeringTesla(PlayerTriggeringTeslaEventArgs ev)
        {
            if (FacilityHorror.Instance.CurrentEvent.TeslaEnabled || FacilityHorror.Instance.EventActive == false)
            {
                ev.IsAllowed = true;
                return;
            }
            ev.IsAllowed = false;
        }
        
        private static void OnWarheadStarting(WarheadStartingEventArgs ev)
        {
            FacilityHorror.Instance.WarheadActive = true;
        }

        private static void OnWarheadStopping(WarheadStoppingEventArgs ev)
        {
            FacilityHorror.Instance.WarheadActive = false;
        }
    }
}