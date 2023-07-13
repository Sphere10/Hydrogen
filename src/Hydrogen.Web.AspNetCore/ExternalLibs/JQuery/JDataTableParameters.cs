// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Web.AspNetCore;

/// <summary>
/// Class that encapsulates most common parameters sent by DataTables plugin
///  http://www.codeproject.com/KB/aspnet/JQuery-DataTables-MVC.aspx
/// </summary>
[Obsolete]
public class JDataTableParameters {
	/// <summary>
	/// Request sequence number sent by DataTable, same value must be returned in response
	/// </summary>       
	public int sEcho { get; set; }

	/// <summary>
	/// Text used for filtering
	/// </summary>
	public string sSearch { get; set; }

	/// <summary>
	/// Number of records that should be shown in table
	/// </summary>
	public int iDisplayLength { get; set; }

	/// <summary>
	/// First record that should be shown(used for paging)
	/// </summary>
	public int iDisplayStart { get; set; }

	/// <summary>
	/// Number of columns in table
	/// </summary>
	public int iColumns { get; set; }


	/// <summary>
	/// The 0-based index of sorting column.
	/// </summary>
	public int iSortCol_0 { get; set; }

	public string sSortDir_0 { get; set; }

	/// <summary>
	/// Number of columns that are used in sorting
	/// </summary>
	public int iSortingCols { get; set; }

	/// <summary>
	/// Comma separated list of column names
	/// </summary>
	public string sColumns { get; set; }

}

// Capture
//type:1
//_:1300773073803
//sEcho:9
//iColumns:3
//sColumns:
//iDisplayStart:0
//iDisplayLength:10
//sSearch:
//bRegex:false
//sSearch_0:
//bRegex_0:false
//bSearchable_0:true
//sSearch_1:
//bRegex_1:false
//bSearchable_1:true
//sSearch_2:
//bRegex_2:false
//bSearchable_2:true
//iSortingCols:1
//iSortCol_0:1
//sSortDir_0:desc
//bSortable_0:true
//bSortable_1:true
//bSortable_2:true
