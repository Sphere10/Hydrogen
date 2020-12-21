//-----------------------------------------------------------------------
// <copyright file="jDataTable.cs" company="Sphere 10 Software">
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

using System.Collections.Generic;
using System.Linq;


namespace Sphere10.Framework.Html {

	public class jDataTable {

		public jDataTable(int sEcho, string[][] aaData)
			: this(sEcho, aaData.Length, aaData.Length, aaData, string.Empty) {
		}

		public jDataTable(int sEcho, string[][] aaData, int totalDataSetRecords)
			: this(sEcho, totalDataSetRecords, totalDataSetRecords, aaData, string.Empty) {
		}

		public jDataTable(int sEcho, string[][] aaData, string sColumns)
			: this(sEcho, aaData.Length, aaData.Length, aaData, sColumns) {
		}

		public jDataTable(int sEcho, int iTotalRecords, int iTotalDisplayRecords, string[][] aaData, string sColumns) {
			this.sEcho = sEcho;
			this.iTotalRecords = iTotalRecords;
			this.iTotalDisplayRecords = iTotalDisplayRecords;
			this.aaData = aaData;
			this.sColumns = sColumns;
		}

		/// <summary>
		/// Integer value that is used by DataTables for synchronization purpose. On each call sent to the server-side page, DataTables plugin sends the sequence number in sEcho parameter. Same value has to be returned in response because DataTables uses this for synchronization and matching requests and responses.
		/// </summary>
		public int sEcho { get; set; }

		/// <summary>
		/// DataTables plugin uses this value to determine how many pages will be required to generate pagination - if this value is less or equal than the current page size pagination buttons will be disabled. When user types in some keyword in a search text box DataTables shows “Showing 1 to 10 of 23 entries (filtered from 51 total entries)” message. In this case iTotalDisplayedRecords value returned in response equals 23.
		/// </summary>
		public int iTotalRecords { get; set; }

		/// <summary>
		/// Integer value that represents total unfiltered number of records that exist on the server-side and that might be displayed if no filter is applied. This value is used only for display purposes; when user types in some keyword in a search text box DataTables shows “Showing 1 to 10 of 23 entries (filtered from 51 total entries)” message. In this case iTotalRecords value returned in response equals 51.
		/// </summary>
		public int iTotalDisplayRecords { get; set; }

		/// <summary>
		/// Two-dimensional array that represents cell values that will be shown in table. When DataTables receives data it will populate table cells with values from the aaData array. Number of columns in two dimensional array must match the number of columns in the HTML table (even the hidden ones) and number of rows should be equal to the number of items that can be shown on the current page (e.g. 10, 25, 50 or 100 this value is selected in the "Show XXX entries" dropdown). 
		/// </summary>
		public string[][] aaData { get; set; }

		/// <summary>
		/// Comma separated list of column names
		/// </summary>
		public string sColumns { get; set; }

		public static jDataTable Parse(jDataTableParameters requestParams, IEnumerable<string[]> results) {
			if (requestParams.iSortingCols > 0) {
				if (requestParams.sSortDir_0 == "asc") {
					results = results.OrderBy(r => r[requestParams.iSortCol_0]);
				} else {
					results = results.OrderByDescending(r => r[requestParams.iSortCol_0]);
				}
			}
			var totalResults = results.Count();

			results = results
				.Skip(requestParams.iDisplayStart)
				.Take(requestParams.iDisplayLength);

			return new jDataTable(requestParams.sEcho, results.ToArray(), totalResults);
		}


		
	}

}
