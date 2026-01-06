using System;
using System.Drawing;
using System.Windows.Forms;

namespace Miner
{
    public partial class TaxControl : UserControl
    {
        private readonly GameState _gameState;

        private Label lblTaxStatus;
        private Label lblMonthProfit;
        private Label lblTaxAmount;
        private Label lblBalance;
        private Button btnPayTax;
        private Panel panelTax;

        public TaxControl(GameState gameState)
        {
            _gameState = gameState;
            InitializeComponent();
        }

        // Публичный метод для UiUpdater
        public void UpdateData()
        {
            UpdateTaxInfo();
            UpdateBalanceLabel();
        }

        public void UpdateTaxInfo()
        {
            lblTaxStatus.Text = _gameState.TaxPaid ? "Налог: оплачен" : "Налог: ожидает оплаты";
            lblMonthProfit.Text = $"Прибыль за месяц: {_gameState.CurrentMonthProfit:N0} грн";
            lblTaxAmount.Text = $"Сумма налога: {(int)Math.Round(_gameState.CurrentMonthProfit * 0.25):N0} грн";
        }

        private void UpdateBalanceLabel()
        {
            lblBalance.Text = $"Баланс: {_gameState.Balance:N0} грн";
        }

        private void btnPayTax_Click(object sender, EventArgs e)
        {
            if (_gameState.PayTax())
            {
                MessageBox.Show("Налог оплачен. Можно продолжать игру.",
                    "Оплата налога", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Недостаточно средств для оплаты налога!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateData(); // обновляем UI после оплаты
        }

        private void InitializeComponent()
        {
            this.lblTaxStatus = new System.Windows.Forms.Label();
            this.lblMonthProfit = new System.Windows.Forms.Label();
            this.lblTaxAmount = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.btnPayTax = new System.Windows.Forms.Button();
            this.panelTax = new System.Windows.Forms.Panel();
            this.panelTax.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTaxStatus
            // 
            this.lblTaxStatus.AutoSize = true;
            this.lblTaxStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTaxStatus.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblTaxStatus.Location = new System.Drawing.Point(20, 20);
            this.lblTaxStatus.Name = "lblTaxStatus";
            this.lblTaxStatus.Size = new System.Drawing.Size(198, 21);
            this.lblTaxStatus.TabIndex = 0;
            this.lblTaxStatus.Text = "Налог: ожидает оплаты";
            // 
            // lblMonthProfit
            // 
            this.lblMonthProfit.AutoSize = true;
            this.lblMonthProfit.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblMonthProfit.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblMonthProfit.Location = new System.Drawing.Point(20, 60);
            this.lblMonthProfit.Name = "lblMonthProfit";
            this.lblMonthProfit.Size = new System.Drawing.Size(183, 20);
            this.lblMonthProfit.TabIndex = 1;
            this.lblMonthProfit.Text = "Прибыль за месяц: 0 грн";
            // 
            // lblTaxAmount
            // 
            this.lblTaxAmount.AutoSize = true;
            this.lblTaxAmount.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTaxAmount.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTaxAmount.Location = new System.Drawing.Point(20, 90);
            this.lblTaxAmount.Name = "lblTaxAmount";
            this.lblTaxAmount.Size = new System.Drawing.Size(150, 20);
            this.lblTaxAmount.TabIndex = 2;
            this.lblTaxAmount.Text = "Сумма налога: 0 грн";
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblBalance.ForeColor = System.Drawing.Color.Gold;
            this.lblBalance.Location = new System.Drawing.Point(20, 120);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(105, 20);
            this.lblBalance.TabIndex = 3;
            this.lblBalance.Text = "Баланс: 0 грн";
            // 
            // btnPayTax
            // 
            this.btnPayTax.BackColor = System.Drawing.Color.DarkGreen;
            this.btnPayTax.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPayTax.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPayTax.ForeColor = System.Drawing.Color.White;
            this.btnPayTax.Location = new System.Drawing.Point(100, 150);
            this.btnPayTax.Name = "btnPayTax";
            this.btnPayTax.Size = new System.Drawing.Size(180, 35);
            this.btnPayTax.TabIndex = 4;
            this.btnPayTax.Text = "💸 Оплатить налог";
            this.btnPayTax.UseVisualStyleBackColor = false;
            this.btnPayTax.Click += new System.EventHandler(this.btnPayTax_Click);
            // 
            // panelTax
            // 
            this.panelTax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.panelTax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTax.Controls.Add(this.lblTaxStatus);
            this.panelTax.Controls.Add(this.lblMonthProfit);
            this.panelTax.Controls.Add(this.lblTaxAmount);
            this.panelTax.Controls.Add(this.lblBalance);
            this.panelTax.Controls.Add(this.btnPayTax);
            this.panelTax.Location = new System.Drawing.Point(10, 10);
            this.panelTax.Name = "panelTax";
            this.panelTax.Size = new System.Drawing.Size(380, 200);
            this.panelTax.TabIndex = 0;
            this.panelTax.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTax_Paint);
            // 
            // TaxControl
            // 
            this.Controls.Add(this.panelTax);
            this.Name = "TaxControl";
            this.Size = new System.Drawing.Size(400, 220);
            this.panelTax.ResumeLayout(false);
            this.panelTax.PerformLayout();
            this.ResumeLayout(false);

        }

        private void panelTax_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
