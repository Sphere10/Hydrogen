// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;
using Hydrogen;

namespace Hydrogen.Tests;

[TestFixture, Timeout(60000)]
public class MemoryObjectSpacesTest : ObjectSpacesTestBase<MemoryObjectSpace> {

	protected override MemoryObjectSpace CreateObjectSpace(ObjectSpaceBuilder builder) => CreateMemoryObjectSpace(new MemoryStream(), false, builder);

	protected virtual MemoryObjectSpace CreateMemoryObjectSpace(MemoryStream stream, bool keepStreamOnDispose, ObjectSpaceBuilder builder) {
		builder.UseMemoryStream(stream);
		var objectSpace = (MemoryObjectSpace)builder.Build();
		if (!keepStreamOnDispose)
			objectSpace.Disposables.Add(stream);
		return objectSpace;
	}

	#region Load

	[Test]
	public void LoadEmptyDoesntThrow() {
		using var stream = new MemoryStream();
		using (var objectSpace = CreateMemoryObjectSpace(stream, true, CreateStandardObjectSpace())) {
			objectSpace.Flush();
		}
		Assert.That(() => { using var _ = CreateMemoryObjectSpace(stream, false, CreateStandardObjectSpace()); }, Throws.Nothing);
	}

	[Test]
	public void LoadDoesntThrow() {
		using var stream = new MemoryStream();
		using (var objectSpace = CreateMemoryObjectSpace(stream, true, CreateStandardObjectSpace())) {
			var savedAccount = CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Flush();
		}
		Assert.That(() => { using var _ = CreateMemoryObjectSpace(stream, false, CreateStandardObjectSpace()); }, Throws.Nothing);
	}

	#endregion



	#region Clear

	[Test]
	public void ClearFlush() {
		using var objectSpace = CreateObjectSpace();
		var savedAccount = CreateAccount();
		objectSpace.Save(savedAccount);
		objectSpace.Clear();
		objectSpace.Flush();

		foreach(var dim in objectSpace.Dimensions)
			Assert.That(dim.ObjectStream.Count, Is.EqualTo(0));
	}

	#endregion


}
