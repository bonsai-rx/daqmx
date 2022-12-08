using System.ComponentModel;
using NationalInstruments.DAQmx;

namespace Bonsai.DAQmx
{
    class DigitalInputPhysicalChannelConverter : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(DaqSystem.Local.GetPhysicalChannels(
                PhysicalChannelTypes.DILine | PhysicalChannelTypes.DIPort,
                PhysicalChannelAccess.External));
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
