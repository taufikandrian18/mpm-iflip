using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using MPM.FLP.Authorization;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ChannelAppService : FLPAppServiceBase, IChannelAppService
    {
        private readonly IRepository<Channels, Guid> _channelRepository;
        

        public ChannelAppService(IRepository<Channels, Guid> ChannelRepository)
        {
            _channelRepository = ChannelRepository;
        }

        public IQueryable<Channels> GetAll()
        {
            return _channelRepository.GetAll();
        }

        public void Create(Channels input)
        {
            _channelRepository.Insert(input);
        }

        public void Update(Channels input)
        {
            _channelRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var Channel = _channelRepository.FirstOrDefault(x => x.Id == id);
            Channel.DeleterUsername = username;
            Channel.DeletionTime = DateTime.Now;
            _channelRepository.Update(Channel);
        }
    }
}
