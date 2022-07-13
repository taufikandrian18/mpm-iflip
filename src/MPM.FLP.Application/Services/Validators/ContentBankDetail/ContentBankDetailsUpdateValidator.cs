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
    public class ContentBankDetailsUpdateValidator : AbstractValidator<ContentBanksDetailsUpdateDto>
    {
        public ContentBankDetailsUpdateValidator(IRepository<ContentBanks, Guid> repositoryContent)
        {
            RuleFor(x => x.ContentBankId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Content Bank Id"))
                .Must((x, y) =>
                {
                    return repositoryContent.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "ContentBankId"));

            RuleFor(x => x.LastModifierUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Creator"));

            RuleForEach(x => x.Details).SetValidator(new UpdateDetailsValidator());
        }
    }

    internal class UpdateDetailsValidator : AbstractValidator<ContentBanksDetailsDto>
    {
        public UpdateDetailsValidator()
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
