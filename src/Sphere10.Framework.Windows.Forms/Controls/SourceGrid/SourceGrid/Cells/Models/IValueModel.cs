//-----------------------------------------------------------------------
// <copyright file="IValueModel.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace SourceGrid.Cells.Models
{
	/// <summary>
	/// A Model interface specific to contain the value of the cell.
	/// </summary>
	public interface IValueModel : IModel
	{
		#region GetValue, SetValue
		/// <summary>
		/// Get the value of the cell at the specified position
		/// </summary>
		/// <param name="cellContext"></param>
		/// <returns></returns>
		object GetValue(CellContext cellContext);

		/// <summary>
		/// Set the value of the cell at the specified position. This method must call OnValueChanging and OnValueChanged() event.
		/// </summary>
		/// <param name="cellContext"></param>
		/// <param name="p_Value"></param>
		void SetValue(CellContext cellContext, object p_Value);
		#endregion
	}
}

