using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class SalesIncentiveProgramGetIdDto
    {
        public string Kota { get; set; }
        public string Jabatan { get; set; }
    }

    [AutoMapTo(typeof(SalesIncentivePrograms))]
    public class SalesIncentiveProgramsCreateDto
    {
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid GUIDProductType { get; set; }
        public bool IsPublished { get; set; }
        public string FeaturedImageUrl { get; set; }
        public decimal Incentive { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }
        public int? TipePembayaran { get; set; }
        //public List<SalesIncentiveProgramAttachmentsDto> attachments { get; set; }
        //public List<SalesIncentiveProgramJabatansDto> jabatans { get; set; }
        //public List<SalesIncentiveProgramKotasDto> kotas { get; set; }
        //public List<SalesIncentiveProgramAssigneeDto> assignee { get; set; }
    }

    [AutoMapTo(typeof(SalesIncentiveProgramAttachments))]
    public class SalesIncentiveProgramAttachmentsDto
    {
        public string Title { get; set; }
        public string StorageUrl { get; set; }
        //public Guid SalesIncentiveProgramId { get; set; }
        public string Order { get; set; }
        public string FileName { get; set; }
    }

    [AutoMapTo(typeof(SalesIncentiveProgramAssignee))]
    public class SalesIncentiveProgramAssigneeDto
    {
        //public Guid GUIDIncentiveProgram { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Kota { get; set; }
        public string Jabatan { get; set; }
    }

    [AutoMapTo(typeof(SalesIncentiveProgramKotas))]
    public class SalesIncentiveProgramKotasDto
    {
        //public Guid SalesIncentiveProgramId { get; set; }
        public string NamaKota { get; set; }
    }

    [AutoMapTo(typeof(SalesIncentiveProgramJabatans))]
    public class SalesIncentiveProgramJabatansDto
    {
        //public Guid SalesIncentiveProgramId { get; set; }
        public string NamaJabatan { get; set; }
    }

    [AutoMapTo(typeof(SalesIncentiveProgramTarget))]
    public class SalesIncentiveProgramTargetCreateDto
    {
        public Guid SalesIncentiveProgramId { get; set; }
        public string Kota { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Karesidenan { get; set; }
        public string EnumTipeTransaksi { get; set; }
        public int Target { get; set; }
        public int Transaksi { get; set; }
        public int Capaian { get; set; }
    }
    public class SalesIncentiveProgramTargetUpdateDto
    {
        public Guid Id { get; set; }
        public Guid SalesIncentiveProgramId { get; set; }
        public string Kota { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Karesidenan { get; set; }
        public string EnumTipeTransaksi { get; set; }
        public int Target { get; set; }
        public int Transaksi { get; set; }
        public int Capaian { get; set; }
    }
    public class SalesIncentiveProgramTargetDeleteDto
    {
        public Guid Id { get; set; }
    }

    public class SalesIncentiveDashboardDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProductType { get; set; }
        public int TipePembayaran { get; set; }
        public decimal Incentive { get; set; }
        public decimal PotensiAchievement { get; set; }
        public List<SalesIncentiveTargetDashboardDto> target { get; set; }
    }
    public class SalesIncentiveTargetDashboardDto
    {
        public string DealerName { get; set; }
        public string Kota { get; set; }
        public string Karesidenan { get; set; }
        public int Target { get; set; }
        public int Transaksi { get; set; }
        public decimal Persentase { get; set; }
    }
}
