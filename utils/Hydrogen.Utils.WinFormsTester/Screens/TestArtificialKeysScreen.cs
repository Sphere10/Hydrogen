// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Data;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class TestArtificialKeysScreen : ApplicationScreen {
	public TestArtificialKeysScreen() {
		InitializeComponent();
	}

	private void _testButton_Click(object sender, EventArgs e) {
		try {
			var artificialKeys =

				#region Create test object

				new ArtificialKeys {
					Tables = new[] {
						new ArtificialKeys.Table() {
							Name = "Table1",
							PrimaryKey = new ArtificialKeys.PrimaryKey {
								Name = "PK1",
								AutoIncrement = true,
								Columns = new[] {
									new ArtificialKeys.Column {
										Name = "ID"
									}
								}
							},
							ForeignKeys = new[] {
								new ArtificialKeys.ForeignKey {
									Name = "PK1",
									ReferenceTable = "Table2",
									Columns = new[] {
										new ArtificialKeys.Column {
											Name = "ID"
										}
									}
								}
							},
							UniqueConstraints = new[] {
								new ArtificialKeys.UniqueConstraint {
									Name = "UC1",
									Columns = new[] {
										new ArtificialKeys.Column {
											Name = "X"
										},
										new ArtificialKeys.Column {
											Name = "Y"
										},

									}
								}
							},
						},
						new ArtificialKeys.Table() {
							Name = "Table2",
							PrimaryKey = new ArtificialKeys.PrimaryKey {
								Name = "PK1",
								Sequence = "Sequence1",
								Columns = new[] {
									new ArtificialKeys.Column {
										Name = "A"
									},
									new ArtificialKeys.Column {
										Name = "B"
									},
									new ArtificialKeys.Column {
										Name = "C"
									}
								}
							}
						}
					}
				};

			#endregion

			var serialized = Tools.Xml.WriteToString(artificialKeys);

			var deserialized = Tools.Xml.ReadFromString<ArtificialKeys>(serialized);

			textBox1.Clear();
			textBox1.AppendText(serialized);

			var reserialized = Tools.Xml.WriteToString(deserialized);
			if (reserialized != serialized) {
				textBox1.AppendText("Deserialization did not match - " + Environment.NewLine);
				textBox1.AppendText(reserialized);
			}
		} catch (Exception error) {
			textBox1.Clear();
			textBox1.AppendText(error.ToDiagnosticString());
		}
	}
}
//<ArtificialKeys>
//    <Table name = "Table1">
//        <PrimaryKey name="PK1" autoIncrement="true">
//            <Column name="ID"/>
//        </PrimaryKey>
//	
//        <ForeignKey name="FK1" foreignKeyTable="Table2">
//            <Column name="A" linksTo="U">
//            <Column name="B" linksTo="V">
//            <Column name="C" linksTo="W">
//        </ForeignKey>
//
//        <UniqueConstraint name="UC1">
//            <Column>X</Column>
//            <Column>Y</Column>
//        </UniqueConstraint>
//    </Table>
//
//    <Table name="Table2">
//        <PrimaryKey name="PK1" sequence="Sequence1">
//            <Column name="A"/>
//        </PrimaryKey>
//    </Table>
//
//</AritificialKeys>
