using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public class UiUpdater
    {
        private readonly Label _lblBalance;
        private readonly Label _lblEmployees;
        private readonly Label _lblMineLevel;
        private readonly ListBox _lstEquipment;
        private readonly WarehouseControl _warehouseControl;
        private readonly TaxControl _taxControl;

        public UiUpdater(Label lblBalance, Label lblEmployees, Label lblMineLevel,
                         ListBox lstEquipment, WarehouseControl warehouseControl, TaxControl taxControl)
        {
            _lblBalance = lblBalance;
            _lblEmployees = lblEmployees;
            _lblMineLevel = lblMineLevel;
            _lstEquipment = lstEquipment;
            _warehouseControl = warehouseControl;
            _taxControl = taxControl;
        }

        public void Refresh(GameState gameState)
        {
            _lblBalance.Text = $"Баланс: {gameState.Balance:N0} грн";

            if (gameState.Workers != null && gameState.Workers.Count >= 2)
            {
                int miners = gameState.Workers[0]?.Count ?? 0;
                int admins = gameState.Workers[1]?.Count ?? 0;
                _lblEmployees.Text = $"Горняки: {miners}, Администраторы: {admins}";
            }
            else
            {
                _lblEmployees.Text = "Сотрудники: нет данных";
            }


            _lblMineLevel.Text = $"Уровень шахты: {gameState.MineLevel}";

            _lstEquipment.Items.Clear();
            if (gameState.Equipments != null)
            {
                foreach (var eq in gameState.Equipments)
                    _lstEquipment.Items.Add($"{eq.Name} x{eq.Count}");
            }

            _warehouseControl?.UpdateData();
            _taxControl?.UpdateData();
        }


    }
}
