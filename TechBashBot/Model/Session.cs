using System;

namespace TechBashBot.Model
{
    [Serializable]
    public class Session
    {
        public string Description { get; set; }
        public string LocationName { get; set; }
        public string SpeakerName { get; set; }
        public string StartTime { get; set; }
        public string Title { get; set; }

    }
}
