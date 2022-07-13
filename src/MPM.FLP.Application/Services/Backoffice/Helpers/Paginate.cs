using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{

    public class Pagination{
        public string ID {get;set;}
        public int Page {get;set;}
        public int Limit {get;set;}
        public string ParentId {get;set;}
        public string Key {get;set;}
        public string Query {get;set;}
        public string Channel {get;set;}
        public string Kerasidenan {get;set;}
        public string Kota{get;set;}
        public string IdJabatan {get;set;}
        public string Jabatan { get; set; }
        public string KodeDealer { get; set; }
        public string UserId {get;set;}
        public bool? IsH1 {get;set;}

        public bool? IsH2 {get;set;}

        public bool? IsH3 {get;set;}

        public bool? IsTBSM {get;set;}

        public bool? IsActive {get;set;}
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EnumStatus { get; set; }
    }
    public class Paginate
    {
        public static Pagination Validate(Pagination request){
            if (request.Page == 0 ){
                request.Page = 1;
            }

            if (request.Limit ==0){
                request.Limit  =10;
            }

            request.Page  = (request.Page-1) * request.Limit;
            return request;
        }
    }
}