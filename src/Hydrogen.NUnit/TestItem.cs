// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.NUnit;

public class TestItem<TInput, TExpected> {
	public TInput Input { get; set; }
	public TExpected Expected { get; set; }
}


public class TestItem<TInput1, TInput2, TExpected> {
	public TInput1 Input1 { get; set; }
	public TInput2 Input2 { get; set; }
	public TExpected Expected { get; set; }
}
