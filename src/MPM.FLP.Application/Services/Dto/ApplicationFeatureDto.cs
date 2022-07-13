using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ApplicationFeature))]
    public class ApplicationFeatureCreateDto
    {
        public string IconUrl { get; set; }
        public string MenuName { get; set; }
        //public string CreatorUsername { get; set; }
        public List<ApplicationFeatureMappingDto> Mapping { get; set; }
    }

    public class ApplicationFeatureUpdateDto
    {
        public Guid Id { get; set; }
        public string IconUrl { get; set; }
        public string MenuName { get; set; }
        //public string LastModifierUsername { get; set; }
        public List<ApplicationFeatureMappingDto> Mapping { get; set; }
    }
    public class ApplicationFeatureMappingUpdateDto
    {
        public Guid Id { get; set; }
        public string EnumChannel { get; set; }
        public Guid GUIDFeature { get; set; }
        public int Status { get; set; }
    }
    public class ApplicationFeatureDeleteDto
    {
        public Guid Id { get; set; }
        //public string DeleterUsername { get; set; }
    }

    [AutoMapTo(typeof(ApplicationFeatureMapping))]
    public class ApplicationFeatureMappingDto
    {
        public string EnumChannel { get; set; }
        public Guid GUIDFeature { get; set; }
        public int Status { get; set; }
    }

    public class ApplicationFeatureResponse
    {
        public Guid Id { get; set; }
        public string IconUrl { get; set; }
        public string MenuName { get; set; }
        public string EnumChannel { get; set; }
        public int Status { get; set; }
    }
}
