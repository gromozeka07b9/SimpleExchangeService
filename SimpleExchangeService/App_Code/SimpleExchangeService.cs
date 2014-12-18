using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Services;
using ExchangeIntegrationCommon;
using ExchangeIntegrationCommon.DAL;
using NLog;

namespace SimpleExchangeService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(Namespace = "http://maxus.ru/SimpleExchangeService", Name = "SimpleExchangeService", ConfigurationName = "SimpleExchangeService", AddressFilterMode = AddressFilterMode.Any)]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SimpleExchangeService : ISimpleExchangeServiceV1
    {
        private readonly IExchangeIntegration _exchangeIntegration;
        private readonly IServiceSettings _serviceSettings;
        private readonly ILogger _logger;

        public SimpleExchangeService(IExchangeIntegration exchangeIntegration, IServiceSettings serviceSettings, ILogger logger)
        {
            _exchangeIntegration = exchangeIntegration;
            _serviceSettings = serviceSettings;
            _logger = logger;
        }

        [WebMethod]
        public CreateMeetingRequestResult CreateMeetingRequest(CreateMeetingParameters createMeetingParameters)
        {
            _logger.Log("CreateMeeting_Author_" + GetUserName(), LoggerLevel.Info);
            _logger.Log("CreateMeetingRequest_Start_Subj_" + createMeetingParameters.Subject, LoggerLevel.Trace);
            ExchangeUserIdentity userSettings = null;
            CreateMeetingRequestResult result = new CreateMeetingRequestResult() { Id = string.Empty, Error = false, ErrorText = string.Empty };
            string checkParametersResult = CheckMeetingParameters(createMeetingParameters);
            if (string.IsNullOrEmpty(checkParametersResult))
            {
                userSettings = _serviceSettings.GetUserSettings();
                if(string.IsNullOrEmpty(userSettings.Error))
                {
                    ExchangeServiceInstance serviceInstance = _exchangeIntegration.GetServiceHandler(userSettings);
                    if ((serviceInstance != null) && (string.IsNullOrEmpty(serviceInstance.Error)))
                    {
                        ExchangeAppointmentItem appointmentItem = _exchangeIntegration.CreateMeetingRequestItem(serviceInstance, createMeetingParameters);
                        if (string.IsNullOrEmpty(appointmentItem.Error))
                        {
                            ExchangeSendObjectResult sendMeetingResult = _exchangeIntegration.SendMeetingRequestItem(serviceInstance, appointmentItem, createMeetingParameters.FromEMail);
                            result.Id = sendMeetingResult.Id;
                            result.Error = sendMeetingResult.Error;
                            result.ErrorText = sendMeetingResult.ErrorText;
                        }
                        else
                        {
                            result.Error = true;
                            result.ErrorText = ServiceMessages.ErrorCreatingItem;
                        }
                    }
                    else
                    {
                        result.Error = true;
                        result.ErrorText = serviceInstance.Error;
                    }
                }
                else
                {
                    result.Error = true;
                    result.ErrorText = userSettings.Error;
                }
            } else
            {
                result.Error = true;
                result.ErrorText = checkParametersResult;
            }
            _logger.Log("CreateMeetingRequest_Stop_Subj_" + createMeetingParameters.Subject, LoggerLevel.Trace);
            return result;
        }

        /// <summary>
        /// Метод создает задачу в Exchange по данным переданной структуры
        /// Задача создается с помощью отпраки EmailMessage с установленным Flag, приводящим к созданию задачи
        /// </summary>
        /// <param name="createTaskParameters">Структура параметров</param>
        /// <returns></returns>
        [WebMethod]
        public CreateTaskResult CreateTask(CreateTaskParameters createTaskParameters)
        {
            _logger.Log("CreateTask_Author_" + GetUserName(), LoggerLevel.Info);
            _logger.Log("CreateTask_Start_Subj_" + createTaskParameters.Subject, LoggerLevel.Trace);
            ExchangeUserIdentity userSettings = null;
            CreateTaskResult result = new CreateTaskResult() {Id = string.Empty, Error = false, ErrorText = string.Empty };
            string checkParametersResult = CheckTaskParameters(createTaskParameters);
            if (string.IsNullOrEmpty(checkParametersResult))
            {
                userSettings = _serviceSettings.GetUserSettings();
                if (string.IsNullOrEmpty(userSettings.Error))
                {
                    ExchangeServiceInstance serviceInstance = _exchangeIntegration.GetServiceHandler(userSettings);
                    if ((serviceInstance != null) && (string.IsNullOrEmpty(serviceInstance.Error)))
                    {
                        ExchangeTaskItem taskItem = _exchangeIntegration.CreateTaskItem(serviceInstance, createTaskParameters);
                        if (string.IsNullOrEmpty(taskItem.Error))
                        {
                            ExchangeSendObjectResult sendResult = _exchangeIntegration.SendTaskItem(serviceInstance, taskItem, string.Empty);
                            result.Id = sendResult.Id;
                            result.Error = sendResult.Error;
                            result.ErrorText = sendResult.ErrorText;
                        }
                        else
                        {
                            result.Error = true;
                            result.ErrorText = ServiceMessages.ErrorCreatingItem;
                        }
                    }
                    else
                    {
                        result.Error = true;
                        result.ErrorText = serviceInstance.Error;
                    }
                }
                else
                {
                    result.Error = true;
                    result.ErrorText = userSettings.Error;
                }
            }
            else
            {
                result.Error = true;
                result.ErrorText = checkParametersResult;
            }
            _logger.Log("CreateTask_Stop_Subj_" + createTaskParameters.Subject, LoggerLevel.Trace);
            return result;
        }

        /// <summary>
        /// Метод валидирует входные параметры создания задачи
        /// </summary>
        /// <param name="createTaskParameters">Структура параметров</param>
        /// <returns></returns>
        private string CheckTaskParameters(CreateTaskParameters createTaskParameters)
        {
            string error = string.Empty;
            int MaxBodyLenght = 5000;
            int MaxSubjLength = 70;
            bool errorReqParameters = false;
            errorReqParameters = (string.IsNullOrEmpty(createTaskParameters.Body)
                                || string.IsNullOrEmpty(createTaskParameters.Subject)
                                || string.IsNullOrEmpty(createTaskParameters.Body)
                                || createTaskParameters.StartDate == null
                                || createTaskParameters.EndDate == null
                                || createTaskParameters.ContactEmail == null);
            error += errorReqParameters == true ? ServiceMessages.MessageParametersNotDefined : string.Empty;
            error += (createTaskParameters.Body != null) && (createTaskParameters.Body.Length > MaxBodyLenght) ? ServiceMessages.MessageBodyVeryLong : string.Empty;
            error += (createTaskParameters.Subject != null) && (createTaskParameters.Subject.Length > MaxSubjLength) ? ServiceMessages.MessageSubjectVeryLong : string.Empty;
            error += createTaskParameters.ReminderMinutesBeforeStart < 0 ? ServiceMessages.MeetingReminderLessZero : string.Empty;
            error += createTaskParameters.EndDate < DateTime.Now ? ServiceMessages.MeetingDateEndIncorrect : string.Empty;
            error += createTaskParameters.StartDate < DateTime.Now ? ServiceMessages.MeetingDateStartIncorrect : string.Empty;
            return error;
        }

        /// <summary>
        /// Метод валидирует входные параметры запроса на митинг
        /// </summary>
        /// <param name="createMeetingParameters">Структура параметров</param>
        /// <returns></returns>
        private string CheckMeetingParameters(CreateMeetingParameters createMeetingParameters)
        {
            string error = string.Empty;
            int MaxBodyLenght = 5000;
            int MaxSubjLength = 70;
            bool errorReqParameters = false;
            errorReqParameters = (string.IsNullOrEmpty(createMeetingParameters.Body)
                                || string.IsNullOrEmpty(createMeetingParameters.Subject)
                                || string.IsNullOrEmpty(createMeetingParameters.Body)
                                || string.IsNullOrEmpty(createMeetingParameters.Location)
                                || createMeetingParameters.Start == null
                                || createMeetingParameters.End == null
                                || createMeetingParameters.Attendies == null);
            error += errorReqParameters == true ? ServiceMessages.MessageParametersNotDefined : string.Empty;
            error += (createMeetingParameters.Body != null) && (createMeetingParameters.Body.Length > MaxBodyLenght) ? ServiceMessages.MessageBodyVeryLong : string.Empty;
            error += (createMeetingParameters.Subject != null) && (createMeetingParameters.Subject.Length > MaxSubjLength) ? ServiceMessages.MessageSubjectVeryLong : string.Empty;
            error += createMeetingParameters.ReminderMinutesBeforeStart < 0 ? ServiceMessages.MeetingReminderLessZero : string.Empty;
            error += createMeetingParameters.End < DateTime.Now ? ServiceMessages.MeetingDateEndIncorrect : string.Empty;
            error += createMeetingParameters.Start < DateTime.Now ? ServiceMessages.MeetingDateStartIncorrect : string.Empty;
            return error;
        }

        /// <summary>
        /// Проверка доступности сервиса, результат 1 означает успешный вызов
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public int Ping()
        {
            return 1;
        }

        /// <summary>
        /// Получаем имя пользователя, выполнившего запрос.
        /// Если анонимная аутентификация, пользователя не будет.
        /// </summary>
        /// <returns></returns>
        private string GetUserName()
        {
            string userName = "Anonimous";
            if (System.ServiceModel.ServiceSecurityContext.Current != null)
            {
                userName = ServiceSecurityContext.Current.WindowsIdentity.Name;
            }
            return userName;
        }

    }
}
