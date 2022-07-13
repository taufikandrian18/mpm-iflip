using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace MPM.FLP.Authorization
{
    public class FLPAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            //context.CreatePermission(PermissionNames.Pages_Users, new FixedLocalizableString("Admin"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.H1, new FixedLocalizableString("Admin H1"));
            context.CreatePermission(PermissionNames.H2, new FixedLocalizableString("Admin H2"));
            context.CreatePermission(PermissionNames.H3, new FixedLocalizableString("Admin H3"));
            context.CreatePermission(PermissionNames.HC3, new FixedLocalizableString("Admin HC3"));
            context.CreatePermission("MobileUsers", new FixedLocalizableString("Mobile Users"));

            //var menu = context.CreatePermission("Menu", new FixedLocalizableString("Menu"));
            //var child = menu.CreateChildPermission("Menu.Katalog", new FixedLocalizableString("Katalog"));
            context.CreatePermission("Menu.Katalog", new FixedLocalizableString("Katalog"));
            context.CreatePermission("Menu.Artikel", new FixedLocalizableString("Artikel"));
            context.CreatePermission("Menu.Panduan", new FixedLocalizableString("Panduan"));
            context.CreatePermission("Menu.Inbox", new FixedLocalizableString("Inbox"));
            context.CreatePermission("Menu.ServiceTalk", new FixedLocalizableString("Service Talk"));
            context.CreatePermission("Menu.BrandCampaign", new FixedLocalizableString("Brand Campaign"));
            context.CreatePermission("Menu.InfoMainDealer", new FixedLocalizableString("Info Main Dealer"));
            context.CreatePermission("Menu.MotivationCards", new FixedLocalizableString("Motivation Card"));
            context.CreatePermission("Menu.CSChampionClub", new FixedLocalizableString("CS Champion Club"));
            context.CreatePermission("Menu.CSChampionClubParticipant", new FixedLocalizableString("CS Champion Club Participant"));
            context.CreatePermission("Menu.CSChampionClubRegistration", new FixedLocalizableString("CS Champion Club Registration"));
            context.CreatePermission("Menu.ClubCommunities", new FixedLocalizableString("Club Communities"));
            context.CreatePermission("Menu.ClubCategories", new FixedLocalizableString("Club Categories")); 
            //context.CreatePermission("Menu.ClaimProgram", new FixedLocalizableString("Claim Program"));
            //context.CreatePermission("Menu.SalesIncentiveProgram", new FixedLocalizableString("Sales Incentive Program"));
            context.CreatePermission("Menu.SalesIncentivePrograms", new FixedLocalizableString("Sales Incentive Programs"));
            context.CreatePermission("Menu.MasterPoints", new FixedLocalizableString("Master Points"));
            context.CreatePermission("Menu.MasterPointsHistories", new FixedLocalizableString("Master Points Histories"));
            //context.CreatePermission("Menu.SalesDevelopmentContest", new FixedLocalizableString("Sales Development Contest"));
            context.CreatePermission("Menu.SalesProgram", new FixedLocalizableString("Sales Program"));
            context.CreatePermission("Menu.ServiceProgram", new FixedLocalizableString("Service Program"));
            context.CreatePermission("Menu.OnlineMagazine", new FixedLocalizableString("Online Magazine"));
            context.CreatePermission("Menu.SalesTalk", new FixedLocalizableString("Sales Talk"));
            context.CreatePermission("Menu.UserManagement", new FixedLocalizableString("Users Management"));

            //fase 4 
            context.CreatePermission("Menu.HomeworkQuiz", new FixedLocalizableString("Homework Quiz"));
            context.CreatePermission("Menu.LiveQuiz", new FixedLocalizableString("Live Quiz"));
            context.CreatePermission("Menu.VerifikasiJabatans", new FixedLocalizableString("Verifikasi Jabatan"));
            context.CreatePermission("Menu.ClaimPrograms", new FixedLocalizableString("Claim Programs"));
            context.CreatePermission("Menu.Roleplay", new FixedLocalizableString("Roleplay"));
            context.CreatePermission("Menu.SelfRecording", new FixedLocalizableString("Self Recording")); 
            context.CreatePermission("Menu.MasterMenuPoint", new FixedLocalizableString("Master Menu Point"));
            context.CreatePermission("Menu.ActivityLog", new FixedLocalizableString("Activity Log"));

            //var administration = context.CreatePermission("Menu", new FixedLocalizableString("Menu"));
            //var productCatalog = administration.CreateChildPermission("Menu.ProductCatalog", new FixedLocalizableString("Product Catalog"));

            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FLPConsts.LocalizationSourceName);
        }
    }
}
