using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class DigitalOutputChannelConfiguration : DigitalChannelConfiguration
    {
        public DigitalOutputChannelConfiguration()
        {
            Lines = string.Empty;
        }

        [TypeConverter(typeof(DigitalOutputPhysicalChannelConverter))]
        public string Lines { get; set; }

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : Lines;
            if (string.IsNullOrEmpty(channelName)) return base.ToString();
            else return channelName;
        }
    }
}
