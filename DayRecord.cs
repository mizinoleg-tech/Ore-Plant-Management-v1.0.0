using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class DayRecord
    {
        public int DayNumber { get; set; }
        public double Production { get; set; }
        public double WarehouseTotal { get; set; }

        public DayRecord(int day, double production, double warehouseTotal)
        {
            DayNumber = day;
            Production = production;
            WarehouseTotal = warehouseTotal;
        }
    }
}
