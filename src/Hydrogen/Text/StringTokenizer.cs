// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using System.Collections;

namespace Hydrogen;

/// <summary>
/// A class that breaks a string into tokens
/// </summary>
public class StringTokenizer {
	/// <summary>
	/// The index of the current token
	/// </summary>
	private int currentIndex;

	/// <summary>
	/// The number of tokens
	/// </summary>
	private int numberOfTokens;

	/// <summary>
	/// Internal list of tokens
	/// </summary>
	private ArrayList tokens;

	/// <summary>
	/// The string to be parsed
	/// </summary>
	private string source;

	/// <summary>
	/// The delimiters
	/// </summary>
	private string delimiter;


	/// <summary>
	/// Initializes a new instance of the StringTokenizer class with the 
	/// specified source string and delimiters
	/// </summary>
	/// <param name="source">The String to be parsed</param>
	/// <param name="delimiter">A String containing the delimiters</param>
	public StringTokenizer(string source, string delimiter) {
		this.tokens = new ArrayList(10);
		this.source = source;
		this.delimiter = delimiter;

		if (delimiter.Length == 0) {
			this.delimiter = " ";
		}

		this.Tokenize();
	}


	/// <summary>
	/// Initializes a new instance of the StringTokenizer class with the 
	/// specified source string and delimiters
	/// </summary>
	/// <param name="source">The String to be parsed</param>
	/// <param name="delimiter">A char array containing the delimiters</param>
	public StringTokenizer(string source, char[] delimiter)
		: this(source, new string(delimiter)) {

	}


	/// <summary>
	/// Initializes a new instance of the StringTokenizer class with the 
	/// specified source string
	/// </summary>
	/// <param name="source">The String to be parsed</param>
	public StringTokenizer(string source)
		: this(source, "") {

	}


	/// <summary>
	/// Parses the source string
	/// </summary>
	private void Tokenize() {
		string s = this.source;
		StringBuilder sb = new StringBuilder();
		this.numberOfTokens = 0;
		this.tokens.Clear();
		this.currentIndex = 0;

		int i = 0;

		while (i < this.source.Length) {
			if (this.delimiter.IndexOf(this.source[i]) != -1) {
				if (sb.Length > 0) {
					this.tokens.Add(sb.ToString());

					sb.Remove(0, sb.Length);
				}
			} else {
				sb.Append(this.source[i]);
			}

			i++;
		}

		this.numberOfTokens = this.tokens.Count;
	}


	/// <summary>
	/// Returns the number of tokens in the string
	/// </summary>
	/// <returns>The number of tokens in the string</returns>
	public int CountTokens() {
		return this.tokens.Count;
	}


	/// <summary>
	/// Checks if there are more tokens available from this tokenizer's 
	/// string
	/// </summary>
	/// <returns>true if more tokens are available, false otherwise</returns>
	public bool HasMoreTokens() {
		if (this.currentIndex <= (this.tokens.Count - 1)) {
			return true;
		} else {
			return false;
		}
	}


	/// <summary>
	/// Returns the current token and moves to the next token
	/// </summary>
	/// <returns>The current token</returns>
	public string NextToken() {
		string s = "";

		if (this.currentIndex <= (this.tokens.Count - 1)) {
			s = (string)tokens[this.currentIndex];

			this.currentIndex++;

			return s;
		} else {
			return null;
		}
	}


	/// <summary>
	/// Moves to the next token without returning the current token
	/// </summary>
	public void SkipToken() {
		if (this.currentIndex <= (this.tokens.Count - 1)) {
			this.currentIndex++;
		}
	}


	/// <summary>
	/// Returns the current token but does not move to the next token
	/// </summary>
	/// <returns></returns>
	public string PeekToken() {
		string s = "";

		if (this.currentIndex <= (this.tokens.Count - 1)) {
			s = (string)tokens[this.currentIndex];

			return s;
		} else {
			return null;
		}
	}


	/// <summary>
	/// Returns the source string
	/// </summary>
	public string Source {
		get { return this.source; }
	}


	/// <summary>
	/// Returns a string that contains the delimiters used to 
	/// parse the source string
	/// </summary>
	public string Delimiter {
		get { return this.delimiter; }
	}
}
