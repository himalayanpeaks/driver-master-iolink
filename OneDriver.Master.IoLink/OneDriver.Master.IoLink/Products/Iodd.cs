using OneDriver.Framework.Libs.DeviceDescriptor;
using ParameterTool.NSwagClass.Generator.Interface;

namespace OneDriver.Master.IoLink.Products
{
    public class Iodd : IDeviceDescriptor
    {
        public List<ParameterDetailsResponse> ReadData(string server, string hashId, int protocolId)
        {
            throw new NotImplementedException();
        }

        public List<ParameterDetailsResponse> ReadData(string server, int deviceId, int protocolId)
        {
            throw new NotImplementedException();
        }
    }
}
