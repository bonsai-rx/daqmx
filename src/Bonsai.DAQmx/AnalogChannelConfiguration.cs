using System.ComponentModel;

namespace Bonsai.DAQmx
{
    /// <summary>
    /// Provides an abstract base class for configuration of virtual analog channels
    /// in DAQmx operators.
    /// </summary>
    [TypeConverter(typeof(AnalogChannelConfigurationConverter))]
    public abstract class AnalogChannelConfiguration
    {
        /// <summary>
        /// Gets or sets the name to assign to the local created virtual channel.
        /// If not specified, the physical channel name will be used.
        /// </summary>
        [Description("The name to assign to the local created virtual channel. If not specified, the physical channel name will be used.")]
        public string ChannelName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value specifying the minimum value to measure or generate,
        /// in the specified voltage units.
        /// </summary>
        [Description("Specifies the minimum value to measure or generate, in the specified voltage units.")]
        public double MinimumValue { get; set; } = -10;

        /// <summary>
        /// Gets or sets a value specifying the maximum value to measure or generate,
        /// in the specified voltage units.
        /// </summary>
        [Description("Specifies the maximum value to measure or generate, in the specified voltage units.")]
        public double MaximumValue { get; set; } = 10;
    }
}
