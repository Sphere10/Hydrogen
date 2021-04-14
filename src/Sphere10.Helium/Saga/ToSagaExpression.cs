using System;
using System.Linq.Expressions;

namespace Sphere10.Helium.Saga {
	public class ToSagaExpression<TSagaData, TMessage> where TSagaData : ISagaDataForSaga {
		private readonly IFindSaga _sagaMessageFindingConfiguration;
		private readonly Expression<Func<TMessage, object>> _messageProperty;

		public ToSagaExpression(IFindSaga sagaMessageFindingConfiguration, Expression<Func<TMessage, object>> messageProperty) {
			_sagaMessageFindingConfiguration = sagaMessageFindingConfiguration;
			_messageProperty = messageProperty;
		}

		public void ToSaga(Expression<Func<TSagaData, object>> sagaEntityProperty) {
			_sagaMessageFindingConfiguration.ConfigureMapping<TSagaData, TMessage>(sagaEntityProperty, _messageProperty);
		}
	}
}
