using Abp.Domain.Repositories;
using FluentValidation;
using MPM.FLP.Common.Constants;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Validators.TBSMUserGuru
{
    public class TBSMUserGurusDeleteValidator : AbstractValidator<TBSMUserGurusDeleteDto>
    {
        public TBSMUserGurusDeleteValidator(IRepository<TBSMUserGurus, Guid> repository)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Id"))
                .Must((x, y) => 
                {
                    return repository.FirstOrDefault(z => z.Id == x.Id && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Id"));

            RuleFor(x => x.DeleterUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Deleter"));
        }
    }
}
