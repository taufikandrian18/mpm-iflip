using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Doku
{
    public class DokuException : Exception
    {
        public string ResponseCode { get; private set; }
        public DokuException(string message) : base(message) { }
        public DokuException(string responseCode, string message) : base(message)
        {
            ResponseCode = responseCode;
        }
    }
}
