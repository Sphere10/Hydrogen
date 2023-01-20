namespace Hydrogen.Utils.WorkflowTester
{
    partial class WorkflowTestForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._outputTextBox = new System.Windows.Forms.TextBox();
            this._test1Button = new System.Windows.Forms.Button();
            this._viewWorkflowsButton = new System.Windows.Forms.Button();
            this._noErrorRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._step3RadioButton = new System.Windows.Forms.RadioButton();
            this._step2RadioButton = new System.Windows.Forms.RadioButton();
            this._step1RadioButton = new System.Windows.Forms.RadioButton();
            this._resumeWorkflow = new System.Windows.Forms.Button();
            this._2SecDelayCheckbox = new System.Windows.Forms.CheckBox();
            this._test2Button = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _outputTextBox
            // 
            this._outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._outputTextBox.Location = new System.Drawing.Point(12, 153);
            this._outputTextBox.Multiline = true;
            this._outputTextBox.Name = "_outputTextBox";
            this._outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._outputTextBox.Size = new System.Drawing.Size(776, 285);
            this._outputTextBox.TabIndex = 1;
            this._outputTextBox.WordWrap = false;
            // 
            // _test1Button
            // 
            this._test1Button.Location = new System.Drawing.Point(12, 12);
            this._test1Button.Name = "_test1Button";
            this._test1Button.Size = new System.Drawing.Size(104, 39);
            this._test1Button.TabIndex = 2;
            this._test1Button.Text = "Test1";
            this._test1Button.UseVisualStyleBackColor = true;
            this._test1Button.Click += new System.EventHandler(this._test1Button_Click);
            // 
            // _viewWorkflowsButton
            // 
            this._viewWorkflowsButton.Location = new System.Drawing.Point(122, 12);
            this._viewWorkflowsButton.Name = "_viewWorkflowsButton";
            this._viewWorkflowsButton.Size = new System.Drawing.Size(104, 39);
            this._viewWorkflowsButton.TabIndex = 3;
            this._viewWorkflowsButton.Text = "View Workflows";
            this._viewWorkflowsButton.UseVisualStyleBackColor = true;
            this._viewWorkflowsButton.Click += new System.EventHandler(this._viewWorkflowsButton_Click);
            // 
            // _noErrorRadioButton
            // 
            this._noErrorRadioButton.AutoSize = true;
            this._noErrorRadioButton.Checked = true;
            this._noErrorRadioButton.Location = new System.Drawing.Point(6, 22);
            this._noErrorRadioButton.Name = "_noErrorRadioButton";
            this._noErrorRadioButton.Size = new System.Drawing.Size(69, 19);
            this._noErrorRadioButton.TabIndex = 4;
            this._noErrorRadioButton.TabStop = true;
            this._noErrorRadioButton.Text = "No Error";
            this._noErrorRadioButton.UseVisualStyleBackColor = true;
            this._noErrorRadioButton.CheckedChanged += new System.EventHandler(this._noErrorRadioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._step3RadioButton);
            this.groupBox1.Controls.Add(this._step2RadioButton);
            this.groupBox1.Controls.Add(this._step1RadioButton);
            this.groupBox1.Controls.Add(this._noErrorRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(540, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 135);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Workflow Error Source";
            // 
            // _step3RadioButton
            // 
            this._step3RadioButton.AutoSize = true;
            this._step3RadioButton.Location = new System.Drawing.Point(6, 97);
            this._step3RadioButton.Name = "_step3RadioButton";
            this._step3RadioButton.Size = new System.Drawing.Size(57, 19);
            this._step3RadioButton.TabIndex = 7;
            this._step3RadioButton.Text = "Step 3";
            this._step3RadioButton.UseVisualStyleBackColor = true;
            this._step3RadioButton.CheckedChanged += new System.EventHandler(this._step3RadioButton_CheckedChanged);
            // 
            // _step2RadioButton
            // 
            this._step2RadioButton.AutoSize = true;
            this._step2RadioButton.Location = new System.Drawing.Point(6, 72);
            this._step2RadioButton.Name = "_step2RadioButton";
            this._step2RadioButton.Size = new System.Drawing.Size(57, 19);
            this._step2RadioButton.TabIndex = 6;
            this._step2RadioButton.Text = "Step 2";
            this._step2RadioButton.UseVisualStyleBackColor = true;
            this._step2RadioButton.CheckedChanged += new System.EventHandler(this._step2RadioButton_CheckedChanged);
            // 
            // _step1RadioButton
            // 
            this._step1RadioButton.AutoSize = true;
            this._step1RadioButton.Location = new System.Drawing.Point(6, 47);
            this._step1RadioButton.Name = "_step1RadioButton";
            this._step1RadioButton.Size = new System.Drawing.Size(57, 19);
            this._step1RadioButton.TabIndex = 5;
            this._step1RadioButton.Text = "Step 1";
            this._step1RadioButton.UseVisualStyleBackColor = true;
            this._step1RadioButton.CheckedChanged += new System.EventHandler(this._step1RadioButton_CheckedChanged);
            // 
            // _resumeWorkflow
            // 
            this._resumeWorkflow.Location = new System.Drawing.Point(232, 12);
            this._resumeWorkflow.Name = "_resumeWorkflow";
            this._resumeWorkflow.Size = new System.Drawing.Size(124, 39);
            this._resumeWorkflow.TabIndex = 6;
            this._resumeWorkflow.Text = "Resume Workflow";
            this._resumeWorkflow.UseVisualStyleBackColor = true;
            this._resumeWorkflow.Click += new System.EventHandler(this._resumeWorkflow_Click);
            // 
            // _2SecDelayCheckbox
            // 
            this._2SecDelayCheckbox.AutoSize = true;
            this._2SecDelayCheckbox.Location = new System.Drawing.Point(392, 12);
            this._2SecDelayCheckbox.Name = "_2SecDelayCheckbox";
            this._2SecDelayCheckbox.Size = new System.Drawing.Size(142, 19);
            this._2SecDelayCheckbox.TabIndex = 7;
            this._2SecDelayCheckbox.Text = "2 second delay at step";
            this._2SecDelayCheckbox.UseVisualStyleBackColor = true;
            this._2SecDelayCheckbox.CheckedChanged += new System.EventHandler(this._2SecDelayCheckbox_CheckedChanged);
            // 
            // _test2Button
            // 
            this._test2Button.Location = new System.Drawing.Point(12, 57);
            this._test2Button.Name = "_test2Button";
            this._test2Button.Size = new System.Drawing.Size(104, 39);
            this._test2Button.TabIndex = 8;
            this._test2Button.Text = "Test2";
            this._test2Button.UseVisualStyleBackColor = true;
            this._test2Button.Click += new System.EventHandler(this._test2Button_Click);
            // 
            // WorkflowTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._test2Button);
            this.Controls.Add(this._2SecDelayCheckbox);
            this.Controls.Add(this._resumeWorkflow);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._viewWorkflowsButton);
            this.Controls.Add(this._test1Button);
            this.Controls.Add(this._outputTextBox);
            this.Name = "WorkflowTestForm";
            this.Text = "Workflow Tester";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox _outputTextBox;
        private Button _test1Button;
		private Button _viewWorkflowsButton;
		private RadioButton _noErrorRadioButton;
		private GroupBox groupBox1;
		private RadioButton _step3RadioButton;
		private RadioButton _step2RadioButton;
		private RadioButton _step1RadioButton;
		private Button _resumeWorkflow;
		private CheckBox _2SecDelayCheckbox;
		private Button _test2Button;
	}
}