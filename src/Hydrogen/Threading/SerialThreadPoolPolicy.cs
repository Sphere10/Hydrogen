// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public enum SerialThreadPoolPolicy {
	/// <summary>
	/// When a thread is acquired, all queued items will be processed and then the thread is released.  This can hog a thread
	/// if the queue is added to faster than it is processed.
	/// </summary>
	Burst,

	/// <summary>
	/// When a thread is acquired, the next queued item is processed then it is released. The next item waits in the thread-pool to acquire an available thread.
	/// </summary>
	Intermittent

}
