using DeviceDescriptor.IoLink.Source;
using OneDriver.Framework.Libs.Validator;
using OneDriver.Master.IoLink;
using OneDriver.Master.IoLink.Products;
class Program
{
    static async Task Main(string[] args)
    {
        LocalStorage iodd = new LocalStorage();
        Device device = new Device("IoLinkDevice", new ComportValidator(), 
            new TmgMaster2(), iodd);
        device.Connect("COMport here");
        device.SelectSensorAtPort(0);
        device.ConnectSensor();
        device.LoadIodd(@"Path to IODD here");
        device.ReadParam(224, 0, out var read);
        device.ReadParameterFromSensor("Param name here", out var readbuff);
        device.DisconnectSensor();
        device.Disconnect();
    }
}