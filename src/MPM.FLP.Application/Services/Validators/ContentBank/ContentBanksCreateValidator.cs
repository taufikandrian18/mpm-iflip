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
    public class ContentBanksCreateValidator : AbstractValidator<ContentBanksCreateDto>
    {
        public ContentBanksCreateValidator(IRepository<ContentBankCategories, Guid> repository)
        {
            RuleFor(x => x.GUIDContentBankCategory)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Category Id"))
                .Must((x, y) => {
                    return repository.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
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

            RuleFor(x => x.Caption)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Caption"));

            RuleFor(x => x.ReadingTime)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Reading Time"));

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

            RuleFor(x => x.IsPublished)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Is Published"))
                .Must(x => x == true || x == false)
                .WithMessage(string.Format(ErrorMessageConstant.NotValidMessage, "Is Published"));

            RuleFor(x => x.CreatorUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Creator"));

            RuleForEach(x => x.Details).SetValidator(new CreateDetailsValidator());
        }
    }

    internal class CreateDetailsValidator : AbstractValidator<ContentBanksDetailsDto>
    {
        public CreateDetailsValidator()
        {
            RuleFor(x => x.Orders)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Order"));

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Name"));

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Description"));

            RuleFor(x => x.Caption)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Caption"));

            RuleFor(x => x.Extension)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Extension"));

            RuleFor(x => x.AttachmentURL)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Attachment URL"))
                .Must((x, y) =>
                {
                    if (string.IsNullOrEmpty(y))
                        return true;

                    return Uri.TryCreate(y, UriKind.Absolute, out Uri outUri) && outUri is Uri
                           && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotValidMessage, "Attachment URL"));
        }
    }
}
