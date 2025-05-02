using System.Runtime.InteropServices;

namespace OneDriver.Master.IoLink.Products
{
    public class Definition
    {
        #region t_eBaudate enum

        public enum t_eBaudate
        {
            SM_BAUD_19200 = 0,

            /**
             * < speed of the connection is 19200 baud
             */
            SM_BAUD_38400 = 1,

            /**
             * < speed of the connection is 38400 baud
             */
            SM_BAUD_230400 = 2 /**< speed of the connection is 230400 baud */
        }

        #endregion

        #region t_eCommands enum

        public enum t_eCommands
        {
            SM_COMMAND_FALLBACK = 5,

            /**
             * < switch Device from IO-Link mode back to SIO
             */
            SM_COMMAND_PD_OUT_VALID = 6,

            /**
             * < send outputs_valid to device
             */
            SM_COMMAND_PD_OUT_INVALID = 7,

            /**
             * < send outputs_invalid to device
             */
            SM_COMMAND_OPERATE = 8,

            /**
             * < switch from preoperate to operate state
             */
            SM_COMMAND_RESTART = 9 /**< restart the connection */
        }

        #endregion

        #region t_eDsConfigureCommands enum

        public enum t_eDsConfigureCommands
        {
            DS_CFG_DISABLED = 0x00,
            DS_CFG_ENABLED = 0x80,

            /**
             * < the data storage is enabled
             */
            DS_CFG_UPLOAD_ENABLED = 0x01 /**< the automatical upload is enabled */
        }

        #endregion

        #region t_eInternal_Return_Codes enum

        public enum t_eInternal_Return_Codes
        {
            RETURN_FIRMWARE_NOT_COMPATIBLE = -16,

            /**
             * < the firmware needs a firmware update because some of the functions are not implemented
             */
            RETURN_FUNCTION_CALLEDFROMCALLBACK = -15,

            /**
             * < calling a DLL function from inside a callback is not allowed
             */
            RETURN_FUNCTION_DELAYED = -14,

            /**
             * < a callback has been defined, so the result may come later with the callback
             */
            RETURN_FUNCTION_NOT_IMPLEMENTED = -13,

            /**
             * < the function is not implemented in the connected IO= Link Master
             */
            RETURN_STATE_CONFLICT = -12,

            /**
             * < the function cannot be used in the actual state of the IO-Link Master
             */
            RETURN_WRONG_COMMAND = -11,

            /**
             * < a wrong answer to a command has been received from the IO-Link Master
             */
            RETURN_WRONG_PARAMETER = -10,

            /**
             * < one of the function parameters is invalid
             */
            RETURN_WRONG_DEVICE = -9,

            /**
             * < the device name was wrong or the device which is connected is not supported
             */
            RETURN_NO_EVENT = -8,

            /**
             * < a Read Event was called, but there is no event
             */
            RETURN_UNKNOWN_HANDLE = -7,

            /**
             * < the handle of the function is unknown
             */
            RETURN_UART_TIMEOUT = -6,

            /**
             * < a timeout has been reached because there as no answer to a command
             */
            RETURN_CONNECTION_LOST = -5,

            /**
             * < the USB master has been unplugged during communication
             */
            RETURN_OUT_OF_MEMORY = -4,

            /**
             * < no more memory available
             */
            RETURN_DEVICE_ERROR = -3,

            /**
             * < error in accessing the USB device driver
             */
            RETURN_DEVICE_NOT_AVAILABLE = -2,

            /**
             * < the device is not available at this moment
             */
            RETURN_INTERNAL_ERROR = -1,

            /**
             * < internal library error. Please restart the program
             */
            RETURN_OK = 0,

            /**
             * < sucessful end of the function
             */
            COMMAND_NOT_APPLICABLE = 1,

            /**
             * < the command is not applicable in the actual state
             */
            RESULT_NOT_SUPPORTED = 2,

            /**
             * < the command is not supported on this device
             */
            RESULT_SERVICE_PENDING = 3,

            /**
             * < a Service is pending. A new service must wait for the end of the pending service
             */
            RESULT_WRONG_PARAMETER_STACK = 4,

            /**
             * < a parameter has been rejected by the USB master
             */
            No_details = 0x8000,
            Index_not_available = 0x8011,
            Subindex_not_available = 0x8012,
            Service_temporarily_not_available = 0x8020,
            Service_temporarily_not_available_local_control = 0x8021,
            Service_temporarily_not_available_device_control = 0x8022,
            Access_denied = 0x8023,
            Parameter_Value_out_of_range = 0x8030,
            Parameter_value_above_limit = 0x8031,
            Parameter_value_below_limit = 0x8032,
            Parameter_length_overrun = 0x8033,
            Parameter_length_underrun = 0x8034,
            Function_not_available = 0x8035,
            Function_temporarily_unavailable = 0x8036,
            Interfering_parameter = 0x8040,
            Inconsistent_parameter_set = 0x8041,
            Application_not_ready = 0x8082,
            Vender_Specific_Error = 0x8100
        }

        #endregion

        #region t_eIoLinkVersion enum

        public enum t_eIoLinkVersion
        {
            VERSION_1_0 = 0x10,
            VERSION_1_1 = 0x11
        }

        #endregion

        #region t_ePortModeDetails enum

        public enum t_ePortModeDetails
        {
            SM_MODE_FREE_RUNNING = 0,
            SM_MODE_SIO_PP_SWITCH = 0x0,

            /**
             * < Digital output works in Push/ Pull mode
             */
            SM_MODE_SIO_HS_SWITCH = 0x80,

            /**
             * < Digital output works as High Side Switch
             */
            SM_MODE_SIO_LS_SWITCH = 0x40,

            /**
             * < Digital output works as Low Side Switch
             */
            SM_MODE_NORMAL_INPUT = 0,

            /**
             * < Digital input works as a normal input
             */
            SM_MODE_DIAGNOSTIC_INPUT = 1,

            /**
             * < Digital input works as a diagnostic input
             */
            SM_MODE_INVERT_INPUT = 2 /**< Digital input works as a inverted input */
        }

        #endregion

        #region t_eSensorState enum

        public enum t_eSensorState
        {
            STATE_DISCONNECTED_GETMODE = 0,
            STATE_PREOPERATE_GETMODE = 0x80,
            STATE_WRONGSENSOR_GETMODE = 0x40,
            STATE_OPERATE_GETMODE = 0xFF
        }

        #endregion

        #region t_eTargetMode enum

        public enum t_eTargetMode
        {
            SM_MODE_RESET = 0,

            /**
             * < Port is deactivated
             */
            SM_MODE_IOLINK_PREOP = 1,

            /**
             * < Port is in IO-Link mode and stops in Preoperate
             */
            SM_MODE_SIO_INPUT = 3,

            /**
             * < Port is in SIO Input mode
             */
            SM_MODE_SIO_OUTPUT = 4,

            /**
             * < Port is in SIO Output mode
             */
            SM_MODE_IOLINK_PREOP_FALLBACK = 10,

            /**
             * < io-link to preoperate, fallback allowed
             */
            SM_MODE_IOLINK_OPER_FALLBACK = 11,

            /**
             * < io-link to operate, fallback allowed
             */
            SM_MODE_IOLINK_OPERATE = 12,

            /**
             * < Io-Link, but go into operate automatically
             */
            SM_MODE_IOLINK_FALLBACK = 13 /**< io-link to preoperate, then automatically to fallback */
        }

        #endregion

        #region t_eValidationMode enum

        public enum t_eValidationMode
        {
            SM_VALIDATION_MODE_NONE = 0,

            /**
             * < no validation, each combination of device and vendor id is allowed
             */
            SM_VALIDATION_MODE_COMPATIBLE = 1,

            /**
             * < device and vendor ID will be checked
             */
            SM_VALIDATION_MODE_IDENTICAL = 2 /**< device and vendor ID and the serial number will be checked */
        }

        #endregion

        private const ushort SYSTEM_ACCESS_INDEX = 0xF3;

        private byte[] SYSTEM_ACCESS_PASSWORD =
            { 0x33, 0x03, 0x30, 0xC0, 0x4C, 0x7C, 0x4C, 0x05, 0x99, 0xA0, 0x70, 0x02, 0x2F, 0xF7, 0xC6, 0x53 };

        #region Nested type: TDeviceIdentification

        [StructLayout(LayoutKind.Sequential)]
        public struct TDeviceIdentification
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] Name;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public char[] ProductCode;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public char[] ViewName;
        }

        #endregion

        #region Nested type: TDllInfo

        public struct TDllInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public char[] Build;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public char[] Datum;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public char[] Version;
        }

        #endregion

        #region Nested type: TEvent

        public struct TEvent
        {
            public ushort Number;
            public ushort Port;
            public ushort EventCode;
            public byte Instance;
            public byte Mode;
            public byte Type;
            public byte PDValid;
            public byte LocalGenerated;
        }

        #endregion

        #region Nested type: TInfo

        [StructLayout(LayoutKind.Sequential)]
        public struct TInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public char[] COM;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] DeviceID;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] VendorID;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FunctionID;

            public byte ActualMode;
            public byte SensorState;
            public byte MasterCycle;
            public byte CurrentBaudrate;
        }

        #endregion

        #region Nested type: TInfoEx

        [StructLayout(LayoutKind.Sequential)]
        public struct TInfoEx
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public char[] COM;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] DirectParameterPage;

            public byte ActualMode;
            public byte SensorStatus;
            public byte CurrentBaudrate;
        }

        #endregion

        #region Nested type: TMasterInfo

        public struct TMasterInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
            public char[] Version;

            public byte Major;
            public byte Minor;
            public byte Build;
            public byte MajorRevisionIOLStack;

            /**
             * < major revision of the IO-Link stack used by the master
             */
            public byte MinorRevisionIOLStack;

            /**
             * < minor revision of the IO-Link stack used by the master
             */
            public byte BuildRevisionIOLStack; /**< build revision of the IO-Link stack used by the master */
        }

        #endregion

        #region Nested type: TParameter

        [StructLayout(LayoutKind.Sequential)]
        public struct TParameter
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Result;

            public ushort Index;
            public byte SubIndex;
            public byte Length;
            public byte ErrorCode;
            public byte AdditionalCode;
        }

        #endregion

        #region Nested type: TPortConfiguration

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TPortConfiguration
        {
            public byte PortModeDetails;
            public byte TargetMode;
            public byte CRID;
            public byte DSConfigure;
            public byte Synchronisation;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FunctionID;

            public byte InspectionLevel;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] VendorID;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] DeviceID;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] SerianNumber;

            public byte InputLength;
            public byte OutputLength;
        }

        #endregion
    }
}
