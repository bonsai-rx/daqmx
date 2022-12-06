using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class DigitalOutputChannelConfiguration : DigitalChannelConfiguration
    {
        [TypeConverter(typeof(DigitalOutputPhysicalChannelConverter))]
        [Description("Specifies the names of the digital lines or ports to use to create the virtual channel.")]
        public string Lines { get; set; } = string.Empty;

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : Lines;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
