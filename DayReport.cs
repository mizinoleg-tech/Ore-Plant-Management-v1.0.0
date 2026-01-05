using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class DayReport
    {
        public DateTime Date { get; set; }
        public double Production { get; set; }
        public double Sold { get; set; }
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double Profit { get; set; }
        public double Balance { get; set; }
    }

}
