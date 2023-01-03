using System.ComponentModel;

using Exiled.API.Interfaces;

namespace LightsOut
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }

        [Description("Chance percentage that the event will activate this round, between 1 and 100. Only round numbers are accepted.")]
        public int ActivationChance { get; private set; } = 50;

        [Description("Cassie settings")]
        public string CassieMessage { get; private set; } = "generator .g3 malfunction detected .g4 .g3 .g3 .g4";
        public bool CassieSoundAlarm { get; private set; } = true;
        public bool CassieDisplaySubtitles { get; private set; } = true;
        public string CassieSubtitles { get; private set; } = "Generator malfunction detected.";
        public bool CassieWaitForToggle { get; private set; } = true;

        [Description("This will be added to the random time the first time it runs.")]
        public int MinStartOffset { get; private set; } = 60;
        [Description("Randomized interval between blackouts")]
        public int MinRandomInterval { get; private set; } = 40;
        public int MaxRandomInterval { get; private set; } = 400;

        [Description("Randomized duration of each blackout")]
        public int MinRandomBlackoutTime { get; private set; } = 10;
        public int MaxRandomBlackoutTime { get; private set; } = 100;
    }
}
