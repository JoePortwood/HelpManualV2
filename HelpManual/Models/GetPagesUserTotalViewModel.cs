using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpManual.Models
{
    public class GetPagesUserTotalViewModel
    {
        public string FullName { get; set; }

        public int PageNo { get; set; }

        public int Total { get; set; }
    }
}
