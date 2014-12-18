using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon
{
    public class ServiceMessages
    {
        public const string MessageBodyVeryLong = "Длина текста тела сообщения должна быть не больше 5000 символов;";
        public const string MessageSubjectVeryLong = "Длина текста темы сообщения должна быть не больше 70 символов;";
        /// <summary>
        /// Сообщение одинаково для EmailMessage и Appointment, отличия только в Attendies и ContactEmail
        /// </summary>
        public const string MessageParametersNotDefined = "Некорректны обязательные параметры: Body, Subject, Start, End, FromEMail, Attendies, ContactEmail;";
        public const string IncorrectParametersForSendObject = "Некорректны обязательные параметры: serviceInstance, Body, Subject, To";
        public const string MeetingDateStartIncorrect = "Дата начала должна быть больше текущей даты;";
        public const string MeetingDateEndIncorrect = "Дата завершения должна быть больше текущей даты и даты начала;";
        public const string ErrorCreatingItem = "Ошибка создания экземпляра объекта Exchange;";
        public const string ErrorReadingParameters = "Ошибка чтения параметров конфигурации;";
        public const string ErrorEmptyServiceUrl = "Не установлен URL веб-сервисов Exchange;";
        public const string MeetingReminderLessZero = "Длительность напоминания не может быть меньше 0;";
        public const string MessageSentedButIdNotFound = "Сообщение отправлено, но идентификатор получить не удалось;";
    }
}
