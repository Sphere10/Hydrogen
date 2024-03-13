// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;


namespace Hydrogen.Tests;

[TestFixture, Timeout(60000)]
public class NonMerkleizedObjectSpacesTest : ObjectSpacesTestBase {

	protected override ObjectSpace CreateObjectSpace(string filePath) {
		var fileDefinition = BuildFileDefinition(filePath);
		var spaceDefinition = BuildSpaceDefinition();
		var serializerFactory = new SerializerFactory(SerializerFactory.Default);
		var comparerFactory = new ComparerFactory(ComparerFactory.Default);
		comparerFactory.RegisterEqualityComparer(CreateAccountComparer());
		return new ObjectSpace(fileDefinition, spaceDefinition, serializerFactory, comparerFactory);
	}
}
