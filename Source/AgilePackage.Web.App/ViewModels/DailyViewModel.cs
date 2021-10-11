using System;
using System.Collections.Generic;

namespace AgilePackage.Web.App.ViewModels
{
    public class DailyViewModel
    {
        public Guid ProjectId { get; set; }
        public DateTime Date { get; set; }
        public List<PersonDailyDto> Dailies { get; set; }
        public string Daily { get; set; } = string.Empty;
    }

    public class PersonDailyDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
    }
}
