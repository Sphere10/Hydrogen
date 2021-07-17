//-----------------------------------------------------------------------
// <copyright file="StandardLicenseEnforcer.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Application {

    public class StandardLicenseEnforcer : ILicenseEnforcer {

        public const string FullVersionText = "You are using the full version of this software.";
        public const string TrialVersionNagText = "You are using an evaluation version of this software.";
        public const string DisabledVersionNagText = "Your evaluation period has expired.";
        public const string DateExpiresText = "Your license is valid up and until {0:yyyy-MM-dd}.";
        public const string DateExpiredText = "Your license expired on the {0:yyyy-MM-dd}.";
        public const string DaysExpiredText = "You have exceeded the maximum number of days permitted in your evalation period.";
        public const string NumDaysUsedText = "This is day {0} out of the {1} day evaluation period.";
        public const string InstancesExpiredText = "You are running too many instances of this application.";
        public const string NumInstancesUsed = "You have {0} seats left out of your licensed {1}.";
        public const string UsesExpiredText = "You have exceeded the maximum number of application starts permitted in your evaluation period.";
        public const string NumUsesText = "You have started this application {0} times out of the permitted {1}";
        public const string WrongVersion = "The registered license applies to version {0} of this product (this is version {1}).";
        public const string PleasePurchaseText = "Please purchase the product to continue using it.";
        public const string PleaseUpgradeToUnlockText = "Please upgrade your license to unlock all the features.";
        public const string PleaseUpgradeBeforeExpiration = "";

        public StandardLicenseEnforcer(ILicenseServices licenseServices, IProductInformationServices productInformationServices, IProductUsageServices productUsageServices, IUserInterfaceServices userInterfaceServices, IUserNotificationServices userNotificationServices, IDuplicateProcessDetector duplicateProcessDetector, IConfigurationServices configurationServices) {
            SimultaneousLicenseEnforcements = 0;
            CountAppliedLicense = 0;
            CountNagged = 0;
            LicenseServices = licenseServices;
            ProductInformationServices = productInformationServices;
            ProductUsageServices = productUsageServices;
            UserInterfaceServices = userInterfaceServices;
            DuplicateProcessDetector = duplicateProcessDetector;
            ConfigurationServices = configurationServices;
            UserNotificationServices = userNotificationServices;
            Rights = DetermineRights();
            ConfigurationServices.ConfigurationChanged += (sender, args) => DetermineRights();

        }

        #region IoC Component Dependencies 

        protected ILicenseServices LicenseServices { get; private set; }

        protected IProductInformationServices ProductInformationServices { get; private set; }

        protected IProductUsageServices ProductUsageServices { get; private set; }

        protected IUserInterfaceServices UserInterfaceServices { get; private set; }

        protected IUserNotificationServices UserNotificationServices { get; private set; }

        protected IDuplicateProcessDetector DuplicateProcessDetector { get; private set; }

        protected IConfigurationServices ConfigurationServices { get; private set; }

        #endregion

        #region Properties

        public ProductRights Rights { get; private set; }

        public int CountAppliedLicense {
            get;
            private set;
        }

        public int CountNagged {
            get;
            private set;
        }

        /// <summary>
        /// Denotes how many threads are currently executing the 'ApplyLicense' method. The situation arises when the nag screen is visible but a background thead has downloaded
        /// a license command. In this case, we don't want to exit the program until the nag screen is actually closed.
        /// </summary>
        protected int SimultaneousLicenseEnforcements { get; set; }

        #endregion

        public void ApplyLicense(bool nagUser = true) {
            try {
                SimultaneousLicenseEnforcements++;
                CountAppliedLicense++;
                string message;
                bool systemShouldNagUser;
                var compliant = DetermineLicenseCompliance(out systemShouldNagUser, out message);
                bool alreadyNaggedUser = false;
                if (systemShouldNagUser && nagUser) {
                    UserInterfaceServices.ShowNagScreen(true, message);
                    CountNagged++;
                    alreadyNaggedUser = true;
                    // Determine compliance again (as user may have activated software in the nag screen)
                    compliant = DetermineLicenseCompliance(out systemShouldNagUser, out message);
                }

                if (!compliant && SimultaneousLicenseEnforcements == 1) {
                    switch (Rights.FeatureRights) {
                        case ProductFeatureRights.None:
                            UserInterfaceServices.Exit(true);
                            return;

                        case ProductFeatureRights.Partial:
                            if (!alreadyNaggedUser) {
                                // If nagUser param is false, we still want to tell user feature set has been disabled.
                                // We don't tell them if they've already been nagged with the actual nag screen.
                                UserNotificationServices.ReportInfo("Feature set", "Feature set has been disabled");
                            }
                            break;
                    }
                }
            } finally {
                SimultaneousLicenseEnforcements--;
            }
        }

        public virtual bool DetermineLicenseCompliance(out bool systemShouldNagUser, out string nagMessage) {
            systemShouldNagUser = false;
            bool compliant = true;
            var message = new ParagraphBuilder();
            var licenseInformation = LicenseServices.LicenseInformation;

            if (licenseInformation.HasLicenseOverrideCommand && licenseInformation.LicenseOverrideCommand.Action != ProductLicenseAction.Enable) {
                // License override is present, so use it's message
                message.AppendSentence(licenseInformation.LicenseOverrideCommand.NoticationMessage);
                systemShouldNagUser = true;
                if (licenseInformation.LicenseOverrideCommand.Action == ProductLicenseAction.DisableSoftware) {
                    compliant = false;
                }
            } else {
                if (Rights.FeatureRights == ProductFeatureRights.None) {
                    message.AppendSentence(DisabledVersionNagText);
                    systemShouldNagUser = true;
                } else if (Rights.FeatureRights == ProductFeatureRights.Partial || Rights.HasFiniteDays || Rights.HasFiniteUses) {
                    message.AppendSentence(TrialVersionNagText);
                    systemShouldNagUser = true;
                } else {
                    message.AppendSentences(FullVersionText);
                    systemShouldNagUser = false;
                }
                if (Rights.AppliesToVersion && Rights.Version != GetProductMajorVersion()) {
                    message.Clear();
                    message.AppendSentence(string.Format(WrongVersion, Rights.Version, GetProductMajorVersion()));
                    compliant = false;
                    systemShouldNagUser = true;
                } else {
                    if (Rights.ExpiresAfterDate) {
                        if (DateTime.UtcNow > Rights.ExpirationDateUTC) {
                            message.AppendSentence(string.Format(DateExpiredText, Rights.ExpirationDateUTC));
                            compliant = false;
                        } else {
                            message.AppendSentence(string.Format(DateExpiresText, Rights.ExpirationDateUTC));
                        }
                    }
                    if (Rights.HasFiniteDays) {
                        if (Rights.DaysRemaining == 0) {
                            message.AppendSentence(DaysExpiredText);
                            compliant = false;
                        }
                        message.AppendSentence(string.Format(NumDaysUsedText, ProductUsageServices.ProductUsageInformation.DaysUsedBySystem, Rights.TotalDaysAllowed));
                        systemShouldNagUser = true;
                    }

                    if (Rights.HasFiniteInstances) {
                        if (Rights.InstancesRemaining == 0) {
                            message.AppendSentence(InstancesExpiredText);
                            compliant = false;
                            systemShouldNagUser = true;
                        } else {
                            message.AppendSentence(string.Format(NumInstancesUsed, Rights.InstancesRemaining - 1, Rights.TotalInstancesAllowed));
                        }
                    }

                    if (Rights.HasFiniteUses) {
                        if (Rights.UsesRemaining == 0) {
                            message.AppendSentence(UsesExpiredText);
                            compliant = false;
                        }
                        message.AppendSentence(string.Format(NumUsesText, ProductUsageServices.ProductUsageInformation.NumberOfUsesBySystem, Rights.TotalUsesAllowed));
                        systemShouldNagUser = true;
                    }

                    if (systemShouldNagUser) {
                        switch (Rights.FeatureRights) {
                            case ProductFeatureRights.None:
                                message.AppendSentence(PleasePurchaseText);
                                break;
                            case ProductFeatureRights.Partial:
                                message.AppendSentence(PleaseUpgradeToUnlockText);
                                break;
                            case ProductFeatureRights.Full:
                                message.AppendSentence(PleaseUpgradeBeforeExpiration);
                                break;
                        }
                    }
                }
            }

            nagMessage = message.ToString();
            return compliant;
        }

        protected virtual ProductRights DetermineRights() {
            ProductRights rights = ProductRights.None;

            // Apply License override if applicable
            if (LicenseServices.LicenseInformation.HasRegisteredLicenseKey) {
                rights = DetermineRightsFromLicense(LicenseServices.LicenseInformation.RegisteredLicense);
            } else if (LicenseServices.LicenseInformation.HasDefaultLicenseKey) {
                rights = DetermineRightsFromLicense(LicenseServices.LicenseInformation.DefaultLicense);
            }

            if (LicenseServices.LicenseInformation.HasLicenseOverrideCommand) {
                rights = ApplyLicenseCommand(rights, LicenseServices.LicenseInformation.LicenseOverrideCommand);
            }
            return rights;
        }

        protected virtual ProductRights DetermineRightsFromLicense(ProductLicense license) {
            var rights = ProductRights.None;

            if (license.VersionAware) {
                rights.AppliesToVersion = true;
                rights.Version = license.MajorVersionApplicable;
            }

            #region Get license data

            if (license.DoesNotExpire) {
                rights.ExpiresAfterDate = false;
                rights.HasFiniteDays = false;
                rights.HasFiniteInstances = false;
                rights.HasFiniteUses = false;
            } else {
                // Date
                if (license.ExpiresAfterDate) {
                    rights.ExpiresAfterDate = true;
                    rights.ExpirationDateUTC = license.ExpirationDate.ToUniversalTime();
                } else {
                    rights.ExpiresAfterDate = false;
                    rights.ExpirationDateUTC = DateTime.MaxValue;
                }
                // Days 
                if (license.ExpiresAfterDays) {
                    rights.HasFiniteDays = true;
                    rights.TotalDaysAllowed = license.ExpirationDays;
                    rights.DaysRemaining = (license.ExpirationDays - ProductUsageServices.ProductUsageInformation.DaysUsedBySystem + 1).ClipTo(0, int.MaxValue);
                } else {
                    rights.HasFiniteDays = false;
                    rights.TotalDaysAllowed = int.MaxValue;
                    rights.DaysRemaining = int.MaxValue;
                }

                // Uses
                if (license.ExpiresAfterLoads) {
                    rights.HasFiniteUses = true;
                    rights.TotalUsesAllowed = license.ExpirationLoads;
                    rights.UsesRemaining = (license.ExpirationLoads - ProductUsageServices.ProductUsageInformation.NumberOfUsesBySystem + 1).ClipTo(0, int.MaxValue);
                } else {
                    rights.HasFiniteUses = false;
                    rights.TotalUsesAllowed = int.MaxValue;
                    rights.UsesRemaining = int.MaxValue;
                }

                // Instances
                if (license.LimitSimultaneousUsers) {
                    rights.HasFiniteInstances = true;
                    rights.TotalInstancesAllowed = license.MaxSimultaneousUsers;
                    rights.InstancesRemaining = (license.MaxSimultaneousUsers - DuplicateProcessDetector.CountRunningInstancesOfThisApplication() + 1).ClipTo(0, int.MaxValue);
                } else {
                    rights.HasFiniteInstances = false;
                    rights.TotalInstancesAllowed = int.MaxValue;
                    rights.InstancesRemaining = int.MaxValue;
                }
            }

            // Get feature rights
            if (license.LimitedFeatureSet) {
                rights.FeatureRights = ProductFeatureRights.Partial;
            } else {
                rights.FeatureRights = ProductFeatureRights.Full;
            }

            #endregion

            // Determine if license has expired
            bool hasExpired = false;
            if (license.ExpiresAfterDate && DateTime.UtcNow > rights.ExpirationDateUTC) {
                hasExpired = true;
            }
            if (license.ExpiresAfterDays && rights.DaysRemaining == 0) {
                hasExpired = true;
            }
            if (license.ExpiresAfterLoads && rights.UsesRemaining == 0) {
                hasExpired = true;
            }
            if (license.LimitSimultaneousUsers && rights.InstancesRemaining == 0) {
                hasExpired = true;
            }

            // Apply expiration effect on license
            if (hasExpired) {
                if (license.CrippleOnExpiration) {
                    rights.FeatureRights = ProductFeatureRights.Partial;
                }
                if (license.DisableOnExpiration) {
                    rights.FeatureRights = ProductFeatureRights.None;
                }
            }

            return rights;
        }

        private byte GetProductMajorVersion() {
            return StandardLicenseTool.GetProductMajorVersion(ProductInformationServices.ProductInformation.ProductVersion);
        }

        protected virtual ProductRights ApplyLicenseCommand(ProductRights rights, ProductLicenseCommand licenseCommand) {

            switch (LicenseServices.LicenseInformation.LicenseOverrideCommand.Action) {
                case ProductLicenseAction.Enable:
                    //rights.FeatureRights = ProductFeatureRights.Full;
                    // If the command is to enable, then we simply remove the override command and let the license rights apply.
                    // This behaviour prohibits enabling of an expired license (user will need new license)
                    rights.FeatureRights = rights.FeatureRights;
                    break;
                case ProductLicenseAction.DisableSoftware:
                    rights.FeatureRights = ProductFeatureRights.None;
                    break;
                case ProductLicenseAction.CrippleSoftware:
                    rights.FeatureRights = ProductFeatureRights.Partial;
                    break;
            }
            return rights;
        }
    }
}

