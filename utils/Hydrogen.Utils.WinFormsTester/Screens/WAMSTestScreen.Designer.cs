namespace Hydrogen.Utils.WinFormsTester {
	partial class WAMSTestScreen {
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
			this._keyMatchingTestsButton = new System.Windows.Forms.Button();
			this._commandToolBox = new System.Windows.Forms.GroupBox();
			this._regFastButton = new System.Windows.Forms.Button();
			this._registerStandardHashers = new System.Windows.Forms.Button();
			this._miscTests2 = new System.Windows.Forms.Button();
			this._miscTests = new System.Windows.Forms.Button();
			this._verificationSpeedButton = new System.Windows.Forms.Button();
			this._outputGroupBox = new System.Windows.Forms.GroupBox();
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this._optionsGroupBox = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._heightControl = new System.Windows.Forms.NumericUpDown();
			this._winternitzControl = new System.Windows.Forms.NumericUpDown();
			this._hashAlgControl = new Hydrogen.Windows.Forms.EnumComboBox();
			this._optimizedPubKeyCheckBox = new System.Windows.Forms.CheckBox();
			this._commandToolBox.SuspendLayout();
			this._outputGroupBox.SuspendLayout();
			this._optionsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._heightControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._winternitzControl)).BeginInit();
			this.SuspendLayout();
			// 
			// _keyMatchingTestsButton
			// 
			this._keyMatchingTestsButton.Location = new System.Drawing.Point(7, 22);
			this._keyMatchingTestsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._keyMatchingTestsButton.Name = "_keyMatchingTestsButton";
			this._keyMatchingTestsButton.Size = new System.Drawing.Size(115, 27);
			this._keyMatchingTestsButton.TabIndex = 0;
			this._keyMatchingTestsButton.Text = "Key Matching Tests";
			this._keyMatchingTestsButton.UseVisualStyleBackColor = true;
			this._keyMatchingTestsButton.Click += new System.EventHandler(this._keyMatchingTestsButton_Click);
			// 
			// _commandToolBox
			// 
			this._commandToolBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._commandToolBox.Controls.Add(this._regFastButton);
			this._commandToolBox.Controls.Add(this._registerStandardHashers);
			this._commandToolBox.Controls.Add(this._miscTests2);
			this._commandToolBox.Controls.Add(this._miscTests);
			this._commandToolBox.Controls.Add(this._verificationSpeedButton);
			this._commandToolBox.Controls.Add(this._keyMatchingTestsButton);
			this._commandToolBox.Location = new System.Drawing.Point(4, 234);
			this._commandToolBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._commandToolBox.Name = "_commandToolBox";
			this._commandToolBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._commandToolBox.Size = new System.Drawing.Size(601, 136);
			this._commandToolBox.TabIndex = 1;
			this._commandToolBox.TabStop = false;
			this._commandToolBox.Text = "Commands";
			// 
			// _regFastButton
			// 
			this._regFastButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._regFastButton.Location = new System.Drawing.Point(478, 55);
			this._regFastButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._regFastButton.Name = "_regFastButton";
			this._regFastButton.Size = new System.Drawing.Size(115, 27);
			this._regFastButton.TabIndex = 3;
			this._regFastButton.Text = "Reg Fast";
			this._regFastButton.UseVisualStyleBackColor = true;
			this._regFastButton.Click += new System.EventHandler(this._regFastButton_Click);
			// 
			// _registerStandardHashers
			// 
			this._registerStandardHashers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._registerStandardHashers.Location = new System.Drawing.Point(478, 22);
			this._registerStandardHashers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._registerStandardHashers.Name = "_registerStandardHashers";
			this._registerStandardHashers.Size = new System.Drawing.Size(115, 27);
			this._registerStandardHashers.TabIndex = 2;
			this._registerStandardHashers.Text = "Reg Standard";
			this._registerStandardHashers.UseVisualStyleBackColor = true;
			this._registerStandardHashers.Click += new System.EventHandler(this._registerStandardHashers_Click);
			// 
			// _miscTests2
			// 
			this._miscTests2.Location = new System.Drawing.Point(131, 88);
			this._miscTests2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._miscTests2.Name = "_miscTests2";
			this._miscTests2.Size = new System.Drawing.Size(115, 27);
			this._miscTests2.TabIndex = 1;
			this._miscTests2.Text = "Misc Tests 2";
			this._miscTests2.UseVisualStyleBackColor = true;
			this._miscTests2.Click += new System.EventHandler(this._miscButton2_Click);
			// 
			// _miscTests
			// 
			this._miscTests.Location = new System.Drawing.Point(8, 88);
			this._miscTests.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._miscTests.Name = "_miscTests";
			this._miscTests.Size = new System.Drawing.Size(115, 27);
			this._miscTests.TabIndex = 1;
			this._miscTests.Text = "Misc Tests";
			this._miscTests.UseVisualStyleBackColor = true;
			this._miscTests.Click += new System.EventHandler(this._miscButton_Click);
			// 
			// _verificationSpeedButton
			// 
			this._verificationSpeedButton.Location = new System.Drawing.Point(7, 55);
			this._verificationSpeedButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._verificationSpeedButton.Name = "_verificationSpeedButton";
			this._verificationSpeedButton.Size = new System.Drawing.Size(115, 27);
			this._verificationSpeedButton.TabIndex = 1;
			this._verificationSpeedButton.Text = "Verification Tests";
			this._verificationSpeedButton.UseVisualStyleBackColor = true;
			this._verificationSpeedButton.Click += new System.EventHandler(this._verificationSpeedButton_Click);
			// 
			// _outputGroupBox
			// 
			this._outputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputGroupBox.Controls.Add(this._outputTextBox);
			this._outputGroupBox.Location = new System.Drawing.Point(4, 3);
			this._outputGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._outputGroupBox.Name = "_outputGroupBox";
			this._outputGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._outputGroupBox.Size = new System.Drawing.Size(849, 223);
			this._outputGroupBox.TabIndex = 2;
			this._outputGroupBox.TabStop = false;
			this._outputGroupBox.Text = "Output";
			// 
			// _outputTextBox
			// 
			this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputTextBox.Location = new System.Drawing.Point(7, 22);
			this._outputTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this._outputTextBox.Multiline = true;
			this._outputTextBox.Name = "_outputTextBox";
			this._outputTextBox.Size = new System.Drawing.Size(834, 195);
			this._outputTextBox.TabIndex = 0;
			// 
			// _optionsGroupBox
			// 
			this._optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._optionsGroupBox.Controls.Add(this._optimizedPubKeyCheckBox);
			this._optionsGroupBox.Controls.Add(this.label3);
			this._optionsGroupBox.Controls.Add(this.label2);
			this._optionsGroupBox.Controls.Add(this.label1);
			this._optionsGroupBox.Controls.Add(this._heightControl);
			this._optionsGroupBox.Controls.Add(this._winternitzControl);
			this._optionsGroupBox.Controls.Add(this._hashAlgControl);
			this._optionsGroupBox.Location = new System.Drawing.Point(612, 234);
			this._optionsGroupBox.Name = "_optionsGroupBox";
			this._optionsGroupBox.Size = new System.Drawing.Size(241, 136);
			this._optionsGroupBox.TabIndex = 3;
			this._optionsGroupBox.TabStop = false;
			this._optionsGroupBox.Text = "Options";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(37, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(61, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "Height (h)";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 15);
			this.label2.TabIndex = 4;
			this.label2.Text = "Winternitz (w)";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(43, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 15);
			this.label1.TabIndex = 3;
			this.label1.Text = "Hash Alg";
			// 
			// _heightControl
			// 
			this._heightControl.Location = new System.Drawing.Point(106, 78);
			this._heightControl.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this._heightControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._heightControl.Name = "_heightControl";
			this._heightControl.Size = new System.Drawing.Size(120, 23);
			this._heightControl.TabIndex = 2;
			this._heightControl.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _winternitzControl
			// 
			this._winternitzControl.Location = new System.Drawing.Point(106, 51);
			this._winternitzControl.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
			this._winternitzControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._winternitzControl.Name = "_winternitzControl";
			this._winternitzControl.Size = new System.Drawing.Size(120, 23);
			this._winternitzControl.TabIndex = 1;
			this._winternitzControl.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			// 
			// _hashAlgControl
			// 
			this._hashAlgControl.AllowEmptyOption = false;
			this._hashAlgControl.DisplayMember = "Display";
			this._hashAlgControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._hashAlgControl.EmptyOptionText = "";
			this._hashAlgControl.FormattingEnabled = true;
			this._hashAlgControl.Location = new System.Drawing.Point(106, 17);
			this._hashAlgControl.Name = "_hashAlgControl";
			this._hashAlgControl.Size = new System.Drawing.Size(121, 23);
			this._hashAlgControl.TabIndex = 0;
			this._hashAlgControl.ValueMember = "Value";
			// 
			// _optimizedPubKeyCheckBox
			// 
			this._optimizedPubKeyCheckBox.AutoSize = true;
			this._optimizedPubKeyCheckBox.Checked = true;
			this._optimizedPubKeyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._optimizedPubKeyCheckBox.Location = new System.Drawing.Point(82, 107);
			this._optimizedPubKeyCheckBox.Name = "_optimizedPubKeyCheckBox";
			this._optimizedPubKeyCheckBox.Size = new System.Drawing.Size(144, 19);
			this._optimizedPubKeyCheckBox.TabIndex = 6;
			this._optimizedPubKeyCheckBox.Text = "Optimized Public Keys";
			this._optimizedPubKeyCheckBox.UseVisualStyleBackColor = true;
			// 
			// WAMSTestScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._optionsGroupBox);
			this.Controls.Add(this._outputGroupBox);
			this.Controls.Add(this._commandToolBox);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "WAMSTestScreen";
			this.Size = new System.Drawing.Size(853, 373);
			this._commandToolBox.ResumeLayout(false);
			this._outputGroupBox.ResumeLayout(false);
			this._outputGroupBox.PerformLayout();
			this._optionsGroupBox.ResumeLayout(false);
			this._optionsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._heightControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._winternitzControl)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _keyMatchingTestsButton;
		private System.Windows.Forms.GroupBox _commandToolBox;
		private System.Windows.Forms.GroupBox _outputGroupBox;
		private System.Windows.Forms.TextBox _outputTextBox;
		private System.Windows.Forms.Button _verificationSpeedButton;
		private System.Windows.Forms.Button _miscTests;
		private System.Windows.Forms.Button _miscTests2;
		private System.Windows.Forms.GroupBox _optionsGroupBox;
		private Hydrogen.Windows.Forms.EnumComboBox _hashAlgControl;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown _heightControl;
		private System.Windows.Forms.NumericUpDown _winternitzControl;
		private System.Windows.Forms.Button _regFastButton;
		private System.Windows.Forms.Button _registerStandardHashers;
		private System.Windows.Forms.CheckBox _optimizedPubKeyCheckBox;
	}
}
