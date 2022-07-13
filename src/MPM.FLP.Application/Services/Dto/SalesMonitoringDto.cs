using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(ProductTypes))]
    public class ProductTypesCreateDto
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }

    public class ProductTypesUpdateDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
    }
    public class ProductTypesDeleteDto
    {
        public Guid Id { get; set; }
    }

    [AutoMapTo(typeof(ProductSeries))]
    public class ProductSeriesCreateDto
    {
        public Guid GUIDProductType { get; set; }
        public string SeriesName { get; set; }
        public string SeriesCode { get; set; }
    }

    public class ProductSeriesUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDProductType { get; set; }
        public string SeriesName { get; set; }
        public string SeriesCode { get; set; }
    }
    public class ProductSeriesDeleteDto
    {
        public Guid Id { get; set; }
    }

    [AutoMapTo(typeof(Fincoy))]
    public class FincoyCreateDto
    {
        public string FincoyName { get; set; }
    }

    public class FincoyUpdateDto
    {
        public Guid Id { get; set; }
        public string FincoyName { get; set; }
    }

    [AutoMapTo(typeof(TargetSales))]
    public class TargetSalesCreateDto
    {
        public string DealerId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? Periode1Start { get; set; }
        public DateTime? Periode1End { get; set; }
        public int Periode1Target { get; set; }
        public DateTime? Periode2Start { get; set; }
        public DateTime? Periode2End { get; set; }
        public int Periode2Target { get; set; }
        public DateTime? Periode3Start { get; set; }
        public DateTime? Periode3End { get; set; }
        public int Periode3Target { get; set; }
        public DateTime? Periode4Start { get; set; }
        public DateTime? Periode4End { get; set; }
        public int Periode4Target { get; set; }
        public DateTime? Periode5Start { get; set; }
        public DateTime? Periode5End { get; set; }
        public int Periode5Target { get; set; }
        public DateTime? Periode6Start { get; set; }
        public DateTime? Periode6End { get; set; }
        public int Periode6Target { get; set; }
        public int TargetTotal { get; set; }
        public int Achievement { get; set; }
    }

    public class TargetSalesUpdateDto
    {
        public Guid Id { get; set; }
        public string DealerId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? Periode1Start { get; set; }
        public DateTime? Periode1End { get; set; }
        public int Periode1Target { get; set; }
        public DateTime? Periode2Start { get; set; }
        public DateTime? Periode2End { get; set; }
        public int Periode2Target { get; set; }
        public DateTime? Periode3Start { get; set; }
        public DateTime? Periode3End { get; set; }
        public int Periode3Target { get; set; }
        public DateTime? Periode4Start { get; set; }
        public DateTime? Periode4End { get; set; }
        public int Periode4Target { get; set; }
        public DateTime? Periode5Start { get; set; }
        public DateTime? Periode5End { get; set; }
        public int Periode5Target { get; set; }
        public DateTime? Periode6Start { get; set; }
        public DateTime? Periode6End { get; set; }
        public int Periode6Target { get; set; }
        public int TargetTotal { get; set; }
        public int Achievement { get; set; }
    }
    public class MasterUnitDto
    {
        public string ITEMGROUPNAME { get; set; }
        public string SERIES_UNIT { get; set; }
        public string SUBSERIES_UNIT { get; set; }
        public string kodetypeunitmpm { get; set; }
        public string KODETYPEUNITAHM { get; set; }
        public string namaunit { get; set; }
        public string namapasarunit { get; set; }
    }
    public class MasterUnitResponseDto
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<MasterUnitDto> data { get; set; }
    }
    public class SalesParamDto
    {
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public string kodeDealer { get; set; }
    }
    public class SalesUnitDto
    {
        public string kddealer { get; set; }
        public string ENGINENO { get; set; }
        public string KODETYPE { get; set; }
        public string KODEWARNA { get; set; }
        public DateTime TGLISINAMA { get; set; }
        public string KOTA { get; set; }
        public string idflp { get; set; }
        public string namaflp { get; set; }
        public string IDGROUPJABATAN { get; set; }
        public string jabatanflp { get; set; }
        public string jnspembayaran { get; set; }
    }
    public class SalesUnitResponseDto
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<SalesUnitDto> data { get; set; }
    }

    public class DealerInfoResponseDto
    {
        public string Kota { get; set; }
        public string Kode { get; set; }
        public string Nama { get; set; }
    }

    public class DealerRankDto
    {
        public string DealerId { get; set; }
        public int Bulan { get; set; }
        public int Tahun { get; set; }
        public int Total { get; set; }
        public int Rank { get; set; }
    }

    public class DealerRankInfoResponseDto
    {
        public string Kota { get; set; }
        public string Kode { get; set; }
        public string Nama { get; set; }
        public int Rank { get; set; }
    }

    public class MonitorSalesDealerFilterDto
    {
        public DateTime CurrentDate { get; set; }
        public string DealerId { get; set; }
        public string PaymentType { get; set; }
        public string UnitType { get; set; }
        public string Series { get; set; }
        public string FinanceCompany { get; set; }
    }

    public class WeeklyInsightResponseDto
    {
        public int Orders { get; set; }
        public DateTime PeriodeStart { get; set; }
        public DateTime PeriodeEnd { get; set; }
        public int CurrentSales { get; set; }
        public int PreviousSales { get; set; }
        public decimal Growth { get; set; }
    }

    public class MonitorSalesKotaFilterDto
    {
        public DateTime CurrentDate { get; set; }
        public string DealerId { get; set; }
        public string PaymentType { get; set; }
        public string UnitType { get; set; }
        public string Series { get; set; }
    }

    public class DailyInsightResponseDto
    {
        public int Orders { get; set; }
        public DateTime Date { get; set; }
        public int CurrentSales { get; set; }
        public int PreviousSales { get; set; }
        public decimal Growth { get; set; }
    }

    public class MonthlySalesGrowthResponseDto
    {
        public string CurrentMonth { get; set; }
        public string PreviousMonth { get; set; }
        public int CurrentSales { get; set; }
        public int PreviousSales { get; set; }
        public decimal Growth { get; set; }
    }

    public class DailySalesGrowthResponseDto
    {
        public DateTime CurrentDate { get; set; }
        public DateTime PreviousDate { get; set; }
        public int CurrentSales { get; set; }
        public int PreviousSales { get; set; }
        public decimal Growth { get; set; }
    }

    public class AchievementGrowthDealerResponseDto
    {
        public int Orders { get; set; }
        public string Periode { get; set; }
        public int Target { get; set; }
        public int Sales { get; set; }
        public decimal Achievement { get; set; }
    }

    public class AchievementGrowthKotaResponseDto
    {
        public int Orders { get; set; }
        public string Periode { get; set; }
        public decimal Tunai { get; set; }
        public decimal Kredit { get; set; }
        public decimal Total { get; set; }
    }
}
