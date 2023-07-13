// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;

namespace DevAge.Drawing;

/// <summary>
/// A wrapper for the GDI Graphic instance. Contains also a set of class used to cache pens and brushes.
/// </summary>
public class GraphicsCache : IDisposable {
	public GraphicsCache(Graphics graphics) {
		mGraphics = graphics;
		mClipRectangle = Rectangle.Empty;
		mPensCache = new PensCache(20);
		mBrushsCache = new BrushsCache(20);
	}
	public GraphicsCache(Graphics graphics, Rectangle clipRectangle) {
		mGraphics = graphics;
		mClipRectangle = clipRectangle;
		mPensCache = new PensCache(20);
		mBrushsCache = new BrushsCache(20);
	}
	public GraphicsCache(Graphics graphics, Rectangle clipRectangle, int pensCapacity, int brushsCapacity) {
		mGraphics = graphics;
		mClipRectangle = clipRectangle;
		mPensCache = new PensCache(pensCapacity);
		mBrushsCache = new BrushsCache(brushsCapacity);
	}

	private Rectangle mClipRectangle;

	public Rectangle ClipRectangle {
		get { return mClipRectangle; }
	}

	private Graphics mGraphics;

	public Graphics Graphics {
		get { return mGraphics; }
	}

	private PensCache mPensCache;

	public PensCache PensCache {
		get { return mPensCache; }
	}

	private BrushsCache mBrushsCache;

	public BrushsCache BrushsCache {
		get { return mBrushsCache; }
	}

	#region IDisposable Members

	public void Dispose() {
		mPensCache.Dispose();
		mPensCache = null;
		mBrushsCache.Dispose();
		mBrushsCache = null;

		mGraphics = null;
	}

	#endregion

}
