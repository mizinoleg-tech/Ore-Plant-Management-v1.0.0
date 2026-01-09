using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class WarehouseItem
    {
        public string Name { get; set; }
        public double Quantity { get; private set; } // приватный сеттер — ок
        public double PricePerTon { get; set; }
        public double Count { get; internal set; }

        public double Capacity { get; set; } = 5000.0;

        public WarehouseItem(string name, double quantity, double pricePerTon)
        {
            Name = name;
            PricePerTon = pricePerTon;
            SetQuantity(quantity); // сразу нормализуем
        }

        // Добавить руду с учётом лимита
        public void Add(double tons)
        {
            SetQuantity(Quantity + tons);
        }

        // Установить количество с учётом лимита
        public void SetQuantity(double value)
        {
            Quantity = Math.Min(Math.Max(value, 0.0), Capacity);
        }

        public double TotalValue() => Quantity * PricePerTon;
    }
}
