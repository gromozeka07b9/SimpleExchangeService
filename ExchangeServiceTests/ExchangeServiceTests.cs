using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Exchange.WebServices.Data;
using System.Net;
using Microsoft.Practices.Unity;
using ExchangeIntegrationCommon.DAL;
using SimpleExchangeService;
using ExchangeIntegrationCommon;

namespace ExchangeServiceTests
{
    [TestClass]
    public class ExchangeServiceTests
    {
        private UnityContainer _container;
        private Mock<IExchangeIntegration> _exchangeIntegrationMock;
        private Mock<IServiceSettings> _serviceSettingsMock;
        private Mock<ILogger> _loggerMock;
        private ISimpleExchangeServiceV1 _simpleExchangeService;
        
        [TestInitialize]
        public void Init()
        {
            _container = new UnityContainer();
            _container
                   .RegisterType<ISimpleExchangeServiceV1, SimpleExchangeService.SimpleExchangeService>()
                   .RegisterType<IExchangeIntegration, ExchangeIntegration>()
                   .RegisterType<IServiceSettings, ExchangeServiceSettings>()
                   .RegisterType<ILogger, Logger>();
            _exchangeIntegrationMock = new Mock<IExchangeIntegration>();
            _container.RegisterInstance(_exchangeIntegrationMock.Object);
            _serviceSettingsMock = new Mock<IServiceSettings>();
            _container.RegisterInstance(_serviceSettingsMock.Object);
            _loggerMock = new Mock<ILogger>();
            _container.RegisterInstance(_loggerMock.Object);
            _simpleExchangeService = _container.Resolve<ISimpleExchangeServiceV1>();
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenVeryBigBody()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MessageBodyVeryLong };
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            parameters.Body = new string('*', 5001);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenVeryBigSubject()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MessageSubjectVeryLong };
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            parameters.Subject = new string('*', 71);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenWithoutRequiredParameters()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MessageParametersNotDefined };
            CreateMeetingParameters parameters = new CreateMeetingParameters();
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.IsTrue(actualResponse.ErrorText.Contains(expectedResponse.ErrorText));
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenDateStartLessCurrentDate()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MeetingDateStartIncorrect };
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            parameters.Start = DateTime.Now.AddHours(-1);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenDateEndLessCurrentDate()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MeetingDateEndIncorrect };
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            parameters.End = DateTime.Now.AddHours(-1);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenReminderLessZero()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MeetingReminderLessZero };
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            parameters.ReminderMinutesBeforeStart = -1;
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateMeetingRequestWhenExchangeDiscoverError()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = true, Id = string.Empty, ErrorText = "пример текста ошибки Exchange" };
            string login = "login";
            string password = "password";
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            _serviceSettingsMock.Setup(p => p.GetUserSettings()).Returns(new ExchangeUserIdentity() { Error = string.Empty, Name = login, Password = password });
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = expectedResponse.ErrorText, Handler = new ExchangeService() { Url = null } });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CanCreateMeetingRequestWhenExchangeDiscoverOk()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = false, Id = string.Empty, ErrorText = string.Empty };
            string login = "login";
            string password = "password";
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            var serviceInstance = new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() { Url = new Uri("http://test.maxus.ru") } };
            _serviceSettingsMock.Setup(p => p.GetUserSettings()).Returns(new ExchangeUserIdentity() { Error = string.Empty, Name = login, Password = password });
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() { Url = new Uri("http://test.maxus.ru") } });
            _exchangeIntegrationMock.Setup(p => p.CreateMeetingRequestItem(It.Is<ExchangeServiceInstance>(v => v.Error == string.Empty && v.Handler != null), It.IsAny<CreateMeetingParameters>())).Returns(new ExchangeAppointmentItem() { Error = string.Empty, ExchangeItem = new Appointment(serviceInstance.Handler) { } });
            _exchangeIntegrationMock.Setup(p => p.SendMeetingRequestItem(It.Is<ExchangeServiceInstance>(v => v.Error == string.Empty && v.Handler != null), It.Is<ExchangeAppointmentItem>(v => v.Error == string.Empty && v.ExchangeItem != null), It.IsAny<string>())).Returns(new ExchangeSendObjectResult() { Error = false, ErrorText = string.Empty, Id = expectedResponse.Id });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CanCreateMeetingRequestWhenParametersOk()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = false, Id = "1234567890", ErrorText = string.Empty };
            CreateMeetingParameters parameters = GetCorrectTestParametersForMeeting();
            parameters.FromEMail = string.Empty;
            string login = "login";
            string password = "password";
            var serviceInstance = new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() { Url = new Uri("http://test.maxus.ru") } };
            _serviceSettingsMock.Setup(p => p.GetUserSettings()).Returns(new ExchangeUserIdentity() { Error = string.Empty, Name = login, Password = password });
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == login && v.Password == password))).Returns(serviceInstance);
            _exchangeIntegrationMock.Setup(p => p.CreateMeetingRequestItem(It.Is<ExchangeServiceInstance>(v => v.Error == string.Empty && v.Handler != null), It.IsAny<CreateMeetingParameters>())).Returns(new ExchangeAppointmentItem() { Error = string.Empty, ExchangeItem = new Appointment(serviceInstance.Handler) { } });
            _exchangeIntegrationMock.Setup(p => p.SendMeetingRequestItem(It.Is<ExchangeServiceInstance>(v => v.Error == string.Empty && v.Handler != null), It.Is<ExchangeAppointmentItem>(v => v.Error == string.Empty && v.ExchangeItem != null), It.IsAny<string>())).Returns(new ExchangeSendObjectResult() { Error = false, ErrorText = string.Empty, Id = expectedResponse.Id });
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        /// <summary>
        /// Создание заведомо корректных параметров для создания митинга. Далее в тестах они модифицируются для моделирования ошибочных ситуаций.
        /// </summary>
        /// <returns></returns>
        private CreateMeetingParameters GetCorrectTestParametersForMeeting()
        {
            CreateMeetingParameters parameters = new CreateMeetingParameters();
            parameters.Body = new string('*', 5000);
            parameters.Start = DateTime.Now.AddHours(1);
            parameters.End = parameters.Start.AddDays(1);
            parameters.FromEMail = "test@maxus.ru";
            parameters.Location = "переговорная 11";
            parameters.Subject = new string('*', 30);
            parameters.Attendies = new string[] { "test2@maxus.ru", "test3@maxus.ru" };
            parameters.ReminderMinutesBeforeStart = 99;
            return parameters;
        }

        //--------------------------------Тесты задач---------------------------------------------------
        [TestMethod]
        public void CanCreateTaskWhenParametersOk()
        {
            var expectedResponse = new CreateTaskResult { Error = false, ErrorText = string.Empty, Id = "1234567890" };
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            string login = "login";
            string password = "password";
            var serviceInstance = new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() { Url = new Uri("http://test.maxus.ru") } };
            _serviceSettingsMock.Setup(p => p.GetUserSettings()).Returns(new ExchangeUserIdentity() { Error = string.Empty, Name = login, Password = password });
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == login && v.Password == password))).Returns(serviceInstance);
            _exchangeIntegrationMock.Setup(p => p.CreateTaskItem(It.Is<ExchangeServiceInstance>(v => v.Error == string.Empty && v.Handler != null), It.IsAny<CreateTaskParameters>())).Returns(new ExchangeTaskItem() { Error = string.Empty, ExchangeItem = new EmailMessage(serviceInstance.Handler) { } });
            _exchangeIntegrationMock.Setup(p => p.SendTaskItem(It.Is<ExchangeServiceInstance>(v => v.Error == string.Empty && v.Handler != null), It.Is<ExchangeTaskItem>(v => v.Error == string.Empty && v.ExchangeItem != null), It.IsAny<string>())).Returns(new ExchangeSendObjectResult() { Error = false, ErrorText = string.Empty, Id = expectedResponse.Id });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateTaskWhenVeryBigSubject()
        {
            var expectedResponse = new CreateTaskResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MessageSubjectVeryLong };
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            parameters.Subject = new string('*', 71);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateTaskWhenWithoutRequiredParameters()
        {
            var expectedResponse = new CreateTaskResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MessageParametersNotDefined };
            CreateTaskParameters parameters = new CreateTaskParameters();
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.IsTrue(actualResponse.ErrorText.Contains(expectedResponse.ErrorText));
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateTaskWhenDateStartLessCurrentDate()
        {
            var expectedResponse = new CreateTaskResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MeetingDateStartIncorrect };
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            parameters.StartDate = DateTime.Now.AddHours(-1);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateTaskWhenDateEndLessCurrentDate()
        {
            var expectedResponse = new CreateTaskResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MeetingDateEndIncorrect };
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            parameters.EndDate = DateTime.Now.AddHours(-1);
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateTaskWhenReminderLessZero()
        {
            var expectedResponse = new CreateTaskResult { Error = true, Id = string.Empty, ErrorText = ServiceMessages.MeetingReminderLessZero };
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            parameters.ReminderMinutesBeforeStart = -1;
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = string.Empty, Handler = new ExchangeService() });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        [TestMethod]
        public void CantCreateTaskWhenExchangeDiscoverError()
        {
            var expectedResponse = new CreateTaskResult { Error = true, Id = string.Empty, ErrorText = "пример текста ошибки Exchange" };
            string login = "login";
            string password = "password";
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            _serviceSettingsMock.Setup(p => p.GetUserSettings()).Returns(new ExchangeUserIdentity() { Error = string.Empty, Name = login, Password = password });
            _exchangeIntegrationMock.Setup(p => p.GetServiceHandler(It.Is<ExchangeUserIdentity>(v => v.Name == "login" && v.Password == "password"))).Returns(new ExchangeServiceInstance() { Error = expectedResponse.ErrorText, Handler = new ExchangeService() { Url = null } });
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.AreEqual(expectedResponse.Id, actualResponse.Id);
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        /// <summary>
        /// Создание заведомо корректных параметров для создания задачи. Далее в тестах они модифицируются для моделирования ошибочных ситуаций.
        /// </summary>
        /// <returns></returns>
        private CreateTaskParameters GetCorrectTestParametersForTask()
        {
            CreateTaskParameters parameters = new CreateTaskParameters();
            parameters.Body = new string('*', 5000);
            parameters.StartDate = DateTime.Now.AddHours(1);
            parameters.EndDate = parameters.StartDate.AddDays(1);
            parameters.Subject = new string('*', 30);
            parameters.ContactEmail = new string[] { "test2@maxus.ru", "test3@maxus.ru" };
            parameters.ReminderMinutesBeforeStart = 99;
            parameters.Importance = ImportanceTypes.High;
            parameters.Status = TasksStatus.NotStarted;
            return parameters;
        }
    }
}
