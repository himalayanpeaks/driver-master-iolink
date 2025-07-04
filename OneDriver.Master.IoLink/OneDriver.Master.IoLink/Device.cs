﻿using OneDriver.Master.Abstract;
using OneDriver.Master.IoLink.Products;
using OneDriver.Framework.Libs.Validator;
using OneDriver.Framework.Module.Parameter;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OneDriver.Framework.Base;
using DeviceDescriptor.IoLink.Variables;
using DeviceDescriptor.Abstract.Variables;
using DeviceDescriptor.Abstract.Helper;
using static DeviceDescriptor.Abstract.Definition;
using DeviceDescriptor.Abstract;
using DeviceDescriptor.IoLink;
using DeviceDescriptor.IoLink.Source;

namespace OneDriver.Master.IoLink
{
    public class Device : CommonDevice<DeviceParams, Variable>
    {
        private IMasterHAL DeviceHAL { get; set; }
        Translator DescriptorTranslator { get; set; }
        public Device(string name, IValidator validator, IMasterHAL deviceHAL, IDescriptorTranslator<Variable> descriptor) :
            base(new DeviceParams(name), validator,
                new ObservableCollection<BaseChannel<DeviceVariables<Variable>>>()) 
        {
            DeviceHAL = deviceHAL;
            DescriptorTranslator = new Translator(descriptor);
            
            Init();
        }

        private void Init()
        {
            Parameters.PropertyChanging += Parameters_PropertyChanging;
            Parameters.PropertyChanged += Parameters_PropertyChanged;
            Parameters.PropertyReadRequested += Parameters_PropertyReadRequested;
            DeviceHAL.AttachToProcessDataEvent(ProcessDataChanged);

            for (var i = 0; i < DeviceHAL.NumberOfChannels; i++)
            {
                var channelParameters = new DeviceVariables<Variable>("Channel_" + i.ToString());
                var item = new BaseChannel<DeviceVariables<Variable>>(channelParameters);
                Elements.Add(item);
                Elements[i].Parameters.PropertyChanged += Parameters_PropertyChanged;
                Elements[i].Parameters.PropertyChanging += Parameters_PropertyChanging;
            }
        }

        private const int HashIndex = 253;
        private BaseChannel<DeviceVariables<Variable>> item;

        private void Parameters_PropertyReadRequested(object sender, PropertyReadRequestedEventArgs e)
        {
            switch (e.PropertyName)
            {                
            }
        }

        private void ProcessDataChanged(object sender, InternalDataHAL e)
        {
            var local = Elements[e.ChannelNumber].Parameters.ProcessData.ProcessDataInCollection.FindAll(x => x.Index == e.Index);
            foreach (var parameter in local)
                parameter.Value =
                    DataConverter.MaskByteArray(e.Data, parameter.Offset, parameter.LengthInBits,
                        parameter.DataType, true);
        }

        private void Parameters_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Parameters.SelectedChannel):
                    DeviceHAL.SensorPortNumber = Parameters.SelectedChannel;
                    break;
                case nameof(Parameters.Mode):
                    switch (Parameters.Mode)
                    {
                        case Abstract.Contracts.Definition.Mode.Communication:
                            DeviceHAL.StopProcessDataAnnouncer();
                            break;
                        case Abstract.Contracts.Definition.Mode.ProcessData:
                            DeviceHAL.StartProcessDataAnnouncer();
                            break;
                        case Abstract.Contracts.Definition.Mode.StandardInputOutput:
                            DeviceHAL.StopProcessDataAnnouncer();
                            break;
                    }
                    break;
            }
        }
        public Products.Definition.t_eInternal_Return_Codes AddProcessDataIndex(int processDataIndex) => DeviceHAL.SetProcessData((ushort)processDataIndex, out var length);
        

        private void Parameters_PropertyChanging(object sender, PropertyValidationEventArgs e)
        {
            //Write validity before property is changed here
            switch (e.PropertyName)
            {

            }
        }

        protected override int CloseConnection() => (int)DeviceHAL.Close();
        protected override int OpenConnection(string initString) => (int)DeviceHAL.Open(initString, validator);

        public override int ConnectSensor()
        {
            var err = DeviceHAL.ConnectSensorWithMaster();
            Log.Information(err.ToString());

            return (err == Products.Definition.t_eInternal_Return_Codes.RETURN_OK) ? 0
                : (int)Abstract.Contracts.Definition.Error.SensorCommunicationError;
        }

        public override int DisconnectSensor() => (int)DeviceHAL.DisconnectSensorFromMaster();

        protected override string GetErrorAsText(int errorCode)
        {
            if (Enum.IsDefined(typeof(Products.Definition.t_eInternal_Return_Codes), errorCode))
                return ((Products.Definition.t_eInternal_Return_Codes)errorCode).ToString();

            return "UnknownError";
        }

        public int ReadParam(int index, int subindex, out byte[]? data)
        {
            var err = DeviceHAL.ReadRecord((ushort)index, (byte)subindex, out data, out _, out _, out _);
            if (data == null)
                throw new Exception("index: " + index + " read value is null");
            if (data.Length == 0)
                throw new Exception("index: " + index + " no data available");
            return (int)err;
        }
        public int WriteParam(int index, int subindex, byte[] data)
        {
            var err = DeviceHAL.WriteRecord((ushort)index, (byte)subindex, data, out _, out _);
            return (int)err;
        }
        protected override int ReadParam(Variable param)
        {
            param.Value = null;
            var err = DeviceHAL.ReadRecord(Convert.ToUInt16(param.Index),
                Convert.ToByte(param.Subindex), out var data, out _, out _, out _);

            if (Equals(data, null))
                throw new Exception("index: " + param.Index + " read value is null");
            if (data.Length == 0)
                throw new Exception("index: " + param.Index + " no data available");


            if (param.DataType == DataType.UINT || param.DataType == DataType.INT || param.DataType == DataType.Float32 ||
                param.DataType == DataType.Byte || param.DataType == DataType.BOOL)
            {
                DataConverter.ToNumber(data, param.DataType, param.LengthInBits, true, out string?[] valueData);
                if (valueData == null)
                {
                    param.Value = string.Join(";", data.Select(x => x.ToString()).ToArray());
                    throw new Exception("index: " + param.Index + " data length mismatch");
                }

                param.Value = string.Join(";", valueData);
            }

            if (param.DataType == DataType.CHAR)
            {
                DataConverter.ToString(data, param.DataType, param.LengthInBits, true, out var val);
                param.Value = val;
            }
            return (int)err;
        }

        public void LoadIodd(string ioddFile)
        {
            if (string.IsNullOrEmpty(ioddFile))
                throw new ArgumentException("IODD file path cannot be null or empty.", nameof(ioddFile));
            var descriptor = DescriptorTranslator.LoadFromWebAsync(ioddFile, "", "");
            if (descriptor == null)
                throw new Exception("Failed to load IODD file: " + ioddFile);
            else
                this.Elements[this.Parameters.SelectedChannel].Parameters = descriptor.Variables;

        }
        protected override int WriteParam(Variable param)
        {
            if (string.IsNullOrEmpty(param.Value))
                Log.Error(param.Name + " Data null");

            string[] dataToWrite = param.Value.Split(';').ToArray();
            DataConverter.DataError dataError;
            if ((dataError = DataConverter.ToByteArray(dataToWrite, param.DataType, param.LengthInBits,
                    true, out var returnedData, param.ArrayCount)) != DataConverter.DataError.NoError)
                return (int)dataError;
            return (int)DeviceHAL.WriteRecord((ushort)param.Index, (byte)param.Subindex, returnedData,
                out _, out _);
        }

        protected override int WriteCommand(Variable command) => WriteParam(command);
    }
}
