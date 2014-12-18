using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExchangeIntegrationCommon.DAL
{
    /// <summary>
    /// Класс для выполнения обращений к серверу Exchange
    /// </summary>
    public interface IExchangeIntegration
    {
        /// <summary>
        /// Предназначен для получения Handler Exchange с автоматическим выполнением Autodiscovery, т.е. получением URL веб-сервиса
        /// </summary>
        /// <param name="exchangeUserIdentity"></param>
        /// <returns></returns>
        ExchangeServiceInstance GetServiceHandler(ExchangeUserIdentity exchangeUserIdentity);

        /// <summary>
        /// Предназначен для создания экземпляра объекта встречи
        /// </summary>
        /// <param name="serviceInstance">Ссылка на объект сервиса</param>
        /// <param name="createMeetingParameters">Параметры для заполнения структуры встречи</param>
        /// <returns></returns>
        ExchangeAppointmentItem CreateMeetingRequestItem(ExchangeServiceInstance serviceInstance, CreateMeetingParameters createMeetingParameters);

        /// <summary>
        /// Сохранение в Exchange объекта встречи
        /// </summary>
        /// <param name="serviceInstance">Ссылка на объект сервиса</param>
        /// <param name="exchangeAppointmentItem">Объект встречи с заполненными параметрами</param>
        /// <param name="FromEMail">Адрес, от имени кого будет организована встреча</param>
        /// <returns></returns>
        ExchangeSendObjectResult SendMeetingRequestItem(ExchangeServiceInstance serviceInstance, ExchangeAppointmentItem exchangeAppointmentItem, string FromEMail);
        
        /// <summary>
        /// Создание экземпляра задачи 
        /// </summary>
        /// <param name="serviceInstance">Ссылка на объект сервиса</param>
        /// <param name="createTaskParameters">Объект встречи с заполненными параметрами</param>
        /// <returns></returns>
        ExchangeTaskItem CreateTaskItem(ExchangeServiceInstance serviceInstance, CreateTaskParameters createTaskParameters);

        /// <summary>
        /// Сохранение в Exchange объекта задачи
        /// </summary>
        /// <param name="serviceInstance">Ссылка на объект сервиса</param>
        /// <param name="exchangeTaskItem">Объект встречи с заполненными параметрами</param>
        /// <param name="FromEMail">Адрес, от имени кого будет отправлена задача</param>
        /// <returns></returns>
        ExchangeSendObjectResult SendTaskItem(ExchangeServiceInstance serviceInstance, ExchangeTaskItem exchangeTaskItem, string FromEMail);
    }
}