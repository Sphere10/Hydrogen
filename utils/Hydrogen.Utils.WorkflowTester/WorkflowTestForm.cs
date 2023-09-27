// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Hydrogen.Utils.WorkflowTester;

public partial class WorkflowTestForm : Form {

	protected static TextWriter OutputWriter;
	protected static bool CancelSubscription;
	protected static int ErrorStep;
	protected static bool Delay;
	protected IWorkflowHost _workflowHost;

	protected List<string> _knownWorkflowInstances;
	private static IServiceProvider ConfigureServices() {
		//setup dependency injection
		IServiceCollection services = new ServiceCollection();
		services.AddLogging();
		services.AddWorkflow();
		//services.AddWorkflow(x => x.UseSqlServer(@"Server=.\;Database=Hydrogen_Workflow_Tester;Trusted_Connection=True;", true, true));

		services.AddSingleton<TestWorkflow1.Step2>();

		var serviceProvider = services.BuildServiceProvider();

		return serviceProvider;
	}

	static WorkflowTestForm() {
		OutputWriter = new ConsoleTextWriter();
	}

	public WorkflowTestForm() {
		InitializeComponent();
		OutputWriter = new MulticastTextWriter(OutputWriter, new TextBoxWriter(_outputTextBox));
		IServiceProvider serviceProvider = ConfigureServices();
		_knownWorkflowInstances = new List<string>();
		_workflowHost = serviceProvider.GetService<IWorkflowHost>();
		_workflowHost.RegisterWorkflow<SubscriptionWorkflow, SubscriptionBag>();
		_workflowHost.RegisterWorkflow<TestWorkflow1>();
		_workflowHost.RegisterWorkflow<TestWorkflow2>();
		_workflowHost.Start();
		CancelSubscription = _cancelSubscriptionCheckBox.Checked;
	}

	public void SetErrorStep() {
		if (_noErrorRadioButton.Checked)
			ErrorStep = 0;
		else if (_step1RadioButton.Checked)
			ErrorStep = 1;
		else if (_step2RadioButton.Checked)
			ErrorStep = 2;
		else if (_step3RadioButton.Checked)
			ErrorStep = 3;
	}

	protected override void OnClosed(EventArgs e) {
		base.OnClosed(e);
		_workflowHost.Stop();
	}

	private Task PrintWorkflow(WorkflowInstance workflow) {
		return OutputWriter.WriteLineAsync(
			$@"ID: {workflow.Id} WorkflowDefinitionId: {workflow.WorkflowDefinitionId} Version: {workflow.Version} Description: {workflow.Description} Reference: {workflow.Reference} Execution Pointers: {workflow.ExecutionPointers.Select(x => x.ToString()).ToDelimittedString(" ")} NextExecution: {workflow.NextExecution} Status: {workflow.Status} Data: {workflow.Data} CreateTime: {workflow.CreateTime} CompleteTime: {workflow.CompleteTime}");
	}


	#region Handlers

	private void _cancelSubscriptionCheckBox_CheckedChanged(object sender, EventArgs e) {
		CancelSubscription = _cancelSubscriptionCheckBox.Checked;
	}

	private async void _subscriptionButton_Click(object sender, EventArgs e) {
		try {
			var workflowInstanceId = await _workflowHost.StartWorkflow("Subscription");
			_knownWorkflowInstances.Add(workflowInstanceId);
			await OutputWriter.WriteLineAsync($"New Workflow Instance: {workflowInstanceId}");
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private async void _test1Button_Click(object sender, EventArgs e) {
		try {
			var workflowInstanceId = await _workflowHost.StartWorkflow("Workflow1");
			_knownWorkflowInstances.Add(workflowInstanceId);
			await OutputWriter.WriteLineAsync($"New Workflow Instance: {workflowInstanceId}");
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}


	private async void _test2Button_Click(object sender, EventArgs e) {
		try {
			var workflowInstanceId = await _workflowHost.StartWorkflow("Workflow2");
			_knownWorkflowInstances.Add(workflowInstanceId);
			await OutputWriter.WriteLineAsync($"New Workflow Instance: {workflowInstanceId}");
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private async void _viewWorkflowsButton_Click(object sender, EventArgs e) {
		try {
			var workflows = await _workflowHost.PersistenceStore.GetWorkflowInstances(_knownWorkflowInstances);

			foreach (var workflow in workflows) {
				await PrintWorkflow(workflow);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}

	}

	private void _noErrorRadioButton_CheckedChanged(object sender, EventArgs e) {
		try {
			SetErrorStep();
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private void _step1RadioButton_CheckedChanged(object sender, EventArgs e) {
		try {
			SetErrorStep();
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private void _step2RadioButton_CheckedChanged(object sender, EventArgs e) {
		try {
			SetErrorStep();
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}

	}

	private void _step3RadioButton_CheckedChanged(object sender, EventArgs e) {
		try {
			SetErrorStep();
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}

	}

	private void _resumeWorkflow_Click(object sender, EventArgs e) {
		try {
			EnterTextDialog.Show(this, "Resume Workflow", "Enter WorkflowID", out var workflowId);
			_workflowHost.ResumeWorkflow(workflowId);
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	private void _2SecDelayCheckbox_CheckedChanged(object sender, EventArgs e) {
		try {
			Delay = _2SecDelayCheckbox.Checked;
		} catch (Exception error) {
			ExceptionDialog.Show(error);
		}
	}

	#endregion

	#region Inner Classes

	public class SubscriptionBag {

		public SubscriptionBag() {
		}

		public int WarningSeconds { get; set; } = 1;

		public int DurationSeconds { get; set; } = 5;

		public bool Active { get; set; } = true;

		public int TimesCharged { get; set; } = 0;

	}


	public class SubscriptionWorkflow : IWorkflow<SubscriptionBag> {
		public void Build(IWorkflowBuilder<SubscriptionBag> builder) {

			var chargeBranch = builder.CreateBranch()
				.StartWith(bag => OutputWriter.WriteLine("CHARGED"));

			var cancelBranch = builder.CreateBranch()
				.StartWith(bag => OutputWriter.WriteLine("CANCELLED"));


			builder
				.UseDefaultErrorBehavior(WorkflowErrorHandling.Suspend)
				.StartWith(_ => OutputWriter.WriteLine("STARTING"))
				.While(bag => bag.Active).Do(then => then
					.Delay(bag => TimeSpan.FromSeconds(bag.DurationSeconds - bag.WarningSeconds))
					.Then(_ => OutputWriter.WriteLine("WARNING ABOUT TO CHARGE"))
					.Delay(bag => TimeSpan.FromSeconds(bag.WarningSeconds))
					.Then(_ => OutputWriter.WriteLine("PROCESS SUBSCRIPTION (CHARGE OR CANCEL)"))
					.Output(bag => bag.Active, step => !CancelSubscription)
				)
				.Then(_ => OutputWriter.WriteLine("FINISHED"));



			//builder
			//	.UseDefaultErrorBehavior(WorkflowErrorHandling.Suspend)
			//	.Schedule(bag => TimeSpan.FromSeconds(bag.DurationSeconds) - TimeSpan.FromSeconds(bag.WarningSeconds)).Do(then =>
			//		then.By(bag => OutputWriter.WriteLine("WARNING ABOUT TO CHARGE"))
			//			.Schedule(bag => TimeSpan.FromSeconds(bag.WarningSeconds)).Do(
			//				then => then.Decide(bag => bag.Active)
			//					.Branch(true, chargeBranch)
			//					.Branch(false, cancelBranch)
			//			)
			//	);

		}

		// .By(bag =>  bag.St { if(bag.TimesCharged == 2) bag. })


		public string Id => "Subscription";
		public int Version { get; } = 1;

	}


	public class TestWorkflow1 : IWorkflow {
		public void Build(IWorkflowBuilder<object> builder) {
			builder
				.UseDefaultErrorBehavior(WorkflowErrorHandling.Suspend)
				.StartWith<Step1>()
				.Then<Step2>()
				.Then<Step3>();
		}

		public string Id => "Workflow1";

		public int Version => 1;


		public class Step1 : StepBodyAsync {
			public static int Instance = 0;
			public Step1() {
				Instance++;
			}

			public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context) {
				await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}: Step 1 (Instance {Instance}) Started:");

				if (ErrorStep == 1)
					throw new InvalidOperationException($"{context.Workflow.Id}:Step {ErrorStep} error");

				if (Delay)
					await Task.Delay(2000);


				await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 1 (Instance {Instance}) Ended:");
				return ExecutionResult.Next();
			}

		}


		public class Step2 : StepBodyAsync {
			public static int Instance = 0;

			public Step2() {
				Instance++;
			}

			public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context) {
				await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 2 (Instance {Instance}) Started:");

				if (ErrorStep == 2)
					throw new InvalidOperationException($"{context.Workflow.Id}:Step {ErrorStep} error");

				if (Delay)
					await Task.Delay(2000);


				await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 2 (Instance {Instance}) Ended:");
				return ExecutionResult.Next();
			}
		}


		public class Step3 : StepBodyAsync {
			public static int Instance = 0;

			public Step3() {
				Instance++;
			}

			public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context) {
				await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 3 (Instance {Instance}) Started:");

				if (ErrorStep == 3)
					throw new InvalidOperationException($"{context.Workflow.Id}:Step {ErrorStep} error");

				if (Delay)
					await Task.Delay(2000);


				await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 3 (Instance {Instance}) Ended:");
				return ExecutionResult.Next();
			}
		}


	}


	public class TestWorkflow2 : IWorkflow {
		public void Build(IWorkflowBuilder<object> builder) {
			builder
				.UseDefaultErrorBehavior(WorkflowErrorHandling.Suspend)
				.StartWith(async context => {
					await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}: Step 1 Started:");
					if (ErrorStep == 1)
						throw new InvalidOperationException($"{context.Workflow.Id}:Step {ErrorStep} error");
					if (Delay)
						await Task.Delay(2000);
					await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 1 Ended:");
				})
				.Then(async context => {
					await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 2 Started:");

					if (ErrorStep == 2)
						throw new InvalidOperationException($"{context.Workflow.Id}:Step {ErrorStep} error");

					if (Delay)
						await Task.Delay(2000);


					await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 2 Ended:");
				})
				.Then(async context => {
					await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 3 Started:");

					if (ErrorStep == 3)
						throw new InvalidOperationException($"{context.Workflow.Id}:Step {ErrorStep} error");

					if (Delay)
						await Task.Delay(2000);


					await WorkflowTestForm.OutputWriter.WriteLineAsync($"{context.Workflow.Id}:Step 3 Ended:");
				});
		}

		public string Id => "Workflow2";

		public int Version => 1;

	}

	#endregion


}
