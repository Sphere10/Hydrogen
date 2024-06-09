// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#define COMPARE_PRIVATE_MEMBERS
//#define COMPARE_ADO


#if SILVERLIGHT
#undef COMPARE_PRIVATE_MEMBERS
#endif

#if __IOS__
#undef COMPARE_PRIVATE_MEMBERS
#endif


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Hydrogen;

/// <summary>
/// Class that allows comparison of two objects of the same type to each other.  Supports classes, lists, arrays, dictionaries, child comparison and more.
/// </summary>
/// <example>
/// 
/// CompareObjects compareObjects = new CompareObjects();
/// 
/// Person person1 = new Person();
/// person1.DateCreated = DateTime.Now;
/// person1.Name = "Greg";
///
/// Person person2 = new Person();
/// person2.Name = "John";
/// person2.DateCreated = person1.DateCreated;
///
/// if (!compareObjects.Compare(person1, person2))
///    Console.WriteLine(compareObjects.DifferencesString);
/// 
/// </example>
public class DeepObjectComparer {

	#region Class Variables

	private readonly Stopwatch _watch = new Stopwatch();

	/// <summary>
	/// Keep track of parent objects in the object hiearchy
	/// </summary>
	private readonly Dictionary<int, int> _parents = new Dictionary<int, int>();

	/// <summary>
	/// Reflection Cache for property info
	/// </summary>
	private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();

	/// <summary>
	/// Reflection Cache for field info
	/// </summary>
	private readonly Dictionary<Type, FieldInfo[]> _fieldCache = new Dictionary<Type, FieldInfo[]>();

	/// <summary>
	/// Reflection Cache for methods
	/// </summary>
	private static readonly Dictionary<Type, MethodInfo[]> _methodList = new Dictionary<Type, MethodInfo[]>();

	#endregion

	#region Properties

	/// <summary>
	/// The amount of time in milliseconds it took for the comparison
	/// </summary>
	public long ElapsedMilliseconds {
		get { return _watch.ElapsedMilliseconds; }
	}

	/// <summary>
	/// Show breadcrumb at each stage of the comparision.  
	/// This is useful for debugging deep object graphs.
	/// The default is false
	/// </summary>
	public bool ShowBreadcrumb { get; set; }

	/// <summary>
	/// Ignore classes, properties, or fields by name during the comparison.
	/// Case sensitive.
	/// </summary>
	/// <example>ElementsToIgnore.Add("CreditCardNumber")</example>
	public List<string> ElementsToIgnore { get; set; }

	/// <summary>
	/// Only compare elements by name for classes, properties, and fields
	/// Case sensitive.
	/// </summary>
	/// <example>ElementsToInclude.Add("FirstName")</example>
	public List<string> ElementsToInclude { get; set; }

	//Security restriction in Silverlight prevents getting private properties and fields
#if COMPARE_PRIVATE_MEMBERS
	/// <summary>
	/// If true, private properties and fields will be compared. The default is false.  Silverlight and WinRT restricts access to private variables.
	/// </summary>
	public bool ComparePrivateProperties { get; set; }

	/// <summary>
	/// If true, private fields will be compared. The default is false.  Silverlight and WinRT restricts access to private variables.
	/// </summary>
	public bool ComparePrivateFields { get; set; }
#endif

	/// <summary>
	/// If true, static properties will be compared.  The default is true.
	/// </summary>
	public bool CompareStaticProperties { get; set; }

	/// <summary>
	/// If true, static fields will be compared.  The default is true.
	/// </summary>
	public bool CompareStaticFields { get; set; }

	/// <summary>
	/// If true, child objects will be compared. The default is true. 
	/// If false, and a list or array is compared list items will be compared but not their children.
	/// </summary>
	public bool CompareChildren { get; set; }

	/// <summary>
	/// If true, compare read only properties (only the getter is implemented).
	/// The default is true.
	/// </summary>
	public bool CompareReadOnly { get; set; }

	/// <summary>
	/// If true, compare fields of a class (see also CompareProperties).
	/// The default is true.
	/// </summary>
	public bool CompareFields { get; set; }

	/// <summary>
	/// If true, compare each item within a collection to every item in the other (warning, setting this to true significantly impacts performance).
	/// The default is false.
	/// </summary>
	public bool IgnoreCollectionOrder { get; set; }

	/// <summary>
	/// If true, compare properties of a class (see also CompareFields).
	/// The default is true.
	/// </summary>
	public bool CompareProperties { get; set; }

	/// <summary>
	/// The maximum number of differences to detect
	/// </summary>
	/// <remarks>
	/// Default is 1 for performance reasons.
	/// </remarks>
	public int MaxDifferences { get; set; }

	/// <summary>
	/// The differences found during the compare
	/// </summary>
	public List<Difference> Differences { get; set; }

	/// <summary>
	/// The differences found in a string suitable for a textbox
	/// </summary>
	public string DifferencesString {
		get {
			StringBuilder sb = new StringBuilder(4096);

			sb.AppendLine();
			sb.AppendLine("Begin Differences:");

			foreach (Difference item in Differences) {
				sb.AppendLine(item.ToString());
			}

			sb.AppendFormat("End Differences (Maximum of {0} differences shown).", MaxDifferences);

			return sb.ToString();
		}
	}

	/// <summary>
	/// Reflection properties and fields are cached. By default this cache is cleared after each compare.  Set to false to keep the cache for multiple compares.
	/// </summary>
	/// <seealso cref="Caching"/>
	/// <seealso cref="ClearCache"/>
	public bool AutoClearCache { get; set; }

	/// <summary>
	/// By default properties and fields for types are cached for each compare.  By default this cache is cleared after each compare.
	/// </summary>
	/// <seealso cref="AutoClearCache"/>
	/// <seealso cref="ClearCache"/>
	public bool Caching { get; set; }

	/// <summary>
	/// A list of attributes to ignore a class, property or field
	/// </summary>
	/// <example>AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));</example>
	public List<Type> AttributesToIgnore { get; set; }

	/// <summary>
	/// If true, objects will be compared ignore their type diferences
	/// </summary>
	public bool IgnoreObjectTypes { get; set; }

	/// <summary>
	/// Func that determine when use CustomComparer for comparing specific type.
	/// Default value return permanent false value.
	/// </summary>
	public Func<Type, bool> IsUseCustomTypeComparer { get; set; }

	/// <summary>
	/// Action that performed for comparing objects.
	/// T1: contain current CompareObjects
	/// T2: object1 for comparing
	/// T3: object1 for comparing
	/// T4: current CompareObjects breadcrumb
	/// </summary>
	public Action<DeepObjectComparer, object, object, string> CustomComparer { get; set; }

	/// <summary>
	/// In the differences string, this is the name for expected name, default is Expected 
	/// </summary>
	public string ExpectedName { get; set; }

	/// <summary>
	/// In the differences string, this is the name for the actual name, default is Actual
	/// </summary>
	public string ActualName { get; set; }

	#endregion

	#region Constructor

	/// <summary>
	/// Set up defaults for the comparison
	/// </summary>
	public DeepObjectComparer() {
		Setup();
	}

	private void Setup() {
		Differences = new List<Difference>();
		AttributesToIgnore = new List<Type>();
		CustomComparer = null;

		SetupWithoutReadingSettings();
	}

	private void SetupWithoutReadingSettings() {
		ElementsToIgnore = new List<string>();
		ElementsToInclude = new List<string>();
		CompareStaticFields = true;
		CompareStaticProperties = true;
#if COMPARE_PRIVATE_MEMBERS
		ComparePrivateProperties = true;
		ComparePrivateFields = true;
#endif
		CompareChildren = true;
		CompareReadOnly = true;
		CompareFields = true;
		IgnoreCollectionOrder = false;
		CompareProperties = true;
		Caching = true;
		AutoClearCache = true;
		IgnoreObjectTypes = false;
		MaxDifferences = 1;
		IsUseCustomTypeComparer = t => false;
		ExpectedName = "Expected";
		ActualName = "Actual";
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Compare two objects of the same type to each other.
	/// </summary>
	/// <remarks>
	/// Check the Differences or DifferencesString Properties for the differences.
	/// Default MaxDifferences is 1 for performance
	/// </remarks>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <returns>True if they are equal</returns>
	public bool Compare(object object1, object object2) {
		_watch.Reset();
		_watch.Start();
		string defaultBreadCrumb = string.Empty;

		Differences.Clear();
		Compare(object1, object2, defaultBreadCrumb);

		if (AutoClearCache)
			ClearCache();

		_watch.Stop();
		return Differences.Count == 0;
	}

	/// <summary>
	/// Reflection properties and fields are cached. By default this cache is cleared automatically after each compare.
	/// </summary>
	/// <seealso cref="AutoClearCache"/>
	/// <seealso cref="Caching"/>
	public void ClearCache() {
		_propertyCache.Clear();
		_fieldCache.Clear();
		_methodList.Clear();
	}

	#endregion

	#region Comparison Methods

	/// <summary>
	/// Compare two objects
	/// </summary>
	/// <param name="object1">The first object to compare</param>
	/// <param name="object2">The second object to compare</param>
	/// <param name="breadCrumb">Where we are in the object hiearchy</param>
	private void Compare(object object1, object object2, string breadCrumb) {
		//If both null return true
		if (object1 == null && object2 == null)
			return;

		//Check if one of them is null
		if (object1 == null) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = "(null)",
				Object2Value = NiceString(object2)
			};
			Differences.Add(difference);
			return;
		}

		if (object2 == null) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = NiceString(object1),
				Object2Value = "(null)"
			};
			Differences.Add(difference);
			return;
		}

		Type t1 = object1.GetType();
		Type t2 = object2.GetType();

		//Objects must be the same type
		if (t1 != t2 && !IgnoreObjectTypes) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = t1.FullName,
				Object2Value = t2.FullName,
				ChildPropertyName = "GetType()",
				MessagePrefix = "Different Types"
			};
			Differences.Add(difference);

			return;
		}

		if (IsUseCustomTypeComparer(t1)) {
			CompareWithCustomComparer(object1, object2, breadCrumb);
		} else if (IsTypeOfType(t1)) {
			CompareType(object1, object2, breadCrumb);
		} else if (IsIpEndPoint(t1)) {
			CompareIpEndPoint(object1, object2, breadCrumb);
		}
#if COMPARE_ADO
 else if (IsDataset(t1)) {
				CompareDataset(object1, object2, breadCrumb);
			} else if (IsDataTable(t1)) {
				CompareDataTable(object1, object2, breadCrumb);
			} else if (IsDataRow(t1)) {
				CompareDataRow(object1, object2, breadCrumb);
			}
#endif
		else if (IsIList(t1)) //This will do arrays, multi-dimensional arrays and generic lists
		{
			CompareIList(object1, object2, breadCrumb);
		} else if (IsHashSet(t1)) {
			CompareHashSet(object1, object2, breadCrumb);
		} else if (IsIDictionary(t1)) {
			CompareIDictionary(object1, object2, breadCrumb);
		} else if (IsEnum(t1)) {
			CompareEnum(object1, object2, breadCrumb);
		} else if (IsPointer(t1)) {
			ComparePointer(object1, object2, breadCrumb);
		} else if (IsUri(t1)) {
			CompareUri(object1, object2, breadCrumb);
		} else if (IsSimpleType(t1)) {
			CompareSimpleType(object1, object2, breadCrumb);
		} else if (IsClass(t1)) {
			CompareClass(object1, object2, breadCrumb);
		} else if (IsTimespan(t1)) {
			CompareTimespan(object1, object2, breadCrumb);
		} else if (IsStruct(t1)) {
			CompareStruct(object1, object2, breadCrumb);
		} else {
			throw new NotSupportedException("Cannot compare object of type " + t1.Name);
		}

	}

	private void CompareIpEndPoint(object object1, object object2, string breadCrumb) {
		IPEndPoint ipEndPoint1 = object1 as IPEndPoint;
		IPEndPoint ipEndPoint2 = object2 as IPEndPoint;

		//Null check happens above
		if (ipEndPoint1 == null || ipEndPoint2 == null)
			return;

		if (ipEndPoint1.Port != ipEndPoint2.Port) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = ipEndPoint1.Port.ToString(CultureInfo.InvariantCulture),
				Object2Value = ipEndPoint2.Port.ToString(CultureInfo.InvariantCulture),
				ChildPropertyName = "Port"
			};
			Differences.Add(difference);
		}

		if (Differences.Count >= MaxDifferences)
			return;

		if (ipEndPoint1.Address.ToString() != ipEndPoint2.Address.ToString()) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = ipEndPoint1.Address.ToString(),
				Object2Value = ipEndPoint2.Address.ToString(),
				ChildPropertyName = "Address"
			};
			Differences.Add(difference);
		}
	}

	/// <summary>
	/// Compare an object of type URI
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareUri(object object1, object object2, string breadCrumb) {
		Uri uri1 = object1 as Uri;
		Uri uri2 = object2 as Uri;

		//This should never happen, null check happens one level up
		if (uri1 == null || uri2 == null)
			return;

		if (uri1.OriginalString != uri2.OriginalString) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = NiceString(uri1.OriginalString),
				Object2Value = NiceString(uri2.OriginalString),
				ChildPropertyName = "OriginalString"
			};
			Differences.Add(difference);
		}
	}

	/// <summary>
	/// Compare an object of type Type (Runtime type)
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareType(object object1, object object2, string breadCrumb) {
		Type t1 = (Type)object1;
		Type t2 = (Type)object2;

		if (t1.FullName != t2.FullName) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = t1.FullName,
				Object2Value = t2.FullName,
				ChildPropertyName = "FullName"
			};
			Differences.Add(difference);
		}
	}

#if COMPARE_ADO
		/// <summary>
		/// Compare all columns in a data row
		/// </summary>
		/// <param name="object1"></param>
		/// <param name="object2"></param>
		/// <param name="breadCrumb"></param>
		private void CompareDataRow(object object1, object object2, string breadCrumb) {
			DataRow dataRow1 = object1 as DataRow;
			DataRow dataRow2 = object2 as DataRow;

			//This should never happen, null check happens one level up
			if (dataRow1 == null || dataRow2 == null)
				return;

			for (int i = 0; i < dataRow1.Table.Columns.Count; i++) {
				//Only compare specific column names
				if (ElementsToInclude.Count > 0 && !ElementsToInclude.Contains(dataRow1.Table.Columns[i].ColumnName))
					continue;

				//If we should ignore it, skip it
				if (ElementsToInclude.Count == 0 && ElementsToIgnore.Contains(dataRow1.Table.Columns[i].ColumnName))
					continue;

				//If we should ignore read only, skip it
				if (!CompareReadOnly && dataRow1.Table.Columns[i].ReadOnly)
					continue;

				//Both are null
				if (dataRow1.IsNull(i) && dataRow2.IsNull(i))
					continue;

				string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, dataRow1.Table.Columns[i].ColumnName);

				//Check if one of them is null
				if (dataRow1.IsNull(i)) {
					Difference difference = new Difference {
						ExpectedName = ExpectedName,
						ActualName = ActualName,
						PropertyName = currentBreadCrumb,
						Object1Value = "(null)",
						Object2Value = NiceString(object2)
					};
					Differences.Add(difference);
					return;
				}

				if (dataRow2.IsNull(i)) {
					Difference difference = new Difference {
						ExpectedName = ExpectedName,
						ActualName = ActualName,
						PropertyName = currentBreadCrumb,
						Object1Value = NiceString(object1),
						Object2Value = "(null)"
					};
					Differences.Add(difference);
					return;
				}

				//Check if one of them is deleted
				if (dataRow1.RowState == DataRowState.Deleted ^ dataRow2.RowState == DataRowState.Deleted) {
					Difference difference = new Difference {
						ExpectedName = ExpectedName,
						ActualName = ActualName,
						PropertyName = currentBreadCrumb,
						Object1Value = dataRow1.RowState.ToString(),
						Object2Value = dataRow2.RowState.ToString()
					};
					Differences.Add(difference);
					return;
				}

				Compare(dataRow1[i], dataRow2[i], currentBreadCrumb);

				if (Differences.Count >= MaxDifferences)
					return;
			}
		}

		/// <summary>
		/// Compare all rows in a data table
		/// </summary>
		/// <param name="object1"></param>
		/// <param name="object2"></param>
		/// <param name="breadCrumb"></param>
		private void CompareDataTable(object object1, object object2, string breadCrumb) {
			DataTable dataTable1 = object1 as DataTable;
			DataTable dataTable2 = object2 as DataTable;

			//This should never happen, null check happens one level up
			if (dataTable1 == null || dataTable2 == null)
				return;

			//Only compare specific table names
			if (ElementsToInclude.Count > 0 && !ElementsToInclude.Contains(dataTable1.TableName))
				return;

			//If we should ignore it, skip it
			if (ElementsToInclude.Count == 0 && ElementsToIgnore.Contains(dataTable1.TableName))
				return;

			//There must be the same amount of rows in the datatable
			if (dataTable1.Rows.Count != dataTable2.Rows.Count) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = breadCrumb,
					Object1Value = dataTable1.Rows.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = dataTable2.Rows.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Rows.Count"
				};
				Differences.Add(difference);

				if (Differences.Count >= MaxDifferences)
					return;
			}

			//There must be the same amount of columns in the datatable
			if (dataTable1.Columns.Count != dataTable2.Columns.Count) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = breadCrumb,
					Object1Value = dataTable1.Columns.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = dataTable2.Columns.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Columns.Count"
				};
				Differences.Add(difference);

				if (Differences.Count >= MaxDifferences)
					return;
			}

			for (int i = 0; i < Math.Min(dataTable1.Rows.Count, dataTable2.Rows.Count); i++) {
				string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Rows", string.Empty, i);

				CompareDataRow(dataTable1.Rows[i], dataTable2.Rows[i], currentBreadCrumb);

				if (Differences.Count >= MaxDifferences)
					return;
			}
		}

		/// <summary>
		/// Compare all tables and all rows in all tables
		/// </summary>
		/// <param name="object1"></param>
		/// <param name="object2"></param>
		/// <param name="breadCrumb"></param>
		private void CompareDataset(object object1, object object2, string breadCrumb) {
			DataSet dataSet1 = object1 as DataSet;
			DataSet dataSet2 = object2 as DataSet;

			//This should never happen, null check happens one level up
			if (dataSet1 == null || dataSet2 == null)
				return;

			//There must be the same amount of tables in the dataset
			if (dataSet1.Tables.Count != dataSet2.Tables.Count) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = breadCrumb,
					Object1Value = dataSet1.Tables.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = dataSet2.Tables.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Tables.Count"
				};
				Differences.Add(difference);

				if (Differences.Count >= MaxDifferences)
					return;
			}

			for (int i = 0; i < Math.Min(dataSet1.Tables.Count, dataSet2.Tables.Count); i++) {
				string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Tables", string.Empty, dataSet1.Tables[i].TableName);

				CompareDataTable(dataSet1.Tables[i], dataSet2.Tables[i], currentBreadCrumb);

				if (Differences.Count >= MaxDifferences)
					return;
			}
		}
#endif

	/// <summary>
	/// Compare a timespan struct
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareTimespan(object object1, object object2, string breadCrumb) {
		if (((TimeSpan)object1).Ticks != ((TimeSpan)object2).Ticks) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = ((TimeSpan)object1).Ticks.ToString(CultureInfo.InvariantCulture),
				Object2Value = ((TimeSpan)object1).Ticks.ToString(CultureInfo.InvariantCulture),
				ChildPropertyName = "Ticks"
			};
			Differences.Add(difference);
		}
	}

	/// <summary>
	/// Compare a pointer struct
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void ComparePointer(object object1, object object2, string breadCrumb) {
		if ((object1 is IntPtr && object2 is IntPtr && ((IntPtr)object1) != ((IntPtr)object2))
		    || (object1 is UIntPtr && object2 is UIntPtr && ((UIntPtr)object1) != ((UIntPtr)object2))) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb
			};
			Differences.Add(difference);
		}
	}

	/// <summary>
	/// Compare an enumeration
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareEnum(object object1, object object2, string breadCrumb) {
		if (object1.ToString() != object2.ToString()) {
			string currentBreadCrumb = AddBreadCrumb(breadCrumb, object1.GetType().Name, string.Empty, -1);

			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = currentBreadCrumb,
				Object1Value = object1.ToString(),
				Object2Value = object2.ToString()
			};
			Differences.Add(difference);
		}
	}

	/// <summary>
	/// Compare a simple type
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareSimpleType(object object1, object object2, string breadCrumb) {
		//This should never happen, null check happens one level up
		if (object2 == null || object1 == null)
			return;

		IComparable valOne = object1 as IComparable;

		if (valOne == null)
			throw new Exception("Expected value does not implement IComparable");

		if (valOne.CompareTo(object2) != 0) {
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = breadCrumb,
				Object1Value = object1.ToString(),
				Object2Value = object2.ToString()
			};
			Differences.Add(difference);
		}
	}

	/// <summary>
	/// Compare a struct
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareStruct(object object1, object object2, string breadCrumb) {
		try {
			AddParent(object1.GetHashCode());
			AddParent(object2.GetHashCode());

			Type t1 = object1.GetType();

			PerformCompareFields(t1, object1, object2, true, breadCrumb);
			PerformCompareProperties(t1, object1, object2, true, breadCrumb);
		} finally {
			RemoveParent(object1.GetHashCode());
			RemoveParent(object2.GetHashCode());
		}
	}

	/// <summary>
	/// Compare the properties, fields of a class
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareClass(object object1, object object2, string breadCrumb) {
		try {
			//Custom classes that implement IEnumerable may have the same hash code
			//Ignore objects with the same hash code
			if (!(object1 is IEnumerable)
			    && object1.GetHashCode() == object2.GetHashCode()) {
				return;
			}

			AddParent(object1.GetHashCode());
			AddParent(object2.GetHashCode());

			Type t1 = object1.GetType();

			//Only compare specific class names
			if (ElementsToInclude.Count > 0 && !ElementsToInclude.Contains(t1.Name))
				return;

			//We ignore the class name
			if ((ElementsToInclude.Count == 0 && ElementsToIgnore.Contains(t1.Name)) || IgnoredByAttribute(t1))
				return;

			//Compare the properties
			if (CompareProperties)
				PerformCompareProperties(t1, object1, object2, false, breadCrumb);

			//Compare the fields
			if (CompareFields)
				PerformCompareFields(t1, object1, object2, false, breadCrumb);
		} finally {
			RemoveParent(object1.GetHashCode());
			RemoveParent(object2.GetHashCode());
		}
	}

	private void CompareWithCustomComparer(object object1, object object2, string breadCrumb) {
		if (CustomComparer != null) {
			CustomComparer(this, object1, object2, breadCrumb);
		}
	}

	/// <summary>
	/// Compare the fields of a class
	/// </summary>
	/// <param name="t1"></param>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="structCompare"></param>
	/// <param name="breadCrumb"></param>
	private void PerformCompareFields(Type t1,
	                                  object object1,
	                                  object object2,
	                                  bool structCompare,
	                                  string breadCrumb) {
		IEnumerable<FieldInfo> currentFields = GetFieldInfo(t1);

		foreach (FieldInfo item in currentFields) {
			//Ignore invalid struct fields
			if (structCompare && !ValidStructSubType(item.FieldType))
				continue;

			//Skip if this is a shallow compare
			if (!CompareChildren && IsChildType(item.FieldType))
				continue;

			//Only compare specific field names
			if (ElementsToInclude.Count > 0 && !ElementsToInclude.Contains(item.Name))
				continue;

			//If we should ignore it, skip it
			if ((ElementsToInclude.Count == 0 && ElementsToIgnore.Contains(item.Name)) || IgnoredByAttribute(item))
				continue;

			object objectValue1 = item.GetValue(object1);
			object objectValue2 = item.GetValue(object2);

			bool object1IsParent = objectValue1 != null && (objectValue1 == object1 || _parents.ContainsKey(objectValue1.GetHashCode()));
			bool object2IsParent = objectValue2 != null && (objectValue2 == object2 || _parents.ContainsKey(objectValue2.GetHashCode()));

			//Skip fields that point to the parent
			if (IsClass(item.FieldType)
			    && (object1IsParent || object2IsParent)) {
				continue;
			}

			string currentCrumb = AddBreadCrumb(breadCrumb, item.Name, string.Empty, -1);

			Compare(objectValue1, objectValue2, currentCrumb);

			if (Differences.Count >= MaxDifferences)
				return;
		}
	}

	/// <summary>
	/// Get a list of the fields within a type
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private IEnumerable<FieldInfo> GetFieldInfo(Type type) {
		if (Caching && _fieldCache.ContainsKey(type))
			return _fieldCache[type];

		FieldInfo[] currentFields;

#if COMPARE_PRIVATE_MEMBERS
		if (ComparePrivateFields && !CompareStaticFields) {
			List<FieldInfo> list = new List<FieldInfo>();
			Type t = type;
			do {
				list.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
				t = t.BaseType;
			} while (t != null);
			currentFields = list.ToArray();
		} else if (ComparePrivateFields && CompareStaticFields) {
			List<FieldInfo> list = new List<FieldInfo>();
			Type t = type;
			do {
				list.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static));
				t = t.BaseType;
			} while (t != null);
			currentFields = list.ToArray();
		} else
#endif
			currentFields = type.GetFields(); //Default is public instance and static

		if (Caching)
			_fieldCache.Add(type, currentFields);

		return currentFields;
	}


	/// <summary>
	/// Compare the properties of a class
	/// </summary>
	/// <param name="t1"></param>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="structCompare"></param>
	/// <param name="breadCrumb"></param>
	private void PerformCompareProperties(Type t1,
	                                      object object1,
	                                      object object2,
	                                      bool structCompare,
	                                      string breadCrumb) {
		IEnumerable<PropertyInfo> currentProperties = GetPropertyInfo(t1);

		foreach (PropertyInfo info in currentProperties) {
			//Ignore invalid struct fields
			if (structCompare && !ValidStructSubType(info.PropertyType))
				continue;

			//If we can't read it, skip it
			if (info.CanRead == false)
				continue;

			//Skip if this is a shallow compare
			if (!CompareChildren && IsChildType(info.PropertyType))
				continue;

			//Only compare specific property names
			if (ElementsToInclude.Count > 0 && !ElementsToInclude.Contains(info.Name))
				continue;

			//If we should ignore it, skip it
			if ((ElementsToInclude.Count == 0 && ElementsToIgnore.Contains(info.Name)) || IgnoredByAttribute(info))
				continue;

			//If we should ignore read only, skip it
			if (!CompareReadOnly && info.CanWrite == false)
				continue;

			//If we ignore types then we must get correct PropertyInfo object
			PropertyInfo secondObjectInfo = null;
			if (IgnoreObjectTypes) {
				var secondObjectPropertyInfos = GetPropertyInfo(object2.GetType());

				foreach (var propertyInfo in secondObjectPropertyInfos) {
					if (propertyInfo.Name != info.Name) continue;

					secondObjectInfo = propertyInfo;
					break;
				}
			} else
				secondObjectInfo = info;

			object objectValue1;
			object objectValue2;
			if (!IsValidIndexer(info, breadCrumb)) {
				objectValue1 = info.GetValue(object1, null);
				objectValue2 = secondObjectInfo != null ? secondObjectInfo.GetValue(object2, null) : null;
			} else {
				CompareIndexer(info, object1, object2, breadCrumb);
				continue;
			}

			bool object1IsParent = objectValue1 != null && (objectValue1 == object1 || _parents.ContainsKey(objectValue1.GetHashCode()));
			bool object2IsParent = objectValue2 != null && (objectValue2 == object2 || _parents.ContainsKey(objectValue2.GetHashCode()));

			//Skip properties where both point to the corresponding parent
			if ((IsClass(info.PropertyType) || IsStruct(info.PropertyType)) && (object1IsParent && object2IsParent)) {
				continue;
			}

			string currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);

			Compare(objectValue1, objectValue2, currentCrumb);

			if (Differences.Count >= MaxDifferences)
				return;
		}
	}

	/// <summary>
	/// Get a list of the properties in a type
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private IEnumerable<PropertyInfo> GetPropertyInfo(Type type) {
		if (Caching && _propertyCache.ContainsKey(type))
			return _propertyCache[type];

		PropertyInfo[] currentProperties;

#if !COMPARE_PRIVATE_MEMBERS
            if (!CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            else
                currentProperties = type.GetProperties(); //Default is public instance and static
#else
		if (ComparePrivateProperties && !CompareStaticProperties)
			currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		else if (ComparePrivateProperties && CompareStaticProperties)
			currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
		else if (!CompareStaticProperties)
			currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
		else
			currentProperties = type.GetProperties(); //Default is public instance and static
#endif

		if (Caching)
			_propertyCache.Add(type, currentProperties);

		return currentProperties;
	}

	/// <summary>
	/// Compare an integer indexer
	/// </summary>
	/// <param name="info"></param>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareIndexer(PropertyInfo info, object object1, object object2, string breadCrumb) {
		string currentCrumb;
		int indexerCount1 = (int)info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object1, new object[] { });
		int indexerCount2 = (int)info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object2, new object[] { });

		//Indexers must be the same length
		if (indexerCount1 != indexerCount2) {
			currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
			Difference difference = new Difference {
				ExpectedName = ExpectedName,
				ActualName = ActualName,
				PropertyName = currentCrumb,
				Object1Value = indexerCount1.ToString(CultureInfo.InvariantCulture),
				Object2Value = indexerCount2.ToString(CultureInfo.InvariantCulture),
				ChildPropertyName = "Count"
			};
			Differences.Add(difference);

			if (Differences.Count >= MaxDifferences)
				return;
		}

		if (IgnoreCollectionOrder) {
			CompareIndexerIgnoreOrder(info, object1, object2, breadCrumb, indexerCount1, indexerCount2);
		} else {
			// Run on indexer
			for (int i = 0; i < indexerCount1; i++) {
				currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
				object objectValue1 = info.GetValue(object1, new object[] { i });
				object objectValue2 = info.GetValue(object2, new object[] { i });
				Compare(objectValue1, objectValue2, currentCrumb);

				if (Differences.Count >= MaxDifferences)
					return;
			}
		}
	}

	private void CompareIndexerIgnoreOrder(PropertyInfo info, object object1, object object2, string breadCrumb,
	                                       int indexerCount1, int indexerCount2) {
		int i = 0;

		while (i < indexerCount1) {
			string currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
			object objectValue1 = info.GetValue(object1, new object[] { i });

			var found = false;

			// expect order, but don't assume it.  this improves performance 
			// where the collections are ordered in sync (wittingly, or un-), but
			// requires some fancy logic to loop back from 0 to i
			var currentLimiter = indexerCount2;
			var j = (i >= indexerCount2) ? 0 : i;
			var loopedBack = false;
			bool needLoopBack = j > 0;

			while (j < currentLimiter) {
				var clone = Clone();
				clone.Differences = new List<Difference>();

				object objectValue2 = info.GetValue(object2, new object[] { j });
				clone.Compare(objectValue1, objectValue2, currentCrumb);

				if (clone.Differences.Count == 0) {
					found = true;
					break;
				}

				j++;
				if (needLoopBack && j == currentLimiter && !loopedBack) {
					j = 0;
					currentLimiter = i;
					loopedBack = true;
				}
			}

			if (!found) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = currentCrumb,
					Object1Value = NiceString(objectValue1),
					Object2Value = null,
					ChildPropertyName = "Item"
				};
				Differences.Add(difference);
			}

			if (Differences.Count >= MaxDifferences)
				return;

			i++;
		}
	}

	private DeepObjectComparer Clone() {
		return MemberwiseClone() as DeepObjectComparer;
	}

	/// <summary>
	/// Compare a dictionary
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareIDictionary(object object1, object object2, string breadCrumb) {
		IDictionary iDict1 = object1 as IDictionary;
		IDictionary iDict2 = object2 as IDictionary;

		//This should never happen, null check happens one level up
		if (iDict1 == null || iDict2 == null)
			return;

		try {
			AddParent(object1.GetHashCode());
			AddParent(object2.GetHashCode());

			//Objects must be the same length
			if (iDict1.Count != iDict2.Count) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = breadCrumb,
					Object1Value = iDict1.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = iDict2.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count"
				};
				Differences.Add(difference);

				if (Differences.Count >= MaxDifferences)
					return;
			}

			var enumerator1 = iDict1.GetEnumerator();
			var enumerator2 = iDict2.GetEnumerator();

			if (IgnoreCollectionOrder) {
				CompareDictionaryIgnoreOrder(breadCrumb, enumerator1, enumerator2);
			} else {
				while (enumerator1.MoveNext() && enumerator2.MoveNext()) {
					string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Key", string.Empty, -1);

					Compare(enumerator1.Key, enumerator2.Key, currentBreadCrumb);

					if (Differences.Count >= MaxDifferences)
						return;

					currentBreadCrumb = AddBreadCrumb(breadCrumb, "Value", string.Empty, -1);

					Compare(enumerator1.Value, enumerator2.Value, currentBreadCrumb);

					if (Differences.Count >= MaxDifferences)
						return;
				}
			}

		} finally {
			RemoveParent(object1.GetHashCode());
			RemoveParent(object2.GetHashCode());
		}
	}

	private void CompareDictionaryIgnoreOrder(string breadCrumb, IDictionaryEnumerator enumerator1,
	                                          IDictionaryEnumerator enumerator2) {
		var loopedBack = false;
		var needLoopBack = false;
		var enumerator2Index = 0;
		int? limit = null;

		while (enumerator1.MoveNext()) {
			//currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
			var found = false;

			// expect order, but don't assume it.  this improves performance 
			// where the collections are ordered in sync (wittingly, or un-), but
			// the logic is a bit different than the above index compare, 

			var movedNext = enumerator2.MoveNext();

			if (!movedNext) {
				enumerator2.Reset();
				movedNext = enumerator2.MoveNext();
				enumerator2Index = 0;
			}

			string currentBreadCrumb = string.Empty;

			while (movedNext) {
				var clone = Clone();
				clone.Differences = new List<Difference>();

				currentBreadCrumb = AddBreadCrumb(breadCrumb, "Key", string.Empty, -1);

				clone.Compare(enumerator1.Key, enumerator2.Key, currentBreadCrumb);

				if (clone.Differences.Count == 0) {
					currentBreadCrumb = AddBreadCrumb(breadCrumb, "Value", string.Empty, -1);

					clone.Compare(enumerator1.Value, enumerator2.Value, currentBreadCrumb);

					if (clone.Differences.Count == 0) {
						found = true;
						break;
					}
				}

				movedNext = enumerator2.MoveNext();
				enumerator2Index++;


				if (enumerator2Index == limit)
					break;

				if (!movedNext && needLoopBack && !loopedBack) {
					enumerator2.Reset();
					limit = enumerator2Index;
					enumerator2Index = 0;
					movedNext = enumerator2.MoveNext();
					loopedBack = true;
				}
			}

			if (!found) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = currentBreadCrumb,
					Object1Value = NiceString(enumerator1.Key),
					Object2Value = null,
					ChildPropertyName = "Item"
				};
				Differences.Add(difference);
			}

			if (Differences.Count >= MaxDifferences)
				return;

			needLoopBack = true;
		}
	}

	/// <summary>
	/// Compare an array or something that implements IList
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareIList(object object1, object object2, string breadCrumb) {
		IList ilist1 = object1 as IList;
		IList ilist2 = object2 as IList;

		//This should never happen, null check happens one level up
		if (ilist1 == null || ilist2 == null)
			return;

		try {
			AddParent(object1.GetHashCode());
			AddParent(object2.GetHashCode());

			//Objects must be the same length
			if (ilist1.Count != ilist2.Count) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = breadCrumb,
					Object1Value = ilist1.Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = ilist2.Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count"
				};
				Differences.Add(difference);

				if (Differences.Count >= MaxDifferences)
					return;
			}

			IEnumerator enumerator1 = ilist1.GetEnumerator();
			IEnumerator enumerator2 = ilist2.GetEnumerator();
			int count = 0;


			if (IgnoreCollectionOrder) {
				CompareEnumeratorIgnoreOrder(enumerator1, enumerator2, breadCrumb);
			} else {
				while (enumerator1.MoveNext() && enumerator2.MoveNext()) {
					string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, count);

					Compare(enumerator1.Current, enumerator2.Current, currentBreadCrumb);

					if (Differences.Count >= MaxDifferences)
						return;

					count++;
				}
			}
		} finally {
			RemoveParent(object1.GetHashCode());
			RemoveParent(object2.GetHashCode());
		}
	}

	private void CompareEnumeratorIgnoreOrder(IEnumerator enumerator1, IEnumerator enumerator2, string breadCrumb) {
		var loopedBack = false;
		var needLoopBack = false;
		var enumerator2Index = 0;
		int? limit = null;

		while (enumerator1.MoveNext()) {
			//currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
			var found = false;

			// expect order, but don't assume it.  this improves performance 
			// where the collections are ordered in sync (wittingly, or un-), but
			// the logic is a bit different than the above index compare,

			var movedNext = enumerator2.MoveNext();

			if (!movedNext) {
				enumerator2.Reset();
				movedNext = enumerator2.MoveNext();
				enumerator2Index = 0;
			}

			string currentBreadCrumb = string.Empty;

			while (movedNext) {
				var clone = Clone();
				clone.Differences = new List<Difference>();

				currentBreadCrumb = AddBreadCrumb(breadCrumb, "Current", string.Empty, -1);

				clone.Compare(enumerator1.Current, enumerator2.Current, currentBreadCrumb);

				if (clone.Differences.Count == 0) {
					found = true;
					break;
				}

				movedNext = enumerator2.MoveNext();
				enumerator2Index++;


				if (enumerator2Index == limit)
					break;

				if (!movedNext && needLoopBack && !loopedBack) {
					enumerator2.Reset();
					limit = enumerator2Index;
					enumerator2Index = 0;
					movedNext = enumerator2.MoveNext();
					loopedBack = true;
				}
			}

			if (!found) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = currentBreadCrumb,
					Object1Value = NiceString(enumerator1.Current),
					Object2Value = null,
					ChildPropertyName = "Item"
				};
				Differences.Add(difference);
			}

			if (Differences.Count >= MaxDifferences)
				return;

			needLoopBack = true;
		}
	}


	/// <summary>
	/// Compare a HashSet
	/// </summary>
	/// <param name="object1"></param>
	/// <param name="object2"></param>
	/// <param name="breadCrumb"></param>
	private void CompareHashSet(object object1, object object2, string breadCrumb) {
		try {
			AddParent(object1.GetHashCode());
			AddParent(object2.GetHashCode());

			Type t1 = object1.GetType();

			//Get count by reflection since we can't cast it to HashSet<>
			int hashSet1Count = (int)GetPropertyValue(t1, object1, "Count");
			int hashSet2Count = (int)GetPropertyValue(t1, object2, "Count");

			//Objects must be the same length
			if (hashSet1Count != hashSet2Count) {
				Difference difference = new Difference {
					ExpectedName = ExpectedName,
					ActualName = ActualName,
					PropertyName = breadCrumb,
					Object1Value = hashSet1Count.ToString(CultureInfo.InvariantCulture),
					Object2Value = hashSet2Count.ToString(CultureInfo.InvariantCulture),
					ChildPropertyName = "Count"
				};
				Differences.Add(difference);

				if (Differences.Count >= MaxDifferences)
					return;
			}

			//Get enumerators by reflection
			MethodInfo methodInfo = GetMethod(t1, "GetEnumerator");
			IEnumerator enumerator1 = (IEnumerator)methodInfo.Invoke(object1, null);
			IEnumerator enumerator2 = (IEnumerator)methodInfo.Invoke(object2, null);

			int count = 0;

			if (IgnoreCollectionOrder) {
				CompareEnumeratorIgnoreOrder(enumerator1, enumerator2, breadCrumb);
			} else {
				while (enumerator1.MoveNext() && enumerator2.MoveNext()) {
					string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, count);

					Compare(enumerator1.Current, enumerator2.Current, currentBreadCrumb);

					if (Differences.Count >= MaxDifferences)
						return;

					count++;
				}
			}
		} finally {
			RemoveParent(object1.GetHashCode());
			RemoveParent(object2.GetHashCode());
		}
	}

	#endregion

	#region IsType methods

	/// <summary>
	/// Returns true if the Type is a Runtime type
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private bool IsTypeOfType(Type type) {
		return (typeof(Type).IsAssignableFrom(type));
	}

	/// <summary>
	/// Check if any type has attributes that should be bypassed
	/// </summary>
	/// <returns></returns>
	private bool IgnoredByAttribute(MemberInfo info) {
		var attributes = info.GetCustomAttributes(true);

		return attributes.Any(a => AttributesToIgnore.Contains(a.GetType()));
	}

	private bool IsTimespan(Type type) {
		return type == typeof(TimeSpan);
	}

	private bool IsPointer(Type type) {
		return type == typeof(IntPtr) || type == typeof(UIntPtr);
	}

	private bool IsEnum(Type type) {
		return type.IsEnum;
	}

	private bool IsStruct(Type type) {
		return type.IsValueType && !IsSimpleType(type);
	}

	private bool IsSimpleType(Type type) {
		if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
			type = Nullable.GetUnderlyingType(type);
		}

		return type.IsPrimitive
		       || type == typeof(DateTime)
		       || type == typeof(decimal)
		       || type == typeof(string)
		       || type == typeof(Guid);

	}

	private bool ValidStructSubType(Type type) {
		return IsSimpleType(type)
		       || IsEnum(type)
		       || IsArray(type)
		       || IsClass(type)
		       || IsIDictionary(type)
		       || IsTimespan(type)
		       || IsIList(type);
	}

	private bool IsArray(Type type) {
		return type.IsArray;
	}

	private bool IsClass(Type type) {
		return type.IsClass;
	}

	private bool IsIDictionary(Type type) {
		return (typeof(IDictionary).IsAssignableFrom(type));
	}

#if COMPARE_ADO
		private bool IsDataset(Type type) {
			return type == typeof(DataSet);
		}

		private bool IsDataRow(Type type) {
			return type == typeof(DataRow);
		}

		private bool IsDataTable(Type type) {
			return type == typeof(DataTable);
		}
#endif

	private bool IsIpEndPoint(Type type) {
		return type == typeof(IPEndPoint);
	}

	private bool IsIList(Type type) {
		return (typeof(IList).IsAssignableFrom(type));
	}

	private bool IsChildType(Type type) {
		return !IsSimpleType(type)
		       && (IsClass(type)
		           || IsArray(type)
		           || IsIDictionary(type)
		           || IsIList(type)
		           || IsStruct(type)
		           || IsHashSet(type)
		       );
	}

	private bool IsUri(Type type) {
		return (typeof(Uri).IsAssignableFrom(type));
	}

	private bool IsHashSet(Type type) {
		return type.IsGenericType
		       && type.GetGenericTypeDefinition().Equals(typeof(HashSet<>));
	}

	#endregion

	#region Validity Checking

	private bool IsValidIndexer(PropertyInfo info, string breadCrumb) {
		ParameterInfo[] indexers = info.GetIndexParameters();

		if (indexers.Length == 0) {
			return false;
		}

		if (indexers.Length > 1) {
			throw new Exception("Cannot compare objects with more than one indexer for object " + breadCrumb);
		}

		if (indexers[0].ParameterType != typeof(Int32)) {
			throw new Exception("Cannot compare objects with a non integer indexer for object " + breadCrumb);
		}

		if (info.ReflectedType.GetProperty("Count") == null) {
			throw new Exception("Indexer must have a corresponding Count property for object " + breadCrumb);
		}

		if (info.ReflectedType.GetProperty("Count").PropertyType != typeof(Int32)) {
			throw new Exception("Indexer must have a corresponding Count property that is an integer for object " + breadCrumb);
		}

		return true;
	}

	#endregion

	#region Supporting Methods

	private void AddParent(int hash) {
		if (!_parents.ContainsKey(hash))
			_parents.Add(hash, hash);
	}

	private void RemoveParent(int hash) {
		if (_parents.ContainsKey(hash))
			_parents.Remove(hash);
	}

	/// <summary>
	/// Get the value of a property
	/// </summary>
	/// <param name="type"></param>
	/// <param name="objectValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	private object GetPropertyValue(Type type, object objectValue, string propertyName) {
		return GetPropertyInfo(type).First(o => o.Name == propertyName).GetValue(objectValue, null);
	}

	/// <summary>
	/// Get a method by name
	/// </summary>
	/// <param name="type"></param>
	/// <param name="methodName"></param>
	/// <returns></returns>
	private MethodInfo GetMethod(Type type, string methodName) {
		return GetMethods(type).FirstOrDefault(m => m.Name == methodName);
	}

	/// <summary>
	/// Get the cached methods for a type
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private IEnumerable<MethodInfo> GetMethods(Type type) {
		if (_methodList.ContainsKey(type))
			return _methodList[type];

		MethodInfo[] myMethodInfo = type.GetMethods();
		_methodList.Add(type, myMethodInfo);
		return myMethodInfo;
	}

	/// <summary>
	/// Convert an object to a nicely formatted string
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	private string NiceString(object obj) {
		try {
			if (obj == null)
				return "(null)";

			if (obj == DBNull.Value)
				return "System.DBNull.Value";

			return obj.ToString();
		} catch {
			return string.Empty;
		}
	}

	/// <summary>
	/// Add a breadcrumb to an existing breadcrumb
	/// </summary>
	/// <param name="existing"></param>
	/// <param name="name"></param>
	/// <param name="extra"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	private string AddBreadCrumb(string existing, string name, string extra, int index) {
		return AddBreadCrumb(existing, name, extra, index >= 0 ? index.ToString(CultureInfo.InvariantCulture) : null);
	}

	/// <summary>
	/// Add a breadcrumb to an existing breadcrumb
	/// </summary>
	/// <param name="existing"></param>
	/// <param name="name"></param>
	/// <param name="extra"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	private string AddBreadCrumb(string existing, string name, string extra, string index) {
		bool useIndex = !String.IsNullOrEmpty(index);
		bool useName = name.Length > 0;
		StringBuilder sb = new StringBuilder();

		sb.Append(existing);

		if (useName) {
			sb.AppendFormat(".");
			sb.Append(name);
		}

		sb.Append(extra);

		if (useIndex) {
			// ReSharper disable RedundantAssignment
			int result = -1;
			// ReSharper restore RedundantAssignment
			sb.AppendFormat(Int32.TryParse(index, out result) ? "[{0}]" : "[\"{0}\"]", index);
		}

		if (ShowBreadcrumb)
			Debug.WriteLine(sb.ToString());

		return sb.ToString();
	}

	#endregion

	#region IEqualityComparer<object> comparer

	public bool Equals(object x, object y) {
		return this.Compare(x, y);
	}

	public int GetHashCode(object obj) {
		return obj.GetHashCode();
	}

	#endregion


	/// <summary>
	/// Detailed information about the difference
	/// </summary>
	public class Difference {
		/// <summary>
		/// Name of Expected Object
		/// </summary>
		public string ExpectedName { get; set; }

		/// <summary>
		/// Name of Actual Object
		/// </summary>
		public string ActualName { get; set; }

		/// <summary>
		/// The breadcrumb of the property leading up to the value
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// The child property name
		/// </summary>
		public string ChildPropertyName { get; set; }

		/// <summary>
		/// Object1 Value
		/// </summary>
		public string Object1Value { get; set; }

		/// <summary>
		/// Object2 Value
		/// </summary>
		public string Object2Value { get; set; }

		/// <summary>
		/// Prefix to put on the beginning of the message
		/// </summary>
		public string MessagePrefix { get; set; }

		/// <summary>
		/// Nicely formatted string
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			string message;

			if (!String.IsNullOrEmpty(PropertyName)) {
				if (String.IsNullOrEmpty(ChildPropertyName))
					message = String.Format("{0}.{2} != {1}.{2} ({3},{4})", ExpectedName, ActualName, PropertyName, Object1Value, Object2Value);
				else
					message = String.Format("{0}.{2}.{5} != {1}.{2}.{5} ({3},{4})", ExpectedName, ActualName, PropertyName, Object1Value, Object2Value, ChildPropertyName);
			} else {
				message = String.Format("{0} != {1} ({2},{3})", ExpectedName, ActualName, Object1Value, Object2Value);
			}

			if (!String.IsNullOrEmpty(MessagePrefix))
				message = String.Format("{0}: {1}", MessagePrefix, message);

			return message;
		}
	}
}
