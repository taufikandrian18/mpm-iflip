using Abp.Domain.Repositories;
using FluentValidation;
using MPM.FLP.Common.Constants;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Validators.TBSMUserSiswa
{
    public class TBSMUserSiswasCreateValidator : AbstractValidator<TBSMUserSiswasCreateDto>
    {
        public TBSMUserSiswasCreateValidator(
            IRepository<TBSMUserSiswas, Guid> repository,
            IRepository<Sekolahs, Guid> sekolahRepository)
        {
            RuleFor(x => x.GUIDSekolah)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Sekolah"))
                .Must((x, y) =>
                {
                    return sekolahRepository.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Sekolah"));

            RuleFor(x => x.KodeMD)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Kode MD"));

            RuleFor(x => x.NamaMD)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Nama MD"));

            RuleFor(x => x.NPSN)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "NPSN"));

            RuleFor(x => x.Nama)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Nama Guru"));

            RuleFor(x => x.NIS)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "NIS"))
                .Must((x, y) =>
                 {
                     return repository.FirstOrDefault(z => z.NIS == y && z.DeletionTime == null) == null;
                 })
                .WithMessage(string.Format(ErrorMessageConstant.ExistsMessage, "NIS"));

            RuleFor(x => x.CreatorUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Creator"));
        }
    }
}
