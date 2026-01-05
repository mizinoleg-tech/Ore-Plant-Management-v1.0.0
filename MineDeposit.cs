using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class MineDeposit
    {
        public int Depth { get; set; }             // глубина (м)
        public double OreReserve { get; set; }     // общий запас (тонн)
        public double RemainingOre { get; set; }   // остаток (тонн)
        public double UpgradeCost { get; set; }    // стоимость перехода (грн)

        public MineDeposit(int depth, double oreReserve, double upgradeCost)
        {
            Depth = depth;
            OreReserve = oreReserve;
            RemainingOre = oreReserve;
            UpgradeCost = upgradeCost;
        }
    }
}