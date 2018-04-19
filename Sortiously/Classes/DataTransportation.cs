using System;

namespace Sortiously
{
    public class DataTransportation
    {
        public DataTransport TransportType { get; set; }
        public Action<string>  PassthroughAction { get; set; }
    }
}
