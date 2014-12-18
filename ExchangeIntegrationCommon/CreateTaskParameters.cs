using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    /// <summary>
    /// Параметры для создания задач
    /// </summary>
    public class CreateTaskParameters
    {
        /// <summary>
        /// Тело сообщения
        /// </summary>
        public string Body;
        /// <summary>
        /// Тема
        /// </summary>
        public string Subject;
        /// <summary>
        /// Дата начала действия задачи
        /// </summary>
        public DateTime StartDate;
        /// <summary>
        /// Дата завершения задачи
        /// </summary>
        public DateTime EndDate;
        /// <summary>
        /// Через сколько минут напомнить адресату о задаче
        /// </summary>
        public int ReminderMinutesBeforeStart;
        /// <summary>
        /// Важность
        /// </summary>
        public ImportanceTypes Importance;
        /// <summary>
        /// Статус задачи
        /// </summary>
        public TasksStatus Status;
        /// <summary>
        /// Адресаты задачи
        /// </summary>
        public string[] ContactEmail;
    }
}
