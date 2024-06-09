// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;

namespace Hydrogen;

public class DeterministicSequentialGuidGenerator : ISequentialGuidGenerator {
	private readonly CHF _hashAlgorithm;
	private byte[] _lastHash;

	public DeterministicSequentialGuidGenerator() : this(Guid.NewGuid().ToByteArray()) {
	}

	public DeterministicSequentialGuidGenerator(byte[] seed) : this(seed, CHF.SHA2_256) {
	}

	public DeterministicSequentialGuidGenerator(byte[] seed, CHF hashAlgorithm)
		: this(seed, false, hashAlgorithm) {
	}

	public DeterministicSequentialGuidGenerator(byte[] seed, bool resumeLastGuid, CHF hashAlgorithm) {
		Guard.Argument(!resumeLastGuid || seed.Length == 32, nameof(seed), "Seed must be 32 byte hash when from last known guid");
		_lastHash = resumeLastGuid ? seed : Hashers.Hash(hashAlgorithm, seed);
		_hashAlgorithm = hashAlgorithm;
		Seed = seed;
	}

	public byte[] Seed { get; }

	public Guid CurrentGuid => new Guid(_lastHash.Take(16).ToArray());

	public Guid NextSequentialGuid() {
		_lastHash = Hashers.Hash(_hashAlgorithm, _lastHash);
		return CurrentGuid;
	}

}
