//-----------------------------------------------------------------------
// <copyright file="ComponentRegistryDefinition.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sphere10.Framework.Application {

	[XmlRoot("ComponentRegistry")]
    public class ComponentRegistryDefinition {

        #region Constructor

        public ComponentRegistryDefinition() {
            PluginFolder = string.Empty;
            RegistrationsDefinition = new RegistrationDefinition[0];
        }

        #endregion

        #region Properties

        [XmlAttribute("pluginFolder")]
        public string PluginFolder { get; set; }

        [XmlElement("Assembly", typeof(AssemblyRegistrationDefinition))]
        [XmlElement("Component", typeof(ComponentRegistrationDefinition))]
        [XmlElement("Proxy", typeof(ProxyInterfaceRegistrationDefinition))]
        [XmlElement("ComponentSet", typeof(ComponentSetRegistrationDefinition))]
        public RegistrationDefinition[] RegistrationsDefinition { get; set; }

        #endregion

        #region Internal Types

        public abstract class RegistrationDefinition {            
        }

        public class AssemblyRegistrationDefinition : RegistrationDefinition {
            [XmlAttribute("dll")]
            public string Dll { get; set; }
        }

        public class ComponentRegistrationDefinition : RegistrationDefinition {            
            [XmlAttribute("dll")]
            public string Dll { get; set; }

            [XmlAttribute("interface")]
            public string Interface { get; set; }
           
            [XmlAttribute("implementation")]
            public string Implementation { get; set; }

            [XmlIgnore]
            public ActivationType? Activation { get; set; }

            [XmlAttribute("activation"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public string ActivationSerialzationSurroage {
                get { return Activation?.ToString(); }
                set { Activation = value != null ? (ActivationType?) Enum.Parse(typeof(ActivationType), value, true) : null; }
            }

            [XmlAttribute("resolveKey")]
            public string ResolveKey { get; set; }
        }

        public class ProxyInterfaceRegistrationDefinition : RegistrationDefinition {
            [XmlAttribute("interface")]
            public string Interface { get; set; }

            [XmlAttribute("proxy")]
            public string Proxy { get; set; }
        }

        public class ComponentSetRegistrationDefinition : RegistrationDefinition {

            public ComponentSetRegistrationDefinition() {
                RegistrationsDefinition = new RegistrationDefinition[0];
            }

            [XmlAttribute("interface")]
            public string Interface { get; set; }

            [XmlElement("Component", typeof(ComponentRegistrationDefinition))]
            [XmlElement("Proxy", typeof(ProxyInterfaceRegistrationDefinition))]
            public RegistrationDefinition[] RegistrationsDefinition { get; set; }

        }

        #endregion
    }
}
