using System;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    class AnalogChannelConfigurationConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var properties = TypeDescriptor.GetProperties(value, true);
            return properties.Sort(new[] { "PhysicalChannel" });
        }
    }
}
