using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class SplashScreenDetailAppService : FLPAppServiceBase, ISplashScreenDetailAppService
    {
        private readonly IRepository<SplashScreen, Guid> _repositorySplashScreen;
        private readonly IRepository<SplashScreenDetails, Guid> _repositoryDetail;
        
        public SplashScreenDetailAppService(
            IRepository<SplashScreen, Guid> repositorySplashScreen,
            IRepository<SplashScreenDetails, Guid> repositoryDetail)
        {
            _repositorySplashScreen = repositorySplashScreen;
            _repositoryDetail = repositoryDetail;
        }

        public BaseResponse GetAllAdmin([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryDetail.GetAll().Include(x => x.SplashScreen).Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            if(request.IsH1 != null){
                query = query.Where(x=> x.SplashScreen.H1);
            }

            if(request.IsH2 != null){
                query = query.Where(x=> x.SplashScreen.H2);
            }

            if(request.IsH3 != null){
                query = query.Where(x=> x.SplashScreen.H3);
            }

            if(request.IsTBSM != null){
                query = query.Where(x=> x.SplashScreen.IsTbsm);
            }


            var count = query.Count();

            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BaseResponse GetUser([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryDetail.GetAll().Include(x=> x.SplashScreen).Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            if(request.IsH1 != null){
                query = query.Where(x=> x.SplashScreen.H1);
            }

            if(request.IsH2 != null){
                query = query.Where(x=> x.SplashScreen.H2);
            }

            if(request.IsH3 != null){
                query = query.Where(x=> x.SplashScreen.H3);
            }

            if(request.IsTBSM != null){
                query = query.Where(x=> x.SplashScreen.IsTbsm);
            }

            var data = query.Where(x=> x.GUIDSplashScreen != null).OrderByDescending(x=> x.CreationTime).ToList();

            return BaseResponse.Ok(data, 0);
        }

        public SplashScreenDetails GetById(Guid Id)
        {
            var data  =_repositoryDetail.GetAll()
                .Include(x => x.SplashScreen)
                .FirstOrDefault(x => x.Id == Id);
            return data;
        }
        public void Create(SplashScreenDetailsCreateDto input)
        {
            #region Create Splash Screen Detail
            var detail = ObjectMapper.Map<SplashScreenDetails>(input);
            detail.CreationTime = DateTime.Now;
            detail.CreatorUsername = this.AbpSession.UserId.ToString();

            Guid detailId = _repositoryDetail.InsertAndGetId(detail);
            #endregion
        }

        public void Update(SplashScreenDetailsUpdateDto input)
        {
            #region Update SplashScreen Detail
            var detail = _repositoryDetail.Get(input.Id);
            detail.Name = input.Name;
            detail.Orders = input.Orders;
            detail.Caption = input.Caption;
            detail.Extension = input.Extension;
            detail.ImageUrl = input.ImageUrl;
            detail.DeepLink = input.DeepLink;
            detail.LastModifierUsername = this.AbpSession.UserId.ToString();
            detail.LastModificationTime = DateTime.Now;

            _repositoryDetail.Update(detail);
            #endregion
        }

        public void SoftDelete(SplashScreenDeleteDto input)
        {
            var detail = _repositoryDetail.Get(input.Id);
            detail.DeleterUsername = this.AbpSession.UserId.ToString();
            detail.DeletionTime = DateTime.Now;
            _repositoryDetail.Update(detail);
        }
    }
}
