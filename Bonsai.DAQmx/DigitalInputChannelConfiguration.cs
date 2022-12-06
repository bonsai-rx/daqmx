using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class DigitalInputChannelConfiguration : DigitalChannelConfiguration
    {
        public DigitalInputChannelConfiguration()
        {
            Lines = string.Empty;
        }

        [TypeConverter(typeof(DigitalInputPhysicalChannelConverter))]
        public string Lines { get; set; }

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : Lines;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
