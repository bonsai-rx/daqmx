using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [TypeConverter(typeof(DigitalChannelConfigurationConverter))]
    public abstract class DigitalChannelConfiguration
    {
        [Description("Specifies the name of the virtual channel.")]
        public string ChannelName { get; set; } = string.Empty;

        [Description("Specifies how to group digital lines into one or more virtual channels.")]
        public ChannelLineGrouping Grouping { get; set; }
    }
}
