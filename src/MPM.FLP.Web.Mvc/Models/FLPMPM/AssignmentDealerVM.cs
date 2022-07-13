﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class AssignmentDealerVM
    {
        public Guid Id { get; set; }
        public Guid DealerId { get; set; }
        public Guid ParentId { get; set; }
        public string Title { get; set; }
        public string KodeDealerMPM { get; set; }
        public string NamaDealer { get; set; }
        public decimal? FLPResult { get; set; }
        public bool? IsVerified { get; set; }
    }
}
