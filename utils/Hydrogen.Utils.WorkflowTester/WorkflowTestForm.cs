using Hydrogen.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Hydrogen.Utils.WorkflowTester {
	public partial class WorkflowTestForm : Form {

		protected static TextWriter OutputWriter;
		protected static int ErrorStep;
		protected static bool Delay;
		protected IWorkflowHost _workflowHost;

		protected List<string> _knownWorkflowInstances;
		private static IServiceProvider ConfigureServices() {
			//setup dependency injection
			IServiceCollection services = new ServiceCollection();
			services.AddLogging();
			//services.AddWorkflow();
			services.AddWorkflow(x => x.UseSqlServer(@"Server=.\;Database=Hydrogen_Workflow_Tester;Trusted_Connection=True;", true, true));

			services.AddSingleton<Step2>();

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
			_workflowHost.RegisterWorkflow<TestWorkflow>();
			_workflowHost.Start();
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
			return OutputWriter.WriteLineAsync($@"ID: {workflow.Id} WorkflowDefinitionId: {workflow.WorkflowDefinitionId} Version: {workflow.Version} Description: {workflow.Description} Reference: {workflow.Reference} Execution Pointers: {workflow.ExecutionPointers.Select(x => x.ToString()).ToDelimittedString(" ")} NextExecution: {workflow.NextExecution} Status: {workflow.Status} Data: {workflow.Data} CreateTime: {workflow.CreateTime} CompleteTime: {workflow.CompleteTime}");
		}



		#region Handlers

		private async void _test1Button_Click(object sender, EventArgs e) {
			try {

				//start the workflow host
				var workflowInstanceId = await _workflowHost.StartWorkflow("Step1");
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

		public class TestWorkflow : IWorkflow {
			public void Build(IWorkflowBuilder<object> builder) {
				builder
					.UseDefaultErrorBehavior(WorkflowErrorHandling.Suspend)
					.StartWith<Step1>()
					.Then<Step2>()
					.Then<Step3>();
			}

			public string Id => "Step1";

			public int Version => 1;

		}

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

		#endregion

	}
}