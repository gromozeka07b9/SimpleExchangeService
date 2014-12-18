using ExchangeIntegrationCommon;
using ExchangeIntegrationCommon.DAL;
using Microsoft.Practices.Unity;
using Unity.Wcf;

namespace SimpleExchangeService
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            container.RegisterType<ISimpleExchangeServiceV1, SimpleExchangeService>();
            container.RegisterType<IExchangeIntegration, ExchangeIntegration>();
            container.RegisterType<IServiceSettings, ExchangeServiceSettings>();
            container.RegisterType<ILogger, Logger>();
        }
    }    
}