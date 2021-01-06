using System;
using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Loader.Services
{
    //https://github.com/dotnet/aspnetcore/blob/master/src/Components/Components/src/DefaultComponentActivator.cs
    public class DefaultComponentActivator : IComponentActivator
    {
        public static IComponentActivator Instance { get; } = new DefaultComponentActivator();
        
        /// <inheritdoc />
        public IComponent CreateInstance(Type componentType)
        {
            var instance = Activator.CreateInstance(componentType);
            if (!(instance is IComponent component))
            {
                throw new ArgumentException(
                    $"The type {componentType.FullName} does not implement {nameof(IComponent)}.",
                    nameof(componentType));
            }

            return component;
        }
    }
}