using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    /// <summary>
    /// Provides an abstract base class for configuration of virtual digital channels
    /// in DAQmx operators.
    /// </summary>
    [TypeConverter(typeof(DigitalChannelConfigurationConverter))]
    public abstract class DigitalChannelConfiguration
    {
        /// <summary>
        /// Gets or sets the name to assign to the local created virtual channel.
        /// If not specified, the physical channel name will be used.
        /// </summary>
        [Description("The name to assign to the local created virtual channel. If not specified, the physical channel name will be used.")]
        public string ChannelName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value specifying how to group digital lines into one or more virtual channels.
        /// </summary>
        [Description("Specifies how to group digital lines into one or more virtual channels.")]
        public ChannelLineGrouping Grouping { get; set; }
    }
}
