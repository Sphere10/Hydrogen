//-----------------------------------------------------------------------
// <copyright file="ProductLicense.cs" company="Sphere 10 Software">
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


namespace Sphere10.Framework.Application {

	[Obfuscation(Exclude = true)]
    public class ProductLicense {

        public ProductLicense () : this(ProductLicenseFlags.DoesNotExpire, 0, DateTime.UtcNow, 0, 0, 0) {
	    }

        public ProductLicense (
            ProductLicenseFlags flags,
            byte majorVersionApplicable,
            DateTime expirationDate,
            byte expirationDays,
            byte expirationLoads,
            byte maxSimultaneousUsers) {
            Flags = flags;
            MajorVersionApplicable = majorVersionApplicable;
            ExpirationDate = expirationDate;
            ExpirationDays = expirationDays;
            ExpirationLoads = expirationLoads;
            MaxSimultaneousUsers = maxSimultaneousUsers;
	    }

        public bool DoesNotExpire {
            get {
                return Flags == ProductLicenseFlags.DoesNotExpire;
            }
            set {
                if (value) {
                    Flags = ProductLicenseFlags.DoesNotExpire;
                } 
            }
        }

        public bool LimitedFeatureSet {
            get {
                return (Flags & ProductLicenseFlags.LimitedFeatureSet) == ProductLicenseFlags.LimitedFeatureSet;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.LimitedFeatureSet;
                } else {
                    Flags &= ~ProductLicenseFlags.LimitedFeatureSet;
                }
            }
        }

        public bool DisableOnExpiration {
            get {
                return (Flags & ProductLicenseFlags.DisableOnExpiration) == ProductLicenseFlags.DisableOnExpiration;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.DisableOnExpiration;
                } else {
                    Flags &= ~ProductLicenseFlags.DisableOnExpiration;
                }
            }
        }
      
        public bool CrippleOnExpiration {
            get {
                return (Flags & ProductLicenseFlags.CrippleOnExpiration) == ProductLicenseFlags.CrippleOnExpiration;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.CrippleOnExpiration;
                } else {
                    Flags &= ~ProductLicenseFlags.CrippleOnExpiration;
                }
            }
        }

        public bool ExpiresAfterDate {
            get {
                return (Flags & ProductLicenseFlags.ExpiresAfterDate) == ProductLicenseFlags.ExpiresAfterDate;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.ExpiresAfterDate;
                } else {
                    Flags &= ~ProductLicenseFlags.ExpiresAfterDate;
                }
            }
        }

        public bool ExpiresAfterDays {
            get {
                return (Flags & ProductLicenseFlags.ExpiresAfterDays) == ProductLicenseFlags.ExpiresAfterDays;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.ExpiresAfterDays;
                } else {
                    Flags &= ~ProductLicenseFlags.ExpiresAfterDays;
                }

            }
        }

        public bool ExpiresAfterLoads {
            get {
                return (Flags & ProductLicenseFlags.ExpiresAfterLoads) == ProductLicenseFlags.ExpiresAfterLoads;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.ExpiresAfterLoads;
                } else {
                    Flags &= ~ProductLicenseFlags.ExpiresAfterLoads;
                }

            }
        }

        public bool LimitSimultaneousUsers {
            get {
                return (Flags & ProductLicenseFlags.LimitSimultaneousUsers) == ProductLicenseFlags.LimitSimultaneousUsers;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.LimitSimultaneousUsers;
                } else {
                    Flags &= ~ProductLicenseFlags.LimitSimultaneousUsers;
                }
            }
        }

        public bool VersionAware {
            get {
                return (Flags & ProductLicenseFlags.VersionAware) == ProductLicenseFlags.VersionAware;
            }
            set {
                if (value) {
                    Flags |= ProductLicenseFlags.VersionAware;
                } else {
                    Flags &= ~ProductLicenseFlags.VersionAware;
                }

            }
        }

        public ProductLicenseFlags Flags { get; set; }

		public byte MajorVersionApplicable { get; set; }

		public DateTime ExpirationDate { get; set; }

		public byte ExpirationDays { get; set; }

		public byte ExpirationLoads { get; set; }

		public byte MaxSimultaneousUsers { get; set; }
       
    }
    
}
