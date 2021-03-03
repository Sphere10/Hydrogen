using System;
using System.Linq.Expressions;

namespace Sphere10.Helium.Saga
{
    public interface IFindSaga
    {
        void ConfigureMapping<TSagaEntity, TMessage>(
            Expression<Func<TSagaEntity, object>> sagaEntityProperty,
            Expression<Func<TMessage, object>> messageProperty)
            where TSagaEntity : ISagaDataForSaga;
    }
}
