using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using MPM.FLP.Authorization;

namespace MPM.FLP.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class FLPNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                //.AddItem(
                //    new MenuItemDefinition(
                //        PageNames.Home,
                //        L("HomePage"),
                //        url: "Home",
                //        icon: "home",
                //        requiresAuthentication: true
                //    )
                //)
                .AddItem(
                    new MenuItemDefinition(
                        "Dashboard",
                        new FixedLocalizableString("Dashboard"),
                        url: "Dashboard",
                        icon: "dashboard",
                        requiresAuthentication: true
                    )
                )
                .AddItem( // Menu items below is just for demonstration!
                    new MenuItemDefinition(
                        "PanduanLayanan",
                        new FixedLocalizableString("Panduan"),
                        icon: "description",
                        requiredPermissionName:"Menu.Panduan"
                    ).AddItem(
                        new MenuItemDefinition(
                            "ManagementPanduanLayanan",
                            new FixedLocalizableString("Management Panduan Layanan"),
                            url: "ManagementPanduanLayanan",
                            requiresAuthentication:true
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "ManagementPanduanTeknikal",
                            new FixedLocalizableString("Management Panduan Teknikal"),
                            url: "ManagementPanduanTeknikal",
                            requiresAuthentication: true
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "KategoriPanduanLayanan",
                            new FixedLocalizableString("Kategori Panduan Layanan"),
                            url: "KategoriPanduanLayanan",
                            requiresAuthentication: true
                        )
                    )
                    //.AddItem(
                    //    new MenuItemDefinition(
                    //        "ManageVideo",
                    //        new FixedLocalizableString("Manage Video"),
                    //        url: "ManagementVideo",
                    //        requiresAuthentication: true
                    //    )
                    //)
                )
                .AddItem(
                   new MenuItemDefinition(
                       "Artikel",
                       new FixedLocalizableString("Artikel"),
                       url: "Artikel",
                       icon: "insert_drive_file",
                       requiresAuthentication: true,
                       requiredPermissionName: "Menu.Artikel"
                   )
                ).AddItem( // Menu items below is just for demonstration!
                    new MenuItemDefinition(
                        "Katalog",
                        new FixedLocalizableString("Katalog"),
                        icon: "motorcycle",
                         requiredPermissionName: "Menu.Katalog"
                    ).AddItem(
                        new MenuItemDefinition(
                            "JenisKatalog",
                            new FixedLocalizableString("Jenis Katalog")
                        ).AddItem(
                        new MenuItemDefinition(
                            "KatalogProduk",
                            new FixedLocalizableString("Katalog Produk"),
                            url: "ProductCatalog",
                            requiresAuthentication: true
                        )
                        ).AddItem(
                            new MenuItemDefinition(
                                "KatalogApparel",
                                new FixedLocalizableString("Katalog Apparel"),
                                url: "Apparel",
                                requiresAuthentication: true
                            )
                        )
                        //.AddItem(
                        //    new MenuItemDefinition(
                        //        "KatalogAksesoris",
                        //        new FixedLocalizableString("Katalog Aksesoris"),
                        //        url: "AccesoriesCatalog"
                        //    )
                        .AddItem(
                            new MenuItemDefinition(
                                "KatalogSparepart",
                                new FixedLocalizableString("Katalog Sparepart"),
                                url: "SparepartCatalog",
                                requiresAuthentication: true
                            )
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "KategoriApparel",
                            new FixedLocalizableString("Kategori Produk"),
                            url: "KategoriProduct",
                            requiresAuthentication: true
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "KategoriApparel",
                            new FixedLocalizableString("Kategori Apparel"),
                            url: "KategoriApparel",
                            requiresAuthentication: true
                        )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "ServiceTalkFlyer",
                        new FixedLocalizableString("Service Talk"),
                        url: "ServiceTalkFlyer",
                        icon: "settings_applications",
                        requiresAuthentication: true,
                        //requiredPermissionName:"Menu.ServiceTalk"
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H2", "Menu.ServiceTalk" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                       "SalesTalk",
                        new FixedLocalizableString("Sales Talk"),
                        url: "SalesTalks",
                        icon: "speaker_notes",
                        requiresAuthentication: true,
                        //requiredPermissionName: "Menu",
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H1", "Menu.SalesTalk" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "ManagementProgram",
                        new FixedLocalizableString("Sales Program"),
                        url: "ProgramManagement",
                        icon: "equalizer",
                        requiresAuthentication: true,
                        //requiredPermissionName: "Menu.Program",
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H1", "Menu.SalesProgram" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "ServiceProgramManagement",
                        new FixedLocalizableString("Service Program"),
                        url: "ServiceProgramManagement",
                        icon: "equalizer",
                        requiresAuthentication: true,
                        //requiredPermissionName: "Menu.Program",
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H2", "Menu.ServiceProgram" })
                    )
                )
                
                .AddItem(
                    new MenuItemDefinition(
                        "BrandCampaign",
                        new FixedLocalizableString("Brand Campaign"),
                        url: "BrandCampaigns",
                        icon: "loyalty",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.BrandCampaign" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "UsersManagement",
                        new FixedLocalizableString("Users Management"),
                        icon: "people",
                         permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.UserManagement" })
                    ).AddItem(
                        new MenuItemDefinition(
                            "InternalUsers",
                            new FixedLocalizableString("Internal Users"),
                            url: "InternalUsers",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H1" })
                        )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "InternalUsers",
                            new FixedLocalizableString("Internal Users"),
                            url: "InternalUsers",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H2" })
                        )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "ExternalUsers",
                            new FixedLocalizableString("External Users"),
                            url: "ExternalUsers",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H3" })
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "ExternalJabatan",
                            new FixedLocalizableString("External Jabatan"),
                            url: "ExternalJabatan",
                            icon: "",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Admin H3" })
                        )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "people",
                        requiredPermissionName: PermissionNames.Pages_Users                    
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Roles,
                        L("Roles"),
                        url: "Roles",
                        icon: "local_offer",
                        requiredPermissionName: PermissionNames.Pages_Roles
                    )
                 )

                // Fase 2

                .AddItem(
                    new MenuItemDefinition(
                        "OnlineMagazines",
                        new FixedLocalizableString("Online Magazines"),
                        url: "OnlineMagazines",
                        icon: "library_books",
                        requiredPermissionName: "Menu.OnlineMagazine",
                        requiresAuthentication: true
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "InfoMainDealers",
                        new FixedLocalizableString("Info Main Dealers"),
                        url: "InfoMainDealers",
                        icon: "perm_device_information",
                        requiresAuthentication: true,
                        //requiredPermissionName: "Menu.Program",
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.InfoMainDealer" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "MotivationCards",
                        new FixedLocalizableString("Motivation Cards"),
                        url: "MotivationCards",
                        icon: "photo_library",
                        requiresAuthentication: true,
                        //requiredPermissionName: "Menu.Program",
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.MotivationCards" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "CSChampionClubs",
                        new FixedLocalizableString("CS Champion Club"),
                        icon: "headset_mic"
                    ).AddItem(
                        new MenuItemDefinition(
                            "Article",
                            new FixedLocalizableString("Article"),
                            url: "CSChampionClubs",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.CSChampionClub" })
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "Participants",
                            new FixedLocalizableString("Participants"),
                            url: "CSChampionClubs/Participants",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.CSChampionClubParticipant" })
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "Registration",
                            new FixedLocalizableString("Registration"),
                            url: "CSChampionClubs/Registration",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.CSChampionClubRegistration" })
                        )
                    )
                )
                
                .AddItem(
                    new MenuItemDefinition(
                        "Inbox",
                        new FixedLocalizableString("Inbox Message"),
                        url: "Inbox",
                        icon: "textsms",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.Inbox" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Clubs",
                        new FixedLocalizableString("Communities"),
                        icon: "group"
                    ).AddItem(
                    new MenuItemDefinition(
                        "ClubCommunities",
                        new FixedLocalizableString("Klub dan Komunitas"),
                        url: "ClubCommunities",
                        icon: "",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.ClubCommunities" })
                        )
                    ).AddItem(
                    new MenuItemDefinition(
                        "ClubCategories",
                        new FixedLocalizableString("Kategori Klub & Komunitas"),
                        url: "ClubCategories",
                        icon: "",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.ClubCategories" })
                        )
                    )
                )

                //Fase 3
                .AddItem(
                    new MenuItemDefinition(
                        "SalesIncentivePrograms",
                        new FixedLocalizableString("Sales Incentive Programs"),
                        url: "SalesIncentivePrograms",
                        icon: "monetization_on",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.SalesIncentivePrograms" })
                    )
                )
                
                .AddItem(
                    new MenuItemDefinition(
                        "SalesDevelopmentContest",
                        new FixedLocalizableString("Sales People Development Contest"),
                        icon: "trending_up"
                    ).AddItem(
                        new MenuItemDefinition(
                            "MasterPoints",
                            new FixedLocalizableString("Master Points"),
                            url: "MasterPoints",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.MasterPoints" })
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "PointsHistories",
                            new FixedLocalizableString("Points Histories"),
                            url: "MasterPointsHistories",
                            requiresAuthentication: true,
                            permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.MasterPointsHistories" })
                        )
                    )
                )

                //fase 4
                .AddItem(
                    new MenuItemDefinition(
                        "Homework Quiz",
                        new FixedLocalizableString("Homework Quiz"),
                        url: "HomeworkQuiz",
                        icon: "event_note",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.HomeworkQuiz" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Live Quiz",
                        new FixedLocalizableString("Live Quiz"),
                        url: "LiveQuiz",
                        icon: "today",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.HomeworkQuiz" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Verifikasi Jabatan",
                        new FixedLocalizableString("Verifikasi Jabatan"),
                        icon: "verified_user",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.VerifikasiJabatans" })
                    ).AddItem(
                        new MenuItemDefinition(
                            "Verifikasi Jabatan",
                            new FixedLocalizableString("Verifikasi Jabatan"),
                            url: "VerifikasiJabatans"
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "Verifikasi Jabatan Histories",
                            new FixedLocalizableString("Verifikasi Jabatan Histories"),
                            url: "VerifikasiJabatans/Histories"
                        )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Claim Programs",
                        new FixedLocalizableString("Claim Programs"),
                        url: "ClaimPrograms",
                        icon: "pan_tool",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.ClaimPrograms" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Roleplay",
                        new FixedLocalizableString("Roleplay"),
                        url: "Roleplay",
                        icon: "camera_roll",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.Roleplay" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "SelfRecording",
                        new FixedLocalizableString("Self Recording"),
                        url: "SelfRecording",
                        icon: "record_voice_over",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.Roleplay" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "MasterMenuPoint",
                        new FixedLocalizableString("Master Menu Point"),
                        url: "MasterMenuPoint",
                        icon: "stars",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.MasterMenuPoint" })
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Activity Log",
                        new FixedLocalizableString("Activity Log"),
                        url: "ActivityLog",
                        icon: "query_builder",
                        requiresAuthentication: true,
                        permissionDependency: new SimplePermissionDependency(true, new string[] { "Menu.ActivityLog" })
                    )
                )
                //.AddItem(
                //    new MenuItemDefinition(
                //        "Roleplay",
                //        new FixedLocalizableString("Roleplay"),
                //        url: "Roleplay",
                //        icon: "camera_roll",
                //        requiresAuthentication: true
                //    )
                //    )

                .AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("About"),
                        url: "About",
                        icon: "info"
                )
                
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FLPConsts.LocalizationSourceName);
        }
    }
}
