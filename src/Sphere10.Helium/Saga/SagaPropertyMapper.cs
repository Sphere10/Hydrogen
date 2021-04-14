using System;
using System.Linq.Expressions;

namespace Sphere10.Helium.Saga {
	public class SagaPropertyMapper<T> where T : ISagaDataForSaga {
		private readonly IFindSaga _sagaFindMap;

		internal SagaPropertyMapper(IFindSaga sagaFindMap) {
			_sagaFindMap = sagaFindMap;
		}

		public ToSagaExpression<T, TMessage> ConfigureMapping<TMessage>(Expression<Func<TMessage, object>> messageProperty) {
			return new ToSagaExpression<T, TMessage>(_sagaFindMap, messageProperty);
		}
	}
}
