using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [TypeConverter(typeof(AnalogChannelConfigurationConverter))]
    public abstract class AnalogChannelConfiguration
    {
        protected AnalogChannelConfiguration()
        {
            ChannelName = string.Empty;
            MinimumValue = -10;
            MaximumValue = 10;
        }

        public string ChannelName { get; set; }

        public double MinimumValue { get; set; }

        public double MaximumValue { get; set; }
    }
}
