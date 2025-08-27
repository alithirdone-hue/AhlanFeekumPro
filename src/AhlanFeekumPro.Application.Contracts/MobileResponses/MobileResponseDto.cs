using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhlanFeekumPro.MobileResponses
{
    public class MobileResponseDto
    {
        public object Data { set; get; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
