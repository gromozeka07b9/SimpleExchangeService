using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon.DAL
{
    public interface IServiceSettings
    {
        ExchangeUserIdentity GetUserSettings();
    }
}
