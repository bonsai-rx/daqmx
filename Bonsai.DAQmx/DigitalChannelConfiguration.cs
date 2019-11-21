using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [TypeConverter(typeof(AnalogChannelConfigurationConverter))]
    public abstract class DigitalChannelConfiguration
    {
        protected DigitalChannelConfiguration()
        {
            ChannelName = string.Empty;
        }

        public string ChannelName { get; set; }

        public ChannelLineGrouping Grouping { get; set; }
    }
}
