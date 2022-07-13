using Abp.Domain.Repositories;
using FluentValidation;
using FluentValidation.Validators;
using MPM.FLP.Common.Constants;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Validators.ContentBankCategory
{
    public class ContentBanksUpdateValidator : AbstractValidator<ContentBanksUpdateDto>
    {
        public ContentBanksUpdateValidator(
            IRepository<ContentBanks, Guid> repositoryContent,
            IRepository<ContentBankCategories, Guid> repositoryCategory)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Id"))
                .Must((x, y) =>
                {
                    return repositoryContent.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Id"));

            RuleFor(x => x.GUIDContentBankCategory)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Category Id"))
                .Must((x, y) => {
                    return repositoryCategory.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Category Id"));

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Name"));

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Description"));

            RuleFor(x => x.StartDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Start Date"));

            RuleFor(x => x.EndDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "End Date"))
                .Must((x, y) => {
                    return x.EndDate >= x.StartDate;
                })
                .WithMessage(string.Format(ErrorMessageConstant.GreatherThanMessage, "End Date", "Start Date"));

            RuleFor(x => x.LastModifierUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Creator"));
        }
    }
}
