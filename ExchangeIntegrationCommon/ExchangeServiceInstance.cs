using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    public class ExchangeServiceInstance
    {
        public string Error { get; set; }

        public Microsoft.Exchange.WebServices.Data.ExchangeService Handler { get; set; }
    }
}
