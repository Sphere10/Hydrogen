using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {

    /// <summary>
    /// Type converter for <see cref="IPAddress"/> which is missing in .NET. Use <see cref="TypeDescriptorEx"/> to activate.
    /// </summary>
    /// <remarks>Can lead to race-condition of instances are shared (they shouldn't be).</remarks>
    public class IPAddressTypeConverter : TypeConverter {
        private IPAddress _lastValidatedConversion;
        private string _lastValidatedValue;

        public override bool IsValid(ITypeDescriptorContext context, object value)
            => value is string valueString && Process(valueString);

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value != null && value is string valueString) {
                if (valueString == _lastValidatedValue)
                    return _lastValidatedConversion;
                return IPAddress.Parse(valueString);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                if (value == null)
                    return null;
                return ((IPAddress)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public bool Process(string value) {
            if (IPAddress.TryParse(value, out var ipAddress)) {
                _lastValidatedValue = value;
                _lastValidatedConversion = ipAddress;
                return true;
            }
            return false;
        }
    }
}
