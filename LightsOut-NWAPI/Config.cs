using System.ComponentModel;

namespace LightsOut
{
    public sealed class Config
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Chance percentage that the event will activate this round, between 1 and 100. Only round numbers are accepted.")]
        public int ActivationChance { get; private set; } = 50;

        [Description("Enable if you want to keep tesla gates working while in blackout. SCP-079 is always able to trigger a tesla, even during a blackout.")]
        public bool KeepTeslaEnabled { get; private set; } = false;

        [Description("The CASSIE message to broadcast.")]
        public string CassieMessage { get; private set; } = "generator .g3 malfunction detected .g4 .g3 .g3 .g4";
        [Description("Enable if the ding-dong at the start of the CASSIE announcement should be played.")]
        public bool CassieSoundAlarm { get; private set; } = true;
        public bool CassieDisplaySubtitles { get; private set; } = true;
        [Description("Enable if the lights should turn off after the full CASSIE announecment has played. If disabled, the lights will turn off while the announcement is playing.")]
        public bool CassieWaitForToggle { get; private set; } = true;

        [Description("Time in seconds after round start that the blackout cannot occur. Note: the min_random_interval will be added to this.\n# For example: min_start_offset: 60 and min_random_interval: 40 means that from the round start at least 100 seconds will pass before the lights event can occur.\n# After the first blackout the event cannot occur for only the time specified in min_random_interval.")]
        public int MinStartOffset { get; private set; } = 60;
        [Description("Randomized time interval in seconds between blackouts.")]
        public int MinRandomInterval { get; private set; } = 40;
        public int MaxRandomInterval { get; private set; } = 400;

        [Description("Randomized duration in seconds of each blackout.")]
        public int MinRandomBlackoutTime { get; private set; } = 10;
        public int MaxRandomBlackoutTime { get; private set; } = 100;
    }
}
