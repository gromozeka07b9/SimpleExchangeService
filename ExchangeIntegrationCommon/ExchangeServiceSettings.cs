using ExchangeIntegrationCommon.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    /// <summary>
    /// Класс получения настроек для работы с Exchange
    /// </summary>
    public class ExchangeServiceSettings : IServiceSettings
    {
        /// <summary>
        /// Получение настроек из web.config, ожидается наличие ExchangeUsername (в виде name@maxus.ru) и ExchangePassword
        /// </summary>
        /// <returns></returns>
        public ExchangeUserIdentity GetUserSettings()
        {
            ExchangeUserIdentity identity = new ExchangeUserIdentity() { Error = string.Empty};
            identity.Name = ConfigurationManager.AppSettings["ExchangeUsername"];
            identity.Password = ConfigurationManager.AppSettings["ExchangePassword"];
            identity.UrlExchangeService = ConfigurationManager.AppSettings["UrlExchangeService"];
            if ((string.IsNullOrEmpty(identity.Name) || (string.IsNullOrEmpty(identity.Password)) || (string.IsNullOrEmpty(identity.UrlExchangeService))))
            {
                identity.Error = ServiceMessages.ErrorReadingParameters;
            }
            return identity;
        }
    }
}
