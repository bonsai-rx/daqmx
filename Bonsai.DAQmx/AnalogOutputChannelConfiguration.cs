using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class AnalogOutputChannelConfiguration : AnalogChannelConfiguration
    {
        public AnalogOutputChannelConfiguration()
        {
            VoltageUnits = AOVoltageUnits.Volts;
        }

        [TypeConverter(typeof(AnalogOutputPhysicalChannelConverter))]
        public string PhysicalChannel { get; set; }

        public AOVoltageUnits VoltageUnits { get; set; }

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : PhysicalChannel;
            if (string.IsNullOrEmpty(channelName)) return base.ToString();
            else return channelName;
        }
    }
}
