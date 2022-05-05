//-----------------------------------------------------------------------
// <copyright file="DACScopeTests.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;
using Hydrogen.NUnit;


namespace Hydrogen.Data.Tests {

    [TestFixture]
    public class DACScopeTests : DACTestFixture {

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_SameDAC_1(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(false)) {
                    using (var scope2 = dac.BeginScope(false)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_SameDAC_2(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(false)) {
                    using (var scope2 = dac.BeginScope(true)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_SameDAC_3(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (var scope2 = dac.BeginScope(false)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_SameDAC_4(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    using (var scope2 = dac.BeginScope(true)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_DifferentDAC_1(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var dac2 = DuplicateDAC(dac);
                using (var scope = dac.BeginScope(false)) {
                    using (var scope2 = dac2.BeginScope(false)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_DifferentDAC_2(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var dac2 = DuplicateDAC(dac);
                using (var scope = dac.BeginScope(false)) {
                    using (var scope2 = dac2.BeginScope(true)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_DifferentDAC_3(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var dac2 = DuplicateDAC(dac);
                using (var scope = dac.BeginScope(true)) {
                    using (var scope2 = dac2.BeginScope(false)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ReuseConnection_DifferentDAC_4(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var dac2 = DuplicateDAC(dac);
                using (var scope = dac.BeginScope(true)) {
                    using (var scope2 = dac2.BeginScope(true)) {
                        Assert.AreSame(scope.Connection, scope2.Connection);
                    }
                }
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public async Task ReuseConnection_DifferentDAC_Async1(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var dac2 = DuplicateDAC(dac);
                using (var scope = dac.BeginScope(true)) {
                    await AssertConnectionPropagationAsync(scope.Connection, dac2);
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public async Task ReuseConnection_DifferentDAC_Async2(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var dac2 = DuplicateDAC(dac);
                using (var scope = dac.BeginScope(true)) {
                    await Task.Run( () => AssertConnectionPropagationAsync(scope.Connection, dac2));
                }
            }
        }

        private async Task AssertConnectionPropagationAsync(IDbConnection expectedConnectionObj, IDAC dac2) {
            using (var scope2 = dac2.BeginScope(true)) {
                Assert.AreSame(expectedConnectionObj, scope2.Connection);
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void Error_TransactionNotClosed(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                var scope = dac.BeginScope(true);
                scope.BeginTransaction();
                dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                Assert.Catch( () => scope.Dispose());
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void CommitTransaction_1(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                    scope.Commit();
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }
        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void RollbackTransaction(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                    scope.Rollback();
                }
                Assert.AreEqual(0, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void RollbackTransaction_NestedScope(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                    var dac2 = DuplicateDAC(dac);
                    using (var scope2 = dac2.BeginScope()) {
                        scope2.BeginTransaction();
                        dac2.Insert("BasicTable", new[] {new ColumnValue("ID", 2)});
                        scope2.Rollback();
                    }
                    scope.Commit();
                }
                Assert.AreEqual(0, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ConsequtiveTransactions(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope = dac.BeginScope(true)) {
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });                    
                    scope.Commit();
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                    scope.Rollback();
                    scope.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 3) });
                    scope.Commit();

                }
                Assert.AreEqual(2, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ConsequtiveTransactionsNested_1(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope1 = dac.BeginScope()) {
                    scope1.BeginTransaction();
                    using (var scope2 = dac.BeginScope(true)) {
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                        scope2.Commit();
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] {new ColumnValue("ID", 2)});
                        scope2.Rollback();
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] {new ColumnValue("ID", 3)});
                        scope2.Commit();
                    }
                    scope1.Commit();
                }
                Assert.AreEqual(0, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ConsequtiveTransactionsNested_2(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope1 = dac.BeginScope()) {
                    scope1.BeginTransaction();
                    using (var scope2 = dac.BeginScope(true)) {
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                        scope2.Commit();
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                        scope2.Commit();
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 3) });
                        scope2.Commit();
                    }
                    scope1.Rollback();
                }
                Assert.AreEqual(0, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ConsequtiveTransactionsNested_3(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope1 = dac.BeginScope()) {
                    scope1.BeginTransaction();
                    using (var scope2 = dac.BeginScope(true)) {
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
                        scope2.Commit();
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
                        scope2.Commit();
                        scope2.BeginTransaction();
                        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 3) });
                        scope2.Commit();
                    }
                    scope1.Commit();
                }
                Assert.AreEqual(3, dac.Count("BasicTable"));
            }
        }

        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void ConsequtiveTransactionsNested_4(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                using (var scope0 = dac.BeginScope()) {
                    using (var scope1 = dac.BeginScope()) {
                        scope1.BeginTransaction();
                        using (var scope2 = dac.BeginScope(true)) {
                            scope2.BeginTransaction();
                            dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1)});
                            scope2.Commit();
                            scope2.BeginTransaction();
                            dac.Insert("BasicTable", new[] {new ColumnValue("ID", 2)});
                            scope2.Rollback();
                            scope2.BeginTransaction();
                            dac.Insert("BasicTable", new[] {new ColumnValue("ID", 3)});
                            scope2.Commit();
                        }
                        scope1.Commit();
                    }
                    scope0.BeginTransaction();
                    dac.Insert("BasicTable", new[] { new ColumnValue("ID", 4) });
                    scope0.Commit();
                }
                Assert.AreEqual(1, dac.Count("BasicTable"));
            }
        }


        [Test]
        [TestCaseSource(nameof(DBMS))]
        public void RepeatTransaction(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                Assert.Catch(() => {
                    using (var scope0 = dac.BeginScope()) {
                        scope0.BeginTransaction();
                        Assert.Catch(() => scope0.BeginTransaction());
                    }
                });
            }
        }
    }
}
