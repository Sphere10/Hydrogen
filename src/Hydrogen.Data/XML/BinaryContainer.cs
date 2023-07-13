// -----------------------------------------------------------------------------------
// Use it as you please, but keep this header.
// Author : Marcus Deecke, 2006
// Web    : www.yaowi.com
// Email  : code@yaowi.com
// http://www.codeproject.com/KB/XML/deepserializer.aspx
// -----------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Hydrogen.Data;

/// <summary>
/// Container for binary data.
/// This class can be used to encapsulate binary data for serialization or transportation purposes.
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable]
[TypeConverter(typeof(BinaryContainerTypeConverter))]
public class BinaryContainer : ISerializable, IObjectReference {
	private byte[] _data = null;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="data"></param>
	public BinaryContainer(byte[] data) {
		this._data = data;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="data"></param>
	public BinaryContainer(Stream data) {
		var conv = new BinaryContainerTypeConverter();
		this._data = conv.ConvertStreamToByteArray(data);
	}

	/// <summary>
	/// Read only property returning the size (length) of the binary data.
	/// </summary>
	public int Size {
		get { return _data != null ? _data.Length : 0; }
	}

	/// <summary>
	/// Sets the data.
	/// </summary>
	/// <returns></returns>
	public byte[] GetData() {
		return _data;
	}

	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <param name="data"></param>
	public void SetData(byte[] data) {
		this._data = data;
	}

	/// <summary>
	/// <see cref="System.Runtime.Serialization.ISerializable.GetObjectData"/>
	/// </summary>
	/// <param name="info"></param>
	/// <param name="context"></param>
	public void GetObjectData(SerializationInfo info, StreamingContext context) {
		info.SetType(typeof(byte[]));
	}

	/// <summary>
	/// <see cref="System.Runtime.Serialization.IObjectReference.GetRealObject"/>
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	public object GetRealObject(StreamingContext context) {
		return this._data;
	}
}


/// <summary>
/// XmlBinaryContainer TypeConverter.<p>
/// Converts the <code>XmlBinaryContainer</code> to or from <code>byte[]</code> and <code>Stream</code>s.
/// </summary>
public class BinaryContainerTypeConverter : TypeConverter {
	/// <summary>
	/// <see cref="System.ComponentModel.TypeConverter.CanConvertFrom"/>
	/// </summary>
	/// <param name="context"></param>
	/// <param name="sourceType"></param>
	/// <returns></returns>
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
		return sourceType == typeof(byte[]) || base.CanConvertFrom(context, sourceType);
	}

	/// <summary>
	/// <see cref="System.ComponentModel.TypeConverter.ConvertFrom"/>
	/// </summary>
	/// <param name="context"></param>
	/// <param name="culture"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
		if (value is byte[]) {
			return new BinaryContainer((byte[])value);
		} else if (value is Stream) {
			return ConvertStreamToByteArray((Stream)value);
		}

		return base.ConvertFrom(context, culture, value);
	}

	/// <summary>
	/// Converts a <code>Stream</code> into <code>byte[]</code>.
	/// </summary>
	/// <param name="s"></param>
	/// <returns></returns>
	public byte[] ConvertStreamToByteArray(Stream s) {
		return s.ReadAll();
	}

	/// <summary>
	/// <see cref="System.ComponentModel.TypeConverter.CanConvertTo"/>
	/// </summary>
	/// <param name="context"></param>
	/// <param name="destinationType"></param>
	/// <returns></returns>
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
		if (destinationType == typeof(byte[])) {
			return true;
		}
		if (destinationType == typeof(Stream)) {
			return true;
		}

		return base.CanConvertTo(context, destinationType);
	}

	/// <summary>
	/// <see cref="System.ComponentModel.TypeConverter.ConvertTo"/>
	/// </summary>
	/// <param name="context"></param>
	/// <param name="culture"></param>
	/// <param name="value"></param>
	/// <param name="destinationType"></param>
	/// <returns></returns>
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
		if (destinationType == typeof(byte[])) {
			return ((BinaryContainer)value).GetData();
		}

		if (destinationType == typeof(Stream)) {
			return new MemoryStream(((BinaryContainer)value).GetData());
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}

}
