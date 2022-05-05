//-----------------------------------------------------------------------
// <copyright file="WindowsSecurityTest.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;
using NUnit.Framework.Internal;
using Hydrogen.Windows;
using Hydrogen.Windows.Security;


namespace Hydrogen.UnitTests {
    [TestFixture]
    public class WindowsSecurityTests {
        public static readonly string RemoteHostName = "ANTARIS";

        #region Basic tests

        [Test]
        public void TestClass_NTRemoteObject() {
            NTHost host = NTHost.CurrentMachine;

            NTRemoteObject obj = new NTRemoteObject(
                host.Name,
                "Domain",
                "Name",
                host.SID,
                WinAPI.ADVAPI32.SidNameUse.Domain
            );

            Assert.AreEqual(host.Name, obj.Host);
            Assert.AreEqual("Domain", obj.Domain);
            Assert.AreEqual("Name", obj.Name);
            Assert.AreEqual(host.SID, obj.SID);
            Assert.AreEqual(WinAPI.ADVAPI32.SidNameUse.Domain, obj.SidNameUsage);

        }


        [Test]
        public void TestClass_NTDanglingObject() {
            NTHost host = NTHost.CurrentMachine;

            NTDanglingObject obj = new NTDanglingObject(
                host.Name,
                "Name"
            );

            Assert.AreEqual(host.Name, obj.Host);
            Assert.AreEqual("Name", obj.Name);
            Assert.IsNull(obj.SID);
        }


        [Test]
        public void TestClass_NTDanglingObject2() {
            NTHost host = NTHost.CurrentMachine;

            NTDanglingObject obj = new NTDanglingObject(
                host.Name,
                host.SID,
                WinAPI.ADVAPI32.SidNameUse.Invalid
            );

            Assert.AreEqual(host.Name, obj.Host);
            Assert.AreEqual(host.SID, obj.SID);
            Assert.AreEqual(WinAPI.ADVAPI32.SidNameUse.Invalid, obj.NameUse);
            Assert.AreEqual(string.Empty, obj.Name);
        }

        #endregion

        #region Local tests

        [Test]
        public void TestLocalHost() {
            NTHost host = NTHost.CurrentMachine;
            Assert.IsNotNull(host);
            Assert.AreEqual(host.Name.ToUpper(), Environment.MachineName.ToUpper());
        }

        [Test]
        public void TestLocalHostGetSid() {
            NTHost host = NTHost.CurrentMachine;
            Assert.IsNotNull(host.SID);
        }

        [Test]
        public void TestLocalHostGetAdministratorUser() {
            NTHost host = NTHost.CurrentMachine;
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.AccountAdministratorSid, host.SID),
                    host.GetLocalUsers()
                )
            );
        }

        [Test]
        public void TestLocalHostAdministratorUserPrivilege() {
            NTHost host = NTHost.CurrentMachine;
            NTLocalUser user = GetObjectByName(host.GetLocalUsers(), "Administrator");
            Assert.IsTrue(user.Privilege == UserPrivilege.Admin);
        }

        [Test]
        public void TestLocalHostGetGuest() {
            NTHost host = NTHost.CurrentMachine;
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.AccountGuestSid, host.SID),
                    host.GetLocalUsers()
                )
            );

        }

        [Test]
        public void TestLocalHostGetAdministratorsGroup() {
            NTHost host = NTHost.CurrentMachine;
            NTLocalGroup[] localGroups = host.GetLocalGroups();
            NTLocalGroup group = GetObjectByName(localGroups, "Administrators");
            SecurityIdentifier localHostSid = host.SID;
            SecurityIdentifier adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, localHostSid);
            Assert.IsTrue(
                ContainsSid(
                    adminSid,
                    host.GetLocalGroups()
                )
            );
        }

        [Test]
        public void TestLocalHostGetGuestsGroup() {
            NTHost host = NTHost.CurrentMachine;
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.BuiltinGuestsSid, null),
                   host.GetLocalGroups()
                )
            );
        }

        [Test]
        public void TestLocalHostGetPowerUsersGroup() {
            NTHost host = NTHost.CurrentMachine;
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.BuiltinPowerUsersSid, null),
                    host.GetLocalGroups()
                )
            );
        }

        [Test]
        public void TestLocalUserCreateDelete() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            Assert.IsNotNull(user);
            Assert.IsTrue(ContainsObjectByName(host.GetLocalUsers(), userName));
            user.Delete();
            Assert.IsFalse(ContainsObjectByName(host.GetLocalUsers(), userName));
        }

        [Test]
        public void TestLocalUserUpdateSID() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.IsNotNull(user.SID);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserSIDContainsHostSID() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.AreEqual(user.SID.AccountDomainSid, host.SID);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateHomeDirectory() {
            NTHost host = NTHost.CurrentMachine;
            string value = "c:\\";
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                user.HomeDirectory = value;
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                Assert.AreEqual(user.HomeDirectory, value);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateLastLogoff() {
            NTHost host = NTHost.CurrentMachine;
            DateTime value = DateTime.MinValue;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                Assert.AreEqual(value, user.LastLogoff);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateLastLogon() {
            NTHost host = NTHost.CurrentMachine;
            DateTime value = DateTime.MinValue;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                Assert.AreEqual(value, user.LastLogon);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateLogonHours() {
            NTHost host = NTHost.CurrentMachine;
            byte[] value = new byte[21] { 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1 };
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            user.LogonHours = value;
            user.Update();
            user = GetObjectByName(host.GetLocalUsers(), userName);
            CollectionAssert.AreEqual(user.LogonHours, value);
            user.Delete();
        }

        [Test]
        public void TestLocalUserUpdateLogonServer() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.AreEqual("\\\\*", user.LogonServer);
            } finally {
                user.Delete();
            }
        }

        [Test,Ignore("Not supported on all platforms")]
        public void TestLocalUserUpdateMaxStorage() {
            NTHost host = NTHost.CurrentMachine;
            uint value = 1000;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
//                user.Flags |= UserFlags.
                user.MaxStorage = value;
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                Assert.AreEqual(value, user.MaxStorage);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateName() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.AreEqual(user.Name, userName);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateNumberOfLogons() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.AreEqual(user.NumberOfLogons, 0);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalPersistUserPassword() {
            NTHost host = NTHost.CurrentMachine;
            string value = "AbCn1122CeF123";
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            user.Password = value;
            user.Update();
            user.Delete();
        }

        [Test]
        public void TestLocalUserUpdatePasswordAge() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.IsTrue(0 <= user.PasswordAge.TotalSeconds && user.PasswordAge.TotalSeconds <= 2);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdatePrivilege() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                Assert.AreEqual(user.Privilege, UserPrivilege.Guest);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserUpdateScriptPath() {
            NTHost host = NTHost.CurrentMachine;
            string value = Process.GetCurrentProcess().StartInfo.FileName;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                user.ScriptPath = value;
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                Assert.AreEqual(user.ScriptPath, value);
            } finally {
                user.Delete();
            }
        }


        [Test, Ignore("Not supported on all platforms")]
        public void TestLocalGetUserUnitsPerWeek() {
            NTHost host = NTHost.CurrentMachine;
            uint value = 5;
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                user.UnitsPerWeek = value;
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                Assert.AreEqual(user.UnitsPerWeek, value);
            } finally {
                user.Delete();
            }
        }

        [Test, Ignore("Not Supported on all platforms")]
        public void TestLocalUserGetWorkstations() {
            NTHost host = NTHost.CurrentMachine;
            string[] value = new string[] { "W1", "w2" };
            // find a unique user name
            string userName = GenerateUserName(host);
            NTLocalUser user = host.CreateLocalUser(userName, "P4ssw0rd123");
            try {
                user.Workstations = value;
                user.Update();
                user = GetObjectByName(host.GetLocalUsers(), userName);
                CollectionAssert.AreEqual(user.Workstations, value);
            } finally {
                user.Delete();
            }
        }

        [Test]
        public void TestLocalUserWithEmptyMembership() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user = null;
            try {
                user = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                CollectionAssert.IsEmpty(user.GetMembership());
            } finally {
                try {
                    if (user != null) {
                        user.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalUserWithSingleGroup() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user = null;
            NTLocalGroup group = null;
            try {
                group = host.CreateLocalGroup(GenerateGroupName(host), null);
                user = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                user.AddMembership(group.Name);
                Assert.IsTrue(ContainsObjectByName(user.GetMembership(), group.Name));
            } finally {
                try {
                    if (user != null) {
                        user.Delete();
                    }
                } catch {
                }

                try {
                    if (user != null) {
                        group.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalUsersWithMultipleGroups() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user1 = null;
            NTLocalUser user2 = null;
            NTLocalGroup group1 = null;
            NTLocalGroup group2 = null;
            NTLocalGroup group3 = null;
            try {
                group1 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group1);
                group2 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group2);
                group3 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group3);


                user1 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                user2 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");

                user1.AddMembership(group1.Name);
                user1.AddMembership(group2.Name);
                user2.AddMembership(group1.Name);
                user2.AddMembership(group3.Name);

                NTLocalGroup[] user1Membership = user1.GetMembership();
                Assert.IsNotNull(user1Membership);

                Assert.IsTrue(ContainsObjectByName(user1Membership, group1.Name));
                Assert.IsTrue(ContainsObjectByName(user1Membership, group2.Name));

                NTLocalGroup[] user2Membership = user2.GetMembership();
                Assert.IsNotNull(user2Membership);

                Assert.IsTrue(ContainsObjectByName(user2Membership, group1.Name));
                Assert.IsTrue(ContainsObjectByName(user2Membership, group3.Name));

            } finally {
                try {
                    if (user1 != null) {
                        user1.Delete();
                    }
                } catch {
                }

                try {
                    if (user2 != null) {
                        user2.Delete();
                    }
                } catch {
                }

                try {
                    if (group1 != null) {
                        group1.Delete();
                    }
                } catch {
                }


                try {
                    if (group2 != null) {
                        group2.Delete();
                    }
                } catch {
                }


                try {
                    if (group3 != null) {
                        group3.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupCreateDelete() {
            NTHost host = NTHost.CurrentMachine;
            string groupName = GenerateGroupName(host);
            NTLocalGroup group = host.CreateLocalGroup(groupName, null);
            Assert.IsNotNull(group);
            Assert.IsTrue(ContainsObjectByName(host.GetLocalGroups(), groupName));
            group.Delete();
            Assert.IsFalse(ContainsObjectByName(host.GetLocalGroups(), groupName));
        }

        [Test]
        public void TestLocalGroupGetDescription() {
            NTHost host = NTHost.CurrentMachine;
            string groupName = GenerateGroupName(host);
            string description = "Test description";
            NTLocalGroup group = host.CreateLocalGroup(groupName, description);
            try {
                group = GetObjectByName(host.GetLocalGroups(), groupName);
                Assert.AreEqual(description, group.Description);
            } finally {
                group.Delete();
            }
        }

        [Test]
        public void TestLocalGroupUpdateDescription() {
            NTHost host = NTHost.CurrentMachine;
            string groupName = GenerateGroupName(host);
            string description = "Test description";
            string newDescription = "New description";
            NTLocalGroup group = host.CreateLocalGroup(groupName, description);
            try {
                group.Description = newDescription;
                group.Update();
                group = GetObjectByName(host.GetLocalGroups(), groupName);
                Assert.AreEqual(newDescription, group.Description);
            } finally {
                group.Delete();
            }
        }

        [Test]
        public void TestLocalGroupEmptyMembers() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalGroup group = null;
            try {
                group = host.CreateLocalGroup(GenerateGroupName(host), "description");
                CollectionAssert.IsEmpty(group.GetLocalMembers());
            } finally {
                try {
                    if (group != null) {
                        group.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupAddMember() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user = null;
            NTLocalGroup group = null;
            try {
                group = host.CreateLocalGroup(GenerateGroupName(host), null);
                user = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                group.AddLocalMember(user.Name);
                Assert.IsTrue(ContainsObjectByName(group.GetLocalMembers(), user.Name));
            } finally {
                try {
                    if (user != null) {
                        user.Delete();
                    }
                } catch {
                }

                try {
                    if (user != null) {
                        group.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupDeleteByObject() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user = null;
            NTLocalGroup group = null;
            try {
                group = host.CreateLocalGroup(GenerateGroupName(host), null);
                user = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                group.AddLocalMember(user.Name);
                Assert.IsTrue(ContainsObjectByName(group.GetLocalMembers(), user.Name));
                group.DeleteMember(user);
                CollectionAssert.IsEmpty(group.GetLocalMembers());
            } finally {
                try {
                    if (user != null) {
                        user.Delete();
                    }
                } catch {
                }

                try {
                    if (user != null) {
                        group.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupDeleteBySID() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user = null;
            NTLocalGroup group = null;
            try {
                group = host.CreateLocalGroup(GenerateGroupName(host), null);
                user = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                group.AddLocalMember(user.Name);
                Assert.IsTrue(ContainsObjectByName(group.GetLocalMembers(), user.Name));
                group.DeleteMember(user.SID);
                CollectionAssert.IsEmpty(group.GetLocalMembers());
            } finally {
                try {
                    if (user != null) {
                        user.Delete();
                    }
                } catch {
                }

                try {
                    if (user != null) {
                        group.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupDeleteByName() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user = null;
            NTLocalGroup group = null;
            try {
                group = host.CreateLocalGroup(GenerateGroupName(host), null);
                user = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                group.AddLocalMember(user.Name);
                Assert.IsTrue(ContainsObjectByName(group.GetLocalMembers(), user.Name));
                group.DeleteLocalMember(user.Name);
                CollectionAssert.IsEmpty(group.GetLocalMembers());
            } finally {
                try {
                    if (user != null) {
                        user.Delete();
                    }
                } catch {
                }

                try {
                    if (user != null) {
                        group.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupWithMultipleMembersWithUserMembershipVerify() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user1 = null;
            NTLocalUser user2 = null;
            NTLocalGroup group1 = null;
            NTLocalGroup group2 = null;
            NTLocalGroup group3 = null;
            try {
                group1 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group1);
                group2 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group2);
                group3 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group3);


                user1 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                user2 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");

                group1.AddLocalMember(user1.Name);
                group1.AddLocalMember(user2.Name);
                group2.AddLocalMember(user1.Name);
                group3.AddLocalMember(user2.Name);

                NTLocalGroup[] user1Membership = user1.GetMembership();
                Assert.IsNotNull(user1Membership);

                Assert.IsTrue(ContainsObjectByName(user1Membership, group1.Name));
                Assert.IsTrue(ContainsObjectByName(user1Membership, group2.Name));

                NTLocalGroup[] user2Membership = user2.GetMembership();
                Assert.IsNotNull(user2Membership);

                Assert.IsTrue(ContainsObjectByName(user2Membership, group1.Name));
                Assert.IsTrue(ContainsObjectByName(user2Membership, group3.Name));

            } finally {
                try {
                    if (user1 != null) {
                        user1.Delete();
                    }
                } catch {
                }

                try {
                    if (user2 != null) {
                        user2.Delete();
                    }
                } catch {
                }

                try {
                    if (group1 != null) {
                        group1.Delete();
                    }
                } catch {
                }


                try {
                    if (group2 != null) {
                        group2.Delete();
                    }
                } catch {
                }


                try {
                    if (group3 != null) {
                        group3.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupWithMultipleMembersWithWithGroupMembershipVerify() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user1 = null;
            NTLocalUser user2 = null;
            NTLocalGroup group1 = null;
            NTLocalGroup group2 = null;
            NTLocalGroup group3 = null;
            try {
                group1 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group1);
                group2 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group2);
                group3 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group3);


                user1 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                user2 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");

                group1.AddLocalMember(user1.Name);
                group1.AddLocalMember(user2.Name);
                group2.AddLocalMember(user1.Name);
                group3.AddLocalMember(user2.Name);

                NTLocalObject[] group1Members = group1.GetLocalMembers();
                Assert.IsNotNull(group1Members);
                Assert.AreEqual(2, group1Members.Length);
                Assert.IsTrue(ContainsObjectByName(group1Members, user1.Name));
                Assert.IsTrue(ContainsObjectByName(group1Members, user2.Name));

                NTLocalObject[] group2Members = group2.GetLocalMembers();
                Assert.IsNotNull(group2Members);
                Assert.AreEqual(1, group2Members.Length);
                Assert.IsTrue(ContainsObjectByName(group2Members, user1.Name));

                NTLocalObject[] group3Members = group3.GetLocalMembers();
                Assert.IsNotNull(group3Members);
                Assert.AreEqual(1, group3Members.Length);
                Assert.IsTrue(ContainsObjectByName(group3Members, user2.Name));

            } finally {
                try {
                    if (user1 != null) {
                        user1.Delete();
                    }
                } catch {
                }

                try {
                    if (user2 != null) {
                        user2.Delete();
                    }
                } catch {
                }

                try {
                    if (group1 != null) {
                        group1.Delete();
                    }
                } catch {
                }


                try {
                    if (group2 != null) {
                        group2.Delete();
                    }
                } catch {
                }


                try {
                    if (group3 != null) {
                        group3.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupDeleteMultipleMembersWithWithUserMembershipVerify() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user1 = null;
            NTLocalUser user2 = null;
            NTLocalGroup group1 = null;
            NTLocalGroup group2 = null;
            NTLocalGroup group3 = null;
            try {
                group1 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group1);
                group2 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group2);
                group3 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group3);

                user1 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                user2 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");

                group1.AddLocalMember(user1.Name);
                group1.AddLocalMember(user2.Name);
                group2.AddLocalMember(user1.Name);
                group3.AddLocalMember(user2.Name);

                group1.DeleteLocalMember(user1.Name);
                group1.DeleteLocalMember(user2.Name);
                group2.DeleteLocalMember(user1.Name);
                group3.DeleteLocalMember(user2.Name);


                NTLocalGroup[] user1Membership = user1.GetMembership();
                Assert.IsNotNull(user1Membership);
                Assert.AreEqual(0, user1Membership.Length);

                NTLocalGroup[] user2Membership = user2.GetMembership();
                Assert.IsNotNull(user2Membership);
                Assert.AreEqual(0, user2Membership.Length);

            } finally {
                try {
                    if (user1 != null) {
                        user1.Delete();
                    }
                } catch {
                }

                try {
                    if (user2 != null) {
                        user2.Delete();
                    }
                } catch {
                }

                try {
                    if (group1 != null) {
                        group1.Delete();
                    }
                } catch {
                }


                try {
                    if (group2 != null) {
                        group2.Delete();
                    }
                } catch {
                }


                try {
                    if (group3 != null) {
                        group3.Delete();
                    }
                } catch {
                }
            }
        }

        [Test]
        public void TestLocalGroupDeleteMultipleMembersWithWithGroupMembershipVerify() {
            NTHost host = NTHost.CurrentMachine;
            // find a unique user name
            NTLocalUser user1 = null;
            NTLocalUser user2 = null;
            NTLocalGroup group1 = null;
            NTLocalGroup group2 = null;
            NTLocalGroup group3 = null;
            try {
                group1 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group1);
                group2 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group2);
                group3 = host.CreateLocalGroup(GenerateGroupName(host), "description");
                Assert.IsNotNull(group3);


                user1 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");
                user2 = host.CreateLocalUser(GenerateUserName(host), "P4ssw0rd123");

                group1.AddLocalMember(user1.Name);
                group1.AddLocalMember(user2.Name);
                group2.AddLocalMember(user1.Name);
                group3.AddLocalMember(user2.Name);

                group1.DeleteLocalMember(user1.Name);
                group1.DeleteLocalMember(user2.Name);
                group2.DeleteLocalMember(user1.Name);
                group3.DeleteLocalMember(user2.Name);


                NTObject[] group1Members = group1.GetMembers();
                Assert.IsNotNull(group1Members);
                Assert.AreEqual(0, group1Members.Length);

                NTObject[] group2Members = group2.GetMembers();
                Assert.IsNotNull(group2Members);
                Assert.AreEqual(0, group2Members.Length);

                NTObject[] group3Members = group3.GetMembers();
                Assert.IsNotNull(group3Members);
                Assert.AreEqual(0, group3Members.Length);

            } finally {
                try {
                    if (user1 != null) {
                        user1.Delete();
                    }
                } catch {
                }

                try {
                    if (user2 != null) {
                        user2.Delete();
                    }
                } catch {
                }

                try {
                    if (group1 != null) {
                        group1.Delete();
                    }
                } catch {
                }


                try {
                    if (group2 != null) {
                        group2.Delete();
                    }
                } catch {
                }


                try {
                    if (group3 != null) {
                        group3.Delete();
                    }
                } catch {
                }
            }
        }


        #endregion

        #region Remote tests

        [Test]
        public void TestRemoteHost() {
            NTHost host = new NTHost(RemoteHostName);
            Assert.IsNotNull(host);
            Assert.AreEqual(RemoteHostName.ToUpper(), host.Name.ToUpper());
        }

        [Test]
        public void TestRemoteHostGetSid() {
            NTHost host = new NTHost(RemoteHostName);
            Assert.IsNotNull(host.SID);
        }

        [Test]
        public void TestRemoteHostGetAdministratorUser() {
            NTHost host = new NTHost(RemoteHostName);
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.AccountAdministratorSid, host.SID),
                    host.GetLocalUsers()
                )
            );
        }

        [Test]
        public void TestRemoteHostAdministratorUserPrivilege() {
            NTHost host = new NTHost(RemoteHostName);
            NTLocalUser user = GetObjectByName(host.GetLocalUsers(), "Administrator");
            Assert.IsTrue(user.Privilege == UserPrivilege.Admin);
        }

        [Test]
        public void TestRemoteHostGetGuest() {
            NTHost host = new NTHost(RemoteHostName);
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.AccountGuestSid, host.SID),
                    host.GetLocalUsers()
                )
            );

        }

        [Test]
        public void TestRemoteHostGetAdministratorsGroup() {
            NTHost host = new NTHost(RemoteHostName);
            NTLocalGroup[] localGroups = host.GetLocalGroups();
            NTLocalGroup group = GetObjectByName(localGroups, "Administrators");
            SecurityIdentifier localHostSid = host.SID;
            SecurityIdentifier adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, localHostSid);
            Assert.IsTrue(
                ContainsSid(
                    adminSid,
                    host.GetLocalGroups()
                )
            );
        }

        [Test]
        public void TestRemoteHostGetAdministratorsGroupMembers() {
            NTHost host = new NTHost(RemoteHostName);
            NTLocalGroup[] localGroups = host.GetLocalGroups();
            NTLocalGroup group = GetObjectByName(localGroups, "Administrators");
            NTLocalObject[] members = group.GetLocalMembers();
            Assert.IsNotNull(members);
            CollectionAssert.IsNotEmpty(members);
        }


        [Test]
        public void TestRemoteHostGetGuestsGroup() {
            NTHost host = new NTHost(RemoteHostName);
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.BuiltinGuestsSid, null),
                   host.GetLocalGroups()
                )
            );
        }

        [Test]
        public void TestRemoteHostGetPowerUsersGroup() {
            NTHost host = new NTHost(RemoteHostName);
            Assert.IsTrue(
                ContainsSid(
                    new SecurityIdentifier(WellKnownSidType.BuiltinPowerUsersSid, null),
                    host.GetLocalGroups()
                )
            );
        }

        #endregion



        private string GenerateUserName(NTHost host) {
            string userName = "_TmpUsr";
            while (ContainsObjectByName(host.GetLocalUsers(), userName)) {
                userName = "_TmpUsr" + Guid.NewGuid().ToString().Substring(0, 4);
            }
            return userName;
        }

        private string GenerateGroupName(NTHost host) {
            string groupName = "_TmpGrp";
            while (ContainsObjectByName(host.GetLocalGroups(), groupName)) {
                groupName = "_TmpGrp" + Guid.NewGuid().ToString().Substring(0, 4);
            }
            return groupName;
        }

        private bool ContainsObjectByName<T>(T[] objects, string name) where T : NTLocalObject {
            foreach (T obj in objects) {
                if (obj.Name == name) {
                    return true;
                }
            }
            return false;
        }

        private bool ContainsSid<T>(SecurityIdentifier sid, T[] objects) where T : NTLocalObject {
            foreach (T obj in objects) {
                if (obj.SID == sid) {
                    return true;
                }
            }
            return false;
        }

        private T GetObjectByName<T>(T[] objects, string name) where T : NTLocalObject {
            foreach (T obj in objects) {
                if (obj.Name == name) {
                    return obj;
                }
            }
            throw new Exception(string.Format("NTLocalObject '{0}' not found", name));
        }




    }
}
