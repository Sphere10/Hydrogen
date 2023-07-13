// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Hydrogen;
//using Hydrogen.Windows.Forms;
//using HydrogenTester.WinForms;
//using HydrogenTester.WinForms.Wizard;
//using Hydrogen.Windows.Forms;


//namespace Hydrogen.Utils.WinFormsTester {
//	public partial class TestLauncherScreen : ApplicationScreen {
//		public TestLauncherScreen() {
//			InitializeComponent();
//		}

//		private void _expandoTesterButton_Click(object sender, EventArgs e) {
//			OpenForm<ExpandoTester>();
//		}

//		private void _applicationServicesTesterButton_Click(object sender, EventArgs e) {
//			OpenForm<ApplicationServicesTester>();
//		}

//		private void _applicationUITesterButton_Click(object sender, EventArgs e) {
//			throw new NotImplementedException();
//		}

//		private void _paragraphBuilderButton_Click(object sender, EventArgs e) {
//			OpenForm<ParagraphBuilderForm>();
//		}

//		private void _expandingCircleButton_Click(object sender, EventArgs e) {
//			//OpenForm<ExpandingCircleTester>();

//			var point = PointToScreen(Location);
//			ExpandingCircle.ShowExpandingCircle(point.X, point.Y, Color.Green, 10, 100,5 );
//		}


//		private void _fakeTransparentFormButton_Click(object sender, EventArgs e) {
//			OpenForm<FakeTransparentFormTester>();
//		}


//		private void OpenForm<T>() where T : Form, new() {
//			try {
//				T f = new T();
//				f.Show(this);
//			} catch (Exception error) {
//				ExceptionDialog.Show(this, error);
//			}
//		}

//		private void _mergableToolStripsButton_Click(object sender, EventArgs e) {
//			OpenForm<VisualInheritanceFixerSub>();
//		}

//		private void _hooksTesterButton_Click(object sender, EventArgs e) {
//			OpenForm<HooksForm>();
//		}

//		private void _testSounds_Click(object sender, EventArgs e) {
//			OpenForm<TestSoundsForm>();
//		}

//		private void _decayGaugeButton_Click(object sender, EventArgs e) {
//			OpenForm<DecayGaugeForm>();
//		}

//        private void _testTabControlButton_Click(object sender, EventArgs e)
//        {
//            OpenForm<TabControlTestForm>();
//        }

//		private void _testArtificialKeysSerialization_Click(object sender, EventArgs e) {
//			OpenForm<TestArtificialKeysForm>();
//		}

//		private void _enumComboTester_Click(object sender, EventArgs e) {
//			OpenForm<EnumComboForm>();
//		}

//		private void button1_Click(object sender, EventArgs e) {
//			OpenForm<CompressionTestForm>();
//		}

//		private void _appointmentBookButton_Click(object sender, EventArgs e) {
//			OpenForm<AppointmentBookForm>();
//		}

//		private void _flagsCheckBoxListTesterButton_Click(object sender, EventArgs e) {
//			OpenForm<FlagsCheckedBoxListTestForm>();
//		}

//		private void _crudTesterButton_Click(object sender, EventArgs e) {
//			OpenForm<CrudTestForm>();
//		}

//		private void _loadingCircleButton_Click(object sender, EventArgs e) {
//			OpenForm<LoadingCircleTestForm>();
//		}

//		private void _cueButton_Click(object sender, EventArgs e) {
//			OpenForm<PlaceHolderTestForm>();
//		}

//		private void _padLockTestForm_Click(object sender, EventArgs e) {
//			OpenForm<PadLockTestForm>();
//		}

//		private void _passwordDialogButton_Click(object sender, EventArgs e) {
//			OpenForm<PasswordDialogTestForm>();
//		}

//		private void _validationIndicatorButton_Click(object sender, EventArgs e) {
//			OpenForm<ValidationIndicatorTestForm>();
//		}

//		private void _regionToolButton_Click(object sender, EventArgs e) {
//			OpenForm<RegionToolTestForm>();
//		}

//		private void _customComboBoxButton_Click(object sender, EventArgs e) {
//			OpenForm<CustomComboBoxForm>();
//		}

//		private void _miscButton_Click(object sender, EventArgs e) {
//			OpenForm<MiscTestForm>();
//		}

//		private void _connectionPanelButton_Click(object sender, EventArgs e) {
//			OpenForm<ConnectionPanelTestForm>();
//		}

//		private void _draggableControlsButton_Click(object sender, EventArgs e) {
//			OpenForm<DraggableControlsTestForm>();
//		}

//		private void _encryptedCompressionTest_Click(object sender, EventArgs e) {
//			OpenForm<EncryptedCompressionTestForm>();
//		}

//        private void _CBACSVConverterButton_Click(object sender, EventArgs e)
//        {
//            OpenForm<CBACSVConverterForm>();
//        }

//        private void _settingsTestButton_Click(object sender, EventArgs e) {
//            OpenForm<SettingsTest>();
//        }

//        private void _imageResizeTestButton_Click(object sender, EventArgs e) {
//            OpenForm<ImageResizeForm>();
//        }

//        private void _schedulerTestButton_Click(object sender, EventArgs e) {
//            OpenForm<ScheduleTestForm>();
//        }

//        private void _observableCollectionsTest_Click(object sender, EventArgs e) {
//            OpenForm<ObservableCollectionsTest>();
//        }

//        private void _pathSelectorFormTestButton_Click(object sender, EventArgs e) {
//            OpenForm<PathSelectorTestForm>();
//        }

//        private void _connectionBarsButton_Click(object sender, EventArgs e) {
//            OpenForm<ConnectionBarTestForm>();
//        }

//        private void _textAreasButton_Click(object sender, EventArgs e) {
//            OpenForm<TextAreaTests>();
//        }

//        private void _urlIDGenButton_Click(object sender, EventArgs e) {
//            OpenForm<UrlIDTestForm>();
//        }


//        private void _levelDBEditorButton_Click(object sender, EventArgs e) {
//            //OpenForm<LevelDBTestForm>();
//        }

//        private void _componentRegistryButton_Click(object sender, EventArgs e) {
//            OpenForm<ComponentRegistryForm>();
//        }
//        private async void _wizardButton_Click(object sender, EventArgs e) {
//            try {
//                var wiz = new ActionWizardManager(
//                    "Wizard Demo",
//                    new Dictionary<string, object>(), 
//                    new[] {
//                        new WizardDialog1(),
//                        new WizardDialog1(),
//                        new WizardDialog1(),
//                    },
//                    async d => {
//                        await Task.Delay(500);
//                        return Result.Default;
//                    },
//                    (d) => Result.Error("Cancel prohibited"));
//                await wiz.Start(ParentForm);
//            } catch (Exception error) {
//                ExceptionDialog.Show(this,error);
//            }
//        }

//		private void _iocButton_Click(object sender, EventArgs e) {
//			try {
//				OpenForm<IoCTestForm>();
//			} catch (Exception error) {
//				ExceptionDialog.Show(error);
//			}
//		}
//	}
//}


