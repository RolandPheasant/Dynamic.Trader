using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Splat;
using StructureMap;

namespace Trader.Client.Infrastucture
{
    /// <summary>
    /// Adapted from http://www.temporalcohesion.co.uk/2013/07/04/custom-structuremap-dependency-resolver-for-reactiveui-5/
    /// </summary>
    public class ReactiveUIDependencyResolver: IMutableDependencyResolver
    {
        private readonly IContainer _container;

        public ReactiveUIDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType, string contract = null)
        {
            return string.IsNullOrEmpty(contract)
                ? _container.GetInstance(serviceType)
                : _container.GetInstance(serviceType, contract);
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        public bool HasRegistration(Type serviceType, string contract = null)
        {
            throw new NotImplementedException();
        }

        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            _container.Configure(x => x.For(serviceType).Use((ctx)=>factory()));
        }

        public void UnregisterCurrent(Type serviceType, string contract = null)
        {
            throw new NotImplementedException();
        }

        public void UnregisterAll(Type serviceType, string contract = null)
        {
            throw new NotImplementedException();
        }

        public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
        {
            return Disposable.Empty;
        }
    }
}