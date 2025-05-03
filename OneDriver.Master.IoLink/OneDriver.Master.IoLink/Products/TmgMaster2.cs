using OneDriver.Framework.Libs.Announcer;
using OneDriver.Framework.Libs.Validator;
using OneDriver.Framework.Module;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;
using static OneDriver.Master.IoLink.Products.Definition;

namespace OneDriver.Master.IoLink.Products
{
    public class TmgMaster2 : DataTunnel<InternalDataHAL>, IMasterHAL
    {
        private ushort ProcessDataIndex { get; set; }
    private byte ProcessDataSubIndex { get; set; }
    protected override void FetchDataForTunnel(ref InternalDataHAL data)
    {
        var err = ReadRecord(ProcessDataIndex, ProcessDataSubIndex, out var readBuffer, out var length, out _, out _);
        data = new InternalDataHAL(SensorPortNumber, ProcessDataIndex, ProcessDataSubIndex, readBuffer);
        if (err != t_eInternal_Return_Codes.RETURN_OK || length == 0)
            Log.Error("Process data index " + ProcessDataIndex + " couldn't be read: " + err);
    }
    private readonly t_eIoLinkVersion _mIoLinkDeviceVersion = t_eIoLinkVersion.VERSION_1_1;
    private int _handle = 0;

    public TmgMaster2()
    {
        NumberOfChannels = 1;
    }

    public ConnectionError Open(string initString, IValidator validator)
    {
        string comport = validator.ValidationRegex.Match(initString).Groups[1].Value;
        _handle = IOL_Create(comport);
        Log.Error("TMG master - PC connection: " + (t_eInternal_Return_Codes)_handle + ", error code " + _handle);
        if (_handle <= 0)
            return ConnectionError.CommunicationError;
        return ConnectionError.NoError;
    }

    public ConnectionError Close()
    {
        var status = 0;
        if (_handle > 0)
        {
            status = IOL_Destroy(_handle);
            _handle = 0;
        }
        Log.Error("TMG master - PC disconnect " + (t_eInternal_Return_Codes)status + ", error code " + status);
        if (status == 0)
            return ConnectionError.NoError;
        return ConnectionError.ErrorInDisconnecting;
    }

    public void StartProcessDataAnnouncer() => StartAnnouncingData();

    public void StopProcessDataAnnouncer() => StopAnnouncingData();

    public void AttachToProcessDataEvent(DataEventHandler processDataEventHandler)
        => DataEvent += processDataEventHandler;

    public int NumberOfChannels { get; }
    public t_eInternal_Return_Codes ConnectSensorWithMaster(t_eTargetMode mode = t_eTargetMode.SM_MODE_IOLINK_OPERATE)
    {
        return SetMode(mode);
    }

    public t_eInternal_Return_Codes DisconnectSensorFromMaster()
    {
        StopAnnouncingData();
        return ConnectSensorWithMaster(t_eTargetMode.SM_MODE_IOLINK_FALLBACK);
    }

    public t_eInternal_Return_Codes ReadRecord(ushort index, byte subIndex, out byte[] readBuffer, out byte readBufferLength,
        out byte errorCode, out byte additionalCode)
    {
        var param = new TParameter
        {
            Index = index,
            SubIndex = subIndex
        };

        var status = IOL_ReadReq(_handle, (uint)SensorPortNumber, ref param);

        var readArray = new byte[param.Length];
        readBuffer = new byte[param.Length];
        if (param.Length != 0)
            for (var i = 0; i < param.Length; i++)
                readArray[i] = param.Result[i];
        readBufferLength = param.Length;
        errorCode = param.ErrorCode;
        additionalCode = param.AdditionalCode;
        readBuffer = Array.ConvertAll<byte, byte>(readArray, input => input);
        return (t_eInternal_Return_Codes)status;
    }

    public t_eInternal_Return_Codes WriteRecord(ushort index, byte subIndex, byte[] writeBuffer, out byte errorCode,
        out byte additionalCode)
    {
        var param = new TParameter();
        var status = 0;
        param.Result = new byte[256];

        param.Index = index;
        param.SubIndex = subIndex;
        if (writeBuffer.Length != 0)
        {
            param.Length = Convert.ToByte(writeBuffer.Length);
            for (var i = 0; i < writeBuffer.Length; i++)
                param.Result[i] = writeBuffer[i];
            status = IOL_WriteReq(_handle, (uint)SensorPortNumber, ref param);
        }

        /***Assign values...****/
        errorCode = param.ErrorCode;
        additionalCode = param.AdditionalCode;
        return (t_eInternal_Return_Codes)status;
    }

    public int SensorPortNumber { get; set; }

    public t_eInternal_Return_Codes SetMode(t_eTargetMode mode, t_ePortModeDetails portDetails, t_eDsConfigureCommands dsConfigure,
        t_eIoLinkVersion crid)
    {
        var portConfig = new TPortConfiguration();
        portConfig.PortModeDetails = 33;
        portConfig.TargetMode = (byte)mode;
        if (_mIoLinkDeviceVersion == t_eIoLinkVersion.VERSION_1_0)
            portConfig.CRID = 0x10;
        if (_mIoLinkDeviceVersion == t_eIoLinkVersion.VERSION_1_1)
            portConfig.CRID = 0x11;
        portConfig.DSConfigure = (byte)t_eDsConfigureCommands.DS_CFG_ENABLED;
        portConfig.InspectionLevel = (byte)t_eValidationMode.SM_VALIDATION_MODE_NONE;
        portConfig.InputLength = 32;
        portConfig.OutputLength = 32;

        var status = IOL_SetPortConfig(_handle, (uint)SensorPortNumber, ref portConfig);
        return (t_eInternal_Return_Codes)status;
    }

    public t_eInternal_Return_Codes SetMode(t_eTargetMode mode, uint cycleTimeInMicroSec, t_eDsConfigureCommands dsConfigure,
        t_eIoLinkVersion crid, byte aInputLength, byte outputLength)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes SetMode(t_eTargetMode mode, uint cycleTimeInMicroSec)
    {
        Int32 status = 0;
        TPortConfiguration _portConfig = new TPortConfiguration();

        _portConfig.PortModeDetails = (byte)(cycleTimeInMicroSec);
        _portConfig.TargetMode = (byte)mode;
        _portConfig.CRID = (byte)0x11;
        _portConfig.DSConfigure = (byte)t_eDsConfigureCommands.DS_CFG_ENABLED;
        _portConfig.InspectionLevel = (byte)t_eValidationMode.SM_VALIDATION_MODE_NONE;
        _portConfig.InputLength = 32;
        _portConfig.OutputLength = 32;

        status = IOL_SetPortConfig(_handle, mDevicePort, ref _portConfig);
        return GetErrorMessage(status);
    }

    public t_eInternal_Return_Codes GetErrorMessage(int aStatusCode)
    {
        return Enum.IsDefined(typeof(t_eInternal_Return_Codes), aStatusCode)
            ? (t_eInternal_Return_Codes)aStatusCode
            : t_eInternal_Return_Codes.No_details;
    }

    public UInt32 mDevicePort = 0;
    public t_eInternal_Return_Codes GetMode(out string aComPort, out byte[] aDeviceId, out byte[] aVendorId,
        out byte[] aFunctionId, out byte aActualMode, out byte aSensorState, out byte aMasterCycle, out byte aBaudRate)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes GetMasterInfo(out string version, out byte major, out byte minor, out byte build)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes GetDllInfo(out string build, out string aDate, out string version)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes SetMode(t_eTargetMode mode)
    {
        var portConfig = new TPortConfiguration();
        portConfig.PortModeDetails = 0;
        portConfig.TargetMode = (byte)mode;
        portConfig.CRID = 0x11;
        portConfig.DSConfigure = (byte)t_eDsConfigureCommands.DS_CFG_ENABLED;
        portConfig.InspectionLevel = (byte)t_eValidationMode.SM_VALIDATION_MODE_NONE;
        portConfig.InputLength = 32;
        portConfig.OutputLength = 32;

        var status = IOL_SetPortConfig(_handle, (uint)SensorPortNumber, ref portConfig);
        return (t_eInternal_Return_Codes)status;
    }
    private ushort _processDataIndex { get; set; }

    public t_eInternal_Return_Codes SetProcessData(ushort index, out int lengthInBytes)
    {
        var err = ReadRecord(index, 0, out var readBuffer, out _, out _, out _);
        lengthInBytes = 0;
        if (err == t_eInternal_Return_Codes.RETURN_OK)
        {
            _processDataIndex = index;
            if (readBuffer != null) lengthInBytes = readBuffer.Length;
        }
        else
            lengthInBytes = 0;
        return err;
    }

    public t_eInternal_Return_Codes SetCommand(t_eCommands command)
    {
        return t_eInternal_Return_Codes.RETURN_OK;
    }

    public t_eInternal_Return_Codes GetSensorStatus(out uint sensorStatus)
    {
        sensorStatus = 0;
        return (t_eInternal_Return_Codes)IOL_GetSensorStatus(_handle, (uint)SensorPortNumber, ref sensorStatus);
    }

    public t_eInternal_Return_Codes GetSensorStatus(out bool isConnected, out bool isEvent, out bool aIsPdValid,
        out bool aIsSensorStateKnown)
    {
        isConnected = false;
        isEvent = false;
        aIsPdValid = false;
        aIsSensorStateKnown = false;
        return t_eInternal_Return_Codes.RETURN_OK;
    }

    public t_eInternal_Return_Codes ReadEvent(out ushort aNumber, out ushort aEventCode, out byte aInstance, out byte mode,
        out byte aType, out byte pdValid, out byte localGenerated, out uint sensorStatus)
    {
        aNumber = 0;
        aEventCode = 0;
        aInstance = 0;
        mode = 0;
        aType = 0;
        pdValid = 0;
        localGenerated = 0;
        sensorStatus = 0;
        return t_eInternal_Return_Codes.RETURN_OK;
    }

    public t_eInternal_Return_Codes GetVariableInfo(string indexName, string subIndexName, out ushort index, out byte subIndex,
        out string type, out uint length, out long @default, out long min, out long max)
    {
        index = 0;
        subIndex = 0;
        type = "";
        length = 0;
        @default = 0;
        min = 0;
        max = 0;
        return t_eInternal_Return_Codes.RETURN_OK;
    }

    public t_eInternal_Return_Codes ProcessDataReadOutputs(ref byte[] pData, out uint length, out uint status)
    {
        length = 0;
        status = 0;
        return t_eInternal_Return_Codes.RETURN_OK;
    }

    public  t_eInternal_Return_Codes ProcessDataReadInputs(ref byte[] pData, out uint length, out uint status)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes ProcessDataWriteOutputs(ref byte[] pData, uint length)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes ProcessDataTransfer(ref byte[] pDataOut, uint lengthOut, ref byte[] pDataIn, out uint lengthIn,
        out uint status)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes ProcessDataStartLogging(string filename, uint sampleTime)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes ProcessDataStopLogging()
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes ProcessDataReadInputSwp(out uint adValue, out uint reserved, out uint binaryOut3,
        out uint binaryOut2, out uint binaryOut1, out bool isConnected, out bool hasEvent, out bool isPdValid)
    {
        throw new NotImplementedException();
    }

    public t_eInternal_Return_Codes GetLastError()
    {
        throw new NotImplementedException();
    }
    // ReSharper disable All

    #region DLL_IMPORT

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_Create(string Device);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_Destroy(Int32 Handle);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_GetUSBDevices(ref TDeviceIdentification[] pDeviceList, Int32 MaxNumberOfEntries);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_SetPortConfig(Int32 Handle, UInt32 Port, ref TPortConfiguration pConfig);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_ReadReq(Int32 Handle, UInt32 Port, ref TParameter Parameter);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_WriteReq(Int32 Handle, UInt32 Port, ref TParameter Parameter);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_GetMasterInfo(Int32 Handle, ref TMasterInfo MasterInfo);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_GetDLLInfo(ref TDllInfo DllInfo);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_GetModeEx(Int32 Handle, UInt32 aPort, ref TInfoEx InfoEx, bool OnlyStatus);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_GetMode(Int32 Handle, UInt32 Port, ref TInfo Info);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_SetCommand(Int32 Handle, UInt32 Port, UInt32 Command);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_GetSensorStatus(Int32 Handle, UInt32 aPort, ref UInt32 Status);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_ReadOutputs(Int32 Handle, UInt32 aPort, IntPtr ProcessData, ref UInt32 Length,
        ref UInt32 Status);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_ReadInputs(Int32 Handle, UInt32 aPort, IntPtr ProcessData, ref UInt32 Length,
        ref UInt32 Status);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_WriteOutputs(Int32 Handle, UInt32 aPort, IntPtr ProcessData, UInt32 Length);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_TransferProcessData(Int32 Handle, UInt32 aPort, IntPtr ProcessDataOut, UInt32 LengthOut,
        IntPtr ProcessdataIn, ref UInt32 LengthIn, ref UInt32 Status);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_StartDataLogging(Int32 Handle, UInt32 Port, StringBuilder Filename, ref UInt32 SampleTime);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_StopDataLogging(Int32 Handle);

    [DllImport("IOLUSBIF20_64.dll")]
    static extern Int32 IOL_ReadEvent(Int32 Handle, ref TEvent Event, ref UInt32 Status);


        #endregion DLL_IMPORT

        // ReSharper restore All
    }
}
