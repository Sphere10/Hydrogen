//-----------------------------------------------------------------------
// <copyright file="SQLStatementType.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Hydrogen.Data {
	/// <summary>
	///  http://www.orafaq.com/faq/what_are_the_difference_between_ddl_dml_and_dcl_commands
	/// </summary>
    [Serializable]
	public enum SQLStatementType {
		DDL,		// Data Definition Language (i.e. create table, etc
		DML,		// Data Manipulation Language (i.e. select, insert, delete, etc)
		DCL,		// Data Control Language  (i.e. grant, revoke, etc)
		TCL			// Transaction Control Language (i.e. begin transaction, commit, rollback, etc)
	}
}
