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
    public class ContentBankAssigneeProofsUpdateValidator : AbstractValidator<ContentBankAssigneeProofsUpdateDto>
    {
        public ContentBankAssigneeProofsUpdateValidator(
            IRepository<ContentBankAssigneeProofs, Guid> repositoryProof,
            IRepository<ContentBankAssignees, Guid> repositoryAssignee,
            IRepository<ContentBankPlatforms, Guid> repositoryPlatform)
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Id"))
                .Must((x, y) =>
                {
                    return repositoryProof.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Id"));

            RuleFor(x => x.GUIDContentBankAssignee)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Content Bank Assignee Id"))
                .Must((x, y) => {
                    return repositoryAssignee.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Content Bank Assignee Id"));

            RuleFor(x => x.GUIDContentBankPlatform)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Platform"))
                .Must((x, y) => {
                    return repositoryPlatform.FirstOrDefault(z => z.Id == y && z.DeletionTime == null) != null;
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotExistsMessage, "Platform"));

            RuleFor(x => x.GUIDEmployee)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Employee ID"));

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

            RuleFor(x => x.RelatedLink)
                .Cascade(CascadeMode.Stop)
                .Must((x, y) =>
                {
                    if (string.IsNullOrEmpty(y))
                        return true;

                    return Uri.TryCreate(y, UriKind.Absolute, out Uri outUri) && outUri is Uri
                           && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
                })
                .WithMessage(string.Format(ErrorMessageConstant.NotValidMessage, "Related Link"));

            RuleFor(x => x.ViewCount)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "View Count"));

            RuleFor(x => x.UploadDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Upload Date"))
                .Must((x, y) => {
                    return DateTime.Now >= x.UploadDate;
                })
                .WithMessage(string.Format(ErrorMessageConstant.GreatherThanMessage, "Upload Date", "Current Date"));

            RuleFor(x => x.LastModifierUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(string.Format(ErrorMessageConstant.NotEmptyMessage, "Modifier"));
        }
    }
}
