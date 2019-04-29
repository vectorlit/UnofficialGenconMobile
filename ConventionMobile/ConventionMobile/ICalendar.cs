using ConventionMobile.Model;
using System;
using System.Collections.Generic;

namespace ConventionMobile
{
    public interface ICalendar
    {
        void AddToCalendar(GenEvent genEvent);
        void CreateService();

        DateTime ConvertToIndy(DateTime dateTime);

    }
}
