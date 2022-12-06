using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class AnalogInputChannelConfiguration : AnalogChannelConfiguration
    {
        [TypeConverter(typeof(AnalogInputPhysicalChannelConverter))]
        [Description("Specifies the name of the physical channel used to create the local virtual channel.")]
        public string PhysicalChannel { get; set; }

        [Description("Specifies the terminal configuration for the channel.")]
        public AITerminalConfiguration TerminalConfiguration { get; set; } = AITerminalConfiguration.Differential;

        [Description("Specifies the units to use to return voltage measurements from the channel.")]
        public AIVoltageUnits VoltageUnits { get; set; } = AIVoltageUnits.Volts;

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : PhysicalChannel;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
