// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;


namespace Hydrogen;

public abstract class BaseMouseHook : BaseDeviceHook, IMouseHook {

	public event EventHandler<MouseMoveEvent> MotionStart;
	public event EventHandler<MouseMoveEvent> Motion;
	public event EventHandler<MouseMoveEvent> MotionStop;
	public event EventHandler<MouseWheelEvent> Scroll;
	public event EventHandler<MouseClickEvent> Click;
	public event EventHandler<MouseEvent> Activity;

	private bool _inMotion = false;
	private DateTime _lastMouseMoveOn = DateTime.UtcNow;
	private double _distanceMovedSinceStart;
	private double _distanceMovedSinceClick;
	private readonly SemaphoreSlim _detectingMouseStoppedSemaphore = new(1);

	protected BaseMouseHook(IActiveApplicationMonitor activeApplicationMonitor, TimeSpan movingStoppedInterval) {
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
		_distanceMovedSinceStart = 0D;
		_distanceMovedSinceClick = 0D;
	}

	public TimeSpan MovingStoppedInterval { get; set; }
	public int CurrentMouseX { get; protected set; }
	public int CurrentMouseY { get; protected set; }
	public int LastClickX { get; protected set; }
	public int LastClickY { get; protected set; }
	public int MotionStartX { get; protected set; }
	public int MotionStartY { get; protected set; }
	protected IActiveApplicationMonitor ActiveApplicationMonitor { get; private set; }
	
	protected int PreviousMouseX { get; private set; }
	protected int PreviousMouseY { get; private set; }

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
				_distanceMovedSinceClick = 0D;
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
				var distanceSinceLastEvent = Math.Sqrt(Math.Pow(CurrentMouseX - PreviousMouseX, 2) + Math.Pow(CurrentMouseY - PreviousMouseY, 2));
				_distanceMovedSinceStart += distanceSinceLastEvent;
				_distanceMovedSinceClick += distanceSinceLastEvent;

				if (!_inMotion) {
					// Mouse was previously stationary, now begins motion
					_inMotion = true;
					_distanceMovedSinceStart = 0D;
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
							_distanceMovedSinceStart,
							distanceSinceLastEvent,
							_distanceMovedSinceClick,
							now
						)
					);
				}

				if (_inMotion) {

					#region Detect mouse stopped

					if (_detectingMouseStoppedSemaphore.Wait(0)) {
						// We detect a stop in motion when the mouse hasn't moved within MovingStoppedInterval
						if (_inMotion) {
							Tools.Lambda.ActionAsAsyncronous(
								() => {
									try {
										while (DateTime.Now.Subtract(_lastMouseMoveOn) < MovingStoppedInterval) {
											Thread.Sleep(MovingStoppedInterval / 2);
										}
										_inMotion = false;

										// The mouse hasn't moved in MovingStoppedInterval, so fire a mouse stopped event
										FireMotionStopEvent(
											new MouseMoveEvent(
												ActiveApplicationMonitor.GetActiveApplicationName(),
												MouseMotionType.Stopped,
												CurrentMouseX,
												CurrentMouseY,
												_distanceMovedSinceStart,
												distanceSinceLastEvent,
												_distanceMovedSinceClick,
												DateTime.UtcNow
											)
										);
									} finally {
										_detectingMouseStoppedSemaphore.Release();
									}
								}
							).Invoke();
						}

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
						_distanceMovedSinceStart,
						distanceSinceLastEvent,
						_distanceMovedSinceClick,
						now
					)
				);

				#endregion

			}

			if (wheelDelta != 0) {

				#region Mouse was scrolled

				FireScrollEvent(
					new MouseWheelEvent(
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
