using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sphere10.Framework;
using Terminal.Gui;
using Key = Sphere10.Framework.Key;

namespace VelocityNET.Presentation.Node {

	// change to IOC
	public static class Navigator {
		private const string TitlePrefix = "VelocityNET";
		private static FrameView _screenFrame;
		private static Screen _currentScreen;
		private static StatusBar _statusBar;
		private static Type[] _applicationScreenTypes;
		private static IDictionary<Type, Screen> _activatedScreens;




		public static void Start() {
			Application.Init();
			_activatedScreens = new Dictionary<Type, Screen>();
			_applicationScreenTypes = ScanApplicationScreens().ToArray();

			_activatedScreens =
				_applicationScreenTypes
				.Where(x => x.GetCustomAttributeOfType<LifetimeAttribute>().Lifetime == ScreenLifetime.Application)
				.ToDictionary(x => x, CreateScreen);

			Application.Top.Add(BuildMenu());
			//Application.Top.Add(BuildStatusBar());
			Show<DashboardScreen>();
			Application.Run();
		}

		private static void Show<TScreen>() where TScreen : Screen, new() {
			ShowScreen(typeof(TScreen));
		}



		public static void NewFile() {

		}

		public static void Close() {

		}

		public static void Quit() {
			Application.Top.Running = false;
		}

		private static IEnumerable<Type> ScanApplicationScreens() {
			// Dynamically build menu from attributes
			var screenTypes = Assembly.GetAssembly(typeof(Screen)).GetDerivedTypes<Screen>().Where(x => !x.IsAbstract).ToArray();

			// make sure each screen has correct attributes
			foreach (var screenType in screenTypes) {
				var error = false;

				// Screen Title check
				if (!screenType.GetCustomAttributesOfType<TitleAttribute>().Any()) {
					error = true;
					SystemLog.Error2(nameof(Navigator), nameof(ScanApplicationScreens), $"Screen '{screenType.FullName}' missing '{nameof(TitleAttribute)}' attribute");
				}

				// Lifetime check
				if (!screenType.GetCustomAttributesOfType<LifetimeAttribute>().Any()) {
					error = true;
					SystemLog.Error2(nameof(Navigator), nameof(ScanApplicationScreens), $"Screen '{screenType.FullName}' missing '{nameof(LifetimeAttribute)}' attribute");
				}

				// Menu Location check
				if (!screenType.GetCustomAttributesOfType<MenuLocationAttribute>().Any()) {
					error = true;
					SystemLog.Error2(nameof(Navigator), nameof(ScanApplicationScreens), $"Screen '{screenType.FullName}' missing '{nameof(MenuLocationAttribute)}' attribute");
				}

				if (!error)
					yield return screenType;

			}

			// NOTE: should load from plugin dll's as well

			var dict = new LookupEx<AppMenu, Tuple<MenuLocationAttribute, Type>>();

			foreach (var screenType in screenTypes) {
				var menuLoc = screenType.GetCustomAttributeOfType<MenuLocationAttribute>(throwOnMissing: false);
				var menyTitle = screenType.GetCustomAttributeOfType<MenuLocationAttribute>(throwOnMissing: false);
				if (menuLoc == default) {
					SystemLog.Error2(nameof(Navigator), nameof(BuildMenu), $"Screen '{screenType.FullName}' missing '{nameof(MenuLocationAttribute)}' attribute");
					continue;
				}


				dict.Add(menuLoc.Menu, Tuple.Create(menuLoc, screenType));
			}
		}

		private static MenuBar BuildMenu() {
			var screensByAppMenu =
				_applicationScreenTypes.ToLookup(
					x => x.GetCustomAttributeOfType<MenuLocationAttribute>().Menu,
					x => Tuple.Create(x.GetCustomAttributeOfType<MenuLocationAttribute>(), x)
				);

			var items =
				Enum
				.GetValues<AppMenu>()
				.Select(x => new MenuBarItem(
					x.GetDescription(),
					screensByAppMenu[x]
					.OrderBy(x => x.Item1.PreferredIndex)
					.Select(x => new MenuItem(x.Item1.Name, null, () => ShowScreen(x.Item2)))
					.ToArray()
				)).ToArray();


			return new MenuBar(items);

		}

		private static StatusBar BuildStatusBar() {
			var statusBar = new StatusBar() {
				X = 0,
				Y = Pos.AnchorEnd(),
				Width = Dim.Fill(),
				Height = 1,
				Visible = true,

			};

			statusBar.Items = new[] {
				new StatusItem(0, "Current Action: ", () => {}),
			};
			return statusBar;
		}


		private static void ShowScreen(Type screenType) {
			if (_currentScreen != null) {
				// Same screen
				if (screenType == _currentScreen.GetType())
					return;


				// Notify current screen disappearing
				_currentScreen.OnDisappearing(out var cancel);
				if (cancel)
					return;
			}

			// Create new screen
			if (!_activatedScreens.TryGetValue(screenType, out var newScreen)) {
				newScreen = CreateScreen(screenType);
				_activatedScreens[screenType] = newScreen;
			}
			newScreen.OnAppearing();

			if (_currentScreen != null) {
				// Remove/destroy current screen

				if (_screenFrame != null) {
					_screenFrame.Subviews[0].Remove(_currentScreen); // remove for not disposing
					Application.Top.Remove(_screenFrame);
					_screenFrame.Dispose();
					_screenFrame = null;
				}
				if (_currentScreen != null) {
					Application.Top.Remove(_currentScreen);
				}
				_currentScreen.OnDisappeared();

				switch (_currentScreen.GetType().GetCustomAttributeOfType<LifetimeAttribute>().Lifetime) {
					case ScreenLifetime.WhenVisible:
						_currentScreen.OnDestroy();
						_activatedScreens.Remove(_currentScreen.GetType());
						_currentScreen.Dispose();
						break;
					case ScreenLifetime.Application:
					case ScreenLifetime.LazyLoad:
						// keep activated
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			// Show new screen
			if (newScreen is MultiPartScreen) {
				// Multi-part screens have no screen frame
				newScreen.X = 0;
				newScreen.Y = 1;
				newScreen.Width = Dim.Fill();
				newScreen.Height = Dim.Fill(1);
				Application.Top.Add(newScreen);
			}
			else {
				_screenFrame = new FrameView($"{TitlePrefix} {newScreen.Title}") {
					X = 0,
					Y = 1, // 1 for menu  
					Width = Dim.Fill(),
					Height = Dim.Fill(1) // 1 for statusbar
				};
				_screenFrame.Add(newScreen);
				Application.Top.Add(_screenFrame);
			}
			Application.Top.LayoutSubviews();
			_currentScreen = newScreen;
			_currentScreen.OnAppeared();
		}

		private static Screen CreateScreen(Type screenType) {
			var screen = Activator.CreateInstance(screenType) as Screen;
			screen.OnCreate();
			return screen;
		}

	}

}

//public static MenuBarItem[] CreateMenus() {
//return new[] {
//	new MenuBarItem("_File", new[] {
//		new MenuItem ("_Dashboard", "", Navigator.ShowDashboard),
//		new MenuItem ("_Quit", "", Navigator.Quit)
//	}),
//	new MenuBarItem("_Wallet", new[] {
//		new MenuItem ("_Keys", null, Navigator.ShowWalleyKeysDialog),
//	}),
//	new MenuBarItem("_Settings", new[] {
//		new MenuItem ("_Storage", null, Navigator.ShowWalleyKeysDialog),
//		new MenuItem ("_Network", null, Navigator.ShowWalleyKeysDialog),
//		new MenuItem ("_Mining", null, Navigator.ShowWalleyKeysDialog),
//	}),
//	new MenuBarItem("_Explorer", new[] {
//		new MenuItem ("_Accounts", "", null),
//		new MenuItem ("_Blocks", "", null),
//		new MenuItem ("_Transactions", "", null),
//		new MenuItem ("_Network", "", null),
//	}),
//	new MenuBarItem("_Diagnostic", new[] {
//		new MenuItem ("_Logs", "", null),
//		new MenuItem ("_Traffic", "", null),
//		new MenuItem ("_Transactions", "", null),
//		new MenuItem ("_Nodes", "", null),
//	}),
//	new MenuBarItem("Developer", new[] {
//		new MenuItem ("_MiningSim", "", null),
//	})
//};
//}