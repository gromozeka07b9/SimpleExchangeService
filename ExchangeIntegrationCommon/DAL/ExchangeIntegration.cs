using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ExchangeIntegrationCommon.DAL
{
    public class ExchangeIntegration : IExchangeIntegration
    {
        /// <summary>
        /// Получение хендлера EWS с использованием параметров пользователя из конфига с автодискавери
        /// </summary>
        /// <param name="exchangeUserIdentity"></param>
        /// <returns></returns>
        public ExchangeServiceInstance GetServiceHandler(ExchangeUserIdentity exchangeUserIdentity)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true); 
            ExchangeServiceInstance exchangeService = new ExchangeServiceInstance();
            exchangeService.Handler = new ExchangeService(ExchangeVersion.Exchange2013);
            ExchangeService instance = exchangeService.Handler;
            instance.Credentials = new NetworkCredential(exchangeUserIdentity.Name, exchangeUserIdentity.Password);
            if (!string.IsNullOrEmpty(exchangeUserIdentity.UrlExchangeService))
            {
                instance.Url = new Uri(exchangeUserIdentity.UrlExchangeService);
            }
            else
            {
                exchangeService.Error = ServiceMessages.ErrorEmptyServiceUrl;
            }
            return exchangeService;
        }

        /// <summary>
        /// Создание объекта Appointment и заполнение его параметров, без отправки
        /// </summary>
        /// <param name="serviceInstance">хендлер EWS</param>
        /// <param name="createMeetingParameters">Параметры митинга</param>
        /// <returns></returns>
        public ExchangeAppointmentItem CreateMeetingRequestItem(ExchangeServiceInstance serviceInstance, CreateMeetingParameters createMeetingParameters)
        {
            var item = new ExchangeAppointmentItem() { Error = string.Empty};
            try
            {
                Appointment meeting = new Appointment(serviceInstance.Handler);
                meeting.Body = createMeetingParameters.Body;
                meeting.Subject = createMeetingParameters.Subject;
                meeting.Start = createMeetingParameters.Start;
                meeting.End = createMeetingParameters.End;
                meeting.Location = createMeetingParameters.Location;
                meeting.ReminderMinutesBeforeStart = createMeetingParameters.ReminderMinutesBeforeStart;
                foreach(var itemAttendie in createMeetingParameters.Attendies)
                {
                    meeting.RequiredAttendees.Add(itemAttendie);
                }
                item.ExchangeItem = meeting;
            }
            catch(Exception E)
            {
                item.Error = E.Message;
            }
            return item;
        }

        /// <summary>
        /// Отправка ранее заполненного объекта митинга
        /// </summary>
        /// <param name="serviceInstance">хендлер EWS</param>
        /// <param name="exchangeAppointmentItem">Заполненный объект митинга</param>
        /// <param name="FromEMail">Имперсонация - от имени кого происходит отправка. Работает только при настроенной имперсонации для учетной записи.</param>
        /// <returns></returns>
        public ExchangeSendObjectResult SendMeetingRequestItem(ExchangeServiceInstance serviceInstance, ExchangeAppointmentItem exchangeAppointmentItem, string FromEMail)
        {
            ExchangeSendObjectResult result = new ExchangeSendObjectResult() { ErrorText = string.Empty, Error = false };
            bool validateResult = ValidateParametersSendMeetingRequest(serviceInstance, exchangeAppointmentItem.ExchangeItem);
            if(validateResult)
            {
                var meeting = exchangeAppointmentItem.ExchangeItem;
                try
                {
                    if (!string.IsNullOrEmpty(FromEMail))
                    {
                        ImpersonatedUserId impersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, FromEMail);
                        serviceInstance.Handler.ImpersonatedUserId = impersonatedUserId;
                    }
                    meeting.Save(SendInvitationsMode.SendToAllAndSaveCopy);
                    result.Id = meeting.Id.ToString();
                }
                catch (Exception E)
                {
                    result.Error = true;
                    result.ErrorText = E.Message;
                }
            }
            else
            {
                result.Error = true;
                result.ErrorText = ServiceMessages.IncorrectParametersForSendObject;
            }
            return result;
        }

        /// <summary>
        /// Создание объекта Exchange и заполнение его свойств, без отправки
        /// </summary>
        /// <param name="serviceInstance">Хендлер сервиса EWS</param>
        /// <param name="createTaskParameters">Параметры задачи</param>
        /// <returns></returns>
        public ExchangeTaskItem CreateTaskItem(ExchangeServiceInstance serviceInstance, CreateTaskParameters createTaskParameters)
        {
            var item = new ExchangeTaskItem() { Error = string.Empty };
            try
            {
                EmailMessage message = new EmailMessage(serviceInstance.Handler);
                //Установка свойства Flag приводит к тому, что письмо создается вместе с задачей
                //На данный момент есть проблема с тем, что задача создается, но по какой-то причине не устанавливаются свойства даты начала задачи и предполагаемого завершения
                message.Flag = new Flag() { DueDate = createTaskParameters.EndDate, FlagStatus = ItemFlagStatus.Flagged, StartDate = createTaskParameters.StartDate };
                message.ReminderDueBy = createTaskParameters.EndDate;
                message.IsReminderSet = true;
                message.ReminderMinutesBeforeStart = createTaskParameters.ReminderMinutesBeforeStart;
                switch(createTaskParameters.Importance)
                {
                    case ImportanceTypes.High:
                        {
                            message.Importance = Importance.High;
                        };break;
                    case ImportanceTypes.Low:
                        {
                            message.Importance = Importance.Low;
                        }; break;
                    case ImportanceTypes.Normal:
                    default:
                        {
                            message.Importance = Importance.Normal;
                        }; break;
                }
                message.Subject = createTaskParameters.Subject;
                message.Body = createTaskParameters.Body;
                foreach (var itemAttendie in createTaskParameters.ContactEmail)
                {
                    message.ToRecipients.Add(itemAttendie);
                }
                item.ExchangeItem = message;
            }
            catch (Exception E)
            {
                item.Error = E.Message;
            }
            return item;
        }

        /// <summary>
        /// Отправка ранее заполненной задачи
        /// </summary>
        /// <param name="serviceInstance">Хендлер сервиса EWS</param>
        /// <param name="exchangeTaskItem">Ранее заполненный объект</param>
        /// <param name="FromEMail">Имперсонация - от имени кого происходит отправка. Работает только при настроенной имперсонации для учетной записи.</param>
        /// <returns></returns>
        public ExchangeSendObjectResult SendTaskItem(ExchangeServiceInstance serviceInstance, ExchangeTaskItem exchangeTaskItem, string FromEMail)
        {
            ExchangeSendObjectResult result = new ExchangeSendObjectResult() { ErrorText = string.Empty, Error = false };
            var exchangeObject = exchangeTaskItem.ExchangeItem;
            bool validateResult = ValidateParametersSendTask(serviceInstance, exchangeTaskItem.ExchangeItem);
            if(validateResult)
            {
                try
                {
                    if (!string.IsNullOrEmpty(FromEMail))
                    {
                        ImpersonatedUserId impersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, FromEMail);
                        serviceInstance.Handler.ImpersonatedUserId = impersonatedUserId;
                    }

                    //идентификатор письма не возвращается после отправки, придется искать свое же письмо в отправленных и получать из него id
                    Guid myPropertySetId = Guid.NewGuid();
                    string propertyNameForFindMessage = "GuidForFindEmailMessage";//по этому параметру будем искать отправленное письмо
                    ExtendedPropertyDefinition myExtendedPropertyDefinition = new ExtendedPropertyDefinition(myPropertySetId, propertyNameForFindMessage, MapiPropertyType.String);
                    exchangeObject.SetExtendedProperty(myExtendedPropertyDefinition, propertyNameForFindMessage);
                    exchangeObject.SendAndSaveCopy();

                    //пытаемся получить идентификатор письма
                    string Id = GetIdForEmailMessage(serviceInstance, propertyNameForFindMessage, myPropertySetId);
                    if (string.IsNullOrEmpty(Id))
                    {
                        result.Error = false;//не критичная ошибка, письмо все-таки отправлено
                        result.ErrorText = ServiceMessages.MessageSentedButIdNotFound;
                    }
                    result.Id = Id;
                }
                catch (Exception E)
                {
                    result.Error = true;
                    result.ErrorText = E.Message;
                }
            }
            else
            {
                result.Error = true;
                result.ErrorText = ServiceMessages.IncorrectParametersForSendObject;
            }
            return result;
        }

        /// <summary>
        /// Проверка обязательных параметров для отправки задач
        /// </summary>
        /// <param name="serviceInstance"></param>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        private bool ValidateParametersSendTask(ExchangeServiceInstance serviceInstance, EmailMessage emailMessage)
        {
            return ((serviceInstance.Handler != null) && (serviceInstance.Handler.Url != null) && (!string.IsNullOrEmpty(emailMessage.Body)) && (!string.IsNullOrEmpty(emailMessage.Subject)) && (emailMessage.ToRecipients.Count > 0));
        }

        /// <summary>
        /// Проверка обязательных параметров для митингов
        /// </summary>
        /// <param name="serviceInstance"></param>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        private bool ValidateParametersSendMeetingRequest(ExchangeServiceInstance serviceInstance, Appointment meeting)
        {
            return ((serviceInstance.Handler != null) && (serviceInstance.Handler.Url != null) && (!string.IsNullOrEmpty(meeting.Body)) && (!string.IsNullOrEmpty(meeting.Subject)) && (meeting.RequiredAttendees.Count > 0));
        }

        /// <summary>
        /// Метод используется для того, чтобы получить идентификар письма, поскольку метод отправки письма не возвращает его
        /// </summary>
        /// <param name="serviceInstance">Хендлер сервиса Exchange</param>
        /// <param name="propertyNameForFindMessage">Название проперти, по которому будем искать письмо</param>
        /// <param name="myPropertySetId">Значение проперти для поиска письма</param>
        /// <returns></returns>
        private string GetIdForEmailMessage(ExchangeServiceInstance serviceInstance, string propertyNameForFindMessage, Guid myPropertySetId)
        {
            int TimeoutForSendMessage = 1000;//таймаут, за который считаем что письмо попало в отправленные и может быть получено
            int maxCountRetry = 5;//Количество попыток, которые будем предпринимать для получения письма
            ItemView view = new ItemView(1);
            ExtendedPropertyDefinition myExtendedPropertyDefinition = new ExtendedPropertyDefinition(myPropertySetId, propertyNameForFindMessage, MapiPropertyType.String);
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(myExtendedPropertyDefinition, propertyNameForFindMessage);
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, myExtendedPropertyDefinition);
            string Id = string.Empty;

            for (int attemptRead = 0; attemptRead < maxCountRetry; attemptRead++)
            {
                System.Threading.Thread.Sleep(TimeoutForSendMessage);
                FindItemsResults<Item> findResults = serviceInstance.Handler.FindItems(WellKnownFolderName.SentItems, searchFilter, view);
                if (findResults.Items.Count > 0)
                {
                    var item = findResults.Items[0];
                    if (item is EmailMessage)
                    {
                        Id = item.Id.UniqueId;
                        break;
                    }
                }
            }

            return Id;
        }

        static bool RedirectionCallback(string url)
        {
            return url.ToLower().StartsWith("https://");
        }
    }
}
