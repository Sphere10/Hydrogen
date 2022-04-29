﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace Hydrogen {

    /// <summary>
    /// Extends the system <see cref="TypeDescriptor"/> class with custom type converters that were missing in .NET standard library
    /// </summary>
    public class TypeDescriptorEx {

        private static Dictionary<Type, TypeConverter> _customConverters = new() {
            { typeof(IPAddress), new IPAddressTypeConverter() }
            // TODO: add for byte[],
        };

        public static TypeConverter GetConverter(Type type) {
            if (_customConverters.TryGetValue(type, out var converter))
                return converter;
            return TypeDescriptor.GetConverter(type);
        }
    }
}
