using System.ComponentModel;
using NationalInstruments.DAQmx;

namespace Bonsai.DAQmx
{
    class DigitalOutputPhysicalChannelConverter : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(DaqSystem.Local.GetPhysicalChannels(
                PhysicalChannelTypes.DOLine | PhysicalChannelTypes.DOPort,
                PhysicalChannelAccess.External));
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
