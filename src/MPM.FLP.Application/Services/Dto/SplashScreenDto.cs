using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(SplashScreen))]
    public class SplashScreenCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Link { get; set; }
        public int Duration { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public bool IsTbsm { get; set; }
        public bool IsPublished { get; set; }
        //public string CreatorUsername { get; set; }
        public List<SplashScreenDetailsDto> Details { get; set; }
    }

    public class SplashScreenUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Link { get; set; }
        public int Duration { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public bool IsTbsm { get; set; }
        public bool IsPublished { get; set; }
        //public string LastModifierUsername { get; set; }
        public List<SplashScreenDetailsDto> Details { get; set; }
    }
    public class SplashScreenDeleteDto
    {
        public Guid Id { get; set; }
        //public string DeleterUsername { get; set; }
    }

    [AutoMapTo(typeof(SplashScreenDetails))]
    public class SplashScreenDetailsDto
    {
        public string Name { get; set; }
        public int Orders { get; set; }
        public string Caption { get; set; }
        public string Extension { get; set; }
        public string ImageUrl { get; set; }
    }

    [AutoMapTo(typeof(SplashScreenDetails))]
    public class SplashScreenDetailsCreateDto
    {
        public Guid GUIDSplashScreen { get; set; }
        public string Name { get; set; }
        public int Orders { get; set; }
        public string Caption { get; set; }
        public string Extension { get; set; }
        public string ImageUrl { get; set; }
        public string DeepLink { get; set; }
    }

    public class SplashScreenDetailsUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDSplashScreen { get; set; }
        public string Name { get; set; }
        public int Orders { get; set; }
        public string Caption { get; set; }
        public string Extension { get; set; }
        public string ImageUrl { get; set; }
        public string DeepLink { get; set; }
    }
}
