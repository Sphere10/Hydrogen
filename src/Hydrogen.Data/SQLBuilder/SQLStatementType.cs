// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

/// <summary>
///  http://www.orafaq.com/faq/what_are_the_difference_between_ddl_dml_and_dcl_commands
/// </summary>
[Serializable]
public enum SQLStatementType {
	DDL, // Data Definition Language (i.e. create table, etc
	DML, // Data Manipulation Language (i.e. select, insert, delete, etc)
	DCL, // Data Control Language  (i.e. grant, revoke, etc)
	TCL // Transaction Control Language (i.e. begin transaction, commit, rollback, etc)
}
