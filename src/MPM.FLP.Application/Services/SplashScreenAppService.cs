using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class SplashScreenAppService : FLPAppServiceBase, ISplashScreenAppService
    {
        private readonly IRepository<SplashScreen, Guid> _repositorySplashScreen;
        private readonly IRepository<SplashScreenDetails, Guid> _repositoryDetail;
        private readonly IAbpSession _abpSession; 
        private readonly LogActivityAppService _logActivityAppService;

        public SplashScreenAppService(
            IRepository<SplashScreen, Guid> repositorySplashScreen,
            IRepository<SplashScreenDetails, Guid> repositoryDetail,
            IAbpSession abpSession, 
            LogActivityAppService logActivityAppService)
        {
            _repositorySplashScreen = repositorySplashScreen;
            _repositoryDetail = repositoryDetail;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public BaseResponse GetAllAdmin([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositorySplashScreen.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query));
            }

            if(request.IsH1 != null){
                query = query.Where(x=> x.H1);
            }

            if(request.IsH2 != null){
                query = query.Where(x=> x.H2);
            }

            if(request.IsH3 != null){
                query = query.Where(x=> x.H3);
            }

            if(request.IsTBSM != null){
                query = query.Where(x=> x.IsTbsm);
            }


            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BaseResponse GetUser([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositorySplashScreen.GetAll().Include(x=> x.SplashScreenDetails).Where(x => x.DeletionTime == null).Where(x=> x.IsPublished);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query));
            }

            if(request.IsH1 != null){
                query = query.Where(x=> x.H1);
            }

            if(request.IsH2 != null){
                query = query.Where(x=> x.H2);
            }

            if(request.IsH3 != null){
                query = query.Where(x=> x.H3);
            }

            if(request.IsTBSM != null){
                query = query.Where(x=> x.IsTbsm);
            }


            
            var data = query.Where(x=> x.SplashScreenDetails != null).OrderByDescending(x=> x.CreationTime).FirstOrDefault();

            if (data.SplashScreenDetails != null){
                data.SplashScreenDetails = data.SplashScreenDetails.OrderBy(x=> x.Orders).ToList();
            }
            return BaseResponse.Ok(data, 0);
        }

        public SplashScreen GetById(Guid Id)
        {
            //var data = _repositorySplashScreen.GetAll()
            //    .Include(x => x.SplashScreenDetails)
            //    .FirstOrDefault(x => x.Id == Id);
            //data.SplashScreenDetails = data.SplashScreenDetails.Where(x => x.DeletionTime == null).OrderBy(x => x.Orders).ToList();
            //return data;

            var data = _repositorySplashScreen.GetAll()
                .Include(x => x.SplashScreenDetails)
                .Where(x => x.SplashScreenDetails.Any(z => string.IsNullOrEmpty(z.DeleterUsername)))
                .FirstOrDefault(x => x.Id == Id);
            return data;
        }
        public void Create(SplashScreenCreateDto input)
        {
            #region Create Splash Screen
            var splashScreen = ObjectMapper.Map<SplashScreen>(input);
            splashScreen.CreationTime = DateTime.Now;
            splashScreen.CreatorUsername = this.AbpSession.UserId.ToString();

            Guid splashScreenId = _repositorySplashScreen.InsertAndGetId(splashScreen);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, this.AbpSession.UserId.ToString(), "Splashscreen", splashScreenId, input.Title, LogAction.Create.ToString(), null, splashScreen);

            #endregion

            #region Create Splash Screen Details
            foreach (var details in input.Details)
            {
                var _details = ObjectMapper.Map<SplashScreenDetails>(details);
                _details.GUIDSplashScreen = splashScreenId;
                _details.CreatorUsername = this.AbpSession.UserId.ToString();
                _details.CreationTime = DateTime.Now;
                _repositoryDetail.Insert(_details);
            }
            #endregion
        }

        public void Update(SplashScreenUpdateDto input)
        {
            #region Update SplashScreen
            var splashscreen = _repositorySplashScreen.Get(input.Id);
            var oldObject = _repositorySplashScreen.Get(input.Id);
            splashscreen.Title = input.Title;
            splashscreen.Description = input.Description;
            splashscreen.Duration = input.Duration;
            splashscreen.Link = input.Link;
            splashscreen.H1 = input.H1;
            splashscreen.H2 = input.H2;
            splashscreen.H3 = input.H3;
            splashscreen.IsTbsm = input.IsTbsm;
            splashscreen.IsPublished = input.IsPublished;
            splashscreen.LastModifierUsername = this.AbpSession.UserId.ToString();
            splashscreen.LastModificationTime = DateTime.Now;

            _repositorySplashScreen.Update(splashscreen);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, this.AbpSession.UserId.ToString(), "Splashscreen", input.Id, input.Title, LogAction.Update.ToString(), oldObject, splashscreen);
            #endregion

            #region Update SplashScreen Details
            var details = _repositoryDetail.GetAllList(x => x.GUIDSplashScreen == input.Id && x.DeletionTime == null);
            foreach (var detail in details)
            {
                var det = _repositoryDetail.GetAllList(x => x.Id == detail.Id).FirstOrDefault();
                if (det != null)
                    _repositoryDetail.Delete(det);
            }

            foreach (var detail in input.Details)
            {
                var _details = ObjectMapper.Map<SplashScreenDetails>(detail);
                _details.GUIDSplashScreen = input.Id;
                _details.CreatorUsername = this.AbpSession.UserId.ToString();
                _details.CreationTime = DateTime.Now;
                _details.LastModifierUsername = this.AbpSession.UserId.ToString();
                _details.LastModificationTime = DateTime.Now;
                _repositoryDetail.Insert(_details);
            }
            //var details = _repositoryDetail.GetAllList(x => x.GUIDSplashScreen == input.Id && x.DeletionTime == null);
            //var i = 0;
            //foreach (var detail in details)
            //{
            //    detail.Name = input.Details[i].Name;
            //    detail.Orders = input.Details[i].Orders;
            //    detail.Caption = input.Details[i].Caption;
            //    detail.Extension = input.Details[i].Extension;
            //    detail.ImageUrl = input.Details[i].ImageUrl;
            //    detail.LastModifierUsername = this.AbpSession.UserId.ToString();
            //    detail.LastModificationTime = DateTime.Now;

            //    _repositoryDetail.Update(detail);
            //    i++;
            //}
            #endregion

        }

        public void SoftDelete(SplashScreenDeleteDto input)
        {
            var splashScreen = _repositorySplashScreen.Get(input.Id);
            var oldObject = _repositorySplashScreen.Get(input.Id);
            splashScreen.DeleterUsername = this.AbpSession.UserId.ToString();
            splashScreen.DeletionTime = DateTime.Now;
            _repositorySplashScreen.Update(splashScreen);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, this.AbpSession.UserId.ToString(), "Splashscreen", input.Id, splashScreen.Title, LogAction.Delete.ToString(), oldObject, splashScreen);

            SoftDeleteDetail(input.Id);
        }

        private void SoftDeleteDetail(Guid SplashScreenId)
        {
            var details = _repositoryDetail.GetAllList(x => x.GUIDSplashScreen == SplashScreenId && x.DeletionTime == null);
            foreach (var detail in details)
            {
                detail.DeleterUsername = this.AbpSession.UserId.ToString();
                detail.DeletionTime = DateTime.Now;
                _repositoryDetail.Update(detail);
            }
        }
    }
}
