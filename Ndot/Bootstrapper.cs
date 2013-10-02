using System.Web.Http;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using MongoRepository;
using Ndot.Helpers;
using Ndot.Models;

namespace Ndot
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer()
                .RegisterType<LogWriter>(new HierarchicalLifetimeManager(),
                                         new InjectionFactory(x => new LogWriterFactory().Create()))
                .RegisterType<IRepository<Sr1FormData>, MongoRepository<Sr1FormData>>(new InjectionConstructor())
                .RegisterType<IEdmundsApiAgent, EdmundsApiAgent>();
                
            return container;
        }
    }
}