
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    namespace Miner
    {
        public class ConsumptionControl : UserControl
        {
            private Label lblWater;
            private Label lblElectricity;
            private Label lblTotal;
            private Button btnPay;
            private GameState gameState;

            public ConsumptionControl(GameState state)
            {
                gameState = state;
                InitUI();
                UpdateData();
            }
        public void OnDayChanged()
        {
            UpdateData();
        }

        private void InitUI()
            {
                this.Dock = DockStyle.Fill;
                this.BackColor = Color.FromArgb(40, 40, 45);

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 4,
                    ColumnCount = 1
                };

                lblWater = new Label { Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
                lblElectricity = new Label { Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
                lblTotal = new Label { Dock = DockStyle.Fill, ForeColor = Color.Yellow, Font = new Font("Segoe UI", 12, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };

                btnPay = new Button
                {
                    Text = "💳 Оплатить тарифы",
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    BackColor = Color.DarkSlateBlue,
                    ForeColor = Color.White
                };
                btnPay.Click += BtnPay_Click;

                layout.Controls.Add(lblWater, 0, 0);
                layout.Controls.Add(lblElectricity, 0, 1);
                layout.Controls.Add(lblTotal, 0, 2);
                layout.Controls.Add(btnPay, 0, 3);

                this.Controls.Add(layout);
            }

            private void BtnPay_Click(object sender, EventArgs e)
            {
                gameState.PayUtilities();   // списываем деньги и обнуляем расход
                UpdateData();               // обновляем панель потребления

                // 👉 обновляем баланс в главной форме
                if (this.ParentForm is Mine1 mainForm)
                {
                    mainForm.UpdateBalanceLabel();
                }
            }

            public void UpdateData()
            {
                double waterCost = (gameState.WaterConsumptionLiters / 1000.0) * gameState.WaterTariff;
                double electricityCost = gameState.ElectricityConsumptionKwh * gameState.ElectricityTariff;
                double total = waterCost + electricityCost;

                lblWater.Text = $"💧 Вода: {gameState.WaterConsumptionLiters:F0} л | {waterCost:F2} грн";
                lblElectricity.Text = $"⚡ Электричество: {gameState.ElectricityConsumptionKwh:F0} кВт·ч | {electricityCost:F2} грн";
                lblTotal.Text = $"Итого к оплате: {total:F2} грн";
            }
        }
    }
