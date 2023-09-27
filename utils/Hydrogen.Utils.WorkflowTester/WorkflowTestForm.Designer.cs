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
			_outputTextBox = new TextBox();
			_test1Button = new Button();
			_viewWorkflowsButton = new Button();
			_noErrorRadioButton = new RadioButton();
			groupBox1 = new GroupBox();
			_step3RadioButton = new RadioButton();
			_step2RadioButton = new RadioButton();
			_step1RadioButton = new RadioButton();
			_resumeWorkflow = new Button();
			_2SecDelayCheckbox = new CheckBox();
			_test2Button = new Button();
			_subscriptionButton = new Button();
			_cancelSubscriptionCheckBox = new CheckBox();
			groupBox1.SuspendLayout();
			SuspendLayout();
			// 
			// _outputTextBox
			// 
			_outputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			_outputTextBox.Location = new Point(12, 153);
			_outputTextBox.Multiline = true;
			_outputTextBox.Name = "_outputTextBox";
			_outputTextBox.ScrollBars = ScrollBars.Both;
			_outputTextBox.Size = new Size(776, 285);
			_outputTextBox.TabIndex = 1;
			_outputTextBox.WordWrap = false;
			// 
			// _test1Button
			// 
			_test1Button.Location = new Point(12, 12);
			_test1Button.Name = "_test1Button";
			_test1Button.Size = new Size(104, 39);
			_test1Button.TabIndex = 2;
			_test1Button.Text = "Test1";
			_test1Button.UseVisualStyleBackColor = true;
			_test1Button.Click += _test1Button_Click;
			// 
			// _viewWorkflowsButton
			// 
			_viewWorkflowsButton.Location = new Point(122, 12);
			_viewWorkflowsButton.Name = "_viewWorkflowsButton";
			_viewWorkflowsButton.Size = new Size(104, 39);
			_viewWorkflowsButton.TabIndex = 3;
			_viewWorkflowsButton.Text = "View Workflows";
			_viewWorkflowsButton.UseVisualStyleBackColor = true;
			_viewWorkflowsButton.Click += _viewWorkflowsButton_Click;
			// 
			// _noErrorRadioButton
			// 
			_noErrorRadioButton.AutoSize = true;
			_noErrorRadioButton.Checked = true;
			_noErrorRadioButton.Location = new Point(6, 22);
			_noErrorRadioButton.Name = "_noErrorRadioButton";
			_noErrorRadioButton.Size = new Size(69, 19);
			_noErrorRadioButton.TabIndex = 4;
			_noErrorRadioButton.TabStop = true;
			_noErrorRadioButton.Text = "No Error";
			_noErrorRadioButton.UseVisualStyleBackColor = true;
			_noErrorRadioButton.CheckedChanged += _noErrorRadioButton_CheckedChanged;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(_step3RadioButton);
			groupBox1.Controls.Add(_step2RadioButton);
			groupBox1.Controls.Add(_step1RadioButton);
			groupBox1.Controls.Add(_noErrorRadioButton);
			groupBox1.Location = new Point(540, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(248, 135);
			groupBox1.TabIndex = 5;
			groupBox1.TabStop = false;
			groupBox1.Text = "Workflow Error Source";
			// 
			// _step3RadioButton
			// 
			_step3RadioButton.AutoSize = true;
			_step3RadioButton.Location = new Point(6, 97);
			_step3RadioButton.Name = "_step3RadioButton";
			_step3RadioButton.Size = new Size(57, 19);
			_step3RadioButton.TabIndex = 7;
			_step3RadioButton.Text = "Step 3";
			_step3RadioButton.UseVisualStyleBackColor = true;
			_step3RadioButton.CheckedChanged += _step3RadioButton_CheckedChanged;
			// 
			// _step2RadioButton
			// 
			_step2RadioButton.AutoSize = true;
			_step2RadioButton.Location = new Point(6, 72);
			_step2RadioButton.Name = "_step2RadioButton";
			_step2RadioButton.Size = new Size(57, 19);
			_step2RadioButton.TabIndex = 6;
			_step2RadioButton.Text = "Step 2";
			_step2RadioButton.UseVisualStyleBackColor = true;
			_step2RadioButton.CheckedChanged += _step2RadioButton_CheckedChanged;
			// 
			// _step1RadioButton
			// 
			_step1RadioButton.AutoSize = true;
			_step1RadioButton.Location = new Point(6, 47);
			_step1RadioButton.Name = "_step1RadioButton";
			_step1RadioButton.Size = new Size(57, 19);
			_step1RadioButton.TabIndex = 5;
			_step1RadioButton.Text = "Step 1";
			_step1RadioButton.UseVisualStyleBackColor = true;
			_step1RadioButton.CheckedChanged += _step1RadioButton_CheckedChanged;
			// 
			// _resumeWorkflow
			// 
			_resumeWorkflow.Location = new Point(232, 12);
			_resumeWorkflow.Name = "_resumeWorkflow";
			_resumeWorkflow.Size = new Size(124, 39);
			_resumeWorkflow.TabIndex = 6;
			_resumeWorkflow.Text = "Resume Workflow";
			_resumeWorkflow.UseVisualStyleBackColor = true;
			_resumeWorkflow.Click += _resumeWorkflow_Click;
			// 
			// _2SecDelayCheckbox
			// 
			_2SecDelayCheckbox.AutoSize = true;
			_2SecDelayCheckbox.Location = new Point(392, 12);
			_2SecDelayCheckbox.Name = "_2SecDelayCheckbox";
			_2SecDelayCheckbox.Size = new Size(142, 19);
			_2SecDelayCheckbox.TabIndex = 7;
			_2SecDelayCheckbox.Text = "2 second delay at step";
			_2SecDelayCheckbox.UseVisualStyleBackColor = true;
			_2SecDelayCheckbox.CheckedChanged += _2SecDelayCheckbox_CheckedChanged;
			// 
			// _test2Button
			// 
			_test2Button.Location = new Point(12, 57);
			_test2Button.Name = "_test2Button";
			_test2Button.Size = new Size(104, 39);
			_test2Button.TabIndex = 8;
			_test2Button.Text = "Test2";
			_test2Button.UseVisualStyleBackColor = true;
			_test2Button.Click += _test2Button_Click;
			// 
			// _subscriptionButton
			// 
			_subscriptionButton.Location = new Point(122, 57);
			_subscriptionButton.Name = "_subscriptionButton";
			_subscriptionButton.Size = new Size(104, 39);
			_subscriptionButton.TabIndex = 9;
			_subscriptionButton.Text = "Subscription";
			_subscriptionButton.UseVisualStyleBackColor = true;
			_subscriptionButton.Click += _subscriptionButton_Click;
			// 
			// _cancelSubscriptionCheckBox
			// 
			_cancelSubscriptionCheckBox.AutoSize = true;
			_cancelSubscriptionCheckBox.Location = new Point(392, 34);
			_cancelSubscriptionCheckBox.Name = "_cancelSubscriptionCheckBox";
			_cancelSubscriptionCheckBox.Size = new Size(136, 19);
			_cancelSubscriptionCheckBox.TabIndex = 10;
			_cancelSubscriptionCheckBox.Text = "Cancel Subscriptions";
			_cancelSubscriptionCheckBox.UseVisualStyleBackColor = true;
			_cancelSubscriptionCheckBox.CheckedChanged += _cancelSubscriptionCheckBox_CheckedChanged;
			// 
			// WorkflowTestForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(_cancelSubscriptionCheckBox);
			Controls.Add(_subscriptionButton);
			Controls.Add(_test2Button);
			Controls.Add(_2SecDelayCheckbox);
			Controls.Add(_resumeWorkflow);
			Controls.Add(groupBox1);
			Controls.Add(_viewWorkflowsButton);
			Controls.Add(_test1Button);
			Controls.Add(_outputTextBox);
			Name = "WorkflowTestForm";
			Text = "Workflow Tester";
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
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
		private Button _subscriptionButton;
		private CheckBox _cancelSubscriptionCheckBox;
	}
}