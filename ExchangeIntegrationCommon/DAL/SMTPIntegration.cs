using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeIntegrationCommon.DAL
{
    /// <summary>
    /// Класс создавался как аналог запросов к веб-сервисам Exchange.
    /// На данный момент не используется.
    /// </summary>
    public class SMTPIntegration : IExchangeIntegration
    {
        public ExchangeServiceInstance GetServiceHandler(ExchangeUserIdentity exchangeUserIdentity)
        {
            ExchangeServiceInstance exchangeService = new ExchangeServiceInstance();
            //exchangeService.Handler = new object();
            /*exchangeService.Handler = new ExchangeService(ExchangeVersion.Exchange2013);
            ExchangeService instance = exchangeService.Handler;
            instance.Credentials = new NetworkCredential(exchangeUserIdentity.Name, exchangeUserIdentity.Password);
            try
            {
                instance.AutodiscoverUrl(exchangeUserIdentity.Name, RedirectionCallback);
            }
            catch (Exception E)
            {
                exchangeService.Error = E.Message;
            }*/
            return exchangeService;
        }

        public ExchangeAppointmentItem CreateMeetingRequestItem(ExchangeServiceInstance serviceInstance, CreateMeetingParameters createMeetingParameters)
        {
            var item = new ExchangeAppointmentItem() { Error = string.Empty };
            /*try
            {
                Appointment meeting = new Appointment(serviceInstance.Handler);
                meeting.Body = createMeetingParameters.Body;
                meeting.Subject = createMeetingParameters.Subject;
                meeting.Start = createMeetingParameters.Start;
                meeting.End = createMeetingParameters.End;
                meeting.Location = createMeetingParameters.Location;
                meeting.ReminderMinutesBeforeStart = createMeetingParameters.ReminderMinutesBeforeStart;
                foreach (var itemAttendie in createMeetingParameters.Attendies)
                {
                    meeting.RequiredAttendees.Add(itemAttendie);
                }
                item.ExchangeItem = meeting;
            }
            catch (Exception E)
            {
                item.Error = E.Message;
            }*/
            return item;
        }

        public ExchangeSendObjectResult SendMeetingRequestItem(ExchangeServiceInstance serviceInstance, ExchangeAppointmentItem exchangeAppointmentItem, string FromEMail)
        {
            ExchangeSendObjectResult result = new ExchangeSendObjectResult() { ErrorText = string.Empty, Error = false };
            /*var meeting = exchangeAppointmentItem.ExchangeItem;
            try
            {
                Mailbox principle = new Mailbox("Delo.base@maxus.ru");
                Folder principleCalendar = Folder.Bind(serviceInstance.Handler, new FolderId(WellKnownFolderName.Calendar, principle));
                meeting.Save(principleCalendar.Id, SendInvitationsMode.SendToAllAndSaveCopy);
                result.Id = meeting.Id.ToString();
            }
            catch (Exception E)
            {
                result.Error = true;
                result.ErrorText = E.Message;
            }*/
            return result;
        }
        
        public ExchangeTaskItem CreateTaskItem(ExchangeServiceInstance serviceInstance, CreateTaskParameters createTaskParameters)
        {
            throw new Exception("Method not realized");
        }

        public ExchangeSendObjectResult SendTaskItem(ExchangeServiceInstance serviceInstance, ExchangeTaskItem exchangeTaskItem, string FromEMail)
        {
            throw new Exception("Method not realized");
        }
    }
}
