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
    public class TrainingAbsenceAppService : FLPAppServiceBase, ITrainingAbsenceAppService
    {
        private readonly IRepository<TrainingAbsence, Guid> _trainingAbsenceRepository;

        public TrainingAbsenceAppService(IRepository<TrainingAbsence, Guid> trainingAbsenceRepository)
        {
            _trainingAbsenceRepository = trainingAbsenceRepository;
        }
        public ServiceResult Absence(TrainingAbsenceDto input)
        {
            bool isSuccess;
            string message;

            var absence = _trainingAbsenceRepository.GetAll()
                .FirstOrDefault(x => x.IDMPM == input.IDMPM
                             && x.IDTraining == input.IDTraining
                             && x.FirstAbsence.Date == DateTime.UtcNow.AddHours(7).Date);

            if (absence == null)
            {
                TrainingAbsence trainingAbsence = new TrainingAbsence()
                {
                    Id = Guid.NewGuid(),
                    IDMPM = input.IDMPM,
                    IDTraining = input.IDTraining,
                    FirstAbsence = DateTime.UtcNow.AddHours(7)
                };

                _trainingAbsenceRepository.Insert(trainingAbsence);

                isSuccess = true; message = "Berhasil absen";
            }
            else
            {
                if (absence.SecondAbsence == null)
                {
                    var now = DateTime.UtcNow.AddHours(7);
                    TimeSpan span = now - absence.FirstAbsence;
                    if (span.TotalMinutes <= 180)
                    {
                        isSuccess = false;
                        var jam = 0;
                        var menit = 0;
                        if (absence.FirstAbsence.TimeOfDay.Hours > 12)
                        {
                            jam = absence.FirstAbsence.TimeOfDay.Hours - 12;
                        }
                        else
                        {
                            jam = absence.FirstAbsence.TimeOfDay.Hours;
                        }
                        menit = absence.FirstAbsence.TimeOfDay.Minutes;

                        message = "Tolong tunggu setidaknya 3 jam untuk absen lagi. Anda absen pada pukul : " + jam +"."+menit+" WIB.";
                    }
                    else
                    {
                        absence.SecondAbsence = now;
                        isSuccess = true; message = "Berhasil absen";
                    }
                }
                else
                { isSuccess = false; message = "Anda sudah absen 2 kali, tidak bisa absen lagi."; }
            }

            return new ServiceResult() { IsSuccess = isSuccess, Message = message };
        }
    }
}
