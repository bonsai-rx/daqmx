using System.ComponentModel;

namespace Bonsai.DAQmx
{
    /// <summary>
    /// Represents the configuration of a virtual digital output channel in DAQmx operators.
    /// </summary>
    public class DigitalOutputChannelConfiguration : DigitalChannelConfiguration
    {
        /// <summary>
        /// Gets or sets the names of the digital lines or ports used to create
        /// the local virtual channel.
        /// </summary>
        [TypeConverter(typeof(DigitalOutputPhysicalChannelConverter))]
        [Description("The names of the digital lines or ports used to create the local virtual channel.")]
        public string Lines { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : Lines;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
