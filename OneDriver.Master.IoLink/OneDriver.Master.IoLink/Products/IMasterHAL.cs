using OneDriver.Framework.Module;
using static OneDriver.Master.IoLink.Products.Definition;

namespace OneDriver.Master.IoLink.Products
{

    public interface IMasterHAL : IHalLayer<InternalDataHAL>
    {
        t_eInternal_Return_Codes ConnectSensorWithMaster(t_eTargetMode mode = t_eTargetMode.SM_MODE_IOLINK_OPERATE);

        Definition.t_eInternal_Return_Codes DisconnectSensorFromMaster();
        t_eInternal_Return_Codes ReadRecord(ushort index, byte subIndex, out byte[] readBuffer,
            out byte readBufferLength, out byte errorCode, out byte additionalCode);

        t_eInternal_Return_Codes WriteRecord(ushort index, byte subIndex, byte[] writeBuffer, out byte errorCode,
            out byte additionalCode);
        int SensorPortNumber { get; set; }
        t_eInternal_Return_Codes SetMode(t_eTargetMode mode, t_ePortModeDetails portDetails,
        t_eDsConfigureCommands dsConfigure, t_eIoLinkVersion crid);

        t_eInternal_Return_Codes SetMode(t_eTargetMode mode, uint cycleTimeInMicroSec, t_eDsConfigureCommands dsConfigure,
            t_eIoLinkVersion crid, byte aInputLength, byte outputLength);

        t_eInternal_Return_Codes SetMode(t_eTargetMode mode, uint cycleTimeInMicroSec);
        public t_eInternal_Return_Codes GetMode(out string aComPort, out byte[] aDeviceId, out byte[] aVendorId,
            out byte[] aFunctionId, out byte aActualMode, out byte aSensorState, out byte aMasterCycle, out byte aBaudRate);
        t_eInternal_Return_Codes GetMasterInfo(out string version, out byte major, out byte minor, out byte build);
        t_eInternal_Return_Codes GetDllInfo(out string build, out string aDate, out string version);
        t_eInternal_Return_Codes SetMode(t_eTargetMode mode);
        t_eInternal_Return_Codes SetProcessData(ushort index, out int lengthInBytes);

        t_eInternal_Return_Codes SetCommand(t_eCommands command);
        t_eInternal_Return_Codes GetSensorStatus(out uint sensorStatus);

        t_eInternal_Return_Codes GetSensorStatus(out bool isConnected, out bool isEvent, out bool aIsPdValid,
            out bool aIsSensorStateKnown);

        t_eInternal_Return_Codes ReadEvent(out ushort aNumber, out ushort aEventCode, out byte aInstance, out byte mode,
            out byte aType, out byte pdValid, out byte localGenerated, out uint sensorStatus);

        t_eInternal_Return_Codes GetVariableInfo(string indexName, string subIndexName, out ushort index, out byte subIndex,
            out string type, out uint length, out long @default, out long min, out long max);

        // This function reads-back the Process Data written to the Process-Data output-buffer 
        // previously with ProcessDataWriteOutputs(...).
        t_eInternal_Return_Codes ProcessDataReadOutputs(ref byte[] pData, out uint length, out uint status);

        // Reads the Process Data from the USB IO-Link Master Gateway, which was received from the sensor.
        t_eInternal_Return_Codes ProcessDataReadInputs(ref byte[] pData, out uint length, out uint status);

        // Write the Process Data to USB-IOLink Master. The data is then transferred to the connected sensor. 
        t_eInternal_Return_Codes ProcessDataWriteOutputs(ref byte[] pData, uint length);

        // This function transfers Process Data in both directions. It first sends out the Process data referenced by pDataOut
        // and then receives the response and writes it's content in pDataIn.
        t_eInternal_Return_Codes ProcessDataTransfer(ref byte[] pDataOut, uint lengthOut, ref byte[] pDataIn,
            out uint lengthIn, out uint status);

        // This function informs the master to send cyclically the process input data to the DLL. The DLL will store 
        // them together with the output data into the file which has been defined by filename. The logging will only 
        // occur if the mode of the port is not in deactivated. The sample time will be given in ms, however
        // the master interface will round this time to a value it can provide. 
        t_eInternal_Return_Codes ProcessDataStartLogging(string filename, uint sampleTime);

        // This function informs the master to stop the logging of the Process Data. 
        t_eInternal_Return_Codes ProcessDataStopLogging();

        // This is a function which returns the Process Data structure defined in the SWP Platform
        t_eInternal_Return_Codes ProcessDataReadInputSwp(out uint adValue, out uint reserved, out uint binaryOut3,
            out uint binaryOut2, out uint binaryOut1, out bool isConnected, out bool hasEvent, out bool isPdValid);

        public t_eInternal_Return_Codes GetLastError();
    }
}
