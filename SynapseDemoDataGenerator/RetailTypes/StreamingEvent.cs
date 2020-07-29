using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace SynapseDemoDataGenerator.RetailTypes
{
    public enum EventType { Start, Stop, Complete }
    [ProtoContract]
    class StreamingEvent
    {
        [ProtoMember(1)]
        public Guid StreamingId { get; set; }
        [ProtoMember(2)]
        public Guid SessionId { get; set; }
        [ProtoMember(3)]
        public DateTime EventTime { get; set; }
        [ProtoMember(4)]
        public int UserId { get; set; }
        [ProtoMember(5)]
        public int MediaId { get; set; }
        [ProtoMember(6)]
        public EventType EventType { get; set; }
        [ProtoMember(7)]
        public int Duration { get; set; }
        [ProtoMember(8)]
        public string PlatformType { get; set; }
        [ProtoMember(9)]
        public string PlatformName { get; set; }
    }
}
