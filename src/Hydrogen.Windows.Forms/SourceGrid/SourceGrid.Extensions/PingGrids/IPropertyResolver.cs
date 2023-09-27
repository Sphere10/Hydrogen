// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Extensions.PingGrids;

/// <summary>
/// An extension point, where users could define their own 
/// property resolver
/// </summary>
public interface IPropertyResolver {
	object ReadValue(object obj, string propertyPath);
}
