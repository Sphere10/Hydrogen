// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms.TextBoxes;

public partial class TextLog : UserControl {
	private readonly ProducerConsumerQueue<char> _appendQueue;
	private readonly LargeCollection<char> _data;
	private readonly Throttle _throttle;
	public TextLog() {
		InitializeComponent();
		_appendQueue = new ProducerConsumerQueue<char>((c) => sizeof(char), 100000);
		_data = new LargeCollection<char>(65536, 1, (c) => sizeof(char));
		_throttle = new Throttle(2.0f);
	}

	public void AppendText() {

	}

	public int SelectionStart {
		get { throw new NotImplementedException(); }
		set { throw new NotImplementedException(); }
	}

	public int SelectionLength {
		get { throw new NotImplementedException(); }
		set { throw new NotImplementedException(); }
	}

	public string SelectedText {
		get { throw new NotImplementedException(); }
	}


}
