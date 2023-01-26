using System.Collections.Generic;
using System.ComponentModel;
using FacilityHorror.ConfigObjects;

namespace FacilityHorror
{
    public sealed class Config
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Chance percentage that the event system will activate this round, between 1 and 100")]
        public int ActivationChance { get; private set; } = 50;

        [Description("Time in seconds after round start that the event system is disabled. Note: the min_random_interval will be added to this.\nFor example: min_start_offset: 60 and min_random_interval: 40 means that from the round start at least 100 seconds will pass before first event can occur.\nAfter the first event cannot occur for only the time specified in min_random_interval.")]
        public int MinStartOffset { get; private set; } = 60;
        [Description("Randomized time interval in seconds between events")]
        public int MinRandomInterval { get; private set; } = 40;
        public int MaxRandomInterval { get; private set; } = 400;

        [Description("List of events")]
        public Dictionary<string, ConfigEventObject> EventList { get; private set; } = new()
        {
            {
                "NormalLightsOut", new()
                {
                    EventChance = 50,
                    MinRandomEventDuration = 10,
                    MaxRandomEventDuration = 100,
                    RunWhenWarhead = false,
                    CassieTextEnabled = true,
                    CassieText = "Generator malfunction detected",
                    CassieShowSubtitles = true,
                    CassieRunJingle = true,
                    TeslaEnabled = false,
                    LightsOut = true,
                    DoorsLocked = false
                }
            },
            {
                "DoorRestart", new()
                {
                    EventChance = 50,
                    MinRandomEventDuration = 10,
                    MaxRandomEventDuration = 100,
                    RunWhenWarhead = false,
                    CassieTextEnabled = true,
                    CassieText = "Door system malfunction detected",
                    CassieShowSubtitles = true,
                    CassieRunJingle = true,
                    TeslaEnabled = true,
                    LightsOut = false,
                    DoorsLocked = true
                }
            },
            {
                "CassieBleep", new()
                {
                    EventChance = 50,
                    MinRandomEventDuration = 10,
                    MaxRandomEventDuration = 15,
                    RunWhenWarhead = true,
                    CassieTextEnabled = true,
                    CassieText = "pitch_0.15 .g7",
                    CassieShowSubtitles = false,
                    CassieRunJingle = false,
                    TeslaEnabled = true,
                    LightsOut = false,
                    DoorsLocked = false,
                }
            }
        };
    }
}
