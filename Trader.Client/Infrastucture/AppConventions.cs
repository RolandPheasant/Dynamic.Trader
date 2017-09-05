using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace Trader.Client.Infrastucture
{
    public class AppConventions : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {

        }

        public void ScanTypes(TypeSet types, Registry registry)
        {
            // Only work on concrete types
            types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)
                .ForEach(type =>
            {
                // Register against all the interfaces implemented
                // by this concrete class
                type.GetInterfaces()
                .Where(@interface => @interface.Name == $"I{type.Name}")
                .ForEach(@interface => registry.For(@interface).Use(type).Singleton());
            });


        }
    }
}