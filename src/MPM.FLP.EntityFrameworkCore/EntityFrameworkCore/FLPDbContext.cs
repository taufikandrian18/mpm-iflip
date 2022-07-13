using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization.Roles;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using MPM.FLP.FLPDb.MechanicalAssistant;
using MPM.FLP.MultiTenancy;

namespace MPM.FLP.EntityFrameworkCore
{
    public class FLPDbContext : AbpZeroDbContext<Tenant, Role, User, FLPDbContext>
    {

        public FLPDbContext(DbContextOptions<FLPDbContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(36000);
        }

        public virtual DbSet<SDMSMessage> SDMSMessage { get; set; }
        public virtual DbSet<SDMSMessageDetail> SDMSMessageDetail { get; set; }
        public virtual DbSet<SDMSMessageWeb> SDMSMessageWeb { get; set; }
        public virtual DbSet<SDMSMessageWebDetail> SDMSMessageWebDetail { get; set; }

        public virtual DbSet<SDMSLogs> SDMSLogs {get;set;}
        public virtual DbSet<Achievements> Achievements { get; set; }
        public virtual DbSet<ArticleAttachments> ArticleAttachments { get; set; }
        public virtual DbSet<Articles> Articles { get; set; }

        public virtual DbSet<GuideAttachments> GuideAttachments { get; set; }
        public virtual DbSet<GuideCategories> GuideCategories { get; set; }
        public virtual DbSet<GuideTechnicalCategories> GuideTechnicalCategories { get; set; }
        public virtual DbSet<Guides> Guides { get; set; }
        public virtual DbSet<ProgramAttachments> ProgramAttachments { get; set; }
        public virtual DbSet<Programs> Programs { get; set; }

        public virtual DbSet<UserAchievements> UserAchievements { get; set; }
        public virtual DbSet<UserDetails> UserDetails { get; set; }
        public virtual DbSet<SalesTalkAttachments> SalesTalkAttachments { get; set; }
        public virtual DbSet<SalesTalks> SalesTalks { get; set; }

        public virtual DbSet<ServiceTalkFlyerAttachments> ServiceTalkFlyerAttachments { get; set; }
        public virtual DbSet<ServiceTalkFlyers> ServiceTalkFlyers { get; set; }

        public virtual DbSet<AccesoriesCatalogs> AccessoriesCatalogs { get; set; }
        public virtual DbSet<ApparelCatalogs> ApparelCatalogs { get; set; }
        public virtual DbSet<ApparelCategories> ApparelCategories { get; set; }
        public virtual DbSet<ProductAccesories> ProductAccesories { get; set; }
        public virtual DbSet<ProductCatalogs> ProductCatalogs { get; set; }
        public virtual DbSet<ProductCatalogAttachments> ProductCatalogAttachments { get; set; }
        public virtual DbSet<ProductCategories> ProductCategories { get; set; }
        public virtual DbSet<ProductColorVariants> ProductColorVariants { get; set; }
        public virtual DbSet<ProductFeatures> ProductFeatures { get; set; }
        public virtual DbSet<ProductPrices> ProductPrices { get; set; }
        public virtual DbSet<SparepartCatalogs> SparepartCatalogs { get; set; }

        public virtual DbSet<Dealers> Dealers { get; set; }
        public virtual DbSet<Jabatans> Jabatans { get; set; }
        public virtual DbSet<InternalUsers> InternalUsers { get; set; }
        public virtual DbSet<ExternalUsers> ExternalUsers { get; set; }
        public virtual DbSet<AdminUsers> AdminUsers { get; set; }
        public virtual DbSet<Channels> Channels { get; set; }
        public virtual DbSet<ExternalJabatans> ExternalJabatans { get; set; }
        public virtual DbSet<OTPHistory> OTPHistory { get; set; }
        public virtual DbSet<BrandCampaigns> BrandCampaigns { get; set; }
        public virtual DbSet<BrandCampaignAttachments> BrandCampaignAttachments { get; set; }


        //Fase 2
        public virtual DbSet<Kotas> Kotas { get; set; }
        public virtual DbSet<ColorVariantCatalogs> ColorVariantCatalogs { get; set; }

        public virtual DbSet<InfoMainDealers> InfoMainDealers { get; set; }
        public virtual DbSet<InfoMainDealerAttachments> InfoMainDealerAttachments { get; set; }
        public virtual DbSet<MotivationCards> MotivationCards { get; set; }
        public virtual DbSet<OnlineMagazines> OnlineMagazines { get; set; }

        public virtual DbSet<CSChampionClubs> CSChampionClubs { get; set; }
        public virtual DbSet<CSChampionClubAttachments> CSChampionClubAttachments { get; set; }
        public virtual DbSet<CSChampionClubRegistrations> CSChampionClubRegistrations { get; set; }
        public virtual DbSet<CSChampionClubParticipants> CSChampionClubParticipants { get; set; }
        public virtual DbSet<ClubCommunities> ClubCommunities { get; set; }
        public virtual DbSet<ClubCommunityCategories> ClubCommunityCategories { get; set; }

        public virtual DbSet<ForumThreads> ForumThreads { get; set; }
        public virtual DbSet<ForumPosts> ForumPosts { get; set; }

        public virtual DbSet<InboxMessages> InboxMessages { get; set; }
        public virtual DbSet<InboxAttachments> InboxAttachments { get; set; }
        public virtual DbSet<InboxRecipients> InboxRecipients { get; set; }

        //Fase 1 Revisi

        public virtual DbSet<SalesProgramAttachments> SalesProgramAttachments { get; set; }
        public virtual DbSet<SalesPrograms> SalesPrograms { get; set; }
        public virtual DbSet<ServiceProgramAttachments> ServiceProgramAttachments { get; set; }
        public virtual DbSet<ServicePrograms> ServicePrograms { get; set; }

        // Fase 3



        public virtual DbSet<RolePlayDetails> RolePlayDetails { get; set; }
        public virtual DbSet<RolePlayResultDetails> RolePlayResultDetails { get; set; }
        public virtual DbSet<RolePlayResults> RolePlayResults { get; set; }
        public virtual DbSet<RolePlays> RolePlays { get; set; }
        public virtual DbSet<RolePlayAssignments> RolePlayAssignments { get; set; }

        public virtual DbSet<SelfRecordingDetails> SelfRecordingDetails { get; set; }
        public virtual DbSet<SelfRecordingResultDetails> SelfRecordingResultDetails { get; set; }
        public virtual DbSet<SelfRecordingResults> SelfRecordingResults { get; set; }
        public virtual DbSet<SelfRecordings> SelfRecordings { get; set; }
        public virtual DbSet<SelfRecordingAssignments> SelfRecordingAssignments { get; set; }

        public virtual DbSet<KoefisienRegresi> KoefisienRegresi { get; set; }
        public virtual DbSet<JenisVariabel> JenisVariabel { get; set; }
        public virtual DbSet<TipeMotor> TipeMotor { get; set; }
        public virtual DbSet<Variabel> Variabel { get; set; }

        public virtual DbSet<Pekerjaans> Pekerjaans { get; set; }
        public virtual DbSet<Kecamatans> Kecamatans { get; set; }
        public virtual DbSet<Agamas> Agamas { get; set; }

        public virtual DbSet<EventAssignments> EventAssignments { get; set; }
        public virtual DbSet<Events> Events { get; set; }

        public virtual DbSet<SPDCMasterPoints> SPDCMasterPoints { get; set; }
        public virtual DbSet<SPDCPointHistories> SPDCPointHistories { get; set; }

        public virtual DbSet<SalesIncentivePrograms> SalesIncentivePrograms { get; set; }
        public virtual DbSet<SalesIncentiveProgramAttachments> SalesIncentiveProgramAttachments { get; set; }
        public virtual DbSet<SalesIncentiveProgramJabatans> SalesIncentiveProgramJabatans { get; set; }
        public virtual DbSet<SalesIncentiveProgramKotas> SalesIncentiveProgramKotas { get; set; }

        // Fase 4.1

        public virtual DbSet<ClaimProgramClaimers> ClaimProgramClaimers { get; set; }
        public virtual DbSet<ClaimProgramAttachments> ClaimProgramAttachments { get; set; }
        public virtual DbSet<ClaimPrograms> ClaimPrograms { get; set; }

        public virtual DbSet<AgendaAssignments> AgendaAssignments { get; set; }
        public virtual DbSet<Agendas> Agendas { get; set; }

        public virtual DbSet<PushNotificationSubscribers> PushNotificationSubscribers { get; set; }
        public virtual DbSet<TrainingAbsence> TrainingAbsence { get; set; }

        // Fase  4.2
        public virtual DbSet<HomeworkQuizzes> HomeworkQuizzes { get; set; }
        public virtual DbSet<HomeworkQuizQuestions> HomeworkQuizQuestions { get; set; }
        public virtual DbSet<HomeworkQuizHistories> HomeworkQuizHistories { get; set; }
        public virtual DbSet<HomeworkQuizAssignments> HomeworkQuizAssignments { get; set; }
        public virtual DbSet<HomeworkQuizAnswers> HomeworkQuizAnswers { get; set; }

        public virtual DbSet<VerifikasiJabatans> VerifikasiJabatans { get; set; }
        public virtual DbSet<VerifikasiJabatanHistories> VerifikasiJabatanHistories { get; set; }
        public virtual DbSet<VerifikasiJabatanQuestions> VerifikasiJabatanQuestions { get; set; }


        // Fase 4.3
        public virtual DbSet<PointConfigurations> PointConfigurations { get; set; }
        public virtual DbSet<Points> Points { get; set; }
        public virtual DbSet<ActivityLogs> ActivityLogs { get; set; }

        // Fase 4.4
        public virtual DbSet<LiveQuizzes> LiveQuizzes { get; set; }
        public virtual DbSet<LiveQuizQuestions> LiveQuizQuestions { get; set; }
        public virtual DbSet<LiveQuizHistories> LiveQuizHistories { get; set; }
        public virtual DbSet<LiveQuizAssignments> LiveQuizAssignments { get; set; }
        public virtual DbSet<LiveQuizAnswers> LiveQuizAnswers { get; set; }

        public virtual DbSet<WebNotifications> WebNotifications { get; set; }

        public virtual DbSet<ContentBankCategories> ContentBankCategories { get; set; }
        public virtual DbSet<ContentBanks> ContentBanks { get; set; }
        public virtual DbSet<ContentBankDetails> ContentBankDetails { get; set; }
        public virtual DbSet<ContentBankAssignees> ContentBankAssignees { get; set; }
        public virtual DbSet<ContentBankAssigneeProofs> ContentBankAssigneeProofs { get; set; }
        public virtual DbSet<SplashScreen> SplashScreen { get; set; }
        public virtual DbSet<SplashScreenDetails> SplashScreenDetails { get; set; }
        public virtual DbSet<LogActivities> LogActivities { get; set; }
        public virtual DbSet<LogActivityDetails> LogActivityDetails { get; set; }
        public virtual DbSet<ApplicationFeature> ApplicationFeature { get; set; }
        public virtual DbSet<ApplicationFeatureMapping> ApplicationFeatureMapping { get; set; }
        public virtual DbSet<ClaimProgramCampaigns> ClaimProgramCampaigns { get; set; }
        public virtual DbSet<ClaimProgramCampaignPrizes> ClaimProgramCampaignPrizes { get; set; }
        public virtual DbSet<ClaimProgramCampaignPoints> ClaimProgramCampaignPoints { get; set; }
        public virtual DbSet<ClaimProgramContents> ClaimProgramContents { get; set; }
        public virtual DbSet<TBSMUserGurus> TBSMUserGurus { get; set; }
        public virtual DbSet<TBSMUserSiswas> TBSMUserSiswas { get; set; }
        public virtual DbSet<Sekolahs> Sekolahs { get; set; }
        public virtual DbSet<DealerH3> DealerH3 { get; set; }
        public virtual DbSet<ClaimProgramAssignees> ClaimProgramAssignees { get; set; }
        public virtual DbSet<ClaimProgramContentAttachments> ClaimProgramContentAttachments { get; set; }
        public virtual DbSet<ClaimProgramContentClaimers> ClaimProgramContentClaimers { get; set; }
        public virtual DbSet<ClaimProgramCampaignPrizeLogs> ClaimProgramCampaignPrizeLogs { get; set; }
        public virtual DbSet<ProductTypes> ProductTypes { get; set; }
        public virtual DbSet<ProductSeries> ProductSeries { get; set; }
        public virtual DbSet<Fincoy> Fincoy { get; set; }
        public virtual DbSet<TargetSales> TargetSales { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<SalesIncentiveProgramAssignee> SalesIncentiveProgramAssignee { get; set; }
        public virtual DbSet<SalesIncentiveProgramTarget> SalesIncentiveProgramTarget { get; set; }
        public virtual DbSet<BASTCategories> BASTCategories { get; set; }
        public virtual DbSet<BASTs> BASTs { get; set; }
        public virtual DbSet<BASTDetails> BASTDetails { get; set; }
        public virtual DbSet<BASTAttachment> BASTAttachment { get; set; }
        public virtual DbSet<BASTReport> BASTReport { get; set; }
        public virtual DbSet<BASTAssignee> BASTAssignee { get; set; }
        public virtual DbSet<BASTReportAttachment> BASTReportAttachment { get; set; }
        public virtual DbSet<TBPermissions> TBPermissions { get; set; }
        public virtual DbSet<RolePermissions> RolePermissions { get; set; }

        #region Mechanic Assitants
        public virtual DbSet<MechanicAssistantCategories> MechanicAssistantCategories { get; set; }
        public virtual DbSet<MechanicAssistants> MechanicAssistants { get; set; }
        public virtual DbSet<MechanicAssistantAssignees> MechanicAssistantAssignees { get; set; }
        public virtual DbSet<MechanicAssistantContacts> MechanicAssistantContacts { get; set; }
        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Achievements>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ArticleAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleAttachments)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArticleAttachments_Articles");
            });

            modelBuilder.Entity<Articles>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256)
                    .HasDefaultValue("");

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Resource)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.IsPublished)
                    .IsRequired()
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<GuideAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Guide)
                    .WithMany(p => p.GuideAttachments)
                    .HasForeignKey(d => d.GuideId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GuideAttachments_Guides");
            });

            modelBuilder.Entity<GuideCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<GuideTechnicalCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Guides>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256).IsRequired();

                entity.Property(e => e.Resource).HasMaxLength(256).IsRequired();

                entity.HasOne(d => d.GuideCategory)
                    .WithMany(p => p.Guides)
                    .HasForeignKey(d => d.GuideCategoryId)
                    .HasConstraintName("FK_Guides_GuideCategories");
                entity.HasOne(d => d.GuideTechnicalCategory)
                    .WithMany(p => p.Guides)
                    .HasForeignKey(d => d.GuideTechnicalCategoryId);
            });

            modelBuilder.Entity<Points>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<ProgramAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Programs)
                    .WithMany(p => p.ProgramAttachments)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProgramAttachments_Programs");
            });

            modelBuilder.Entity<Programs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(10);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Resource)
                   .IsRequired()
                   .HasMaxLength(256);

            });

            modelBuilder.Entity<UserAchievements>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Achievement)
                    .WithMany(p => p.UserAchievements)
                    .HasForeignKey(d => d.AchievementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAchievements_Achievements");
            });

            modelBuilder.Entity<UserDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<SalesTalkAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SalesTalks)
                    .WithMany(p => p.SalesTalkAttachments)
                    .HasForeignKey(d => d.SalesTalkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesTalkAttachments_SalesTalks");
            });

            modelBuilder.Entity<SalesTalks>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);


            });

            modelBuilder.Entity<ServiceTalkFlyerAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.ServiceTalkFlyers)
                    .WithMany(p => p.ServiceTalkFlyerAttachments)
                    .HasForeignKey(d => d.ServiceTalkFlyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceTalkFlyerAttachments_ServiceTalkFlyers");
            });

            modelBuilder.Entity<ServiceTalkFlyers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);


            });

            modelBuilder.Entity<ProductCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.IconUrl)
                  .HasMaxLength(256);
            });

            modelBuilder.Entity<ProductCatalogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.ProductCatalogs)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductCatalogs_ProductCategories");
            });

            modelBuilder.Entity<ProductCatalogAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.ProductCatalog)
                    .WithMany(p => p.ProductCatalogAttachments)
                    .HasForeignKey(d => d.ProductCatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductCatalogAttachments_ProductCatalogs");
            });


            modelBuilder.Entity<ProductAccesories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);


                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.HasOne(d => d.ProductCatalogs)
                    .WithMany(p => p.ProductAccesories)
                    .HasForeignKey(d => d.ProductCatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductAccesories_ProductCatalogs");
            });

            modelBuilder.Entity<ProductColorVariants>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);


                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.HasOne(d => d.ProductCatalogs)
                    .WithMany(p => p.ProductColorVariants)
                    .HasForeignKey(d => d.ProductCatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductColorVariants_ProductCatalogs");
            });

            modelBuilder.Entity<ProductPrices>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);


                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);


                entity.HasOne(d => d.ProductColorVariants)
                    .WithMany(p => p.ProductPrices)
                    .HasForeignKey(d => d.ProductColorVariantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductPrices_ProductColorVariants");

                //entity.HasOne(d => d.Dealers)
                //    .WithMany(p => p.ProductPrices)
                //    .HasForeignKey(d => d.KodeDealerMPM)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_ProductPrices_Dealers");
            });

            modelBuilder.Entity<ProductFeatures>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);


                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.HasOne(d => d.ProductCatalogs)
                    .WithMany(p => p.ProductFeatures)
                    .HasForeignKey(d => d.ProductCatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductFeatures_ProductCatalogs");
            });

            modelBuilder.Entity<SparepartCatalogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

            });

            modelBuilder.Entity<AccesoriesCatalogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

            });

            modelBuilder.Entity<ApparelCatalogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.HasOne(d => d.ApparelCategory)
                    .WithMany(p => p.ApparelCatalogs)
                    .HasForeignKey(d => d.ApparelCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApparelCatalogs_ApparelCategories");

            });

            modelBuilder.Entity<ApparelCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.IconUrl)
                  .HasMaxLength(256);
            });

            modelBuilder.Entity<Dealers>(entity =>
            {
                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Channel)
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Jabatans>(entity =>
            {
                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Channel)
                    .HasMaxLength(4);
            });

            modelBuilder.Entity<InternalUsers>(entity =>
            {
                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);


                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Nama).HasMaxLength(256);

                //entity.HasOne(d => d.Dealers)
                //    .WithMany(p => p.InternalUsers)
                //    .HasForeignKey(d => d.KodeDealerMPM)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Dealers_InternalUsers");

                //entity.HasOne(d => d.Jabatans)
                //    .WithMany(p => p.InternalUsers)
                //    .HasForeignKey(d => d.IDJabatan)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Jabatans_InternalUsers");
            });

            modelBuilder.Entity<ExternalUsers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(4);
            });

            modelBuilder.Entity<AdminUsers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(4);
            });

            modelBuilder.Entity<Channels>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasMaxLength(4);
            });

            modelBuilder.Entity<BrandCampaignAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.BrandCampaign)
                    .WithMany(p => p.BrandCampaignAttachments)
                    .HasForeignKey(d => d.BrandCampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BrandCampaignAttachments_BrandCampaigns");
            });

            modelBuilder.Entity<BrandCampaigns>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

            });

            // Fase 2

            modelBuilder.Entity<Kotas>(entity =>
            {
                entity.HasKey(e => e.CountyId);

                entity.Property(e => e.CountyId)
                    .HasColumnName("CountyID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.NamaKota)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<InfoMainDealerAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.InfoMainDealer)
                    .WithMany(p => p.InfoMainDealerAttachments)
                    .HasForeignKey(d => d.InfoMainDealerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InfoMainDealerAttachments_InfoMainDealers");
            });

            modelBuilder.Entity<InfoMainDealers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

            });

            modelBuilder.Entity<CSChampionClubParticipants>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);


                entity.HasOne(d => d.InternalUser)
                    .WithMany(p => p.CSChampionClubParticipants)
                    .HasForeignKey(d => d.IDMPM)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CSChampionClubParticipants_InternalUsers");
            });

            modelBuilder.Entity<CSChampionClubAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.CSChampionClub)
                    .WithMany(p => p.CSChampionClubAttachments)
                    .HasForeignKey(d => d.CSChampionClubId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CSChampionClubAttachments_CSChampionClubs");
            });

            modelBuilder.Entity<CSChampionClubs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl);
                //.HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

            });

            modelBuilder.Entity<ClubCommunityCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ClubCommunities>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);


                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);


                entity.HasOne(d => d.ClubCommunityCategories)
                    .WithMany(p => p.ClubCommunities)
                    .HasForeignKey(d => d.ClubCommunityCategoryId)
                    .HasConstraintName("FK_ClubCommunities_ClubCommunityCategories");
            });

            modelBuilder.Entity<ForumPosts>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);


                entity.HasOne(d => d.ForumThreads)
                    .WithMany(p => p.ForumPosts)
                    .HasForeignKey(d => d.ForumThreadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ForumPosts_ForumThreads");
            });

            modelBuilder.Entity<ForumThreads>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);


                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                //entity.HasOne(d => d.Dealers)
                //    .WithMany(p => p.ForumThreads)
                //    .HasForeignKey(d => d.KodeDealerMPM)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_ForumThreads_Dealers");
            });

            modelBuilder.Entity<InboxRecipients>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);


                entity.HasOne(d => d.InboxMessages)
                    .WithMany(p => p.InboxRecipients)
                    .HasForeignKey(d => d.InboxMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InboxRecipients_InboxMessages");

                entity.HasOne(d => d.InternalUser)
                    .WithMany(p => p.InboxRecipients)
                    .HasForeignKey(d => d.IDMPM)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InboxRecipients_InternalUsers");
            });

            modelBuilder.Entity<InboxAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.InboxMessages)
                    .WithMany(p => p.InboxAttachments)
                    .HasForeignKey(d => d.InboxMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InboxAttachments_InboxMessages");
            });

            modelBuilder.Entity<InboxMessages>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

            });

            // Fase 1 Revisi

            modelBuilder.Entity<SalesProgramAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SalesPrograms)
                    .WithMany(p => p.SalesProgramAttachments)
                    .HasForeignKey(d => d.SalesProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesProgramAttachments_SalesPrograms");
            });

            modelBuilder.Entity<SalesPrograms>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(10);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

            });

            modelBuilder.Entity<ServiceProgramAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.ServicePrograms)
                    .WithMany(p => p.ServiceProgramAttachments)
                    .HasForeignKey(d => d.ServiceProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceProgramAttachments_ServicePrograms");
            });

            modelBuilder.Entity<ServicePrograms>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(10);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

            });

            // Fase 3

            modelBuilder.Entity<HomeworkQuizAnswers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Question).IsRequired();

                entity.HasOne(d => d.HomeworkQuizHistory)
                    .WithMany(p => p.HomeworkQuizAnswers)
                    .HasForeignKey(d => d.HomeworkQuizHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HomeworkQuizAnwers_HomeworkQuizHistories");
            });

            modelBuilder.Entity<HomeworkQuizAssignments>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IDMPM);

                entity.HasOne(d => d.HomeworkQuiz)
                    .WithMany(p => p.HomeworkQuizAssignments)
                    .HasForeignKey(d => d.HomeworkQuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HomeworkQuizAssignments_HomeworkQuizzes");

                //entity.HasOne(d => d.InternalUser)
                //    .WithMany(p => p.HomeworkQuizAssignments)
                //    .HasForeignKey(d => d.IDMPM)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_HomeworkQuizAssignments_InternalUsers");
            });

            modelBuilder.Entity<HomeworkQuizHistories>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Dealer).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.IDMPM);

                entity.Property(e => e.Jabatan).HasMaxLength(256);

                entity.Property(e => e.Kota).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.HomeworkQuiz)
                    .WithMany(p => p.HomeworkQuizHistories)
                    .HasForeignKey(d => d.HomeworkQuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HomeworkQuizHistories_HomeworkQuizzes");
            });

            modelBuilder.Entity<HomeworkQuizQuestions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CorrectOption)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.ImageUrl).IsRequired();

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.OptionA).IsRequired();

                entity.Property(e => e.OptionB).IsRequired();

                entity.Property(e => e.OptionC).IsRequired();

                entity.Property(e => e.OptionD).IsRequired();

                entity.Property(e => e.OptionE).IsRequired();

                entity.Property(e => e.Question).IsRequired();

                entity.HasOne(d => d.HomeworkQuiz)
                    .WithMany(p => p.HomeworkQuizQuestions)
                    .HasForeignKey(d => d.HomeworkQuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HomeworkQuizQuestions_HomeworkQuizzes");
            });

            modelBuilder.Entity<HomeworkQuizzes>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<VerifikasiJabatanQuestions>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CorrectOption)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.ImageUrl).IsRequired();

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.OptionA).IsRequired();

                entity.Property(e => e.OptionB).IsRequired();

                entity.Property(e => e.OptionC).IsRequired();

                entity.Property(e => e.OptionD).IsRequired();

                entity.Property(e => e.OptionE).IsRequired();

                entity.Property(e => e.Question).IsRequired();

                entity.HasOne(d => d.VerfikasiJabatan)
                    .WithMany(p => p.VerifikasiJabatanQuestions)
                    .HasForeignKey(d => d.VerfikasiJabatanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VerifikasiJabatanQuestions_VerifikasiJabatans");
            });

            modelBuilder.Entity<VerifikasiJabatans>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.IDGroupJabatan).IsRequired();

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.PassingScore).HasColumnType("numeric(5, 2)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                //entity.HasOne(d => d.Jabatans)
                //    .WithMany(p => p.VerifikasiJabatans)
                //    .HasForeignKey(d => d.IDJabatan)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_VerifikasiJabatans_Jabatans");
            });

            modelBuilder.Entity<VerifikasiJabatanHistories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Channel).HasMaxLength(4);

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DealerKota).HasMaxLength(256);

                entity.Property(e => e.DealerName).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.Handphone).HasMaxLength(256);

                entity.Property(e => e.IDGroupJabatan)
                    .HasMaxLength(256);

                entity.Property(e => e.IDGroupJabatanAtasan)
                    .HasMaxLength(256);

                entity.Property(e => e.IDHonda);

                entity.Property(e => e.IDJabatan);

                entity.Property(e => e.IDMPM);

                entity.Property(e => e.Jabatan).HasMaxLength(256);

                entity.Property(e => e.JabatanAtasan).HasMaxLength(256);

                entity.Property(e => e.KodeDealerAHM)
                    .HasMaxLength(10);

                entity.Property(e => e.KodeDealerMPM)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.MPMKodeAtasan);

                entity.Property(e => e.Nama).HasMaxLength(256);

                entity.Property(e => e.NamaAtasan).HasMaxLength(256);

                entity.Property(e => e.NoKTP)
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<RolePlayDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired();

                entity.HasOne(d => d.RolePlay)
                    .WithMany(p => p.RolePlayDetails)
                    .HasForeignKey(d => d.RolePlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePlayDetails_RolePlays");
            });

            modelBuilder.Entity<RolePlayResultDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired();

                entity.HasOne(d => d.RolePlayResult)
                    .WithMany(p => p.RolePlayResultDetails)
                    .HasForeignKey(d => d.RolePlayResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePlayResultDetails_RolePlayResults");
            });

            modelBuilder.Entity<RolePlayResults>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FLPResult)
                    .HasColumnType("numeric(5, 2)");

                entity.Property(e => e.KodeDealerMPM)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.NamaDealerMPM)
                    .HasMaxLength(256);

                entity.Property(e => e.NamaFLP)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StorageUrl);

                entity.Property(e => e.VerificationResult).HasColumnType("numeric(5, 2)");

                entity.HasOne(d => d.RolePlay)
                    .WithMany(p => p.RolePlayResults)
                    .HasForeignKey(d => d.RolePlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePlayResults_RolePlayResults");
            });

            modelBuilder.Entity<RolePlays>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<RolePlayAssignments>(entity =>
            {
                entity.Property(e => e.Id);

                entity.Property(e => e.KodeDealerMPM);

                entity.HasOne(d => d.RolePlay)
                    .WithMany(p => p.RolePlayAssignments)
                    .HasForeignKey(d => d.RolePlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePlayAssignments_RolePlays");
            });

            modelBuilder.Entity<SelfRecordingDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired();

                entity.HasOne(d => d.SelfRecording)
                    .WithMany(p => p.SelfRecordingDetails)
                    .HasForeignKey(d => d.SelfRecordingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SelfRecordingDetails_SelfRecordingDetails");
            });

            modelBuilder.Entity<SelfRecordingResultDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired();

                entity.HasOne(d => d.SelfRecordingResult)
                    .WithMany(p => p.SelfRecordingResultDetails)
                    .HasForeignKey(d => d.SelfRecordingResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SelfRecordingResultDetails_SelfRecordingResultDetails");
            });

            modelBuilder.Entity<SelfRecordingResults>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FLPResult)
                    .HasColumnType("numeric(5, 2)");

                entity.Property(e => e.KodeDealerMPM)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.NamaDealerMPM)
                    .HasMaxLength(256);

                entity.Property(e => e.NamaFLP)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.StorageUrl);

                entity.Property(e => e.VerificationResult).HasColumnType("numeric(5, 2)");

                entity.HasOne(d => d.SelfRecording)
                    .WithMany(p => p.SelfRecordingResults)
                    .HasForeignKey(d => d.SelfRecordingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SelfRecordingResults_SelfRecordingResults");
            });

            modelBuilder.Entity<SelfRecordings>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SelfRecordingAssignments>(entity =>
            {
                entity.Property(e => e.Id);

                entity.Property(e => e.KodeDealerMPM);

                entity.HasOne(d => d.SelfRecording)
                    .WithMany(p => p.SelfRecordingAssignments)
                    .HasForeignKey(d => d.SelfRecordingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SelfRecordingAssignments_SelfRecordings");
            });

            modelBuilder.Entity<EventAssignments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.DeletionTime).HasColumnType("datetime2(2)");

                entity.Property(e => e.IDMPM);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventAssignments)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAssignments_Events");

                entity.HasOne(d => d.InternalUsers)
                    .WithMany(p => p.EventAssignments)
                    .HasForeignKey(d => d.IDMPM)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventAssignments_InternalUsers");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Budget).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Contents).IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Location).IsRequired();

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SalesIncentiveProgramAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");


                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Order).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SalesIncentiveProgram)
                    .WithMany(p => p.SalesIncentiveProgramAttachments)
                    .HasForeignKey(d => d.SalesIncentiveProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesIncentiveProgramAttachments_SalesIncentivePrograms");
            });

            modelBuilder.Entity<SalesIncentiveProgramJabatans>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");


                entity.Property(e => e.NamaJabatan)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SalesIncentiveProgram)
                    .WithMany(p => p.SalesIncentiveProgramJabatans)
                    .HasForeignKey(d => d.SalesIncentiveProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesIncentiveProgramJabatans_SalesIncentivePrograms");
            });

            modelBuilder.Entity<SalesIncentiveProgramKotas>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");


                entity.Property(e => e.NamaKota)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SalesIncentiveProgram)
                    .WithMany(p => p.SalesIncentiveProgramKotas)
                    .HasForeignKey(d => d.SalesIncentiveProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesIncentiveProgramKotas_SalesIncentiveProgramKotas");
            });

            modelBuilder.Entity<SalesIncentivePrograms>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.Incentive).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                //entity.Property(e => e.TipeMotor)
                //    .IsRequired()
                //    .HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SPDCMasterPoints>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Weight).HasColumnType("decimal(3, 2)");
            });

            modelBuilder.Entity<SPDCPointHistories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.IDMPM);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.SPDCMasterPointId);

                entity.HasOne(d => d.InternalUsers)
                    .WithMany(p => p.SPDCPointHistories)
                    .HasForeignKey(d => d.IDMPM)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SPDCPointHistories_SPDCPointHistories");

                entity.HasOne(d => d.SPDCMasterPoints)
                    .WithMany(p => p.SPDCPointHistories)
                    .HasForeignKey(d => d.SPDCMasterPointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SPDCPointHistories_SPDCMasterDatas");
            });

            modelBuilder.Entity<Kecamatans>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");


                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.NamaKecamatan)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.HasOne(d => d.Kotas)
                    .WithMany(p => p.Kecamatans)
                    .HasForeignKey(d => d.CountyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kecamatans_Kotas");
            });

            // Fase 4

            modelBuilder.Entity<ClaimProgramClaimers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.HasOne(d => d.ClaimPrograms)
                    .WithMany(p => p.ClaimProgramClaimers)
                    .HasForeignKey(d => d.ClaimProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClaimProgramClaimers_ClaimPrograms");
            });

            modelBuilder.Entity<ClaimProgramAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.StorageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.ClaimPrograms)
                    .WithMany(p => p.ClaimProgramAttachments)
                    .HasForeignKey(d => d.ClaimProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClaimProgramAttachments_ClaimPrograms");
            });

            modelBuilder.Entity<ClaimPrograms>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Contents);//.IsRequired();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.FeaturedImageUrl)
                    .HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<AgendaAssignments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.DeletionTime).HasColumnType("datetime2(2)");

                entity.Property(e => e.IDMPM);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.Agenda)
                    .WithMany(p => p.AgendaAssignments)
                    .HasForeignKey(d => d.AgendaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AgendaAssignments_Agenda");

                entity.HasOne(d => d.InternalUsers)
                    .WithMany(p => p.AgendaAssignments)
                    .HasForeignKey(d => d.IDMPM)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AgendaAssignments_InternalUsers");
            });

            modelBuilder.Entity<Agendas>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");


                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);


                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

            });

            // Fase 4.3
            modelBuilder.Entity<Points>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<PointConfigurations>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<ActivityLogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            });

            // Fese 4.4

            modelBuilder.Entity<LiveQuizAnswers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Question).IsRequired();

                entity.HasOne(d => d.LiveQuizHistory)
                    .WithMany(p => p.LiveQuizAnswers)
                    .HasForeignKey(d => d.LiveQuizHistoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LiveQuizAnwers_LiveQuizHistories");
            });

            modelBuilder.Entity<LiveQuizAssignments>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IDMPM);

                entity.HasOne(d => d.LiveQuiz)
                    .WithMany(p => p.LiveQuizAssignments)
                    .HasForeignKey(d => d.LiveQuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LiveQuizAssignments_LiveQuizzes");

                //entity.HasOne(d => d.InternalUser)
                //    .WithMany(p => p.HomeworkQuizAssignments)
                //    .HasForeignKey(d => d.IDMPM)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_HomeworkQuizAssignments_InternalUsers");
            });

            modelBuilder.Entity<LiveQuizHistories>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Dealer).HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.IDMPM);

                entity.Property(e => e.Jabatan).HasMaxLength(256);

                entity.Property(e => e.Kota).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.LiveQuiz)
                    .WithMany(p => p.LiveQuizHistories)
                    .HasForeignKey(d => d.LiveQuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LiveQuizHistories_LiveQuizzes");
            });

            modelBuilder.Entity<LiveQuizQuestions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CorrectOption)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.ImageUrl).IsRequired();

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.OptionA).IsRequired();

                entity.Property(e => e.OptionB).IsRequired();

                entity.Property(e => e.OptionC).IsRequired();

                entity.Property(e => e.OptionD).IsRequired();

                entity.Property(e => e.OptionE).IsRequired();

                entity.Property(e => e.Question).IsRequired();

                entity.HasOne(d => d.LiveQuiz)
                    .WithMany(p => p.LiveQuizQuestions)
                    .HasForeignKey(d => d.LiveQuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LiveQuizQuestions_LiveQuizzes");
            });

            modelBuilder.Entity<LiveQuizzes>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SDMSMessageWeb>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SDMSMessageWebDetail>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SDMSMessageWeb)
                 .WithMany(p => p.SDMSMessageDetail)
                 .HasForeignKey(d => d.MessageId)
                 .OnDelete(DeleteBehavior.ClientSetNull)
                 .HasConstraintName("FK_SDMDSMessageWEBDetail_SDMSMessageWEB");
            });


            modelBuilder.Entity<SDMSMessage>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SDMSLogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<SDMSMessageDetail>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.SDMSMessage)
                 .WithMany(p => p.SDMSMessageDetail)
                 .HasForeignKey(d => d.MessageId)
                 .OnDelete(DeleteBehavior.ClientSetNull)
                 .HasConstraintName("FK_SDMDSMessageDetail_SDMSMessage");
            });

            #region Content Bank
            modelBuilder.Entity<ContentBankCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<ContentBanks>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ContentBankCategories)
                      .WithMany(p => p.ContentBanks)
                      .HasForeignKey(d => d.GUIDContentBankCategory)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ContentBankDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ContentBanks)
                      .WithMany(p => p.ContentBankDetails)
                      .HasForeignKey(d => d.GUIDContentBank)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ContentBankAssignees>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ContentBankDetails)
                      .WithMany(p => p.ContentBankAssignees)
                      .HasForeignKey(d => d.GUIDContentBankDetail)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ContentBankAssigneeProofs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ContentBankAssignee)
                      .WithMany(p => p.ContentBankAssigneeProofs)
                      .HasForeignKey(d => d.GUIDContentBankAssignee)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            #endregion

            #region Splashscreen
            modelBuilder.Entity<SplashScreen>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<SplashScreenDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.SplashScreen)
                      .WithMany(p => p.SplashScreenDetails)
                      .HasForeignKey(d => d.GUIDSplashScreen)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            #endregion

            #region Log Activity
            modelBuilder.Entity<LogActivities>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<LogActivityDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(e => e.LogActivities)
                    .WithOne(e => e.LogActivityDetails)
                    .HasForeignKey<LogActivityDetails>(e => e.LogActivityGUID);
            });
            #endregion

            #region Menu Setting
            modelBuilder.Entity<ApplicationFeature>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<ApplicationFeatureMapping>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ApplicationFeature)
                      .WithMany(p => p.ApplicationFeatureMapping)
                      .HasForeignKey(d => d.GUIDFeature)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            #endregion

            #region TBSM
            modelBuilder.Entity<Sekolahs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<TBSMUserGurus>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.Sekolah)
                      .WithMany(p => p.TBSMUserGurus)
                      .HasForeignKey(d => d.GUIDSekolah)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TBSMUserSiswas>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.Sekolah)
                      .WithMany(p => p.TBSMUserSiswas)
                      .HasForeignKey(d => d.GUIDSekolah)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            #endregion

            #region Claim Program
            modelBuilder.Entity<ClaimProgramCampaigns>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<ClaimProgramCampaignPrizes>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ClaimProgramCampaigns)
                      .WithMany(p => p.ClaimProgramCampaignPrizes)
                      .HasForeignKey(d => d.GUIDClaimProgramCampaign)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ClaimProgramCampaignPoints>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ClaimProgramCampaigns)
                      .WithMany(p => p.ClaimProgramCampaignPoints)
                      .HasForeignKey(d => d.GUIDClaimProgramCampaign)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ClaimProgramContents>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<ClaimProgramAssignees>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ClaimProgramContents)
                      .WithMany(p => p.ClaimProgramAssignees)
                      .HasForeignKey(d => d.GUIDClaimProgramContent)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ClaimProgramContentAttachments>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ClaimProgramContents)
                      .WithMany(p => p.ClaimProgramContentAttachments)
                      .HasForeignKey(d => d.GUIDClaimProgramContent)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ClaimProgramContentClaimers>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                //entity.HasOne(d => d.ClaimProgramCampaignPrizes)
                //      .WithMany(p => p.ClaimProgramContentClaimers)
                //      .HasForeignKey(d => d.GUIDClaimProgramPrize)
                //      .OnDelete(DeleteBehavior.ClientSetNull);

                //entity.HasOne(d => d.ClaimProgramCampaignPoints)
                //      .WithMany(p => p.ClaimProgramContentClaimers)
                //      .HasForeignKey(d => d.GUIDClaimProgramPoint)
                //      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ClaimProgramContents)
                      .WithMany(p => p.ClaimProgramContentClaimers)
                      .HasForeignKey(d => d.GUIDClaimProgramContent)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ClaimProgramCampaignPrizeLogs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ClaimProgramCampaignPrizes)
                      .WithMany(p => p.ClaimProgramCampaignPrizeLogs)
                      .HasForeignKey(d => d.GUIDClaimProgramCampaignPrizes)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ClaimProgramCampaignPoints)
                      .WithMany(p => p.ClaimProgramCampaignPrizeLogs)
                      .HasForeignKey(d => d.GUIDClaimProgramCampaignPoints)
                      .OnDelete(DeleteBehavior.ClientSetNull);

            });
            #endregion

            #region Sales Monitoring
            modelBuilder.Entity<ProductTypes>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<ProductSeries>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ProductTypes)
                      .WithMany(p => p.ProductSeries)
                      .HasForeignKey(d => d.GUIDProductType)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Fincoy>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<TargetSales>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<Sales>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                entity.HasOne(d => d.ProductTypes)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(d => d.GUIDProductType)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProductSeries)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(d => d.GUIDProductSeries)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Fincoy)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(d => d.GUIDFincoy)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
            #endregion

            #region Sales Incentive
            modelBuilder.Entity<SalesIncentiveProgramAssignee>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);

                //entity.HasOne(d => d.SalesIncentivePrograms)
                //      .WithMany(p => p.SalesIncentiveProgramAssignee)
                //      .HasForeignKey(d => d.GUIDIncentiveProgram)
                //      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SalesIncentiveProgramTarget>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });
            modelBuilder.Entity<SalesIncentiveProgramTarget>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            #endregion

            #region BAST
            modelBuilder.Entity<BASTCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<BASTs>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<BASTDetails>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<BASTAttachment>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<BASTAssignee>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<BASTReport>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<BASTReportAttachment>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });
            #endregion

            #region Mechanical Assistants
            modelBuilder.Entity<MechanicAssistants>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<MechanicAssistantCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<MechanicAssistantAssignees>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });

            modelBuilder.Entity<MechanicAssistantContacts>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });
            #endregion

            modelBuilder.Entity<InboxMessageCategories>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatorUsername)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.DeleterUsername).HasMaxLength(256);

                entity.Property(e => e.LastModifierUsername).HasMaxLength(256);
            });
        }
    }
}
