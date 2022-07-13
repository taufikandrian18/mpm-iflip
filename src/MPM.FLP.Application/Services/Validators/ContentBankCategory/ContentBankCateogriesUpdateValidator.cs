using Abp.Domain.Repositories;
using FluentValidation;
using MPM.FLP.Common.Constants;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;

namespace MPM.FLP.Services.Validators.ContentBankCategory
{
    public class ContentBankCateogriesUpdateValidator : AbstractValidator<ContentBankCategoriesUpdateDto>
    {
        public ContentBankCateogriesUpdateValidator(IRepository<ContentBankCategories, Guid> repository)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Id"))
                .Must((x, y) => 
                {
                    return repository.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Id"));

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Category Name"));

            RuleFor(x => x.Orders)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Order"))
                .GreaterThan(0)
                .WithMessage(string.Format(ErrorMessageConstant.GreatherThanMessage, "Order", "0"));

            RuleFor(x => x.AttachmentUrl)
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

            RuleFor(x => x.LastModifierUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Modifier"));
        }
    }
}
