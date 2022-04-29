//-----------------------------------------------------------------------
// <copyright file="TransactionScopeCompatibilityTests.cs" company="Sphere 10 Software">
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
using System.Transactions;
using NUnit.Framework;
using Sphere10.Framework.Data;
using Sphere10.Framework.NUnit;
using Sphere10.Framework.UnitTests.DAC;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class TransactionScopeCompatibilityTests : DACTestFixture {
 
        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Commit_AutoEnlist(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var txn = new TransactionScope(TransactionScopeOption.Required)) {
                    using (dac.BeginScope(true)) {
                        dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                        txn.Complete();
                    }                    
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Commit_ManualEnlist(DBMSType dbmsType) {

            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (var txn = new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                        txn.Complete();
                    }                    
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Rollback_AutoEnlist(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (new TransactionScope(TransactionScopeOption.Required)) {
                    using (dac.BeginScope(true)) {
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                    }
                }
                Assert.AreEqual(0, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Rollback_ManualEnlist_1(DBMSType dbmsType) {

            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                    }
                }
                Assert.AreEqual(0, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Rollback_ManualEnlist_2(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                    }
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void MixedScope_SystemThenDAC_1(DBMSType dbmsType) {

            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (var tx = new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                        tx.Complete();
                    }
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                    scope.Commit();
                }
                Assert.AreEqual(2, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void MixedScope_SystemThenDAC_2(DBMSType dbmsType) {

            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                    }
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                    scope.Commit();
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void MixedScope_DACThenSystem_1(DBMSType dbmsType) {

            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                    scope.Commit();

                    using (var tx = new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                        tx.Complete();
                    }
                }
                Assert.AreEqual(2, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void MixedScope_DACThenSystem_2(DBMSType dbmsType) {

            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                    scope.Commit();

                    using (new TransactionScope(TransactionScopeOption.Required)) {
                        scope.EnlistInSystemTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                    }
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Error_BeginTransactionAfterAutoEnlisted(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (new TransactionScope(TransactionScopeOption.Required)) {
                    using (var scope = dac.BeginScope(true)) {
                        Assert.Catch(() => scope.BeginTransaction());
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Error_ManualEnlistAfterBeginTransaction(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    using (new TransactionScope(TransactionScopeOption.Required)) {
                        Assert.Catch( () => scope.EnlistInSystemTransaction());
                    }
                    scope.Commit();
                }
            }
        }
    }

}
