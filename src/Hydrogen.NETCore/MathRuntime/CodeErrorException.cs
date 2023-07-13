// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Maths.Compiler;

public class CodeErrorException : CompilerException {
	private int _lineNumber;
	private int _startPosition;
	private int _endPosition;
	private int _lineNumberOffset;
	private int _positionOffset;

	public CodeErrorException(int lineNumber, int startPosition, int endPosition, string errMsg)
		: this(lineNumber, startPosition, endPosition, errMsg, null) {
	}

	public CodeErrorException(int lineNumber, int startPosition, int endPosition, string errMsg, Exception innerException)
		: base(errMsg, innerException) {
		_lineNumber = lineNumber;
		_startPosition = startPosition;
		_endPosition = endPosition;
		_lineNumberOffset = 0;
		_positionOffset = 0;
	}

	public virtual int LineNumber {
		get { return _lineNumber; }
		set { _lineNumber = value; }
	}

	public virtual int StartPosition {
		get { return _startPosition; }
		set { _startPosition = value; }
	}

	public virtual int EndPosition {
		get { return _endPosition; }
		set { _endPosition = value; }
	}

	/// <summary>
	/// When constructing error messages this can be used to offset the line number of reported error message.
	/// </summary>
	public virtual int LineNumberOffset {
		get { return _lineNumberOffset; }
		set { _lineNumberOffset = value; }
	}

	/// <summary>
	/// When constructing error messages this can be used to offset the line position of reported error message.
	/// </summary>
	public virtual int PositionOffset {
		get { return _positionOffset; }
		set { _positionOffset = value; }
	}

	public override string Message {
		get {
			return string.Format(
				"[Line: {0} Position: {1} To: {2}] {3}",
				LineNumber + LineNumberOffset,
				StartPosition + PositionOffset,
				EndPosition + PositionOffset,
				base.Message
			);
		}
	}


}
