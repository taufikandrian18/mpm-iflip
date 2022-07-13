using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ContentBankReportingAppService : FLPAppServiceBase, IContentBankReportingAppService
    {
        private readonly IRepository<ContentBanks, Guid> _repositoryContent;
        private readonly IRepository<ContentBankDetails, Guid> _repositoryDetail;
        private readonly IRepository<ContentBankAssignees, Guid> _repositoryAssignee;
        private readonly IRepository<InternalUsers> _repositoryInternalUser;
        private readonly IRepository<Dealers, string> _repositoryDealer;
        private readonly IRepository<ContentBankAssigneeProofs, Guid> _repositoryAssigneeProofs;
        
        public ContentBankReportingAppService(
            IRepository<ContentBanks, Guid> repositoryContent,
            IRepository<ContentBankDetails, Guid> repositoryDetail,
            IRepository<ContentBankAssignees, Guid> repositoryAssignee,
            IRepository<InternalUsers> repositoryInternalUser,
            IRepository<Dealers, string> repositoryDealer,
            IRepository<ContentBankAssigneeProofs, Guid> repositoryAssigneeProofs)
        {
            _repositoryContent = repositoryContent;
            _repositoryDetail = repositoryDetail;
            _repositoryAssignee = repositoryAssignee;
            _repositoryInternalUser = repositoryInternalUser;
            _repositoryDealer = repositoryDealer;
            _repositoryAssigneeProofs = repositoryAssigneeProofs;
        }

        public BaseResponse GetAllDownload([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var allList = (from content in _repositoryContent.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           join det in _repositoryDetail.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on content.Id equals det.GUIDContentBank
                           join assignee in _repositoryAssignee.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on det.Id equals assignee.GUIDContentBankDetail
                           join abp in _repositoryInternalUser.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on assignee.GUIDEmployee equals abp.AbpUserId
                           join dealermpm in _repositoryDealer.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on abp.KodeDealerMPM equals dealermpm.KodeDealerMPM
                           select new ContentBankSosmedDto
                           {
                               Id = content.Id,
                               CreationTime = content.CreationTime,
                               Name = content.Name,
                               KodeDealerMPM = abp.KodeDealerMPM,
                               KodeDealerAHM = abp.KodeDealerAHM,
                               NamaDealer = abp.DealerName,
                               Kota = abp.DealerKota,
                               Channel = abp.Channel,
                               Status = assignee.Status,
                               Karesidenan = dealermpm.Karesidenan
                           });

            if (!string.IsNullOrEmpty(request.Channel))
            {
                allList = allList.Where(x => x.Channel.Contains(request.Channel));
            }

            var query = from item in allList
                        let key = new
                        {
                            Karesidenan = item.Karesidenan,
                            KodeDealerAHM = item.KodeDealerAHM,
                            KodeDealerMPM = item.KodeDealerMPM,
                            NamaDealer = item.NamaDealer
                        }
                        group new
                        {
                            Content = item.Name,
                            Status = item.Status
                        } by key;

            var output = new List<ContentBankDownloadDto>();

            foreach (var item in query)
            {
                foreach (var detail in item)
                {
                    var category = new ContentBankDownloadDto
                    {
                        Karesidenan = item.Key.Karesidenan,
                        KodeDealerAHM = item.Key.KodeDealerAHM,
                        KodeDealerMPM = item.Key.KodeDealerMPM,
                        NamaDealer = item.Key.NamaDealer,
                        Content = detail.Content,
                        Status = detail.Status
                    };
                    output.Add(category);
                }
            }

            if (!string.IsNullOrEmpty(request.Query))
            {
                output = output.Where(x => x.Karesidenan.Contains(request.Query)
                                        || x.KodeDealerAHM.Contains(request.Query)
                                        || x.KodeDealerMPM.Contains(request.Query)
                                        || x.NamaDealer.Contains(request.Query)
                                        || x.Content.Contains(request.Query)).ToList();
            }

            var count = output.ToList().Count();
            var data = output.Skip(request.Page).Take(request.Limit).ToList();
            return BaseResponse.Ok(data, count);
        }

        public List<ContentBankDownloadDto> ExportExcelDownload(string channel, string search)
        {
            var allList = (from content in _repositoryContent.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           join det in _repositoryDetail.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on content.Id equals det.GUIDContentBank
                           join assignee in _repositoryAssignee.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on det.Id equals assignee.GUIDContentBankDetail
                           join abp in _repositoryInternalUser.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on assignee.GUIDEmployee equals abp.AbpUserId
                           join dealermpm in _repositoryDealer.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                           on abp.KodeDealerMPM equals dealermpm.KodeDealerMPM
                           select new ContentBankSosmedDto
                           {
                               Id = content.Id,
                               CreationTime = content.CreationTime,
                               Name = content.Name,
                               KodeDealerMPM = abp.KodeDealerMPM,
                               KodeDealerAHM = abp.KodeDealerAHM,
                               NamaDealer = abp.DealerName,
                               Kota = abp.DealerKota,
                               Channel = abp.Channel,
                               Status = assignee.Status,
                               Karesidenan = dealermpm.Karesidenan
                           });

            if (!string.IsNullOrEmpty(channel))
            {
                allList = allList.Where(x => x.Channel.Contains(channel));
            }

            var query = from item in allList
                        let key = new
                        {
                            Karesidenan = item.Karesidenan,
                            KodeDealer = item.KodeDealerAHM,
                            NamaDealer = item.NamaDealer
                        }
                        group new
                        {
                            Content = item.Name,
                            Status = item.Status
                        } by key;

            var output = new List<ContentBankDownloadDto>();

            foreach (var item in query)
            {
                foreach (var detail in item)
                {
                    var category = new ContentBankDownloadDto
                    {
                        Karesidenan = item.Key.Karesidenan,
                        KodeDealerAHM = item.Key.KodeDealer,
                        NamaDealer = item.Key.NamaDealer,
                        Content = detail.Content,
                        Status = detail.Status
                    };
                    output.Add(category);
                }

            }

            if (!string.IsNullOrEmpty(search))
            {
                output = output.Where(x => x.Karesidenan.Contains(search)
                                        || x.KodeDealerAHM.Contains(search)
                                        || x.KodeDealerMPM.Contains(search)
                                        || x.NamaDealer.Contains(search)
                                        || x.Content.Contains(search)).ToList();
            }

            return output;
        }

        public BaseResponse GetAllSosmed([FromQuery] Pagination request)
        {
            try
            {
                request = Paginate.Validate(request);

                Guid idwa = Guid.Parse("9913B909-1622-4794-AED8-422596221638");
                Guid idfb = Guid.Parse("F8D6B7A2-3E38-4330-9B94-1052C6091D22");
                Guid idig = Guid.Parse("039109EC-5994-4EFF-A72D-6627840A61F6");

                var query = (from content in _repositoryContent.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                             join det in _repositoryDetail.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                             on content.Id equals det.GUIDContentBank 
                             join assignee in _repositoryAssignee.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                             on det.Id equals assignee.GUIDContentBankDetail 
                             join abp in _repositoryInternalUser.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                             on assignee.GUIDEmployee equals abp.AbpUserId 
                             join dealermpm in _repositoryDealer.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                             on abp.KodeDealerMPM equals dealermpm.KodeDealerMPM 
                             select new ContentBankSosmedDto
                             {
                                 Id = content.Id,
                                 CreationTime = content.CreationTime,
                                 Name = content.Name,
                                 UserId = abp.IDMPM,
                                 KodeDealerAHM = abp.KodeDealerAHM,
                                 KodeDealerMPM = abp.KodeDealerMPM,
                                 NamaDealer = abp.DealerName,
                                 Username = abp.Nama,
                                 Kota = abp.DealerKota,
                                 Channel = abp.Channel,
                                 Karesidenan = dealermpm.Karesidenan,
                                 DownloadDate = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).First() :
                                             (DateTime?)null,
                                 UploadWa = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.AttachmentURL).Count() > 0 ) ? 
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.AttachmentURL).First() : 
                                             "",
                                 TotalViewWa = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).First() :
                                             0,
                                 UploadDateFb = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).First() :
                                             (DateTime?)null,
                                 LinkFb = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).First() :
                                             "",
                                 TotalViewFb = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).First() :
                                             0,
                                 UploadDateIg = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).First() :
                                             (DateTime?)null,
                                 LinkIg = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).First() :
                                             "",
                                 TotalViewIg = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).Count() > 0) ?
                                             _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).First() :
                                             0,
                             });

                if (!string.IsNullOrEmpty(request.Channel))
                {
                    query = query.Where(x => x.Channel.Contains(request.Channel));
                }
                if (!string.IsNullOrEmpty(request.EnumStatus))
                {
                    if (request.EnumStatus == "Sudah Upload")
                    {
                        query = query.Where(x => x.UploadWa != "" && x.UploadDateFb != null && x.UploadDateIg != null);
                    }
                    else if (request.EnumStatus == "Belum Upload")
                    {
                        query = query.Where(x => x.UploadWa == "" && x.UploadDateFb == null && x.UploadDateIg == null);
                    }
                }
                if (!string.IsNullOrEmpty(request.Query))
                {
                    query = query.Where(x => x.UserId.ToString().Contains(request.Query)
                                          || x.Name.Contains(request.Query)
                                          || x.KodeDealerMPM.Contains(request.Query)
                                          || x.NamaDealer.Contains(request.Query)
                                          || x.Kota.Contains(request.Query));

                }

                var count = query.ToList().Count();
                var data = query.Skip(request.Page).Take(request.Limit).ToList();

                return BaseResponse.Ok(data, count);
            } catch (Exception e) {
                return BaseResponse.Ok(e.Message, 0);
            } 

        }

        public List<ContentBankSosmedDto> ExportExcelSosmed(string channel = "", string EnumStatus = "")
        {
            Guid idwa = Guid.Parse("9913B909-1622-4794-AED8-422596221638");
            Guid idfb = Guid.Parse("F8D6B7A2-3E38-4330-9B94-1052C6091D22");
            Guid idig = Guid.Parse("039109EC-5994-4EFF-A72D-6627840A61F6");

            var query = (from content in _repositoryContent.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            join det in _repositoryDetail.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            on content.Id equals det.GUIDContentBank
                            join assignee in _repositoryAssignee.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            on det.Id equals assignee.GUIDContentBankDetail
                            join abp in _repositoryInternalUser.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            on assignee.GUIDEmployee equals abp.AbpUserId
                            join dealermpm in _repositoryDealer.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            on abp.KodeDealerMPM equals dealermpm.KodeDealerMPM
                            select new ContentBankSosmedDto
                            {
                                Id = content.Id,
                                CreationTime = content.CreationTime,
                                Name = content.Name,
                                UserId = abp.IDMPM,
                                KodeDealerAHM = abp.KodeDealerAHM,
                                NamaDealer = abp.DealerName,
                                Username = abp.Nama,
                                Kota = abp.DealerKota,
                                Channel = abp.Channel,
                                Karesidenan = dealermpm.Karesidenan,
                                DownloadDate = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).First() :
                                            (DateTime?)null,
                                UploadWa = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.AttachmentURL).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.AttachmentURL).First() :
                                            "",
                                TotalViewWa = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idwa && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).First() :
                                            0,
                                UploadDateFb = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).First() :
                                            (DateTime?)null,
                                LinkFb = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).First() :
                                            "",
                                TotalViewFb = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idfb && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).First() :
                                            0,
                                UploadDateIg = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.UploadDate).First() :
                                            (DateTime?)null,
                                LinkIg = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.RelatedLink).First() :
                                            "",
                                TotalViewIg = (_repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).Count() > 0) ?
                                            _repositoryAssigneeProofs.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.GUIDContentBankPlatform == idig && x.GUIDContentBankAssignee == assignee.Id).Select(x => x.ViewCount).First() :
                                            0,
                            });

            if (!string.IsNullOrEmpty(channel))
            {
                query = query.Where(x => x.Channel.Contains(channel));
            }

            if (!string.IsNullOrEmpty(EnumStatus))
            {
                if (EnumStatus == "Sudah Upload")
                {
                    query = query.Where(x => x.UploadWa != "" && x.UploadDateFb != null && x.UploadDateIg != null);
                }
                else if (EnumStatus == "Belum Upload")
                {
                    query = query.Where(x => x.UploadWa == "" && x.UploadDateFb == null && x.UploadDateIg == null);
                }
            }

            return query.ToList();
        }
    }
}
