//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;

//namespace Hydrogen.ObjectSpace {
//	public class ObjectSpace {

//		public ObjectSpaceDefinition Definition { get; }

//		public ObjectContainer[] Containers { get; }


//		public IStreamMappedRecyclableList<TItem> GetContainer<TItem>() {
//			throw new NotImplementedException();
//		}

//		public IStreamMappedDictionary<TKey, TValue> GetContainer<TKey, TValue>() {
//			throw new NotImplementedException();
//		}

//		public IReadOnlyDictionary<TMember, IFuture<TItem>> GetUniqueLookup<TItem, TMember>(Expression<Func<TItem, TMember>> memberExpression) {
//			throw new NotImplementedException();
//		}

//		public ILookup<TMember, IFuture<TItem>> GetLookup<TItem, TMember>(Expression<Func<TItem, TMember>> memberExpression) {
//			throw new NotImplementedException();
//		}

//	}
//}
