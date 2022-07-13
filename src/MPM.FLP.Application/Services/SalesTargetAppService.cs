using Abp.Authorization;
using Abp.Domain.Repositories;
using Castle.Windsor.Installer;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SalesTargetAppService : FLPAppServiceBase, ISalesTargetAppService
    {
        private readonly IRepository<TargetSales, Guid> _repositoryTargetSales;
        private readonly IRepository<Sales, Guid> _repositorySales;
        private readonly IRepository<ProductTypes, Guid> _repositoryProductTypes;
        private readonly IRepository<Dealers, string> _repositoryDealer;

        public SalesTargetAppService(
            IRepository<TargetSales, Guid> repositoryTargetSales,
            IRepository<Sales, Guid> repositorySales,
            IRepository<ProductTypes, Guid> repositoryProductTypes,
            IRepository<Dealers, string> repositoryDealer)
        {
            _repositoryTargetSales = repositoryTargetSales;
            _repositorySales = repositorySales;
            _repositoryProductTypes = repositoryProductTypes;
            _repositoryDealer = repositoryDealer;
        }

        public void Create(TargetSales input)
        {
            _repositoryTargetSales.Insert(input);
        }

        public void CreateMultiple(List<TargetSales> input)
        {
            foreach(var targetSales in input) _repositoryTargetSales.Insert(targetSales);
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryTargetSales.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.DealerId.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        public TargetSales GetById(Guid Id)
        {
            var data = _repositoryTargetSales.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }
        public void CreateSingle(TargetSalesCreateDto input)
        {
            #region Create Target Sales
            var target = ObjectMapper.Map<TargetSales>(input);
            target.CreationTime = DateTime.Now;
            target.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositoryTargetSales.Insert(target);
            #endregion
        }

        public void Update(TargetSalesUpdateDto input)
        {
            #region Update Target Sales
            var target = _repositoryTargetSales.Get(input.Id);
            target.DealerId = input.DealerId;
            target.Month = input.Month;
            target.Year = input.Year;
            target.Periode1Start = input.Periode1Start;
            target.Periode1End = input.Periode1End;
            target.Periode1Target = input.Periode1Target;
            target.Periode2Start = input.Periode2Start;
            target.Periode2End = input.Periode2End;
            target.Periode2Target = input.Periode2Target;
            target.Periode3Start = input.Periode3Start;
            target.Periode3End = input.Periode3End;
            target.Periode3Target = input.Periode3Target;
            target.Periode4Start = input.Periode4Start;
            target.Periode4End = input.Periode4End;
            target.Periode4Target = input.Periode4Target;
            target.Periode5Start = input.Periode5Start;
            target.Periode5End = input.Periode5End;
            target.Periode5Target = input.Periode5Target;
            target.Periode6Start = input.Periode6Start;
            target.Periode6End = input.Periode6End;
            target.Periode6Target = input.Periode6Target;
            target.TargetTotal = input.TargetTotal;
            target.Achievement = input.Achievement;
            target.LastModifierUsername = this.AbpSession.UserId.ToString();
            target.LastModificationTime = DateTime.Now;

            _repositoryTargetSales.Update(target);
            #endregion

        }

        public void SoftDelete(ProductSeriesDeleteDto input)
        {
            var target = _repositoryTargetSales.Get(input.Id);
            target.DeleterUsername = this.AbpSession.UserId.ToString();
            target.DeletionTime = DateTime.Now;
            _repositoryTargetSales.Update(target);
        }

        public async Task<ServiceResult> SyncSales(SalesParamDto input)
        {
            try
            {
                var url = string.Format(AppConstants.MPMPenjualanUrl, input.datefrom, input.dateto, input.kodeDealer);

                var client = new HttpClient();

                var getSalesResult = await client.GetAsync(url);
                var salesJson = await getSalesResult.Content.ReadAsStringAsync();

                SalesUnitResponseDto salesResponse = JsonConvert.DeserializeObject<SalesUnitResponseDto>(salesJson);
                //result = productResponse.data.ToList();
                if (salesResponse.status == 1)
                {
                    //Sync Insert and Update sales

                    foreach (var sales in salesResponse.data)
                    {
                        // get guid tipe
                        var tipeProduct = _repositoryProductTypes.GetAll()
                                           .Where(x => x.ProductCode == sales.KODETYPE)
                                           .Select(x => x.Id)
                                           .FirstOrDefault();
                        if (tipeProduct != null)
                        {
                            var _sales = new Sales
                            {
                                GUIDProductType = tipeProduct,
                                DealerId = sales.kddealer,
                                TransactionDate = sales.TGLISINAMA,
                                Amount = 0,
                                EnumPaymentCategory = sales.jnspembayaran,
                                CreationTime = DateTime.Now,
                                CreatorUsername = "system"
                            };
                            _repositorySales.InsertOrUpdate(_sales);
                        }
                        
                    }
                    return new ServiceResult { IsSuccess = true, Message = "Sync Success" };
                }
                else
                {
                    return new ServiceResult { IsSuccess = false, Message = salesResponse.message };
                }
            }
            catch (Exception ex)
            {

                return new ServiceResult { IsSuccess = false, Message = ex.Message };
            }
        }

        #region Grafik
        public async Task<DealerInfoResponseDto> GetDealerInfo(string DealerId)
        {
            var dealer = await _repositoryDealer.FirstOrDefaultAsync(x => x.KodeDealerMPM == DealerId);

            if (dealer == null)
                return null;

            return new DealerInfoResponseDto
            {
                Kota = dealer.Kota,
                Kode = dealer.KodeDealerMPM,
                Nama = dealer.Nama
            };
        }

        public async Task<DealerRankInfoResponseDto> GetDealerRankInfo(string DealerId)
        {
            var dealer = await _repositoryDealer.FirstOrDefaultAsync(x => x.KodeDealerMPM == DealerId);

            if (dealer == null)
                return null;

            var rank = await (from sales in _repositorySales.GetAll()
                              where sales.TransactionDate.Month == DateTime.Now.Month
                                    && sales.TransactionDate.Year == DateTime.Now.Year
                              group sales by new { sales.DealerId, sales.TransactionDate.Month, sales.TransactionDate.Year } into grpSales
                              select new DealerRankDto
                              {
                                  DealerId = grpSales.Key.DealerId,
                                  Bulan = grpSales.Key.Month,
                                  Tahun = grpSales.Key.Year,
                                  Total = grpSales.Sum(x => x.Amount)
                              }).OrderByDescending(x => x.Total).ToListAsync();

            var dealerRank = rank.Select((x, y) => new DealerRankDto
            {
                DealerId = x.DealerId,
                Bulan = x.Bulan,
                Tahun = x.Tahun,
                Total = x.Total,
                Rank = y + 1
            }).ToList();

            return new DealerRankInfoResponseDto
            {
                Kota = dealer.Kota,
                Kode = dealer.KodeDealerMPM,
                Nama = dealer.Nama,
                Rank = dealerRank.FirstOrDefault(x => x.DealerId == dealer.KodeDealerMPM).Rank
            };
        }

        public async Task<List<WeeklyInsightResponseDto>> GetWeeklyInsight(MonitorSalesDealerFilterDto request)
        {
            var curMonth = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, 1);
            var prevMonth = curMonth.AddDays(-1);
            var results = new List<WeeklyInsightResponseDto>();

            var targetCurMonth = await _repositoryTargetSales
                .FirstOrDefaultAsync(x => x.DealerId == request.DealerId
                    && x.Month == curMonth.Month
                    && x.Year == curMonth.Year);

            var targetPrevMonth = await _repositoryTargetSales
                .FirstOrDefaultAsync(x => x.DealerId == request.DealerId
                    && x.Month == prevMonth.Month
                    && x.Year == prevMonth.Year);

            if (targetCurMonth == null || targetPrevMonth == null)
                return null;

            int CurSales = 0;
            int PrevSales = 0;
            int Orders = 1;

            if (targetCurMonth.Periode1Start != null && targetCurMonth.Periode1End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode1Start, targetCurMonth.Periode1End, targetPrevMonth.Periode1Start, targetPrevMonth.Periode1End, out CurSales, out PrevSales);
                results.Add(new WeeklyInsightResponseDto
                {
                    Orders = Orders,
                    PeriodeStart = targetCurMonth.Periode1Start.Value,
                    PeriodeEnd = targetCurMonth.Periode1End.Value,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode2Start != null && targetCurMonth.Periode2End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode2Start, targetCurMonth.Periode2End, targetPrevMonth.Periode2Start, targetPrevMonth.Periode2End, out CurSales, out PrevSales);
                results.Add(new WeeklyInsightResponseDto
                {
                    Orders = Orders,
                    PeriodeStart = targetCurMonth.Periode2Start.Value,
                    PeriodeEnd = targetCurMonth.Periode2End.Value,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode3Start != null && targetCurMonth.Periode3End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode3Start, targetCurMonth.Periode3End, targetPrevMonth.Periode3Start, targetPrevMonth.Periode3End, out CurSales, out PrevSales);
                results.Add(new WeeklyInsightResponseDto
                {
                    Orders = Orders,
                    PeriodeStart = targetCurMonth.Periode3Start.Value,
                    PeriodeEnd = targetCurMonth.Periode3End.Value,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode4Start != null && targetCurMonth.Periode4End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode4Start, targetCurMonth.Periode4End, targetPrevMonth.Periode4Start, targetPrevMonth.Periode4End, out CurSales, out PrevSales);
                results.Add(new WeeklyInsightResponseDto
                {
                    Orders = Orders,
                    PeriodeStart = targetCurMonth.Periode4Start.Value,
                    PeriodeEnd = targetCurMonth.Periode4End.Value,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode5Start != null && targetCurMonth.Periode5End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode5Start, targetCurMonth.Periode5End, targetPrevMonth.Periode5Start, targetPrevMonth.Periode5End, out CurSales, out PrevSales);
                results.Add(new WeeklyInsightResponseDto
                {
                    Orders = Orders,
                    PeriodeStart = targetCurMonth.Periode5Start.Value,
                    PeriodeEnd = targetCurMonth.Periode5End.Value,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode6Start != null && targetCurMonth.Periode6End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode6Start, targetCurMonth.Periode6End, targetPrevMonth.Periode6Start, targetPrevMonth.Periode6End, out CurSales, out PrevSales);
                results.Add(new WeeklyInsightResponseDto
                {
                    Orders = Orders,
                    PeriodeStart = targetCurMonth.Periode6Start.Value,
                    PeriodeEnd = targetCurMonth.Periode6End.Value,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            return results;
        }

        protected void setSales(string DealerId, DateTime? CurPeriodeStart, DateTime? CurPeriodeEnd, DateTime? PrevPeriodeStart, DateTime? PrevPeriodeEnd, out int CurSales, out int PrevSales)
        {
            CurSales = 0;
            PrevSales = 0;

            if (CurPeriodeStart == null || CurPeriodeEnd == null)
                return;

            if (PrevPeriodeStart == null || PrevPeriodeEnd == null)
                return;

            var salesCurMonth = _repositorySales
                .GetAllList(x => x.DealerId == DealerId
                    && x.TransactionDate >= CurPeriodeStart
                    && x.TransactionDate <= CurPeriodeEnd);

            var salesPrevMonth = _repositorySales
                .GetAllList(x => x.DealerId == DealerId
                    && x.TransactionDate >= PrevPeriodeStart
                    && x.TransactionDate <= PrevPeriodeEnd);

            if (salesCurMonth != null)
                CurSales = salesCurMonth.Sum(x => x.Amount);

            if (salesPrevMonth != null)
                PrevSales = salesPrevMonth.Sum(x => x.Amount);
        }

        public async Task<List<DailyInsightResponseDto>> GetDailyInsight(MonitorSalesKotaFilterDto request)
        {
            var curMonthStart = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, 1);
            var curMonthEnd = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, request.CurrentDate.Day);

            var results = new List<DailyInsightResponseDto>();

            int Orders = 1;

            for(DateTime dt = curMonthStart.Date; dt.Date <= curMonthEnd.Date; dt = dt.AddDays(1))
            {
                var prevDt = dt.AddMonths(-1);
                int CurSales = 0;
                int PrevSales = 0;

                var salesCurMonth = await _repositorySales
                    .GetAllListAsync(x => x.DealerId == request.DealerId && x.TransactionDate.Date == dt.Date);

                var salesPrevMonth = await _repositorySales
                    .GetAllListAsync(x => x.DealerId == request.DealerId && x.TransactionDate.Date == prevDt.Date);

                if (salesCurMonth != null)
                    CurSales = salesCurMonth.Sum(x => x.Amount);

                if (salesPrevMonth != null)
                    PrevSales = salesPrevMonth.Sum(x => x.Amount);

                results.Add(new DailyInsightResponseDto
                {
                    Orders = Orders,
                    Date = dt,
                    CurrentSales = CurSales,
                    PreviousSales = PrevSales,
                    Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
                });
                Orders += 1;
            }

            return results;
        }
        #endregion

        #region Sales Growth
        public async Task<MonthlySalesGrowthResponseDto> GetMonthlySalesGrowth(MonitorSalesDealerFilterDto request)
        {
            var curMonth = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, 1);
            var prevMonth = curMonth.AddDays(-1);

            int CurSales = 0;
            int PrevSales = 0;

            var salesCurMonth = await _repositorySales
                .GetAllListAsync(x => x.DealerId == request.DealerId
                    && x.TransactionDate.Month == curMonth.Month
                    && x.TransactionDate.Year == curMonth.Year);

            var salesPrevMonth = await _repositorySales
                .GetAllListAsync(x => x.DealerId == request.DealerId
                    && x.TransactionDate.Month == prevMonth.Month
                    && x.TransactionDate.Year == prevMonth.Year);

            if (salesCurMonth != null)
                CurSales = salesCurMonth.Sum(x => x.Amount);

            if (salesPrevMonth != null)
                PrevSales = salesPrevMonth.Sum(x => x.Amount);

            var result = new MonthlySalesGrowthResponseDto
            {
                CurrentMonth = curMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("id")),
                PreviousMonth = prevMonth.ToString("MMMM", CultureInfo.CreateSpecificCulture("id")),
                CurrentSales = CurSales,
                PreviousSales = PrevSales,
                Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
            };

            return result;
        }

        public async Task<DailySalesGrowthResponseDto> GetDailySalesGrowth(MonitorSalesDealerFilterDto request)
        {
            var curDate = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, request.CurrentDate.Day);
            var prevDate = curDate.AddDays(-1);

            int CurSales = 0;
            int PrevSales = 0;

            var salesCurMonth = await _repositorySales
                .GetAllListAsync(x => x.DealerId == request.DealerId
                    && x.TransactionDate.Date == curDate.Date);

            var salesPrevMonth = await _repositorySales
                .GetAllListAsync(x => x.DealerId == request.DealerId
                    && x.TransactionDate.Date == prevDate.Date);

            if (salesCurMonth != null)
                CurSales = salesCurMonth.Sum(x => x.Amount);

            if (salesPrevMonth != null)
                PrevSales = salesPrevMonth.Sum(x => x.Amount);

            var result = new DailySalesGrowthResponseDto
            {
                CurrentDate = curDate,
                PreviousDate = prevDate,
                CurrentSales = CurSales,
                PreviousSales = PrevSales,
                Growth = (PrevSales == 0 && CurSales == 0) ? 0 : PrevSales == 0 ? 100 : ((decimal)(CurSales - PrevSales) / PrevSales) * 100
            };

            return result;
        }
        
        public async Task<List<AchievementGrowthDealerResponseDto>> GetAchievementGrowthDealer(MonitorSalesDealerFilterDto request)
        {
            var curMonth = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, 1);
            var results = new List<AchievementGrowthDealerResponseDto>();

            var targetCurMonth = await _repositoryTargetSales
                .FirstOrDefaultAsync(x => x.DealerId == request.DealerId
                    && x.Month == curMonth.Month
                    && x.Year == curMonth.Year);

            if (targetCurMonth == null)
                return null;

            int Sales = 0;
            int Orders = 1;

            setSales(request.DealerId, curMonth.Month, curMonth.Year, out Sales);
            results.Add(new AchievementGrowthDealerResponseDto
            {
                Orders = Orders,
                Periode = curMonth.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("id")),
                Target = targetCurMonth.TargetTotal,
                Sales = Sales,
                Achievement = targetCurMonth.TargetTotal == 0 ? 0 : (decimal)(Sales / targetCurMonth.TargetTotal) * 100
            });
            Orders += 1;

            if (targetCurMonth.Periode1Start != null && targetCurMonth.Periode1End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode1Start, targetCurMonth.Periode1End, out Sales);
                results.Add(new AchievementGrowthDealerResponseDto
                {
                    Orders = Orders,
                    Periode = string.Format("{0} - {1} {2}", targetCurMonth.Periode1Start.Value.ToString("dd"), targetCurMonth.Periode1End.Value.ToString("dd"), targetCurMonth.Periode1End.Value.ToString("MMM", CultureInfo.CreateSpecificCulture("id"))),
                    Target = targetCurMonth.Periode1Target,
                    Sales = Sales,
                    Achievement = targetCurMonth.Periode1Target == 0 ? 0 : (decimal)(Sales / targetCurMonth.Periode1Target) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode2Start != null && targetCurMonth.Periode2End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode2Start, targetCurMonth.Periode2End, out Sales);
                results.Add(new AchievementGrowthDealerResponseDto
                {
                    Orders = Orders,
                    Periode = string.Format("{0} - {1} {2}", targetCurMonth.Periode2Start.Value.ToString("dd"), targetCurMonth.Periode2End.Value.ToString("dd"), targetCurMonth.Periode2End.Value.ToString("MMM", CultureInfo.CreateSpecificCulture("id"))),
                    Target = targetCurMonth.Periode2Target,
                    Sales = Sales,
                    Achievement = targetCurMonth.Periode2Target == 0 ? 0 : (decimal)(Sales / targetCurMonth.Periode2Target) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode3Start != null && targetCurMonth.Periode3End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode3Start, targetCurMonth.Periode3End, out Sales);
                results.Add(new AchievementGrowthDealerResponseDto
                {
                    Orders = Orders,
                    Periode = string.Format("{0} - {1} {2}", targetCurMonth.Periode3Start.Value.ToString("dd"), targetCurMonth.Periode3End.Value.ToString("dd"), targetCurMonth.Periode3End.Value.ToString("MMM", CultureInfo.CreateSpecificCulture("id"))),
                    Target = targetCurMonth.Periode3Target,
                    Sales = Sales,
                    Achievement = targetCurMonth.Periode3Target == 0 ? 0 : (decimal)(Sales / targetCurMonth.Periode3Target) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode4Start != null && targetCurMonth.Periode4End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode4Start, targetCurMonth.Periode4End, out Sales);
                results.Add(new AchievementGrowthDealerResponseDto
                {
                    Orders = Orders,
                    Periode = string.Format("{0} - {1} {2}", targetCurMonth.Periode4Start.Value.ToString("dd"), targetCurMonth.Periode4End.Value.ToString("dd"), targetCurMonth.Periode4End.Value.ToString("MMM", CultureInfo.CreateSpecificCulture("id"))),
                    Target = targetCurMonth.Periode4Target,
                    Sales = Sales,
                    Achievement = targetCurMonth.Periode4Target == 0 ? 0 : (decimal)(Sales / targetCurMonth.Periode4Target) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode5Start != null && targetCurMonth.Periode5End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode5Start, targetCurMonth.Periode5End, out Sales);
                results.Add(new AchievementGrowthDealerResponseDto
                {
                    Orders = Orders,
                    Periode = string.Format("{0} - {1} {2}", targetCurMonth.Periode5Start.Value.ToString("dd"), targetCurMonth.Periode5End.Value.ToString("dd"), targetCurMonth.Periode5End.Value.ToString("MMM", CultureInfo.CreateSpecificCulture("id"))),
                    Target = targetCurMonth.Periode5Target,
                    Sales = Sales,
                    Achievement = targetCurMonth.Periode5Target == 0 ? 0 : (decimal)(Sales / targetCurMonth.Periode5Target) * 100
                });
                Orders += 1;
            }

            if (targetCurMonth.Periode6Start != null && targetCurMonth.Periode6End != null)
            {
                setSales(request.DealerId, targetCurMonth.Periode6Start, targetCurMonth.Periode6End, out Sales);
                results.Add(new AchievementGrowthDealerResponseDto
                {
                    Orders = Orders,
                    Periode = string.Format("{0} - {1} {2}", targetCurMonth.Periode6Start.Value.ToString("dd"), targetCurMonth.Periode6End.Value.ToString("dd"), targetCurMonth.Periode6End.Value.ToString("MMM", CultureInfo.CreateSpecificCulture("id"))),
                    Target = targetCurMonth.Periode6Target,
                    Sales = Sales,
                    Achievement = targetCurMonth.Periode6Target == 0 ? 0 : (decimal)(Sales / targetCurMonth.Periode6Target) * 100
                });
                Orders += 1;
            }

            return results;
        }

        public async Task<List<AchievementGrowthKotaResponseDto>> GetAchievementGrowthKota(MonitorSalesKotaFilterDto request)
        {
            var curMonth = new DateTime(request.CurrentDate.Year, request.CurrentDate.Month, 1);
            var prevMonth = curMonth.AddDays(-1);
            var results = new List<AchievementGrowthKotaResponseDto>();

            var prevSales = await _repositorySales
                .GetAllListAsync(x => x.DealerId == request.DealerId
                    && x.TransactionDate.Month == prevMonth.Month
                    && x.TransactionDate.Year == prevMonth.Year);

            var prevSalesTunai = prevSales.Where(x => x.EnumPaymentCategory == "TUNAI").Sum(x => x.Amount);
            var prevSalesKredit = prevSales.Where(x => x.EnumPaymentCategory != "TUNAI").Sum(x => x.Amount);
            var prevSalesTotal = prevSales.Sum(x => x.Amount);

            var curSales = await _repositorySales
                .GetAllListAsync(x => x.DealerId == request.DealerId
                    && x.TransactionDate.Month == curMonth.Month
                    && x.TransactionDate.Year == curMonth.Year);

            var curSalesTunai = curSales.Where(x => x.EnumPaymentCategory == "TUNAI").Sum(x => x.Amount);
            var curSalesKredit = curSales.Where(x => x.EnumPaymentCategory != "TUNAI").Sum(x => x.Amount);
            var curSalesTotal = curSales.Sum(x => x.Amount);

            var pencapaianTunai = (prevSalesTunai == 0 && curSalesTunai == 0) ? 0 : prevSalesTunai == 0 ? 100 : ((decimal)(curSalesTunai - prevSalesTunai) / prevSalesTunai) * 100;
            var pencapaianKredit = (prevSalesKredit == 0 && curSalesKredit == 0) ? 0 : prevSalesKredit == 0 ? 100 : ((decimal)(curSalesKredit - prevSalesKredit) / prevSalesKredit) * 100;
            var pencapaianTotal = (prevSalesTotal == 0 && curSalesTotal == 0) ? 0 : prevSalesTotal == 0 ? 100 : ((decimal)(curSalesTotal - prevSalesTotal) / prevSalesTotal) * 100;

            results.Add(
                new AchievementGrowthKotaResponseDto
                {
                    Orders = 1,
                    Periode = "Bulan Lalu",
                    Tunai = prevSalesTunai,
                    Kredit = prevSalesKredit,
                    Total = prevSalesTotal
                }
            );

            results.Add(
                new AchievementGrowthKotaResponseDto
                {
                    Orders = 2,
                    Periode = "Bulan Ini",
                    Tunai = curSalesTunai,
                    Kredit = curSalesKredit,
                    Total = curSalesTotal
                }
            );

            results.Add(
                new AchievementGrowthKotaResponseDto
                {
                    Orders = 3,
                    Periode = "Pencapaian",
                    Tunai = pencapaianTunai,
                    Kredit = pencapaianKredit,
                    Total = pencapaianTotal
                }
            );

            return results;
        }

        protected void setSales(string DealerId, int Month, int Year, out int Sales)
        {
            Sales = 0;

            var salesCurMonth = _repositorySales
                .GetAllList(x => x.DealerId == DealerId
                    && x.TransactionDate.Month == Month
                    && x.TransactionDate.Year == Year);

            if (salesCurMonth != null)
                Sales = salesCurMonth.Sum(x => x.Amount);

        }

        protected void setSales(string DealerId, DateTime? CurPeriodeStart, DateTime? CurPeriodeEnd, out int Sales)
        {
            Sales = 0;

            if (CurPeriodeStart == null || CurPeriodeEnd == null)
                return;

            var salesCurMonth = _repositorySales
                .GetAllList(x => x.DealerId == DealerId
                    && x.TransactionDate >= CurPeriodeStart
                    && x.TransactionDate <= CurPeriodeEnd);

            if (salesCurMonth != null)
                Sales = salesCurMonth.Sum(x => x.Amount);

        }
        #endregion
    }
}
