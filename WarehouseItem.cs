using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class WarehouseItem
    {
        public string Name { get; set; }          // Название ресурса
        public double Quantity { get; set; }      // Количество тонн
        public double PricePerTon { get; set; }   // Цена за тонну (грн)
        public double Count { get; internal set; }

        public WarehouseItem(string name, double quantity, double pricePerTon)
        {
            Name = name;
            Quantity = quantity;
            PricePerTon = pricePerTon;
        }

        // Добавить руду в склад
        public void Add(double tons) => Quantity += tons;

        // Общая стоимость запасов
        public double TotalValue() => Quantity * PricePerTon;
    }
}





