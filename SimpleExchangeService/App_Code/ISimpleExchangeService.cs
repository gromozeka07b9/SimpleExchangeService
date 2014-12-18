using ExchangeIntegrationCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WSDLGeneration;

namespace SimpleExchangeService
{
    [ServiceContract(Namespace = "http://maxus.ru/SimpleExchangeService", Name = "SimpleExchangeService")]
    [WsdlDocumentation("Сервис для упрощенного доступа к API Exchange")]
    public interface ISimpleExchangeServiceV1
    {

        [OperationContract]
        [WsdlDocumentation("Создание запроса на встречу")]
        CreateMeetingRequestResult CreateMeetingRequest(CreateMeetingParameters createMeetingParameters);

        [OperationContract]
        [WsdlDocumentation("Создание задачи")]
        CreateTaskResult CreateTask(CreateTaskParameters createTaskParameters);

        [OperationContract(Action="http://maxus.ru/SimpleExchangeService/SimpleExchangeService/Ping", Name="Ping")]
        [WsdlDocumentation("Проверка доступности сервиса, результат 1 означает успешный вызов")]
        int Ping();
    }
}

        /*[OperationContract]
        [WsdlDocumentation("Удаление запроса на встречу")]
        DeleteMeetingResult DeleteMeetingRequest(string MeetingRequestId);


        [OperationContract]
        [WsdlDocumentation("Удаление задачи")]
        DeleteTaskResult DeleteTask(string TaskId);

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.
/*[DataContract]
public class CompositeType
{
	bool boolValue = true;
	string stringValue = "Hello ";

	[DataMember]
	public bool BoolValue
	{
		get { return boolValue; }
		set { boolValue = value; }
	}

	[DataMember]
	public string StringValue
	{
		get { return stringValue; }
		set { stringValue = value; }
	}
}
*/

/*
CreateMeetingRequest (Создание митинга)
http://msdn.microsoft.com/en-us/library/office/microsoft.exchange.webservices.data.meetingrequest_members(v=exchg.80).aspx
Параметры (структура):
•         Body (Содержание описания встречи, string)
•         Subject (Тема встречи, string)
•         Start (Начало встречи, datetime)
•         End (Завершение встречи, datetime)
•         Location (место встречи, string)
•         ReminderMinutesBeforeStart (За сколько минут до старта должно появиться оповещение, int)
•         FromEMail (Адрес того, кто назначил встречу, string)
•         Attendies (структура для списка приглашенных)
o   AttendieEMail (Почта приглашенного, string)
Метод возвращает структуру:
•         Id (Id созданного митинга, string)
•         Result (Признак успешности операции, Boolean)
•         Error (Описание ошибки, string)
 
DeleteMeetingRequest (Удаление митинга)
http://msdn.microsoft.com/en-us/library/office/microsoft.exchange.webservices.data.meetingrequest.delete(v=exchg.80).aspx
Параметры:
•         Id (string)
Метод возвращает структуру:
•         Result (Признак успешности операции, Boolean)
•         Error (Описание ошибки, string)
 
CreateTask (Создание задачи)
http://msdn.microsoft.com/en-us/library/office/microsoft.exchange.webservices.data.task_members(v=exchg.80).aspx
Параметры (структура):
•         Body (Содержание описания встречи, string)
•         Subject (Тема встречи, string)
•         StartDate (Начало встречи, datetime)
•         EndDate (Завершение встречи, datetime)
•         ReminderMinutesBeforeStart (За сколько минут до старта должно появиться оповещение, int)
•         Importance (Важность, перечисление Low/Normal/High)
•         Status (Статус задачи, перечисление NotStarted/InProgress/Completed/WaitingOnOther/Deferred)
•         Contacts (структура для списка ответственных)
o   ContactEMail (Почта ответственного, string)
Метод возвращает структуру:
•         Id (Id созданной задачи, string)
•         Result (Признак успешности операции, Boolean)
•         Error (Описание ошибки, string)
 
DeleteTask (Удаление задачи)
http://msdn.microsoft.com/en-us/library/office/microsoft.exchange.webservices.data.task.delete(v=exchg.80).aspx
Параметры:
•         Id (string)
Метод возвращает структуру:
•         Result (Признак успешности операции, Boolean)
•         Error (Описание ошибки, string)
*/