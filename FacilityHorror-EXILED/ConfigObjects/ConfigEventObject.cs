using System.ComponentModel;

namespace FacilityHorror.ConfigObjects
{
    public class ConfigEventObject
    {
        [Description("Randomized duration in seconds of this event. Not used when lights_out is false and tesla_enabled is true")]
        public int MinRandomEventDuration { get; set; } = 10;
        public int MaxRandomEventDuration { get; set; } = 100;
        [Description("Should the event run while the warhead is active (might cause issues)")]
        public bool RunWhenWarhead { get; set; } = false;

        [Description("Should CASSIE broadcast a message when an event starts")] 
        public bool CassieTextEnabled { get; set; } = true;
        [Description("The text CASSIE needs to broadcast if cassie_text_enabled is true")] 
        public string CassieText { get; set; } = "Generator malfunction detected";
        [Description("Should CASSIE subtitles be shown")]
        public bool CassieShowSubtitles { get; set; } = true;
        [Description("Should CASSIE play the announcement jingle before announcing")]
        public bool CassieRunJingle { get; set; } = true;
        [Description("Should CASSIE wait before the announcement is finished before executing settings below")]
        public bool CassieWaitForMessage { get; set; } = false;

        [Description("Should tesla gates be enabled during the event. SCP-079 will always be able to use tesla, even during an event")]
        public bool TeslaEnabled { get; set; } = false;
        [Description("Should the lights go out during the event")]
        public bool LightsOut { get; set; } = true;
        [Description("Should the doors lock during the event")]
        public bool DoorsLocked = false;

    }
}