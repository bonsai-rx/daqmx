﻿using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [TypeConverter(typeof(DigitalChannelConfigurationConverter))]
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
