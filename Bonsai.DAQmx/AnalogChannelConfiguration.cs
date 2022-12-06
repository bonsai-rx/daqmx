using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [TypeConverter(typeof(AnalogChannelConfigurationConverter))]
    public abstract class AnalogChannelConfiguration
    {
        [Description("Specifies the name to assign to the local created virtual channel. If not specified, the physical channel name will be used.")]
        public string ChannelName { get; set; } = string.Empty;

        [Description("Specifies the minimum value to measure or generate, in the specified voltage units.")]
        public double MinimumValue { get; set; } = -10;

        [Description("Specifies the maximum value to measure or generate, in the specified voltage units.")]
        public double MaximumValue { get; set; } = 10;
    }
}
