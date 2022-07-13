using Abp.AutoMapper;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(BASTCategories))]
    public class BASTCategoryCreateDto
    {
        public string Name { get; set; }
    }

    public class BASTCategoryUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class BASTCategoryDeleteDto
    {
        public Guid Id { get; set; }
    }

    [AutoMapTo(typeof(BASTs))]
    public class BASTCreateDto
    {
        public Guid GUIDCategory { get; set; }
        public string Name { get; set; }
        public bool IsTbsm { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public string KodeAHM { get; set; }
        public string KodeMPM { get; set; }
        public string KodeH3AHM { get; set; }
        public string KodeH3MPM { get; set; }
        public string RoadLetter { get; set; }
        public string Description { get; set; }
        public List<BASTDetailsDto> details { get; set; }
    }

    public class BASTUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDCategory { get; set; }
        public string Name { get; set; }
        public bool IsTbsm { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public string KodeAHM { get; set; }
        public string KodeMPM { get; set; }
        public string KodeH3AHM { get; set; }
        public string KodeH3MPM { get; set; }
        public string RoadLetter { get; set; }
        public string Description { get; set; }
        //public List<BASTDetailsDto> Details { get; set; }
    }
    public class BASTDeleteDto
    {
        public Guid Id { get; set; }
    }

    [AutoMapTo(typeof(BASTDetails))]
    public class BASTDetailsDto
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public List<BASTAttachmentDto> Attachment { get; set; }
    }

    [AutoMapTo(typeof(BASTAttachment))]
    public class BASTAttachmentDto
    {
        public string Name { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
    }
    public class BASTAttachmentCreateDto
    {
        public Guid GUIDBASTDetail { get; set; }
        public string Name { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
    }

    public class BASTAttachmentUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDBASTDetail { get; set; }
        public string Name { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
    }
    
    [AutoMapTo(typeof(BASTAssignee))]
    public class BASTAssigneeCreateDto
    {
        public Guid GUIDBAST { get; set; }
        public long? GUIDEmployee { get; set; }
        public string Jabatan { get; set; }
        public string DealerName { get; set; }
        public string Channel { get; set; }
        public string KodeJaringan { get; set; }
        public string TipeJaringan { get; set; }
        public string Kota { get; set; }
    }

    public class BASTAssigneeUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDBAST { get; set; }
        public long? GUIDEmployee { get; set; }
        public string Jabatan { get; set; }
        public string DealerName { get; set; }
        public string Channel { get; set; }
        public string KodeJaringan { get; set; }
        public string TipeJaringan { get; set; }
        public string Kota { get; set; }
    }

    [AutoMapTo(typeof(BASTReport))]
    public class BASTReportCreateDto
    {
        public Guid GUIDBAST { get; set; }
        public Guid GUIDReporter { get; set; }
        public string NamaReporter { get; set; }
        public Guid GUIDPenerima { get; set; }
        public string NamaPenerima { get; set; }
        public Guid GUIDUpdater { get; set; }
        public string NamaUpdater { get; set; }
        public string KodeAHM { get; set; }
        public string KodeMPM { get; set; }
        public string Feedback { get; set; }
        public int JumlahDatang { get; set; }
        public int JumlahReject { get; set; }
    }

    public class BASTReportUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDBAST { get; set; }
        public Guid GUIDReporter { get; set; }
        public string NamaReporter { get; set; }
        public Guid GUIDPenerima { get; set; }
        public string NamaPenerima { get; set; }
        public Guid GUIDUpdater { get; set; }
        public string NamaUpdater { get; set; }
        public string KodeAHM { get; set; }
        public string KodeMPM { get; set; }
        public string Feedback { get; set; }
        public int JumlahDatang { get; set; }
        public int JumlahReject { get; set; }
    }

    [AutoMapTo(typeof(BASTReportAttachment))]
    public class BASTReportAttachmentCreateDto
    {
        public Guid GUIDBAST { get; set; }
        public Guid GUIDReport { get; set; }
        public string MIME { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
    }

    public class BASTReportAttachmentUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDBAST { get; set; }
        public Guid GUIDReport { get; set; }
        public string MIME { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
    }
}
