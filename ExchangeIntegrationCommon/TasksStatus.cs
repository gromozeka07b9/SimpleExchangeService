using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    /// <summary>
    /// Типы статусов задач
    /// </summary>
    public enum TasksStatus
    {
        NotStarted = 0, InProgress, Completed, WaitingOnOther, Deferred
    }
}
