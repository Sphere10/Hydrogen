using System;
using System.Linq.Expressions;

namespace Sphere10.Helium.Bus
{
    public interface IComponentConfig
    {
        IComponentConfig ConfigureProperty(string name, object value);
    }

    public interface IComponentConfig<T>
    {
        IComponentConfig<T> ConfigureProperty(Expression<Func<T, object>> property, object value);
    }
}
