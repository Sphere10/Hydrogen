// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

public enum SQLOperator {
	IsNull,
	IsNotNull,
	Equal,
	NotEqual,
	In,
	Like,
	Plus,
	Mul,
	Div,
	BitwiseOr,
	BitwiseAnd,
	BitwiseXor,
	Minus,
	BitwiseComplement,
	InnerJoin,
	LeftJoin,
	RightJoin,
	ColumnAlias,
	TableAlias,
	GreaterThan,
	GreaterThanEqualTo,
	LessThan,
	LessThanEqualTo,
	Exists
}
