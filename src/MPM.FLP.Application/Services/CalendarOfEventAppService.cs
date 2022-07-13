using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class CalendarOfEventAppService : FLPAppServiceBase, ICalendarOfEventAppService
    {
        private readonly IRepository<Events, Guid> _eventRepository;
        private readonly IRepository<EventAssignments, Guid> _eventAssignmentRepository;

        public CalendarOfEventAppService(IRepository<Events, Guid> eventRepository, 
                                         IRepository<EventAssignments, Guid> eventAssignmentRepository)
        {
            _eventRepository = eventRepository;
            _eventAssignmentRepository = eventAssignmentRepository;
        }

        public void Assign(CalendarOfEventAssignDto input)
        {
            foreach (var idmpm in input.Assignments) 
            {
                EventAssignments assignment = new EventAssignments();
                assignment.Id = Guid.NewGuid();
                assignment.CreatorUsername = input.CreatorUsername;
                assignment.CreationTime = DateTime.UtcNow.AddHours(7);
                assignment.EventId = input.EventId;
                assignment.IDMPM = idmpm;

                _eventAssignmentRepository.Insert(assignment);
            }
        }

        public void Create(CalendarOfEventCreateDto input)
        {
            Events events = new Events();
            events.Id = Guid.NewGuid();
            events.Title = input.Title;
            events.Contents = input.Contents;
            events.StartDate = input.StartDate;
            events.EndDate = input.EndDate;
            events.InformationProduct = input.Contents;
            events.Budget = input.Budget;
            events.Location = input.Location;
            events.TargetParticipant = input.TargetParticipant;
            events.TargetProspectDb = input.TargetProspectDb;
            events.TargetSales = input.TargetSales;
            events.TargetTestRide = input.TargetTestRide;
            events.CreatorUsername = input.CreatorUsername;
            events.CreationTime = DateTime.UtcNow.AddHours(7);
            events.EventAssignments = new List<EventAssignments>();

            foreach (var idmpm in input.Assignments) 
            {
                EventAssignments assignments = new EventAssignments();
                assignments.Id = Guid.NewGuid();
                assignments.IDMPM = idmpm;
                assignments.EventId = events.Id;
                assignments.CreatorUsername = input.CreatorUsername;
                assignments.CreationTime = DateTime.UtcNow.AddHours(7);
                events.EventAssignments.Add(assignments);
            }

            _eventRepository.Insert(events);
            
        }

        public List<Events> GetEventByUser(int idmpm)
        {
            List<Guid> eventIds = _eventAssignmentRepository.GetAll().Where(x => x.IDMPM == idmpm).Select(x => x.EventId).ToList();

            return _eventRepository.GetAllIncluding(x => x.EventAssignments).Where(x => eventIds.Contains(x.Id)).ToList();
        }

        public async Task<List<MPMEventDto>> GetEventMPMAsync(string kodeDealer)
        {
            var result = new List<MPMEventDto>();
            var key = await AppHelpers.MPMLogin();
            
            var url = AppConstants.MPMEventUrl;
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key);
            client.DefaultRequestHeaders.Add("User", AppConstants.MpmLoginUsername);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var getCustomerResult = await client.GetAsync(url);
            var eventJson = await getCustomerResult.Content.ReadAsStringAsync();

            EventMPMResposeDto eventResponse = JsonConvert.DeserializeObject<EventMPMResposeDto>(eventJson);
            result = eventResponse.data.Where(x => x.KDDEALER == kodeDealer).ToList();

            return result;
        }

        public void UnAssign(CalendarOfEventDeleteDto input)
        {
            foreach (var idmpm in input.IDMPM)
            {
                var assignment =  _eventAssignmentRepository.GetAll().Where(x => x.IDMPM == idmpm && x.EventId == input.EventId).FirstOrDefault();
                _eventAssignmentRepository.Delete(assignment);
            }
        }
    }
}
