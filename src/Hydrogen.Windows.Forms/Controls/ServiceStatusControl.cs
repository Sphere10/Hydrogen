﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;


namespace Hydrogen.Windows.Forms;

public partial class ServiceStatusControl : UserControlEx {
	public const int RefreshRateSec = 2;

	private ServiceStatus _serviceStatus;

	public ServiceStatusControl() {
		InitializeComponent();
	}

	[Category("Behavior")]
	[DefaultValue(false)]
	public bool UseNssm { get; set; }

	public ServiceStatus Status {
		get => _serviceStatus;
		set {
			_serviceStatus = value;
			if (Tools.WinTool.TryGetServiceController(ServiceName, out var serviceController)) {

				switch (_serviceStatus) {
					case ServiceStatus.NotInstalled:
						_trafficLight.BackColor = Color.Gray;
						_trafficLightLabel.Text = "Not installed";
						_serviceDetailLabel.Text = $"{ServiceName} is not intalled";
						_serviceButton.Visible = false;
						break;
					case ServiceStatus.Starting:
						_trafficLight.BackColor = Color.Orange;
						_trafficLightLabel.Text = "Starting";
						_serviceDetailLabel.Text = $"{serviceController.DisplayName} is starting";
						_serviceButton.Visible = false;
						break;
					case ServiceStatus.Started:
						_trafficLight.BackColor = Color.Green;
						_trafficLightLabel.Text = "Started";
						_serviceDetailLabel.Text = serviceController.DisplayName + " is running";
						_serviceButton.Text = "Stop";
						_serviceButton.Visible = true;
						break;
					case ServiceStatus.Stopping:
						_trafficLight.BackColor = Color.Orange;
						_trafficLightLabel.Text = "Stopping";
						_serviceDetailLabel.Text = "Service is stopping";
						_serviceButton.Visible = false;
						break;
					case ServiceStatus.Stopped:
						_trafficLight.BackColor = Color.Red;
						_trafficLightLabel.Text = "Stopped";
						_serviceDetailLabel.Text = $"{serviceController.DisplayName} is stopped";
						_serviceButton.Text = "Start";
						_serviceButton.Visible = true;
						break;
					case ServiceStatus.Error:
						_trafficLight.BackColor = Color.Red;
						_trafficLightLabel.Text = "Error";
						_serviceButton.Visible = false;
						break;
					default:
						break;
				}
			} else {
				_trafficLight.BackColor = Color.Gray;
				_trafficLightLabel.Text = "Not installed";
				_serviceDetailLabel.Text = $"{ServiceName} is not intalled";
				_serviceButton.Visible = false;
			}
		}
	}

	[Category("Behavior")]
	public string ServiceName { get; set; }

	public void Start() {
		var job = JobBuilder
				  .For(MonitorService)
				  .Repeat.OnInterval(DateTime.Now, TimeSpan.FromSeconds(RefreshRateSec))
				  .RunSyncronously()
				  .Build();
		Scheduler.Synchronous.AddJob(job);
		ParentForm.Disposed += (sender, args) => {
			Scheduler.Synchronous.RemoveJob(job);
		};
	}




	private void MonitorService() {
		try {
			ServiceStatus newStatus = default;
			try {
				if (!Tools.WinTool.TryGetServiceController(ServiceName, out var serviceController)) {
					newStatus = ServiceStatus.NotInstalled;
					return;
				}

				switch (serviceController.Status) {
					case ServiceControllerStatus.Stopped:
						newStatus = ServiceStatus.Stopped;
						break;
					case ServiceControllerStatus.StartPending:
						newStatus = ServiceStatus.Starting;
						break;
					case ServiceControllerStatus.StopPending:
						newStatus = ServiceStatus.Stopping;
						break;
					case ServiceControllerStatus.Running:
						newStatus = ServiceStatus.Started;
						break;
					case ServiceControllerStatus.ContinuePending:
					case ServiceControllerStatus.PausePending:
					case ServiceControllerStatus.Paused:
					default:
						newStatus = ServiceStatus.Error;
						break;
				}

			} finally {
				this.InvokeEx(() => Status = newStatus);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private async void _serviceButton_Click(object sender, EventArgs e) {
		if (_serviceStatus is ServiceStatus.Started or ServiceStatus.Starting) {
			if (UseNssm) {
				var nssmSentry = new NssmSentry();
				await nssmSentry.StopAsync(ServiceName);
			} else {
				if (!Tools.WinTool.TryGetServiceController(ServiceName, out var serviceController))
					serviceController.Stop();
			}
		} else {
			if (UseNssm) {
				var nssmSentry = new NssmSentry();
				await nssmSentry.StartAsync(ServiceName);
			} else {
				if (!Tools.WinTool.TryGetServiceController(ServiceName, out var serviceController))
					serviceController.Start();

			}
		}
	}
}

