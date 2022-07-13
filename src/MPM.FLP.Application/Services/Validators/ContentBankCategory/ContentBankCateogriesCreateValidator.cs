using FluentValidation;
using MPM.FLP.Common.Constants;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Validators.ContentBankCategory
{
    public class ContentBankCateogriesCreateValidator : AbstractValidator<ContentBankCategoriesCreateDto>
    {
        public ContentBankCateogriesCreateValidator()
        {
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

            RuleFor(x => x.CreatorUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Creator"));
        }
    }
}
