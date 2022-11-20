//-----------------------------------------------------------------------
// <copyright file="EmptyPingSource.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SourceGrid.Extensions.PingGrids
{
	public class ListPingSource<T> : List<T>, IPingData
	{
		public bool AllowSort {
			get {
				return true;
			}
			set {
			}
		}
		
		public void ApplySort(string propertyName, bool @ascending)
		{
			this.Sort();
		}
		
		public object GetItemValue(int index, string propertyName)
		{
			throw new NotImplementedException();
		}
		
	}
	
	public class EmptyPingSource : IPingData
	{
		public object GetItemValue(int index, string propertyName)
		{
			throw new NotImplementedException();
		}
		
		public int Count {
			get {
			return 0; 
			}
			set {
			}
		}
		
		public bool AllowSort {
			get {
				return false;
			}
			set {
			}
		}
		
		public void ApplySort(string propertyName, bool @ascending)
		{
			throw new NotImplementedException();
		}
	}
}
