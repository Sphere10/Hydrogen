//-----------------------------------------------------------------------
// <copyright file="Reference.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	public class Reference {
        private readonly int _hashcode;

        protected Reference(object @object) {
            Object = @object;
            _hashcode = @object != null ? @object.GetHashCode() : 0;
        }

		public object Object { get; protected set; }

        public override bool Equals(object obj) {
            if (obj is Reference)
                obj = ((Reference)obj).Object;
            return ReferenceEquals(Object, obj);
        }

        public override int GetHashCode() {
            return _hashcode;
        }

	    public static Reference<T> For<T>(T obj) where T:class {
	        return new Reference<T>(obj);
	    } 

	}

	public class Reference<TObject> : Reference where TObject : class {

	    internal Reference(TObject @object) 
			: base(@object) {
	    }

	    public new TObject Object {
			get {
				return base.Object as TObject;
			} 
			protected set {
				base.Object = value;
			}
		}


		// implicit operators
		public static explicit operator TObject(Reference<TObject> reference) {
			return reference.Object;
		}

		public static explicit operator Reference<TObject>(TObject @object) {
			return new Reference<TObject>(@object);
		}
	}
}
