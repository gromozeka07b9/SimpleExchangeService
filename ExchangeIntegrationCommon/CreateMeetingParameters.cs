using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSDLGeneration;

namespace ExchangeIntegrationCommon
{
    public class CreateMeetingParameters
    {
        public string Body;
        public string Subject;
        public DateTime Start;
        public DateTime End;
        public string Location;
        public int ReminderMinutesBeforeStart;
        public string FromEMail;
        //public string Organizer;
        public string[] Attendies;

        public CreateMeetingParameters()
        {

        }
    }
}
