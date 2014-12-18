using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using Moq;
using SimpleExchangeService;
using ExchangeIntegrationCommon.DAL;
using ExchangeIntegrationCommon;
using Microsoft.Exchange.WebServices.Data;

namespace ExchangeServiceTestIntegration
{
    [TestClass]
    public class ExchangeServiceTestIntegration
    {
        private UnityContainer _container;
        private ISimpleExchangeServiceV1 _simpleExchangeService;

        [TestInitialize]
        public void Init()
        {
            _container = new UnityContainer();
            _container.RegisterType<ISimpleExchangeServiceV1, SimpleExchangeService.SimpleExchangeService>();
            _container.RegisterType<IExchangeIntegration, ExchangeIntegration>();
            _container.RegisterType<IServiceSettings, ExchangeServiceSettings>();
            _container.RegisterType<ILogger, Logger>();
            _simpleExchangeService = _container.Resolve<ISimpleExchangeServiceV1>();            
        }

        [TestMethod]
        public void Int_CanCreateMeetingRequestWhenParametersOk()
        {
            var expectedResponse = new CreateMeetingRequestResult { Error = false, ErrorText = string.Empty };
            CreateMeetingParameters parameters = GetCorrectTestParameters();
            parameters.FromEMail = string.Empty;
            CreateMeetingRequestResult actualResponse = _simpleExchangeService.CreateMeetingRequest(parameters);
            Assert.IsTrue(!string.IsNullOrEmpty(actualResponse.Id));
            Assert.AreEqual(expectedResponse.ErrorText, actualResponse.ErrorText);
            Assert.AreEqual(expectedResponse.Error, actualResponse.Error);
        }

        /// <summary>
        /// Создание заведомо корректных параметров. Далее в тестах они модифицируются для моделирования ошибочных ситуаций.
        /// </summary>
        /// <returns></returns>
        private CreateMeetingParameters GetCorrectTestParameters()
        {
            CreateMeetingParameters parameters = new CreateMeetingParameters();
            string TestEMail = "sdyachenko@maxus.ru";// на этот адрес придет оповещение
            parameters.Body = new string('*', 5000);
            parameters.Start = DateTime.Now.AddHours(1);
            parameters.End = parameters.Start.AddDays(1);
            parameters.FromEMail = TestEMail;
            parameters.Location = "переговорная 11";
            parameters.Subject = "Встреча создана на основании интеграционного теста";
            parameters.Attendies = new string[] { TestEMail };
            parameters.ReminderMinutesBeforeStart = 99;
            return parameters;
        }

        //--------------------------------------тесты задач--------------------------------------

        [TestMethod]
        public void Int_CanCreateTaskWhenParametersOk()
        {
            var expectedResponse = new CreateTaskResult { Error = false, ErrorText = string.Empty };
            CreateTaskParameters parameters = GetCorrectTestParametersForTask();
            //parameters.FromEMail = string.Empty;
            CreateTaskResult actualResponse = _simpleExchangeService.CreateTask(parameters);
            Assert.IsTrue(!string.IsNullOrEmpty(actualResponse.Id));
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
            parameters.StartDate = DateTime.Now.AddMinutes(10);
            parameters.EndDate = parameters.StartDate.AddDays(1);
            parameters.Subject = new string('*', 30);
            parameters.ContactEmail = new string[] { "sdyachenko@maxus.ru" };
            parameters.ReminderMinutesBeforeStart = 99;
            parameters.Importance = ImportanceTypes.High;
            parameters.Status = TasksStatus.NotStarted;
            return parameters;
        }

    }
}
