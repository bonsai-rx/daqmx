using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class AnalogOutputChannelConfiguration : AnalogChannelConfiguration
    {
        [TypeConverter(typeof(AnalogOutputPhysicalChannelConverter))]
        [Description("Specifies the name of the physical channel used to create the local virtual channel.")]
        public string PhysicalChannel { get; set; }

        [Description("Specifies in what units to generate voltage on the channel.")]
        public AOVoltageUnits VoltageUnits { get; set; } = AOVoltageUnits.Volts;

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : PhysicalChannel;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
