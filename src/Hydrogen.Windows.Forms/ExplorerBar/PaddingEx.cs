// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Reflection;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// Specifies the amount of space between the border and any contained 
/// items along each edge of an object
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable,
 TypeConverter(typeof(PaddingConverter))]
public class PaddingEx {

	#region Class Data

	/// <summary>
	/// Represents a Padding structure with its properties 
	/// left uninitialized
	/// </summary>
	[NonSerialized()] public static readonly PaddingEx Empty = new PaddingEx(0, 0, 0, 0);

	/// <summary>
	/// The width of the left padding
	/// </summary>
	private int left;

	/// <summary>
	/// The width of the right padding
	/// </summary>
	private int right;

	/// <summary>
	/// The width of the top padding
	/// </summary>
	private int top;

	/// <summary>
	/// The width of the bottom padding
	/// </summary>
	private int bottom;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the Padding class with default settings
	/// </summary>
	public PaddingEx()
		: this(0, 0, 0, 0) {

	}


	/// <summary>
	/// Initializes a new instance of the Padding class
	/// </summary>
	/// <param name="left">The width of the left padding value</param>
	/// <param name="top">The height of top padding value</param>
	/// <param name="right">The width of the right padding value</param>
	/// <param name="bottom">The height of bottom padding value</param>
	public PaddingEx(int left, int top, int right, int bottom) {
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Tests whether obj is a Padding structure with the same values as 
	/// this Padding structure
	/// </summary>
	/// <param name="obj">The Object to test</param>
	/// <returns>This method returns true if obj is a Padding structure 
	/// and its Left, Top, Right, and Bottom properties are equal to 
	/// the corresponding properties of this Padding structure; 
	/// otherwise, false</returns>
	public override bool Equals(object obj) {
		if (!(obj is PaddingEx)) {
			return false;
		}

		PaddingEx padding = (PaddingEx)obj;

		if (((padding.Left == this.Left) && (padding.Top == this.Top)) && (padding.Right == this.Right)) {
			return (padding.Bottom == this.Bottom);
		}

		return false;
	}


	/// <summary>
	/// Returns the hash code for this Padding structure
	/// </summary>
	/// <returns>An integer that represents the hashcode for this 
	/// padding</returns>
	public override int GetHashCode() {
		return (((this.Left ^ ((this.Top << 13) | (this.Top >> 0x13))) ^ ((this.Right << 0x1a) | (this.Right >> 6))) ^ ((this.Bottom << 7) | (this.Bottom >> 0x19)));
	}

	#endregion


	#region Properties

	/// <summary>
	/// Gets or sets the width of the left padding value
	/// </summary>
	public int Left {
		get { return this.left; }

		set {
			if (value < 0) {
				value = 0;
			}

			this.left = value;
		}
	}


	/// <summary>
	/// Gets or sets the width of the right padding value
	/// </summary>
	public int Right {
		get { return this.right; }

		set {
			if (value < 0) {
				value = 0;
			}

			this.right = value;
		}
	}


	/// <summary>
	/// Gets or sets the height of the top padding value
	/// </summary>
	public int Top {
		get { return this.top; }

		set {
			if (value < 0) {
				value = 0;
			}

			this.top = value;
		}
	}


	/// <summary>
	/// Gets or sets the height of the bottom padding value
	/// </summary>
	public int Bottom {
		get { return this.bottom; }

		set {
			if (value < 0) {
				value = 0;
			}

			this.bottom = value;
		}
	}


	/// <summary>
	/// Tests whether all numeric properties of this Padding have 
	/// values of zero
	/// </summary>
	[Browsable(false)]
	public bool IsEmpty {
		get {
			if (((this.Left == 0) && (this.Top == 0)) && (this.Right == 0)) {
				return (this.Bottom == 0);
			}

			return false;
		}
	}

	#endregion


	#region Operators

	/// <summary>
	/// Tests whether two Padding structures have equal Left, Top, 
	/// Right, and Bottom properties
	/// </summary>
	/// <param name="left">The Padding structure that is to the left 
	/// of the equality operator</param>
	/// <param name="right">The Padding structure that is to the right 
	/// of the equality operator</param>
	/// <returns>This operator returns true if the two Padding structures 
	/// have equal Left, Top, Right, and Bottom properties</returns>
	public static bool operator ==(PaddingEx left, PaddingEx right) {
		if (((left.Left == right.Left) && (left.Top == right.Top)) && (left.Right == right.Right)) {
			return (left.Bottom == right.Bottom);
		}

		return false;
	}


	/// <summary>
	/// Tests whether two Padding structures differ in their Left, Top, 
	/// Right, and Bottom properties
	/// </summary>
	/// <param name="left">The Padding structure that is to the left 
	/// of the equality operator</param>
	/// <param name="right">The Padding structure that is to the right 
	/// of the equality operator</param>
	/// <returns>This operator returns true if any of the Left, Top, Right, 
	/// and Bottom properties of the two Padding structures are unequal; 
	/// otherwise false</returns>
	public static bool operator !=(PaddingEx left, PaddingEx right) {
		return !(left == right);
	}

	#endregion

}
