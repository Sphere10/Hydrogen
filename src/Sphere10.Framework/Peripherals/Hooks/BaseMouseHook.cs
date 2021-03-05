//-----------------------------------------------------------------------
// <copyright file="BaseMouseHook.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading;


namespace Sphere10.Framework  {

	public abstract class BaseMouseHook : BaseDeviceHook, IMouseHook  {
		
		public event EventHandler<MouseMoveEvent>  MotionStart;
		public event EventHandler<MouseMoveEvent>  Motion;
		public event EventHandler<MouseMoveEvent>  MotionStop;
		public event EventHandler<MouseWheelEvent> Scroll;
		public event EventHandler<MouseClickEvent>  Click;
		public event EventHandler<MouseEvent>  Activity;
		
		public int CurrentMouseX { get; protected set; }
		public int CurrentMouseY { get; protected set; }
		public int LastClickX { get; protected set; }
		public int LastClickY { get; protected set; }
		public int MotionStartX { get; protected set; }
		public int MotionStartY { get; protected set; }

		protected IActiveApplicationMonitor ActiveApplicationMonitor { get; private set; }
		protected TimeSpan MovingStoppedInterval { get; private set; }
		protected int PreviousMouseX { get; private set; }
		protected int PreviousMouseY { get; private set; }


		private bool _mouseStoppedMonitorActive = false;
		private Thread _mouseStoppedMonitorThread = null;
		private bool _inMotion = false;
		private DateTime _lastMouseMoveOn = DateTime.UtcNow;
		private int _largestDistanceMovedSinceMotionStart = 0;
		private int _largestDistanceMovedSinceLastClick = 0;
		private DateTime _lastAppEventRaised = DateTime.UtcNow;
		SemaphoreSlim  _detectingMouseStoppedSemaphore = new SemaphoreSlim(1);

		public BaseMouseHook(IActiveApplicationMonitor activeApplicationMonitor, TimeSpan movingStoppedInterval) {
			MovingStoppedInterval = movingStoppedInterval;
			ActiveApplicationMonitor = activeApplicationMonitor;
			CurrentMouseX = 0;
			CurrentMouseY = 0;
			PreviousMouseX = 0;
			PreviousMouseY = 0;
			LastClickX = 0;
			LastClickY = 0;
			MotionStartX = 0;
			MotionStartY = 0;
		}

		public override void StartHook() {
			base.StartHook();
		}
		public override void StopHook() {
			base.StopHook();
		}

		public abstract void Simulate(MouseButton button, MouseButtonState buttonState, int screenX, int screenY);


		protected virtual void OnClick(MouseClickEvent mouseClickEvent) {
		}

		protected virtual void OnMotion(MouseMoveEvent mouseMoveEvent) {
		}

		protected virtual void OnMotionStart(MouseMoveEvent mouseMoveEvent) {
		}

		protected virtual void OnMotionStop(MouseMoveEvent mouseMoveEnt) {
		}

		protected virtual void OnScroll(MouseWheelEvent mouseScrollEvent) {
		}


		protected virtual void OnActivity(MouseEvent mouseEvent) {
		}

		protected void FireMotionStartEvent(MouseMoveEvent mouseMoveEvent) {
			OnMotionStart(mouseMoveEvent);
			if (MotionStart != null) {
				MotionStart(this, mouseMoveEvent);
			}
		}

		protected void FireMotionEvent(MouseMoveEvent mouseMoveEvent) {
			OnMotion(mouseMoveEvent);
			if (Motion != null) {
				Motion(this, mouseMoveEvent);
			}
		}

		protected void FireMotionStopEvent(MouseMoveEvent mouseMoveEvent) {
			OnMotionStop(mouseMoveEvent);
			if (MotionStop != null) {
				MotionStop(this, mouseMoveEvent);
			}
		}

		protected void FireClickEvent(MouseClickEvent mouseClickEvent) {
			OnClick(mouseClickEvent);
			if (Click != null) {
				Click(this, mouseClickEvent);
			}
		}

		protected void FireScrollEvent(MouseWheelEvent mouseScrollEvent) {
			OnScroll(mouseScrollEvent);
			if (Scroll != null) {
				Scroll(this, mouseScrollEvent);
			}
		}
		protected void FireActivityEvent(MouseEvent mouseEvent) {
			OnActivity(mouseEvent);
			if (Activity != null) {
				Activity(this, mouseEvent);
			}
		}



		
		protected virtual void ProcessMouseActivity(int currentX, int currentY, MouseButton buttonClicked, MouseButtonState buttonState, MouseClickType clickType, int wheelDelta) {
			lock (this) {
				string activeProcessName = ActiveApplicationMonitor.GetActiveApplicationName();
				DateTime now = DateTime.Now;
				if (buttonClicked != MouseButton.None) {
					#region Mouse was clicked
					LastClickX = CurrentMouseX;
					LastClickY = CurrentMouseY;
					_largestDistanceMovedSinceLastClick = 0;

					// Fire click event
					FireClickEvent(
						new MouseClickEvent(
							activeProcessName,
							currentX,
							currentY,
							buttonClicked,
							buttonState,
							clickType,
							now
							)
						);
					#endregion
				}

				if (CurrentMouseX != currentX || CurrentMouseY != currentY) {
					#region Mouse was moved
					_lastMouseMoveOn = now;
					PreviousMouseX = CurrentMouseX;
					PreviousMouseY = CurrentMouseY;
					CurrentMouseX = currentX;
					CurrentMouseY = currentY;

					// Keep track of distances moved by mouse
					int distanceFromStart =
						(int)
						Math.Round(Math.Sqrt(Math.Pow(CurrentMouseX - MotionStartX, 2) + Math.Pow(CurrentMouseY - MotionStartY, 2)), 0);
					if (distanceFromStart > _largestDistanceMovedSinceMotionStart) {
						_largestDistanceMovedSinceMotionStart = distanceFromStart;
					}
					int distanceFromClick =
						(int) Math.Round(Math.Sqrt(Math.Pow(CurrentMouseX - LastClickX, 2) + Math.Pow(CurrentMouseY - LastClickY, 2)), 0);
					if (distanceFromClick > _largestDistanceMovedSinceLastClick) {
						_largestDistanceMovedSinceLastClick = distanceFromClick;
					}
					int distanceFromLastEvent =
						(int)
						Math.Round(Math.Sqrt(Math.Pow(CurrentMouseX - PreviousMouseX, 2) + Math.Pow(CurrentMouseY - PreviousMouseY, 2)), 0);

					if (!_inMotion) {
						// Mouse was previously stationary, now begins motion
						_inMotion = true;
						_largestDistanceMovedSinceMotionStart = 0;
						MotionStartX = CurrentMouseX;
						MotionStartY = CurrentMouseY;
						PreviousMouseX = CurrentMouseX;
						PreviousMouseY = CurrentMouseY;

						// Raise move start moving event
						FireMotionStartEvent(
							new MouseMoveEvent(
								activeProcessName,
								MouseMotionType.Started,
								CurrentMouseX,
								CurrentMouseY,
								distanceFromStart,
								distanceFromLastEvent,
								distanceFromClick,
								now
								)
							);
					}


					if (_inMotion) {
						#region Detect mouse stopped
						if (_detectingMouseStoppedSemaphore.Wait(0)) {
							// We detect a stop in motion when the mouse hasn't moved within MovingStoppedInterval
							Tools.Lambda.ActionAsAsyncronous(
								() => {
									try {
										while (DateTime.Now.Subtract(_lastMouseMoveOn) < MovingStoppedInterval) {
											Thread.Sleep(MovingStoppedInterval);
										}
										_inMotion = false;

										// The mouse hasn't moved in MovingStoppedInterval, so fire a mouse stopped event
										FireMotionStopEvent(
											new MouseMoveEvent(
												ActiveApplicationMonitor.GetActiveApplicationName(),
												MouseMotionType.Stopped,
												CurrentMouseX,
												CurrentMouseY,
												_largestDistanceMovedSinceMotionStart,
												0,
												_largestDistanceMovedSinceLastClick,
												DateTime.UtcNow
											)
										);
									} finally {
										_detectingMouseStoppedSemaphore.Release();
									}
								}).Invoke();
						#endregion
						}
					}

					// Raise mouse motion event
					FireMotionEvent(
						new MouseMoveEvent(
							activeProcessName,
							MouseMotionType.Move,
							CurrentMouseX,
							CurrentMouseY,
							distanceFromStart,
							distanceFromLastEvent,
							distanceFromClick,
							now
							)
						);
					#endregion
				}

				if (wheelDelta != 0) {
					#region Mouse was scrolled
					FireScrollEvent(
						new MouseWheelEvent (
							activeProcessName,
							currentX,
							currentY,
							now,
							wheelDelta
						)
					);
					#endregion
				}

				// Raise generic mouse activity event
				FireActivityEvent(
					new MouseEvent(
						activeProcessName,
						currentX,
						currentY,
						now
						)
					);
			}
		}
		
	}
}
