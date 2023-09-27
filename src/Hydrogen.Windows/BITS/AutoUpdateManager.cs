// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Diagnostics;


namespace Hydrogen.Windows.BITS;

public interface IPatch {
	void Execute();
}


public class AutoUpdater {
	Uri _patchFile;
	BitsJob _downloadJob;
	BitsManager _bitsManager;
	string _productName;

	public AutoUpdater(string productName, Uri patchUri) {
		_productName = productName;
		_downloadJob = null;
		_patchFile = patchUri;
		_bitsManager = new BitsManager();

		// delete the downloaded patch file if it exists
		if (File.Exists(LocalPatchFile)) {
			try {
				File.Delete(LocalPatchFile);
			} catch {
			}
		}
	}

	public AutoUpdater()
		: this(string.Empty, new Uri("")) {
	}


	public void Start() {
		try {
			BitsJob downloadJob = GetDownloadManifestJob();
		} catch {
			// ignore
		}
	}

	private BitsJob GetDownloadManifestJob() {
		if (_downloadJob == null) {
			string jobName = string.Format(
				"{0} Patch Manifest",
				_productName
			);

			// delete all relevant jobs
			foreach (BitsJob job in _bitsManager.EnumJobs(JobOwner.CurrentUser).Values) {
				if (job.DisplayName == jobName) {
					job.Cancel();
				}
			}

			// if job is still null then create the job
			_downloadJob = _bitsManager.CreateJob(
				jobName,
				JobType.Download
			);
			_downloadJob.NotificationFlags =
				NotificationFlags.JobTransferred |
				NotificationFlags.JobErrorOccured |
				NotificationFlags.JobModified;
			_downloadJob.ProxySettings.ProxyUsage = ProxyUsage.AutoDetect;
			if (File.Exists(LocalPatchFile)) {
				try {
					File.Delete(LocalPatchFile);
				} catch {
				}
			}
			_downloadJob.AddFile(
				_patchFile.ToString(),
				LocalPatchFile
			);
			_downloadJob.Resume();

			AddEventListenersToJob(_downloadJob);
		}
		return _downloadJob;
	}

	public string LocalPatchFile {
		get {
			string localPatchFile =
				string.Format(
					"{0}{1}RSIWarriorPatch.cs",
					Tools.Text.FormatEx("{SystemDataDir}"),
					Path.DirectorySeparatorChar
				);
			return localPatchFile;
		}
	}


	public Uri PatchUrl {
		get { return _patchFile; }
		set { _patchFile = value; }
	}

	private void AddEventListenersToJob(BitsJob job) {
		job.OnJobErrorEvent += new EventHandler<JobErrorNotificationEventArgs>(downloadJob_OnJobErrorEvent);
		job.OnJobModifiedEvent += new EventHandler<JobNotificationEventArgs>(downloadJob_OnJobModifiedEvent);
		job.OnJobTransferredEvent += new EventHandler<JobNotificationEventArgs>(downloadJob_OnJobTransferredEvent);
	}

	void downloadJob_OnJobTransferredEvent(object sender, JobNotificationEventArgs e) {
		try {
			// complete the job
			_downloadJob.Complete();
			CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
			ICodeCompiler compiler = provider.CreateCompiler();
			CompilerParameters cpar = new CompilerParameters();
			cpar.GenerateInMemory = true;
			cpar.GenerateExecutable = false;
			cpar.ReferencedAssemblies.Add(
				Process.GetCurrentProcess().MainModule.FileName
			);
			cpar.ReferencedAssemblies.Add("System.dll");
			cpar.ReferencedAssemblies.Add("System.Drawing.dll");
			cpar.ReferencedAssemblies.Add("System.Windows.Forms.dll");
			cpar.ReferencedAssemblies.Add("RSIWarrior.Common.dll");
			cpar.ReferencedAssemblies.Add("RSIWarrior.AutoUpdate.dll");
			CompilerResults cres = compiler.CompileAssemblyFromFile(
				cpar,
				LocalPatchFile
			);

			foreach (CompilerError ce in cres.Errors) {
				throw new Exception(ce.ErrorText);
			}
			if (cres.Errors.Count == 0 && cres.CompiledAssembly != null) {
				Type[] exportedTypes = cres.CompiledAssembly.GetExportedTypes();
				foreach (Type type in exportedTypes) {
					if (type.GetInterface("RSIWarrior.AutoUpdate.IPatch") != null &&
					    !type.IsAbstract) {
						IPatch patch = (IPatch)Activator.CreateInstance(type);
						patch.Execute();
					}
				}
			}

		} catch (Exception ex) {
		} finally {
			try {
				File.Delete(LocalPatchFile);
			} catch {
			}
		}
	}

	void downloadJob_OnJobModifiedEvent(object sender, JobNotificationEventArgs e) {
	}

	void downloadJob_OnJobErrorEvent(object sender, JobErrorNotificationEventArgs e) {
	}
}
