using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    /// <summary>
    /// Класс-прослойка для доступа к объекту Exchange 
    /// Вместо объекта Exchange Task используется EmailMessage поскольку EWS запрещает прямое назначение объекта TASK пользователю, отличному от того, под кем выполняется запрос
    /// </summary>
    public class ExchangeTaskItem
    {
        public EmailMessage ExchangeItem;
        public string Error;
    }
}
