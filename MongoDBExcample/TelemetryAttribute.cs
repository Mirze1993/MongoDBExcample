namespace MongoDBExcample
{
    public class TelemetryAttribute : Attribute
    {
        public TelemetryEvent Event { get; set; }

        public TelemetryAttribute(TelemetryEvent ev)
        {
            Event = ev;
        }
    }

    public enum TelemetryEvent { Req, Resp,ReqQuery }
}
