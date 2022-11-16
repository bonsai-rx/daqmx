using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.DAQmx;
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
    }
}
