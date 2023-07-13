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
/// Specifies the width of the border along each edge of an object
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable(),
 TypeConverter(typeof(BorderConverter))]
public class Border {

	#region Class Data

	/// <summary>
	/// Represents a Border structure with its properties 
	/// left uninitialized
	/// </summary>
	[NonSerialized()] public static readonly Border Empty = new Border(0, 0, 0, 0);

	/// <summary>
	/// The width of the left border
	/// </summary>
	private int left;

	/// <summary>
	/// The width of the right border
	/// </summary>
	private int right;

	/// <summary>
	/// The width of the top border
	/// </summary>
	private int top;

	/// <summary>
	/// The width of the bottom border
	/// </summary>
	private int bottom;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the Border class with default settings
	/// </summary>
	public Border()
		: this(0, 0, 0, 0) {

	}


	/// <summary>
	/// Initializes a new instance of the Border class
	/// </summary>
	/// <param name="left">The width of the left border</param>
	/// <param name="top">The Height of the top border</param>
	/// <param name="right">The width of the right border</param>
	/// <param name="bottom">The Height of the bottom border</param>
	public Border(int left, int top, int right, int bottom) {
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Tests whether obj is a Border structure with the same values as 
	/// this Border structure
	/// </summary>
	/// <param name="obj">The Object to test</param>
	/// <returns>This method returns true if obj is a Border structure 
	/// and its Left, Top, Right, and Bottom properties are equal to 
	/// the corresponding properties of this Border structure; 
	/// otherwise, false</returns>
	public override bool Equals(object obj) {
		if (!(obj is Border)) {
			return false;
		}

		Border border = (Border)obj;

		if (((border.Left == this.Left) && (border.Top == this.Top)) && (border.Right == this.Right)) {
			return (border.Bottom == this.Bottom);
		}

		return false;
	}


	/// <summary>
	/// Returns the hash code for this Border structure
	/// </summary>
	/// <returns>An integer that represents the hashcode for this 
	/// border</returns>
	public override int GetHashCode() {
		return (((this.Left ^ ((this.Top << 13) | (this.Top >> 0x13))) ^ ((this.Right << 0x1a) | (this.Right >> 6))) ^ ((this.Bottom << 7) | (this.Bottom >> 0x19)));
	}

	#endregion


	#region Properties

	/// <summary>
	/// Gets or sets the value of the left border
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
	/// Gets or sets the value of the right border
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
	/// Gets or sets the value of the top border
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
	/// Gets or sets the value of the bottom border
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
	/// Tests whether all numeric properties of this Border have 
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
	/// Tests whether two Border structures have equal Left, Top, 
	/// Right, and Bottom properties
	/// </summary>
	/// <param name="left">The Border structure that is to the left 
	/// of the equality operator</param>
	/// <param name="right">The Border structure that is to the right 
	/// of the equality operator</param>
	/// <returns>This operator returns true if the two Border structures 
	/// have equal Left, Top, Right, and Bottom properties</returns>
	public static bool operator ==(Border left, Border right) {
		if (((left.Left == right.Left) && (left.Top == right.Top)) && (left.Right == right.Right)) {
			return (left.Bottom == right.Bottom);
		}

		return false;
	}


	/// <summary>
	/// Tests whether two Border structures differ in their Left, Top, 
	/// Right, and Bottom properties
	/// </summary>
	/// <param name="left">The Border structure that is to the left 
	/// of the equality operator</param>
	/// <param name="right">The Border structure that is to the right 
	/// of the equality operator</param>
	/// <returns>This operator returns true if any of the Left, Top, Right, 
	/// and Bottom properties of the two Border structures are unequal; 
	/// otherwise false</returns>
	public static bool operator !=(Border left, Border right) {
		return !(left == right);
	}

	#endregion

}
