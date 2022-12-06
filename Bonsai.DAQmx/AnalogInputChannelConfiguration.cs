﻿using NationalInstruments.DAQmx;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    public class AnalogInputChannelConfiguration : AnalogChannelConfiguration
    {
        public AnalogInputChannelConfiguration()
        {
            TerminalConfiguration = AITerminalConfiguration.Differential;
            VoltageUnits = AIVoltageUnits.Volts;
        }

        [TypeConverter(typeof(AnalogInputPhysicalChannelConverter))]
        public string PhysicalChannel { get; set; }

        public AITerminalConfiguration TerminalConfiguration { get; set; }

        public AIVoltageUnits VoltageUnits { get; set; }

        public override string ToString()
        {
            var channelName = !string.IsNullOrEmpty(ChannelName) ? ChannelName : PhysicalChannel;
            return string.IsNullOrEmpty(channelName) ? GetType().Name : channelName;
        }
    }
}
