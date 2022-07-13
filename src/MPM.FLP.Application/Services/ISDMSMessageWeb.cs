using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ISDMSMesssageWeb : IApplicationService
    {

        IList<SDMSMessageWeb> GetAll();
        IList<SDMSMessageVM> GetSentBox(string userId, int limit, int page);
        IList<SDMSMessageVM> GetInbox(string userId, int limit, int page);
        SDMSMessageWeb GetDetail(Guid messageID);
        void Create(SDMSMessageWeb input);
        void SoftDelete(Guid id);
        void Read(Guid messageID, string readerID);
    }
}
