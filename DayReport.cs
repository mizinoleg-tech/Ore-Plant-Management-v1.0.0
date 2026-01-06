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

        // Производство и продажи
        public double Production { get; set; }
        public double Sold { get; set; }

        // Финансы
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double Profit { get; set; }
        public double Balance { get; set; }

        // 👉 Новые поля для налогов
        public double TaxAmount { get; set; }      // сумма налога (25% от прибыли месяца)
        public bool TaxPaid { get; set; }          // статус оплаты налога
        public double MonthProfit { get; set; }    // накопленная прибыль за месяц
    }
}

