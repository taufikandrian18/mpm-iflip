using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ISDMSMessage : IApplicationService
    {

        IList<SDMSMessage> GetAll();
        IList<SDMSMessage> GetSentBox(string userId, int limit, int page);
        IList<SDMSMessageVM> GetInbox(string userId, int limit, int page);
        SDMSMessage GetDetail(Guid messageID);
        void Create(SDMSMessage input);
        void SoftDelete(Guid id);
        void Read(Guid messageID, string readerID);
    }
}
