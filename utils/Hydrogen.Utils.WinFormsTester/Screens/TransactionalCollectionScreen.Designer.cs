namespace Hydrogen.Utils.WinFormsTester {
	partial class TransactionalCollectionScreen {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this._listAppendTestButton = new System.Windows.Forms.Button();
            this._testsGroupBox = new System.Windows.Forms.GroupBox();
            this._bufferTestButton = new System.Windows.Forms.Button();
            this._dictionaryAppend = new System.Windows.Forms.Button();
            this._storageStreamButton = new System.Windows.Forms.Button();
            this._outputGroupBox = new System.Windows.Forms.GroupBox();
            this._copyButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._outputTextBox = new System.Windows.Forms.TextBox();
            this.mergeableMenuStrip1 = new Hydrogen.Windows.Forms.MergeableMenuStrip();
            this._configGroupBox = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this._listTypeComboBox = new Hydrogen.Windows.Forms.EnumComboBox();
            this._batchIntBox = new Hydrogen.Windows.Forms.IntBox();
            this.label6 = new System.Windows.Forms.Label();
            this._commitCheckBox = new System.Windows.Forms.CheckBox();
            this._itemSizeIntBox = new Hydrogen.Windows.Forms.IntBox();
            this.label5 = new System.Windows.Forms.Label();
            this._cacheSizeIntBox = new Hydrogen.Windows.Forms.IntBox();
            this.label4 = new System.Windows.Forms.Label();
            this._pageSizeIntBox = new Hydrogen.Windows.Forms.IntBox();
            this.label3 = new System.Windows.Forms.Label();
            this._clusterSizeIntBox = new Hydrogen.Windows.Forms.IntBox();
            this.label2 = new System.Windows.Forms.Label();
            this._itemsIntBox = new Hydrogen.Windows.Forms.IntBox();
            this.label1 = new System.Windows.Forms.Label();
            this._policyBox = new Hydrogen.Windows.Forms.FlagsCheckedListBox();
            this._testsGroupBox.SuspendLayout();
            this._outputGroupBox.SuspendLayout();
            this._configGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _listAppendTestButton
            // 
            this._listAppendTestButton.Location = new System.Drawing.Point(10, 37);
            this._listAppendTestButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._listAppendTestButton.Name = "_listAppendTestButton";
            this._listAppendTestButton.Size = new System.Drawing.Size(164, 45);
            this._listAppendTestButton.TabIndex = 0;
            this._listAppendTestButton.Text = "List Append";
            this._listAppendTestButton.UseVisualStyleBackColor = true;
            this._listAppendTestButton.Click += new System.EventHandler(this._appendTestButton_Click);
            // 
            // _testsGroupBox
            // 
            this._testsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._testsGroupBox.Controls.Add(this._bufferTestButton);
            this._testsGroupBox.Controls.Add(this._dictionaryAppend);
            this._testsGroupBox.Controls.Add(this._storageStreamButton);
            this._testsGroupBox.Controls.Add(this._listAppendTestButton);
            this._testsGroupBox.Location = new System.Drawing.Point(6, 1009);
            this._testsGroupBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._testsGroupBox.Name = "_testsGroupBox";
            this._testsGroupBox.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._testsGroupBox.Size = new System.Drawing.Size(1949, 100);
            this._testsGroupBox.TabIndex = 1;
            this._testsGroupBox.TabStop = false;
            this._testsGroupBox.Text = "Tests";
            // 
            // _bufferTestButton
            // 
            this._bufferTestButton.Location = new System.Drawing.Point(584, 37);
            this._bufferTestButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._bufferTestButton.Name = "_bufferTestButton";
            this._bufferTestButton.Size = new System.Drawing.Size(164, 45);
            this._bufferTestButton.TabIndex = 3;
            this._bufferTestButton.Text = "Simple List";
            this._bufferTestButton.UseVisualStyleBackColor = true;
            this._bufferTestButton.Click += new System.EventHandler(this._bufferTestButton_Click);
            // 
            // _dictionaryAppend
            // 
            this._dictionaryAppend.Location = new System.Drawing.Point(361, 37);
            this._dictionaryAppend.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._dictionaryAppend.Name = "_dictionaryAppend";
            this._dictionaryAppend.Size = new System.Drawing.Size(214, 45);
            this._dictionaryAppend.TabIndex = 2;
            this._dictionaryAppend.Text = "Dictionary Append";
            this._dictionaryAppend.UseVisualStyleBackColor = true;
            this._dictionaryAppend.Click += new System.EventHandler(this._dictionaryAppend_Click);
            // 
            // _storageStreamButton
            // 
            this._storageStreamButton.Location = new System.Drawing.Point(186, 37);
            this._storageStreamButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._storageStreamButton.Name = "_storageStreamButton";
            this._storageStreamButton.Size = new System.Drawing.Size(164, 45);
            this._storageStreamButton.TabIndex = 1;
            this._storageStreamButton.Text = "Stream Append";
            this._storageStreamButton.UseVisualStyleBackColor = true;
            this._storageStreamButton.Click += new System.EventHandler(this._streamButton_Click);
            // 
            // _outputGroupBox
            // 
            this._outputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._outputGroupBox.Controls.Add(this._copyButton);
            this._outputGroupBox.Controls.Add(this._clearButton);
            this._outputGroupBox.Controls.Add(this._outputTextBox);
            this._outputGroupBox.Controls.Add(this.mergeableMenuStrip1);
            this._outputGroupBox.Location = new System.Drawing.Point(6, 369);
            this._outputGroupBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._outputGroupBox.Name = "_outputGroupBox";
            this._outputGroupBox.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._outputGroupBox.Size = new System.Drawing.Size(1949, 629);
            this._outputGroupBox.TabIndex = 2;
            this._outputGroupBox.TabStop = false;
            this._outputGroupBox.Text = "Output";
            // 
            // _copyButton
            // 
            this._copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._copyButton.Location = new System.Drawing.Point(1840, 581);
            this._copyButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._copyButton.Name = "_copyButton";
            this._copyButton.Size = new System.Drawing.Size(44, 38);
            this._copyButton.TabIndex = 3;
            this._copyButton.UseVisualStyleBackColor = true;
            this._copyButton.Click += new System.EventHandler(this._copyButton_Click);
            // 
            // _clearButton
            // 
            this._clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._clearButton.Location = new System.Drawing.Point(1893, 581);
            this._clearButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(44, 38);
            this._clearButton.TabIndex = 2;
            this._clearButton.UseVisualStyleBackColor = true;
            this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
            // 
            // _outputTextBox
            // 
            this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._outputTextBox.Location = new System.Drawing.Point(10, 37);
            this._outputTextBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._outputTextBox.Multiline = true;
            this._outputTextBox.Name = "_outputTextBox";
            this._outputTextBox.Size = new System.Drawing.Size(1926, 538);
            this._outputTextBox.TabIndex = 0;
            // 
            // mergeableMenuStrip1
            // 
            this.mergeableMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mergeableMenuStrip1.InheritedToolStrip = null;
            this.mergeableMenuStrip1.Location = new System.Drawing.Point(6, 29);
            this.mergeableMenuStrip1.Name = "mergeableMenuStrip1";
            this.mergeableMenuStrip1.Size = new System.Drawing.Size(1937, 24);
            this.mergeableMenuStrip1.TabIndex = 1;
            this.mergeableMenuStrip1.Text = "mergeableMenuStrip1";
            // 
            // _configGroupBox
            // 
            this._configGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._configGroupBox.Controls.Add(this._policyBox);
            this._configGroupBox.Controls.Add(this.label7);
            this._configGroupBox.Controls.Add(this._listTypeComboBox);
            this._configGroupBox.Controls.Add(this._batchIntBox);
            this._configGroupBox.Controls.Add(this.label6);
            this._configGroupBox.Controls.Add(this._commitCheckBox);
            this._configGroupBox.Controls.Add(this._itemSizeIntBox);
            this._configGroupBox.Controls.Add(this.label5);
            this._configGroupBox.Controls.Add(this._cacheSizeIntBox);
            this._configGroupBox.Controls.Add(this.label4);
            this._configGroupBox.Controls.Add(this._pageSizeIntBox);
            this._configGroupBox.Controls.Add(this.label3);
            this._configGroupBox.Controls.Add(this._clusterSizeIntBox);
            this._configGroupBox.Controls.Add(this.label2);
            this._configGroupBox.Controls.Add(this._itemsIntBox);
            this._configGroupBox.Controls.Add(this.label1);
            this._configGroupBox.Location = new System.Drawing.Point(6, 0);
            this._configGroupBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._configGroupBox.Name = "_configGroupBox";
            this._configGroupBox.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._configGroupBox.Size = new System.Drawing.Size(1949, 359);
            this._configGroupBox.TabIndex = 3;
            this._configGroupBox.TabStop = false;
            this._configGroupBox.Text = "Test Config";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 284);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 25);
            this.label7.TabIndex = 19;
            this.label7.Text = "Type";
            // 
            // _listTypeComboBox
            // 
            this._listTypeComboBox.AllowEmptyOption = false;
            this._listTypeComboBox.DisplayMember = "Display";
            this._listTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._listTypeComboBox.EmptyOptionText = "";
            this._listTypeComboBox.FormattingEnabled = true;
            this._listTypeComboBox.Location = new System.Drawing.Point(150, 281);
            this._listTypeComboBox.Name = "_listTypeComboBox";
            this._listTypeComboBox.Size = new System.Drawing.Size(182, 33);
            this._listTypeComboBox.TabIndex = 18;
            this._listTypeComboBox.ValueMember = "Value";
            // 
            // _batchIntBox
            // 
            this._batchIntBox.Location = new System.Drawing.Point(150, 81);
            this._batchIntBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._batchIntBox.Name = "_batchIntBox";
            this._batchIntBox.Size = new System.Drawing.Size(78, 31);
            this._batchIntBox.TabIndex = 16;
            this._batchIntBox.Text = "100";
            this._batchIntBox.Value = 100;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 84);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 25);
            this.label6.TabIndex = 15;
            this.label6.Text = "Batch Size";
            // 
            // _commitCheckBox
            // 
            this._commitCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._commitCheckBox.AutoSize = true;
            this._commitCheckBox.Location = new System.Drawing.Point(395, 285);
            this._commitCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commitCheckBox.Name = "_commitCheckBox";
            this._commitCheckBox.Size = new System.Drawing.Size(239, 29);
            this._commitCheckBox.TabIndex = 12;
            this._commitCheckBox.Text = "Commit after every batch";
            this._commitCheckBox.UseVisualStyleBackColor = true;
            // 
            // _itemSizeIntBox
            // 
            this._itemSizeIntBox.Location = new System.Drawing.Point(150, 242);
            this._itemSizeIntBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._itemSizeIntBox.Name = "_itemSizeIntBox";
            this._itemSizeIntBox.Size = new System.Drawing.Size(141, 31);
            this._itemSizeIntBox.TabIndex = 11;
            this._itemSizeIntBox.Text = "300";
            this._itemSizeIntBox.Value = 300;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 245);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Item Size (b)";
            // 
            // _cacheSizeIntBox
            // 
            this._cacheSizeIntBox.Location = new System.Drawing.Point(150, 201);
            this._cacheSizeIntBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cacheSizeIntBox.Name = "_cacheSizeIntBox";
            this._cacheSizeIntBox.Size = new System.Drawing.Size(141, 31);
            this._cacheSizeIntBox.TabIndex = 9;
            this._cacheSizeIntBox.Text = "10000000";
            this._cacheSizeIntBox.Value = 10000000;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 204);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "Cache Size (b)";
            // 
            // _pageSizeIntBox
            // 
            this._pageSizeIntBox.Location = new System.Drawing.Point(150, 163);
            this._pageSizeIntBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._pageSizeIntBox.Name = "_pageSizeIntBox";
            this._pageSizeIntBox.Size = new System.Drawing.Size(141, 31);
            this._pageSizeIntBox.TabIndex = 7;
            this._pageSizeIntBox.Text = "256000";
            this._pageSizeIntBox.Value = 256000;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 166);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "Page Size (b)";
            // 
            // _clusterSizeIntBox
            // 
            this._clusterSizeIntBox.Location = new System.Drawing.Point(150, 122);
            this._clusterSizeIntBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._clusterSizeIntBox.Name = "_clusterSizeIntBox";
            this._clusterSizeIntBox.Size = new System.Drawing.Size(141, 31);
            this._clusterSizeIntBox.TabIndex = 5;
            this._clusterSizeIntBox.Text = "310";
            this._clusterSizeIntBox.Value = 310;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 125);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Cluster Size (b)";
            // 
            // _itemsIntBox
            // 
            this._itemsIntBox.Location = new System.Drawing.Point(150, 43);
            this._itemsIntBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._itemsIntBox.Name = "_itemsIntBox";
            this._itemsIntBox.Size = new System.Drawing.Size(78, 31);
            this._itemsIntBox.TabIndex = 3;
            this._itemsIntBox.Text = "5000";
            this._itemsIntBox.Value = 5000;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Items";
            // 
            // _policyBox
            // 
            this._policyBox.CheckOnClick = true;
            this._policyBox.FormattingEnabled = true;
            this._policyBox.Location = new System.Drawing.Point(395, 43);
            this._policyBox.Name = "_policyBox";
            this._policyBox.Size = new System.Drawing.Size(304, 228);
            this._policyBox.TabIndex = 4;
            // 
            // TransactionalCollectionScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._configGroupBox);
            this.Controls.Add(this._outputGroupBox);
            this.Controls.Add(this._testsGroupBox);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "TransactionalCollectionScreen";
            this.Size = new System.Drawing.Size(1954, 1114);
            this._testsGroupBox.ResumeLayout(false);
            this._outputGroupBox.ResumeLayout(false);
            this._outputGroupBox.PerformLayout();
            this._configGroupBox.ResumeLayout(false);
            this._configGroupBox.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _listAppendTestButton;
		private System.Windows.Forms.GroupBox _testsGroupBox;
		private System.Windows.Forms.GroupBox _outputGroupBox;
		private System.Windows.Forms.TextBox _outputTextBox;
		private Hydrogen.Windows.Forms.MergeableMenuStrip mergeableMenuStrip1;
		private System.Windows.Forms.GroupBox _configGroupBox;
		private Hydrogen.Windows.Forms.IntBox _itemsIntBox;
		private System.Windows.Forms.Label label1;
		private Hydrogen.Windows.Forms.IntBox _clusterSizeIntBox;
		private System.Windows.Forms.Label label2;
		private Hydrogen.Windows.Forms.IntBox _pageSizeIntBox;
		private System.Windows.Forms.Label label3;
		private Hydrogen.Windows.Forms.IntBox _cacheSizeIntBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.Button _copyButton;
		private Hydrogen.Windows.Forms.IntBox _itemSizeIntBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox _commitCheckBox;
		private Hydrogen.Windows.Forms.IntBox _batchIntBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button _storageStreamButton;
		private System.Windows.Forms.Button _dictionaryAppend;
		private System.Windows.Forms.Label label7;
		private Hydrogen.Windows.Forms.EnumComboBox _listTypeComboBox;
		private System.Windows.Forms.Button _bufferTestButton;
		private Hydrogen.Windows.Forms.FlagsCheckedListBox _policyBox;
	}
}
