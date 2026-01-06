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
            _lblEmployees.Text = $"Сотрудники: {gameState.Employees}";
            _lblMineLevel.Text = $"Уровень шахты: {gameState.MineLevel}";

            _lstEquipment.Items.Clear();
            foreach (var eq in gameState.Equipment)
                _lstEquipment.Items.Add(eq);

            _warehouseControl.UpdateData();
            _taxControl.UpdateData();
        }
    }
}
