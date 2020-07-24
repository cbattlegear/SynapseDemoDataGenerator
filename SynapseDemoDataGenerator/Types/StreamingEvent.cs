using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.Types
{
    public enum EventType { Start, Stop, Complete }
    class StreamingEvent
    {
        public Guid StreamingId { get; set; }
        public Guid SessionId { get; set; }
        public DateTime EventTime { get; set; }
        public int UserId { get; set; }
        public int MediaId { get; set; }
        public EventType EventType { get; set; }
        public int Duration { get; set; }
        public string PlatformType { get; set; }
        public string PlatformName { get; set; }
    }
}
