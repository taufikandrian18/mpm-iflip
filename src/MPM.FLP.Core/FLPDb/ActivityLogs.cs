using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MPM.FLP.FLPDb
{
    public class ActivityLogs : Entity<Guid> {
        public override Guid Id { get; set; }
        [Required, MaxLength(256)]
        public string Username { get; set; }
        [Required, MaxLength(256)]
        public string ContentType {get;set;}
        [Required, MaxLength(256)]
        public string ContentId {get;set;}
        [Required, MaxLength(256)]
        public string ActivityType {get;set;}
        [Required, MaxLength(256)]
        public string ContentTitle { get; set; }
        [Required]
        public DateTime Time { get; private set; }

        public ActivityLogs() {
            Time = DateTime.Now;
        }
        public ActivityLogs(string contentType, string contentId, string contentTitle, string activityType) {
            ContentType = contentType;
            ContentId = contentId;
            ContentTitle = contentTitle;
            ActivityType = activityType;
            Time = DateTime.Now;
        }

        public static string GetContentTypeText(string contentType)
        {
            switch (contentType)
            {
                case "article":
                    return "Article";
                case "guide":
                    return "Panduan";
                case "salesprogram":
                    return "Sales Program";
                case "serviceprogram":
                    return "Service Program";
                case "salestalk":
                    return "Sales Talk";
                case "servicetalkflyer":
                    return "Service Talk Flyer";
                case "brandcampaign":
                    return "Brand Campaign";
                case "infomaindealer":
                    return "Info Main Dealer";
                case "homeworkquiz":
                    return "Homework Quiz";
                case "livequiz":
                    return "Live Quiz";
                case "productcatalog":
                    return "Katalog Produk";
                case "apparelcatalog":
                    return "Katalog Apparel";
                case "roleplay":
                    return "Role Play";
                case "selfrecording":
                    return "Self Recording";
                case "forum":
                    return "Forum";
                case "customerengagement":
                    return "Customer Engagement";
                case "customerscanning":
                    return "Customer Scanning";
                case "onlinemagazine":
                    return "Online Magazine";
                case "calendarofevent":
                    return "Calendar of Events";
                case "clubcommunity":
                    return "Club Community";
                case "agenda":
                    return "Agenda";
                case "cschampionclub":
                    return "CS Champion Club";
                case "motivationcard": 
                    return "Motivation Card";
                case "salesincentiveprogram":
                    return "Sales Incentive Program";
                case "claimprogram":
                    return "Claim Program";
                case "spdcontest":
                    return "Sales People Development Contest";
                default:
                    return contentType;
            }
        }
    }
}
