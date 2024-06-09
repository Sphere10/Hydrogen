// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public enum FilterDataType {
	Date,
	Text,
	List,
	Number,
	Boolean
}


public enum FilterOrder {
	Ascending,
	Descending
}


public enum FilterOperator {
	None,
	Contains,
	StartsWith,
	EndsWith,
	In,
	Equals,
	NotEquals,
	GreaterThan,
	GreaterThanEqual,
	LessThan,
	LessThanEqual,
	Between
}


public class Filter<T> {

	private FilterOperator Operator { get; set; }
	private FilterOrder Order { get; set; }
	private FilterDataType DataType { get; set; }

	public Filter(FilterOperator op = FilterOperator.None, FilterOrder order = FilterOrder.Ascending) {
		var dataType = typeof(T).ToString();

		switch (dataType) {
			case "System.String":
			case "System.Char":
				DataType = FilterDataType.Text;
				break;

			case "System.Double":
			case "System.Decimal":

				break;

			case "System.DateTime":

				break;


		}
	}

	public bool DoFilter(object data, object filterValue1, object filterValue2 = null) {

		return true;
	}
}
