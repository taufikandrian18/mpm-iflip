using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AutoMapper;
using MPM.FLP.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace MPM.FLP
{
    [DependsOn(
        typeof(FLPCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class FLPApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<FLPAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(FLPApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
