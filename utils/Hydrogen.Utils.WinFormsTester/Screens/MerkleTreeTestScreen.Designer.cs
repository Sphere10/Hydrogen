namespace Hydrogen.Utils.WinFormsTester {
	partial class MerkleTreeTestScreen {
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
			this._printAlphabetButton = new System.Windows.Forms.Button();
			this._commandToolBox = new System.Windows.Forms.GroupBox();
			this._treePerfTestsButton = new System.Windows.Forms.Button();
			this._printInvPow2Button = new System.Windows.Forms.Button();
			this._perfectMerkleButton = new System.Windows.Forms.Button();
			this._sumPow2Button = new System.Windows.Forms.Button();
			this._printIntegersButton = new System.Windows.Forms.Button();
			this._outputGroupBox = new System.Windows.Forms.GroupBox();
			this._outputTextBox = new System.Windows.Forms.TextBox();
			this.mergeableMenuStrip1 = new Hydrogen.Windows.Forms.MergeableMenuStrip();
			this._commandToolBox.SuspendLayout();
			this._outputGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _printAlphabetButton
			// 
			this._printAlphabetButton.Location = new System.Drawing.Point(10, 37);
			this._printAlphabetButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._printAlphabetButton.Name = "_printAlphabetButton";
			this._printAlphabetButton.Size = new System.Drawing.Size(164, 45);
			this._printAlphabetButton.TabIndex = 0;
			this._printAlphabetButton.Text = "Print Alphabet";
			this._printAlphabetButton.UseVisualStyleBackColor = true;
			this._printAlphabetButton.Click += new System.EventHandler(this._printAlphabetButton_Click);
			// 
			// _commandToolBox
			// 
			this._commandToolBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._commandToolBox.Controls.Add(this._treePerfTestsButton);
			this._commandToolBox.Controls.Add(this._printInvPow2Button);
			this._commandToolBox.Controls.Add(this._perfectMerkleButton);
			this._commandToolBox.Controls.Add(this._sumPow2Button);
			this._commandToolBox.Controls.Add(this._printIntegersButton);
			this._commandToolBox.Controls.Add(this._printAlphabetButton);
			this._commandToolBox.Location = new System.Drawing.Point(6, 510);
			this._commandToolBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._commandToolBox.Name = "_commandToolBox";
			this._commandToolBox.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._commandToolBox.Size = new System.Drawing.Size(1219, 227);
			this._commandToolBox.TabIndex = 1;
			this._commandToolBox.TabStop = false;
			this._commandToolBox.Text = "Commands";
			// 
			// _treePerfTestsButton
			// 
			this._treePerfTestsButton.Location = new System.Drawing.Point(186, 153);
			this._treePerfTestsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this._treePerfTestsButton.Name = "_treePerfTestsButton";
			this._treePerfTestsButton.Size = new System.Drawing.Size(164, 38);
			this._treePerfTestsButton.TabIndex = 3;
			this._treePerfTestsButton.Text = "Tree Perf Tests";
			this._treePerfTestsButton.UseVisualStyleBackColor = true;
			this._treePerfTestsButton.Click += new System.EventHandler(this._treePerfTestsButton_Click);
			// 
			// _printInvPow2Button
			// 
			this._printInvPow2Button.Location = new System.Drawing.Point(186, 98);
			this._printInvPow2Button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this._printInvPow2Button.Name = "_printInvPow2Button";
			this._printInvPow2Button.Size = new System.Drawing.Size(164, 38);
			this._printInvPow2Button.TabIndex = 2;
			this._printInvPow2Button.Text = "Inv Pow2";
			this._printInvPow2Button.UseVisualStyleBackColor = true;
			this._printInvPow2Button.Click += new System.EventHandler(this._printInvPow2Button_Click);
			// 
			// _perfectMerkleButton
			// 
			this._perfectMerkleButton.Location = new System.Drawing.Point(186, 37);
			this._perfectMerkleButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._perfectMerkleButton.Name = "_perfectMerkleButton";
			this._perfectMerkleButton.Size = new System.Drawing.Size(164, 45);
			this._perfectMerkleButton.TabIndex = 1;
			this._perfectMerkleButton.Text = "Perfect Merkle";
			this._perfectMerkleButton.UseVisualStyleBackColor = true;
			this._perfectMerkleButton.Click += new System.EventHandler(this._perfectMerkleButton_Click);
			// 
			// _sumPow2Button
			// 
			this._sumPow2Button.Location = new System.Drawing.Point(10, 147);
			this._sumPow2Button.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._sumPow2Button.Name = "_sumPow2Button";
			this._sumPow2Button.Size = new System.Drawing.Size(164, 45);
			this._sumPow2Button.TabIndex = 1;
			this._sumPow2Button.Text = "Sum Pow2";
			this._sumPow2Button.UseVisualStyleBackColor = true;
			this._sumPow2Button.Click += new System.EventHandler(this._sumPow2Button_Click);
			// 
			// _printIntegersButton
			// 
			this._printIntegersButton.Location = new System.Drawing.Point(10, 92);
			this._printIntegersButton.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._printIntegersButton.Name = "_printIntegersButton";
			this._printIntegersButton.Size = new System.Drawing.Size(164, 45);
			this._printIntegersButton.TabIndex = 1;
			this._printIntegersButton.Text = "Print Integers ";
			this._printIntegersButton.UseVisualStyleBackColor = true;
			this._printIntegersButton.Click += new System.EventHandler(this._printIntegersButton_Click);
			// 
			// _outputGroupBox
			// 
			this._outputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._outputGroupBox.Controls.Add(this._outputTextBox);
			this._outputGroupBox.Controls.Add(this.mergeableMenuStrip1);
			this._outputGroupBox.Location = new System.Drawing.Point(6, 5);
			this._outputGroupBox.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._outputGroupBox.Name = "_outputGroupBox";
			this._outputGroupBox.Padding = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this._outputGroupBox.Size = new System.Drawing.Size(1219, 492);
			this._outputGroupBox.TabIndex = 2;
			this._outputGroupBox.TabStop = false;
			this._outputGroupBox.Text = "Output";
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
			this._outputTextBox.Size = new System.Drawing.Size(1195, 441);
			this._outputTextBox.TabIndex = 0;
			// 
			// mergeableMenuStrip1
			// 
			this.mergeableMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.mergeableMenuStrip1.InheritedToolStrip = null;
			this.mergeableMenuStrip1.Location = new System.Drawing.Point(644, 16);
			this.mergeableMenuStrip1.Name = "mergeableMenuStrip1";
			this.mergeableMenuStrip1.Size = new System.Drawing.Size(303, 36);
			this.mergeableMenuStrip1.TabIndex = 1;
			this.mergeableMenuStrip1.Text = "mergeableMenuStrip1";
			// 
			// MerkleTreeTestScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._outputGroupBox);
			this.Controls.Add(this._commandToolBox);
			this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this.Name = "MerkleTreeTestScreen";
			this.Size = new System.Drawing.Size(1223, 742);
			this._commandToolBox.ResumeLayout(false);
			this._outputGroupBox.ResumeLayout(false);
			this._outputGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _printAlphabetButton;
		private System.Windows.Forms.GroupBox _commandToolBox;
		private System.Windows.Forms.GroupBox _outputGroupBox;
		private System.Windows.Forms.TextBox _outputTextBox;
		private System.Windows.Forms.Button _printIntegersButton;
		private System.Windows.Forms.Button _sumPow2Button;
		private System.Windows.Forms.Button _perfectMerkleButton;
		private System.Windows.Forms.Button _printInvPow2Button;
		private System.Windows.Forms.Button _treePerfTestsButton;
		private Hydrogen.Windows.Forms.MergeableMenuStrip mergeableMenuStrip1;
	}
}
