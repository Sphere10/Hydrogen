// -----------------------------------------------------------------------------------
// Use it as you please, but keep this header.
// Author : Marcus Deecke, 2006
// Web    : www.yaowi.com
// Email  : code@yaowi.com
// http://www.codeproject.com/KB/XML/deepserializer.aspx
// -----------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Diagnostics;

namespace Hydrogen.Data;

/// <summary>
/// Serializes arbitrary objects to XML.
/// </summary>
public class XmlDeepSerializer : IDisposable {

	#region Members

	private IXmlSerializationTag taglib = new XmlSerializationTag();
	private bool ignoreserialisationerrors = false;

	// All serialized objects are registered here
	private ArrayList objlist = new ArrayList();
	private bool ignoreserializableattribute = false;

	// If a type dictionary is used, store the Types here
	private Hashtable typedictionary = new Hashtable();

	// Usage of a type dictionary is optional, but it's advised
	private bool usetypedictionary = true;

	public const string TYPE_KEY_PREFIX = "TK";

	private Type serializationIgnoredAttributeType = null;

	#endregion Members

	#region Properties

	/// <summary>
	/// Gets or sets the attribute that, when applied to a property enable its serialization. If null every property is serialized.
	/// Even "Type" does not specialize the kind of Type it is obvious that only Attributes can be applied to properties.
	/// </summary>
	[Description("Gets or sets Attribute Type which marks a property to be ignored.")]
	public Type SerializationIgnoredAttributeType {
		get { return serializationIgnoredAttributeType; }
		set { serializationIgnoredAttributeType = value; }
	}

	/// <summary>
	/// Gets or sets whether errors during serialisation shall be ignored.
	/// </summary>
	[Description("Gets or sets whether errors during serialisation shall be ignored.")]
	public bool IgnoreSerialisationErrors {
		get { return ignoreserialisationerrors; }
		set { ignoreserialisationerrors = value; }
	}

	/// <summary>
	/// Gets or sets the dictionary of XML-tags.
	/// </summary>
	[Description("Gets or sets the dictionary of XML-tags.")]
	public IXmlSerializationTag TagLib {
		get { return taglib; }
		set { taglib = value; }
	}

	/// <summary>
	/// Gets or sets whether a type dictionary is used to store Type information.
	/// </summary>
	[Description("Gets or sets whether a type dictionary is used to store Type information.")]
	public bool UseTypeDictionary {
		get { return usetypedictionary; }
		set { usetypedictionary = value; }
	}

	/// <summary>
	/// Gets or sets whether the ISerializable Attribute is ignored.
	/// </summary>
	/// <remarks>
	/// Set this property only to true if you know about side effects.
	/// </remarks>
	[Description("Gets or sets whether the ISerializable Attribute is ignored.")]
	public bool IgnoreSerializableAttribute {
		get { return ignoreserializableattribute; }
		set { ignoreserializableattribute = value; }
	}

	#endregion Properties

	#region Serialize

	/// <summary>
	/// Serializes an Object to a file.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="filename"></param>
	/// <returns></returns>
	public void Serialize(object obj, string filename) {
		XmlDocument doc = Serialize(obj);
		doc.Save(filename);
	}


	/// <summary>
	/// Serializes an Object to a TextWriter.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="filename"></param>
	/// <returns></returns>
	public void Serialize(object obj, TextWriter textWriter) {
		XmlDocument doc = Serialize(obj);
		doc.Save(textWriter);
	}

	/// <summary>
	/// Serializes an Object to a new XmlDocument.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public XmlDocument Serialize(object obj) {
		XmlDocument doc = new XmlDocument();
		XmlDeclaration xd = doc.CreateXmlDeclaration("1.0", "utf-8", null);
		doc.AppendChild(xd);

		Serialize(obj, null, doc);

		return doc;
	}

	/// <summary>
	/// Serializes an Object and appends it to root (DocumentElement) of the specified XmlDocument.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="doc"></param>
	public void Serialize(object obj, String name, XmlDocument doc) {
		// Reset();

		XmlElement root = doc.CreateElement(taglib.OBJECT_TAG);

		XmlComment comment = root.OwnerDocument.CreateComment(" Data section : Don't edit any attributes ! ");
		root.AppendChild(comment);

		SetObjectInfoAttributes(name, obj.GetType(), root);

		if (doc.DocumentElement == null)
			doc.AppendChild(root);
		else
			doc.DocumentElement.AppendChild(root);

		Type ctortype = TypeInfo.GetBinaryConstructorType(obj.GetType());

		if (ctortype != null) {
			SerializeBinaryObject(obj, ctortype, root);
		} else {
			SerializeProperties(obj, root);
		}

		WriteTypeDictionary(root);
	}

	/// <summary>
	/// Serializes an Object and appends it to the specified XmlNode.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="parent"></param>
	public void Serialize(object obj, String name, XmlNode parent) {
		// Reset();

		XmlDocument doc = parent.OwnerDocument;

		XmlElement root = doc.CreateElement(taglib.OBJECT_TAG);
		parent.AppendChild(root);

		XmlComment comment = root.OwnerDocument.CreateComment(" Data section : Don't edit any attributes ! ");
		root.AppendChild(comment);

		SetObjectInfoAttributes(name, obj.GetType(), root);

		Type ctortype = TypeInfo.GetBinaryConstructorType(obj.GetType());

		if (ctortype != null) {
			SerializeBinaryObject(obj, ctortype, root);
		} else {
			SerializeProperties(obj, root);
		}

		WriteTypeDictionary(root);
	}

	#endregion Serialize

	#region ObjectInfo

	/// <summary>
	/// Returns an ObjectInfo filled with the values of Name, Type, and Assembly.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	private ObjectInfo GetObjectInfo(string name, Type type) {
		ObjectInfo oi = new ObjectInfo();

		oi.Name = name;
		oi.Type = type.FullName;
		oi.Assembly = type.Assembly.FullName;

		return oi;
	}

	/// <summary>
	/// Sets the property attributes of a Property to an XmlNode.
	/// </summary>
	/// <param name="propertyName"></param>
	/// <param name="type"></param>
	/// <param name="node"></param>
	private void SetObjectInfoAttributes(String propertyName, Type type, XmlNode node) {
		ObjectInfo objinfo = new ObjectInfo();

		GetObjectInfo(propertyName, type);

		if (type != null) {
			objinfo = GetObjectInfo(propertyName, type);
		}

		// Use of a TypeDictionary?
		if (usetypedictionary) {
			// TypeDictionary
			String typekey = GetTypeKey(type);

			XmlAttribute att = node.OwnerDocument.CreateAttribute(taglib.NAME_TAG);
			att.Value = objinfo.Name;
			node.Attributes.Append(att);

			att = node.OwnerDocument.CreateAttribute(taglib.TYPE_TAG);
			att.Value = typekey;
			node.Attributes.Append(att);

			// The assembly will be set, also, but it's always empty.
			att = node.OwnerDocument.CreateAttribute(taglib.ASSEMBLY_TAG);
			att.Value = "";
			node.Attributes.Append(att);
		} else {
			// No TypeDictionary
			XmlAttribute att = node.OwnerDocument.CreateAttribute(taglib.NAME_TAG);
			att.Value = objinfo.Name;
			node.Attributes.Append(att);

			att = node.OwnerDocument.CreateAttribute(taglib.TYPE_TAG);
			att.Value = objinfo.Type;
			node.Attributes.Append(att);

			att = node.OwnerDocument.CreateAttribute(taglib.ASSEMBLY_TAG);
			att.Value = objinfo.Assembly;
			node.Attributes.Append(att);
		}
	}

	#endregion ObjectInfo

	#region Properties

	/// <summary>
	/// Returns wether the Property has to be serialized or not (depending on SerializationIgnoredAttributeType).
	/// </summary>
	/// <param name="pi"></param>
	/// <returns></returns>
	protected bool CheckPropertyHasToBeSerialized(PropertyInfo pi) {
		if (serializationIgnoredAttributeType != null) {
			return pi.GetCustomAttributes(serializationIgnoredAttributeType, true).Length == 0;
		}
		return true;
	}

	/// <summary>
	/// Serializes the properties an Object and appends them to the specified XmlNode.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="parent"></param>
	protected void SerializeProperties(object obj, XmlNode parent) {
		if (TypeInfo.IsCollection(obj.GetType())) {
			SetCollectionItems(obj, (ICollection)obj, parent);
		} else {
			XmlElement node = parent.OwnerDocument.CreateElement(taglib.PROPERTIES_TAG);

			SetProperties(obj, node);

			parent.AppendChild(node);
		}
	}

	protected void SetProperties(object obj, XmlElement node) {
		PropertyInfo[] piarr = obj.GetType().GetProperties();
		Debug.Assert(piarr.Length > 0, "No property found to serialize for type " + obj.GetType().Name + "! Current implementation of Ellisys.Util.Serialization.XmlSerializer only work on public properties with get and set");
		for (int i = 0; i < piarr.Length; i++) {
			PropertyInfo pi = piarr[i];
			SetProperty(obj, pi, node);
		}
	}
	/// <summary>
	/// Sets a property.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="pi"></param>
	/// <param name="parent"></param>
	protected void SetProperty(object obj, PropertyInfo pi, XmlNode parent) {
		objlist.Add(obj);

		object val = pi.GetValue(obj, null);
		// If the value there's nothing to do
		if (val == null)
			return;

		// If the the value already exists in the list of processed objects/properties
		// ignore it o avoid circular calls.
		if (objlist.Contains(val))
			return;

		SetProperty(obj, val, pi, parent);
	}

	/// <summary>
	/// Sets a property.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="pi"></param>
	/// <param name="parent"></param>
	/// <remarks>
	/// This is the central method which is called recursivly!
	/// </remarks>
	protected void SetProperty(object obj, object value, PropertyInfo pi, XmlNode parent) {
		//      object val = value; ??

		try {


			// Empty values are ignored (no need to restore null references or empty Strings)
			if (value == null || value.Equals(""))
				return;

			// Get the Type
			//Type pt = pi.PropertyType;
			Type pt = value.GetType();

			// Check whether this property can be serialized and deserialized
			if (CheckPropertyHasToBeSerialized(pi) && (pt.IsSerializable || IgnoreSerializableAttribute) && (pi.CanWrite) && ((pt.IsPublic) || (pt.IsEnum))) {
				XmlElement prop = parent.OwnerDocument.CreateElement(taglib.PROPERTY_TAG);

				SetObjectInfoAttributes(pi.Name, pt, prop);

				// Try to find a constructor for binary data.
				// If found remember the parameter's Type.
				Type binctortype = TypeInfo.GetBinaryConstructorType(pt);

				if (binctortype != null) // a binary contructor was found
				{
					/*
					 * b. Trying to handle binary data
					 */

					SerializeBinaryObject(pi.GetValue(obj, null), binctortype, prop);
				} else if (TypeInfo.IsCollection(pt)) {
					/*
					 * a. Collections ask for a specific handling
					 */
					SetCollectionItems(obj, (ICollection)value, prop);
				} else {
					/*
					 * c. "normal" classes
					 */

					SetXmlElementFromBasicPropertyValue(prop, pt, value, parent);

				}
				// Append the property node to the paren XmlNode
				parent.AppendChild(prop);
			}
		} catch (Exception exc) {
			if (!IgnoreSerialisationErrors) {
				throw exc;
			} else {
				// perhaps logging
			}
		}
	}

	protected void SetXmlElementFromBasicPropertyValue(XmlElement prop, Type pt, object value, XmlNode parent) {

		// If possible, convert this property to a string
		if (pt.IsAssignableFrom(typeof(string))) {
			prop.InnerText = value.ToString();
			return;
		} else {
			TypeConverter tc = TypeDescriptor.GetConverter(pt);
			if (tc.CanConvertFrom(typeof(string)) && tc.CanConvertTo(typeof(string))) {
				prop.InnerText = (string)tc.ConvertTo(value, typeof(string));
				return;
			} else {
				bool complexclass = false; // Holds whether the propertys type is an complex type (the properties of objects have to be iterated, either)

				// Get all properties
				PropertyInfo[] piarr2 = pt.GetProperties();
				XmlElement proplist = null;
				Debug.Assert(piarr2.Length > 0, "No property found to serialize for type " + pt.Name + "! Current implementation of Ellisys.Util.Serialization.XmlSerializer only work on public properties with get and set");
				// Loop all properties
				for (int j = 0; j < piarr2.Length; j++) {
					PropertyInfo pi2 = piarr2[j];
					// Check whether this property can be serialized and deserialized
					if (CheckPropertyHasToBeSerialized(pi2) && (pi2.PropertyType.IsSerializable || IgnoreSerializableAttribute) && (pi2.CanWrite) && ((pi2.PropertyType.IsPublic) || (pi2.PropertyType.IsEnum))) {
						// Seems to be a complex type
						complexclass = true;

						// Add a properties parent node
						if (proplist == null) {
							proplist = parent.OwnerDocument.CreateElement(taglib.PROPERTIES_TAG);
							prop.AppendChild(proplist);
						}

						// Set the property (recursive call of this method!)
						SetProperty(value, pi2, proplist);
					}
				}

				// Ok, that was not a complex class either
				if (!complexclass) {
					// Converting to string was not possible, just set the value by ToString()
					prop.InnerText = value.ToString();
				}
			}
		}
	}

	/// <summary>
	/// Serializes binary data to a XmlNode.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="ctorParamType"></param>
	/// <param name="parent"></param>
	protected void SerializeBinaryObject(Object obj, Type ctorParamType, XmlNode parent) {
		XmlElement proplist = null;
		String val = null;

		try {
			// If the objact is a Stream or can be converted to a byte[]...
			TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
			if (tc.CanConvertTo(typeof(byte[])) || typeof(Stream).IsAssignableFrom(obj.GetType())) {
				byte[] barr = null;

				// Convert to byte[]
				if (typeof(Stream).IsAssignableFrom(obj.GetType())) {
					// Convert a Stream to byte[]
					BinaryContainerTypeConverter bctc = new BinaryContainerTypeConverter();
					barr = bctc.ConvertStreamToByteArray((Stream)obj);
				} else {
					// Convert the object to a byte[]
					barr = (byte[])tc.ConvertTo(obj, typeof(byte[]));
				}

				// Create a constructor node
				proplist = parent.OwnerDocument.CreateElement(taglib.CONSTRUCTOR_TAG);
				parent.AppendChild(proplist);

				// Set info about the constructor type as attributes  
				SetObjectInfoAttributes("0", ctorParamType, proplist);

				// Create a node for the binary data
				XmlNode bindata = proplist.OwnerDocument.CreateElement(taglib.BINARY_DATA_TAG);
				proplist.AppendChild(bindata);

				// Set info about the binary data type as attributes (currently it's always byte[]) 
				SetObjectInfoAttributes("0", typeof(byte[]), bindata);

				// Convert the byte array to a string so it's easy to store it in XML
				val = System.Convert.ToBase64String(barr, 0, barr.Length);

				bindata.InnerText = val.ToString();
			}
		} catch (Exception exc) {
			if (!IgnoreSerialisationErrors) {
				throw exc;
			} else {
				// perhaps logging
			}
		}
	}

	#endregion Properties

	#region SetCollectionItems

	/// <summary>
	/// Sets the items on a collection.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="pi"></param>
	/// <param name="parent"></param>
	/// <remarks>
	/// This method could be simplified since it's mainly the same code you can find in SetProperty()
	/// </remarks>
	protected void SetCollectionItems(object obj, ICollection value, XmlNode parent) {
		// Validating the parameters
		if (obj == null || value == null || parent == null)
			return;

		try {
			ICollection coll = value;

			XmlElement collnode = parent.OwnerDocument.CreateElement(taglib.ITEMS_TAG);
			parent.AppendChild(collnode);

			int cnt = 0;

			// What kind of Collection?
			if (TypeInfo.IsDictionary(coll.GetType())) {
				// IDictionary
				IDictionary dict = (IDictionary)coll;
				IDictionaryEnumerator de = dict.GetEnumerator();
				while (de.MoveNext()) {
					object key = de.Key;
					object val = de.Value;

					XmlElement itemnode = parent.OwnerDocument.CreateElement(taglib.ITEM_TAG);
					collnode.AppendChild(itemnode);

					object curr = de.Current;

					XmlElement propsnode = parent.OwnerDocument.CreateElement(taglib.PROPERTIES_TAG);
					itemnode.AppendChild(propsnode);

					SetProperties(curr, propsnode);
				}
			} else {
				// Everything else
				IEnumerator ie = coll.GetEnumerator();
				while (ie.MoveNext()) {
					object obj2 = ie.Current;

					XmlElement itemnode = parent.OwnerDocument.CreateElement(taglib.ITEM_TAG);

					if (obj2 != null) {
						SetObjectInfoAttributes(null, obj2.GetType(), itemnode);
					} else {
						SetObjectInfoAttributes(null, null, itemnode);
					}

					itemnode.Attributes[taglib.NAME_TAG].Value = "" + cnt;

					cnt++;

					collnode.AppendChild(itemnode);

					if (obj2 == null)
						continue;

					Type pt = obj2.GetType();

					if (TypeInfo.IsCollection(pt)) {
						SetCollectionItems(obj, (ICollection)obj2, itemnode);
					} else {
						SetXmlElementFromBasicPropertyValue(itemnode, pt, obj2, parent);
					} // IsCollection?
				} // Loop collection
			} // IsDictionary?
		} catch (Exception exc) {
			if (!IgnoreSerialisationErrors) {
				throw exc;
			} else {
				// perhaps logging
			}
		}
	}

	#endregion SetCollectionItems

	#region Misc

	/// <summary>
	/// Builds the Hashtable that will be written to XML as the type dictionary.<P>
	/// TODO: Why Hashtable? Better use a typesafe generic Dictionary. Maybe filesize can be decreased.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// While serialization the key of the type dictionary is the Type so it's easy to determine
	/// whether a Type is registered already. For deserialization the order is reverse: find a Type
	/// for a given key. 
	/// This methods creates a reversed Hashtable with the Types information stored in TypeInfo instances.
	/// </remarks>
	protected Hashtable BuildSerializeableTypeDictionary() {
		Hashtable ht = new Hashtable();

		IDictionaryEnumerator de = typedictionary.GetEnumerator();
		while (de.MoveNext()) {
			Type type = (Type)de.Entry.Key;
			string key = (String)de.Value;

			TypeInfo ti = new TypeInfo(type);
			ht.Add(key, ti);
		}
		return ht;
	}

	/// <summary>
	/// Gets the key of a Type from the type dictionary.
	/// <p>If the Type is not registered, yet, it will be registered here.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns>If the given Object is null, null, otherwise the Key of the Objects Type.</returns>
	protected string GetTypeKey(object obj) {
		if (obj == null)
			return null;

		return GetTypeKey(obj.GetType());
	}

	/// <summary>
	/// Gets the key of a Type from the type dictionary.
	/// <p>If the Type is not registered, yet, it will be registered here.
	/// </summary>
	/// <param name="type"></param>
	/// <returns>If the given Type is null, null, otherwise the Key of the Type.</returns>
	protected string GetTypeKey(Type type) {
		if (type == null)
			return null;

		if (!typedictionary.ContainsKey(type)) {
			typedictionary.Add(type, TYPE_KEY_PREFIX + typedictionary.Count);
		}

		return (String)typedictionary[type];
	}

	/// <summary>
	/// Writes the TypeDictionary to XML.
	/// </summary>
	/// <param name="parentNode"></param>
	protected void WriteTypeDictionary(XmlNode parentNode) {
		bool usedict = UseTypeDictionary;

		try {
			if (UseTypeDictionary) {
				XmlComment comment = parentNode.OwnerDocument.CreateComment(" TypeDictionary : Don't edit anything in this section at all ! ");
				parentNode.AppendChild(comment);

				XmlElement dictelem = parentNode.OwnerDocument.CreateElement(taglib.TYPE_DICTIONARY_TAG);
				parentNode.AppendChild(dictelem);
				Hashtable dict = BuildSerializeableTypeDictionary();

				// Temporary set UseTypeDictionary to false, otherwise TypeKeys instead of the
				// Type information will  be written
				UseTypeDictionary = false;

				SetObjectInfoAttributes(null, dict.GetType(), dictelem);
				SerializeProperties(dict, dictelem);

				// Reset UseTypeDictionary
				UseTypeDictionary = true;
			}
		} catch (Exception e) {
			throw e;
		} finally {
			UseTypeDictionary = usedict;
		}
	}

	/// <summary>
	/// Clears the Collections.
	/// </summary>
	public void Reset() {
		if (this.objlist != null)
			this.objlist.Clear();

		if (this.typedictionary != null)
			this.typedictionary.Clear();
	}

	/// <summary>
	/// Dispose, release references.
	/// </summary>
	public void Dispose() {
		Reset();
	}

	#endregion Misc

}
