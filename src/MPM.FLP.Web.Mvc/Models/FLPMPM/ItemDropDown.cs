using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class ItemDropDown
    {
        public ItemDropDown(string show, string value)
        {
            this.show = show;
            this.value = value;
        }

        public string show { get; set; }
        public string value { get; set; }
    }
}
