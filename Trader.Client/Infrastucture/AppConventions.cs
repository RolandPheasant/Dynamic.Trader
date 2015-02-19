using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace Trader.Client.Infrastucture
{
    public class AppConventions : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            // Only work on concrete types
            if (!type.IsConcrete() || type.IsGenericType) return;

            // Register against all the interfaces implemented
            // by this concrete class
            type.GetInterfaces()
                .Where(@interface => @interface.Name == string.Format("I{0}",type.Name))
                .ForEach(@interface => registry.For(@interface).Use(type).Singleton());
        }
    }
}