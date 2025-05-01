using System;
using JetBrains.Annotations;

namespace Infrastructure.DI.Injector
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        
    }
    
}