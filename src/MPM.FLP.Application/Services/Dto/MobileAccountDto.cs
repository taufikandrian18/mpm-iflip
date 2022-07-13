using Microsoft.AspNetCore.Http;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{

    public class TemporaryMobileDashboardDto
    {
        public List<Articles> Articles { get; set; }

        public BrandCampaigns BrandCampaigns { get; set; }
        public List<SalesTalks> SalesTalks { get; set; }
        public List<ServiceTalkFlyers> ServiceTalkFlyers { get; set; }
    }

    public class UpdateProfilePictureDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public IFormFile ProfilePictureFile { get; set; }
        [Required]
        public string Channel { get; set; }
    }
}
