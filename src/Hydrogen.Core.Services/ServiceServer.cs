////-----------------------------------------------------------------------
//// <copyright file="ServiceServer.cs" company="Sphere 10 Software">
////
//// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// <author>Herman Schoenfeld</author>
//// <date>2018</date>
//// </copyright>
////-----------------------------------------------------------------------

//using System;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Description;
//using System.ServiceModel;
//using System.Threading;

//namespace Sphere10.Framework.Services {

//	public class ServiceServer<TServiceContract, TServiceImpl> : Disposable
//		where TServiceContract : class
//		where TServiceImpl : class, TServiceContract {
//		private readonly object _threadLock;
//		private readonly TServiceImpl _singletonImpl;
//		private ManualResetEvent _resetEvent;

//		public ServiceServer(Uri uri, InstanceContextMode contextMode = InstanceContextMode.PerSession)
//			: this(uri, BindingFactory.CreateStandardBinding(), contextMode:contextMode) {
//		}


//		public ServiceServer(Uri uri, TServiceImpl singletonInstance)
//			: this(uri, BindingFactory.CreateStandardBinding(), singletonInstance) {
//		}

//		public ServiceServer (Uri uri, Binding binding, TServiceImpl singletonInstance = null, InstanceContextMode contextMode = InstanceContextMode.PerSession) {
//			ServiceUri = uri;
//			Binding = binding;
//			Started = false;
//			_threadLock = new object();
//			_singletonImpl = singletonInstance;
//			ContextMode = _singletonImpl != null ? InstanceContextMode.Single : contextMode;
//		}

//		public Uri ServiceUri { get; private set;}

//		public Binding Binding { get; private set;}

//		public InstanceContextMode ContextMode { get; private set; }

//		public bool Started { get; private set; }

//		public void Start() {
//			lock (_threadLock) {
//				if (!Started) {
//					_resetEvent = new ManualResetEvent(false);
//					new Thread(() => {
//						var host = 
//							_singletonImpl != null ?  
//							new ServiceHost(_singletonImpl, new[] { ServiceUri }) :
//							new ServiceHost(typeof(TServiceImpl), new[] { ServiceUri });
//						using (host) {
//							host.Description.Behaviors.Add(
//								new ServiceMetadataBehavior {
//									HttpGetEnabled = true,
//									//MetadataExporter = {
//										//PolicyVersion = PolicyVersion.Policy15
//									//}
//								}
//							);
//							var debugBehavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
//							if (debugBehavior == null) {
//								host.Description.Behaviors.Add(
//									 new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
//							}

//							host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
//							host.Description.Behaviors.Find<ServiceDebugBehavior>().HttpHelpPageUrl = ServiceUri;
							
//							var behaviour = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
//							behaviour.InstanceContextMode = ContextMode;
							

//							host.AddServiceEndpoint(typeof(TServiceContract), Binding, ServiceUri);

//							SystemLog.Info("Service '{0}' Starting", typeof(TServiceImpl).FullName);
//						    try {
//						        host.Open();
//						    } catch (Exception error) {
//						        SystemLog.Exception(error);
//						        return;
//						    }
//						    SystemLog.Info("Service '{0}' Started", typeof(TServiceImpl).FullName);
//							Started = true;
//							_resetEvent.WaitOne();
//							SystemLog.Info("Service '{0}' Stopped", typeof(TServiceImpl).FullName);
//						}
//					}).Start();
//				}
//			}
//		}

//		public void Stop() {
//			lock (_threadLock) {
//				if (Started) {
//					_resetEvent.Set();
//					Started = false;
//				}
//			}
//		}

//	    protected override void FreeManagedResources() {
//			if (!Started)
//				Stop();
//	    }
//    }
//}
