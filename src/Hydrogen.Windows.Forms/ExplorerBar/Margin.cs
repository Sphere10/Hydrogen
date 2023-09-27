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
/// Specifies the amount of space arouund an object along each side
/// </summary>
[Obfuscation(Exclude = true)]
[Serializable,
 TypeConverter(typeof(MarginConverter))]
public class Margin {

	#region Class Data

	/// <summary>
	/// Represents a Margin structure with its properties 
	/// left uninitialized
	/// </summary>
	[NonSerialized()] public static readonly Margin Empty = new Margin(0, 0, 0, 0);

	/// <summary>
	/// The width of the left margin
	/// </summary>
	private int left;

	/// <summary>
	/// The width of the right margin
	/// </summary>
	private int right;

	/// <summary>
	/// The width of the top margin
	/// </summary>
	private int top;

	/// <summary>
	/// The width of the bottom margin
	/// </summary>
	private int bottom;

	#endregion


	#region Constructor

	/// <summary>
	/// Initializes a new instance of the Margin class with default settings
	/// </summary>
	public Margin()
		: this(0, 0, 0, 0) {

	}


	/// <summary>
	/// Initializes a new instance of the Margin class
	/// </summary>
	/// <param name="left">The width of the left margin value</param>
	/// <param name="top">The height of the top margin value</param>
	/// <param name="right">The width of the right margin value</param>
	/// <param name="bottom">The height of the bottom margin value</param>
	public Margin(int left, int top, int right, int bottom) {
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	#endregion


	#region Methods

	/// <summary>
	/// Tests whether obj is a Margin structure with the same values as 
	/// this Border structure
	/// </summary>
	/// <param name="obj">The Object to test</param>
	/// <returns>This method returns true if obj is a Margin structure 
	/// and its Left, Top, Right, and Bottom properties are equal to 
	/// the corresponding properties of this Margin structure; 
	/// otherwise, false</returns>
	public override bool Equals(object obj) {
		if (!(obj is Margin)) {
			return false;
		}

		Margin margin = (Margin)obj;

		if (((margin.Left == this.Left) && (margin.Top == this.Top)) && (margin.Right == this.Right)) {
			return (margin.Bottom == this.Bottom);
		}

		return false;
	}


	/// <summary>
	/// Returns the hash code for this Margin structure
	/// </summary>
	/// <returns>An integer that represents the hashcode for this 
	/// margin</returns>
	public override int GetHashCode() {
		return (((this.Left ^ ((this.Top << 13) | (this.Top >> 0x13))) ^ ((this.Right << 0x1a) | (this.Right >> 6))) ^ ((this.Bottom << 7) | (this.Bottom >> 0x19)));
	}

	#endregion


	#region Properties

	/// <summary>
	/// Gets or sets the left margin value
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
	/// Gets or sets the right margin value
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
	/// Gets or sets the top margin value
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
	/// Gets or sets the bottom margin value
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
	/// Tests whether all numeric properties of this Margin have 
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
	/// Tests whether two Margin structures have equal Left, Top, 
	/// Right, and Bottom properties
	/// </summary>
	/// <param name="left">The Margin structure that is to the left 
	/// of the equality operator</param>
	/// <param name="right">The Margin structure that is to the right 
	/// of the equality operator</param>
	/// <returns>This operator returns true if the two Margin structures 
	/// have equal Left, Top, Right, and Bottom properties</returns>
	public static bool operator ==(Margin left, Margin right) {
		if (((left.Left == right.Left) && (left.Top == right.Top)) && (left.Right == right.Right)) {
			return (left.Bottom == right.Bottom);
		}

		return false;
	}


	/// <summary>
	/// Tests whether two Margin structures differ in their Left, Top, 
	/// Right, and Bottom properties
	/// </summary>
	/// <param name="left">The Margin structure that is to the left 
	/// of the equality operator</param>
	/// <param name="right">The Margin structure that is to the right 
	/// of the equality operator</param>
	/// <returns>This operator returns true if any of the Left, Top, Right, 
	/// and Bottom properties of the two Margin structures are unequal; 
	/// otherwise false</returns>
	public static bool operator !=(Margin left, Margin right) {
		return !(left == right);
	}

	#endregion

}
