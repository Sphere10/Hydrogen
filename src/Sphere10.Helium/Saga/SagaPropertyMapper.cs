using System;
using System.Linq.Expressions;

namespace Sphere10.Helium.Saga
{
    public class SagaPropertyMapper<TSagaData> where TSagaData : ISagaDataForSaga
    {
        private readonly IFindSaga _sagaFindMap;

        internal SagaPropertyMapper(IFindSaga sagaFindMap)
        {
            _sagaFindMap = sagaFindMap;
        }

        public ToSagaExpression<TSagaData, TMessage> ConfigureMapping<TMessage>(Expression<Func<TMessage, object>> messageProperty)
        {
            return new ToSagaExpression<TSagaData, TMessage>(_sagaFindMap, messageProperty);
        }
    }
}
