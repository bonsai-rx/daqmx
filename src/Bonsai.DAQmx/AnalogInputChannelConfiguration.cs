using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    /// <summary>
    /// Represents the configuration of a virtual analog input channel in DAQmx operators.
    /// </summary>
    public class AnalogInputChannelConfiguration : AnalogChannelConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the physical channel used to create the local
        /// virtual channel.
        /// </summary>
        [TypeConverter(typeof(AnalogInputPhysicalChannelConverter))]
        [Description("The name of the physical channel used to create the local virtual channel.")]
        public string PhysicalChannel { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the terminal configuration for the channel.
        /// </summary>
        [Description("Specifies the terminal configuration for the channel.")]
        public AITerminalConfiguration TerminalConfiguration { get; set; } = AITerminalConfiguration.Differential;

        /// <summary>
        /// Gets or sets a value specifying the units used to return voltage
        /// measurements from the channel.
        /// </summary>
        [Description("Specifies the units used to return voltage measurements from the channel.")]
        public AIVoltageUnits VoltageUnits { get; set; } = AIVoltageUnits.Volts;

        /// <inheritdoc/>
        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : PhysicalChannel;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
