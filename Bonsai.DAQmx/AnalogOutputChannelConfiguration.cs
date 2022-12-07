using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    /// <summary>
    /// Represents the configuration of a virtual analog output channel in DAQmx operators.
    /// </summary>
    public class AnalogOutputChannelConfiguration : AnalogChannelConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the physical channel used to create the local
        /// virtual channel.
        /// </summary>
        [TypeConverter(typeof(AnalogOutputPhysicalChannelConverter))]
        [Description("The name of the physical channel used to create the local virtual channel.")]
        public string PhysicalChannel { get; set; }

        /// <summary>
        /// Gets or sets a value specifying in what units to generate voltage on the channel.
        /// </summary>
        [Description("Specifies in what units to generate voltage on the channel.")]
        public AOVoltageUnits VoltageUnits { get; set; } = AOVoltageUnits.Volts;

        /// <inheritdoc/>
        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : PhysicalChannel;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
