using System;
using System.Linq;

namespace Sortiously
{
    public static class DataTransportationFactory
    {

        public static DataTransportation CreateAsFile()
        {
            return new DataTransportation() { TransportType = DataTransport.File };
        }

        public static DataTransportation CreateAsPassthrough(Action<string> passAction)
        {
            return new DataTransportation() { TransportType = DataTransport.Passthrough,  PassthroughAction = passAction };
        }

        public static DataTransportation CreateAsFileAndPassthrough(Action<string> passAction)
        {
            return new DataTransportation() { TransportType = DataTransport.File | DataTransport.Passthrough, PassthroughAction = passAction };
        }
    }
}
