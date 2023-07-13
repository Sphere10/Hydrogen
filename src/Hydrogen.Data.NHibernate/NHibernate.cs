// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


// ReSharper disable CheckNamespace

namespace Tools;

public static class NHibernate {

	public static int NVarCharMaxValue = 4001; // NHibernate has weird rule that defaults int.MaxValue -> 255, this works around it

}
