using System;
using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Services
{
    public interface IComponentFactory
    {
        IComponent Activate(Type componentType);
    }
}