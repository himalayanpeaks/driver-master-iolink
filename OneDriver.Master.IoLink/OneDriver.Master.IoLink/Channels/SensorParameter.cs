using OneDriver.Helper;
using OneDriver.Master.Abstract.Channels;

namespace OneDriver.Master.IoLink.Channels
{
    public class SensorParameter : CommonSensorParameter
    {
        private int _subindex;

        public int Subindex
        {
            get => _subindex;
            set => SetProperty(ref _subindex, value);
        }
        public SensorParameter(string name) : base(name)
        {
        }

        public SensorParameter(string name, int index, int subindex, Abstract.Definition.AccessType access, Definitions.DataType dataType, int arrayCount, int lengthInBits, int offset, string? value, string? @default, string? minimum, string? maximum, string? valid) : base(name, index, access, dataType, arrayCount, lengthInBits, offset, value, @default, minimum, maximum, valid)
        {
            _subindex = subindex;
        }

        public SensorParameter() : base("")
        {

        }
        public SensorParameter(CommonSensorParameter commonSensorParameter) :
        base(commonSensorParameter.Name, commonSensorParameter.Index, commonSensorParameter.Access, commonSensorParameter.DataType,
        commonSensorParameter.ArrayCount, commonSensorParameter.LengthInBits, commonSensorParameter.Offset,
        commonSensorParameter.Value, commonSensorParameter.Default, commonSensorParameter.Minimum,
        commonSensorParameter.Maximum, commonSensorParameter.Valid)
        {
            _subindex = 0;
        }
    }
}
