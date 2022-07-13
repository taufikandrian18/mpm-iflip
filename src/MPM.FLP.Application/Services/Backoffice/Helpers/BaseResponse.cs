using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
   
    public class Message
    {
        public string En { get; set; }
        public string Id { get; set; }
    }

    public class BaseResponse {
        public string EnumCode { get; set; }
        public object Data { get; set; }
        public Message Msg { get; set; }

        public int Count { get; set; }

         public static BaseResponse Ok(object data, int count)
        {
            var resp = new BaseResponse
            {
                EnumCode = "200",
                Msg = new Message
                {
                    En = "Success.",
                    Id = "Berhasil."
                },
                Data = data,
                Count = count,
            };
            return resp;
        }

        public static BaseResponse Ok()
        {
            var resp = new BaseResponse
            {
                EnumCode = "200",
                Msg = new Message
                {
                    En = "Success.",
                    Id = "Berhasil."
                },
                Data = null,

            };
            return resp;
        }

        public static BaseResponse BadRequest(object data, Message msg)
        {
            var resp = new BaseResponse
            {
                EnumCode = "400",
                Msg = msg,
                Data = data
            };
            return resp;
        }

        public static BaseResponse BadRequest(object data)
        {
            var resp = new BaseResponse
            {
                EnumCode = "400",
                Msg = new Message
                {
                    En = "Invalid data format.",
                    Id = "Data tidak valid."
                },
                Data = data
            };
            return resp;
        }

        public static BaseResponse Created(object data)
        {
            var resp = new BaseResponse
            {
                EnumCode = "201",
                Msg = new Message
                {
                    En = "Data has been created.",
                    Id = "Data telah berhasil ditambahkan."
                },
                Data = data
            };
            return resp;
        }

        public static BaseResponse Error(object data, Exception ex)
        {
            var resp = new BaseResponse
            {
                EnumCode = "500",
                Msg = new Message
                {
                    En = "Internal server error.",
                    Id = "Terjadi kesalahan pada server."
                },
                Data = ex.Message
            };
            return resp;
        }
    }
}
