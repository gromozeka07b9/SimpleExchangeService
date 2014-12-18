using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    public class ExchangeUserIdentity
    {
        /// <summary>
        /// Имя учетной записи AD для отправки почты
        /// </summary>
        public string Name;
        /// <summary>
        /// Пароль учетной записи AD для отправки почты
        /// </summary>
        public string Password;
        /// <summary>
        /// Ошибка при работе с сервером Exchange
        /// </summary>
        public string Error;
        /// <summary>
        /// Основной адрес веб-сервисов Exchange
        /// </summary>
        public string UrlExchangeService;
    }
}
