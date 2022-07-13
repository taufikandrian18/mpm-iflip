using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class CustomerScanningAppService : FLPAppServiceBase, ICustomerScanningAppService
    {
        private readonly IRepository<Variabel> _variabelRepository;
        private readonly IRepository<TipeMotor> _tipeMotorRepository;
        private readonly IRepository<KoefisienRegresi> _koefisienRegresiRepository;

        public CustomerScanningAppService(IRepository<Variabel> variabelRepository,
                                          IRepository<TipeMotor> tipeMotorRepository,
                                          IRepository<KoefisienRegresi> koefisienRegresiRepository) 
        {
            _variabelRepository = variabelRepository;
            _tipeMotorRepository = tipeMotorRepository;
            _koefisienRegresiRepository = koefisienRegresiRepository;
        }
        public CustomerScanningOptionDto GetCustomerScanningOption()
        {
            CustomerScanningOptionDto options = new CustomerScanningOptionDto();
            var variabel = _variabelRepository.GetAll();
            options.JenisKelamin = variabel.Where(x => x.JenisVariabelId == 1).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama}).ToList();
            options.Usia = variabel.Where(x => x.JenisVariabelId == 2).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama }).ToList();
            options.Pekerjaan = variabel.Where(x => x.JenisVariabelId == 3).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama }).ToList();
            options.SES = variabel.Where(x => x.JenisVariabelId == 4).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama }).ToList();
            options.TipeSebelum = variabel.Where(x => x.JenisVariabelId == 5).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama }).ToList();
            options.JumlahMotor = variabel.Where(x => x.JenisVariabelId == 6).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama }).ToList();
            options.AlasanBeli = variabel.Where(x => x.JenisVariabelId == 7).Select(x => new VariabelDto() { Id = x.Id, Nama = x.Nama }).ToList();
            return options;
        }

        public List<CustomerScanningResultDto> ScanCustomer(CustomerScanningSubmitDto input)
        {
            List<KoefisienRegresi> koefisiens = _koefisienRegresiRepository.GetAll().ToList();
            List<TipeMotor> tipeMotors = _tipeMotorRepository.GetAll().ToList();
            List<CustomerScanningResultDto> results = new List<CustomerScanningResultDto>();
            List<CustomerScanningCalculationDto> calculations = new List<CustomerScanningCalculationDto>();

            foreach (var motor in tipeMotors) 
            {
                CustomerScanningCalculationDto calc = new CustomerScanningCalculationDto();
                calc.TipeMotorId = motor.Id;
                calc.NamaMotor = motor.Nama;
                var nilaiKoefisien = koefisiens.Where(x => x.TipeMotorId == motor.Id && (
                                                        x.VariabelId == 1 ||
                                                        x.VariabelId == input.jenisKelaminId ||
                                                        x.VariabelId == input.usiaId ||
                                                        x.VariabelId == input.pekerjaanId ||
                                                        x.VariabelId == input.sesId ||
                                                        x.VariabelId == input.tipeSebelumId ||
                                                        x.VariabelId == input.jumlahMotorId ||
                                                        x.VariabelId == input.alasanBeliId))
                                                        .Select(x => (double)x.Nilai).Sum();
                calc.Koefisien = Math.Exp(nilaiKoefisien);

                calculations.Add(calc);
            }

            CustomerScanningCalculationDto pesaingVario150 = new CustomerScanningCalculationDto()
            {
                TipeMotorId = 0,
                NamaMotor = "pesaing Vario 150",
                Koefisien = 0
            };

            calculations.Add(pesaingVario150);

            var totalKoefisien = calculations.Select(x => x.Koefisien).Sum();

            foreach (var calc in calculations) 
            {
                CustomerScanningResultDto result = new CustomerScanningResultDto();
                result.Merk = calc.NamaMotor;
                if (calc.TipeMotorId == 0)
                {
                    result.Peluang = 1 / (1 + totalKoefisien);
                }
                else 
                {
                    result.Peluang = calc.Koefisien / (1 + totalKoefisien);
                }

                results.Add(result);

            }

            return results;
        }
    }
}
