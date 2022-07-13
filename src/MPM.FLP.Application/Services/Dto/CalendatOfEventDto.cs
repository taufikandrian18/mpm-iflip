using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class CalendarOfEventCreateDto
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InformationProduct { get; set; }
        public decimal Budget { get; set; }
        public string Location { get; set; }
        public int TargetParticipant { get; set; }
        public int TargetProspectDb { get; set; }
        public int TargetSales { get; set; }
        public int TargetTestRide { get; set; }

        public string CreatorUsername { get; set; }
        public List<int> Assignments { get; set; }

    }

    public class CalendarOfEventAssignDto 
    {
        public Guid EventId { get; set; }
        public string CreatorUsername { get; set; }
        public List<int> Assignments { get; set; }
    }

    public class CalendarOfEventDeleteDto 
    {
        public Guid EventId { get; set; }
        public List<int> IDMPM { get; set; }
    }

    public class MPMEventDto 
    {
        public string KDDEALER { get; set; }
        public string PROPOSALID { get; set; }
        public string PROPOSALNAME { get; set; }
        public string ACTIVITYDESCRIPTION { get; set; }
        public string INFORMATIONPRODUCT { get; set; }
        public int BUDGET { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public string LOCATION { get; set; }
        public int TARGETPARTICIPANT { get; set; }
        public int TARGETPROSPECTDB { get; set; }
        public int TARGETSALES { get; set; }
        public int TARGETTESTRIDE { get; set; }
    }
}
