using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public partial class TaxControl : UserControl
    {
       
        private GameState gameState;

        public TaxControl(GameState state)
        {
            InitializeComponent();
            gameState = state;
            UpdateTaxInfo();
        }

        private void UpdateTaxInfo()
        {
            lblTaxStatus.Text = gameState.TaxPaid ? "Налог: оплачен" : "Налог: ожидает оплаты";
            lblMonthProfit.Text = $"Прибыль за месяц: {gameState.CurrentMonthProfit} грн";
            lblTaxAmount.Text = $"Сумма налога: {(int)Math.Round(gameState.CurrentMonthProfit * 0.25)} грн";
        }

        

        private void btnPayTax_Click(object sender, EventArgs e)
        {
            if (gameState.PayTax())
            {
                MessageBox.Show("Налог оплачен. Можно продолжать игру.",
                    "Оплата налога", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Недостаточно средств для оплаты налога!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateTaxInfo();
        }

        private System.Windows.Forms.Label lblTaxStatus;
        private System.Windows.Forms.Label lblMonthProfit;
        private System.Windows.Forms.Label lblTaxAmount;
        private System.Windows.Forms.Button btnPayTax;
        private void InitializeComponent()
        {
            this.lblTaxStatus = new System.Windows.Forms.Label();
            this.lblMonthProfit = new System.Windows.Forms.Label();
            this.lblTaxAmount = new System.Windows.Forms.Label();
            this.btnPayTax = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTaxStatus
            // 
            this.lblTaxStatus.AutoSize = true;
            this.lblTaxStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTaxStatus.Location = new System.Drawing.Point(81, 19);
            this.lblTaxStatus.Name = "lblTaxStatus";
            this.lblTaxStatus.Size = new System.Drawing.Size(198, 21);
            this.lblTaxStatus.TabIndex = 0;
            this.lblTaxStatus.Text = "Налог: ожидает оплаты";
            // 
            // lblMonthProfit
            // 
            this.lblMonthProfit.AutoSize = true;
            this.lblMonthProfit.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblMonthProfit.Location = new System.Drawing.Point(53, 65);
            this.lblMonthProfit.Name = "lblMonthProfit";
            this.lblMonthProfit.Size = new System.Drawing.Size(183, 20);
            this.lblMonthProfit.TabIndex = 1;
            this.lblMonthProfit.Text = "Прибыль за месяц: 0 грн";
            // 
            // lblTaxAmount
            // 
            this.lblTaxAmount.AutoSize = true;
            this.lblTaxAmount.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTaxAmount.Location = new System.Drawing.Point(53, 100);
            this.lblTaxAmount.Name = "lblTaxAmount";
            this.lblTaxAmount.Size = new System.Drawing.Size(150, 20);
            this.lblTaxAmount.TabIndex = 2;
            this.lblTaxAmount.Text = "Сумма налога: 0 грн";
            // 
            // btnPayTax
            // 
            this.btnPayTax.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPayTax.Location = new System.Drawing.Point(79, 142);
            this.btnPayTax.Name = "btnPayTax";
            this.btnPayTax.Size = new System.Drawing.Size(200, 35);
            this.btnPayTax.TabIndex = 3;
            this.btnPayTax.Text = "💸 Оплатить налог";
            this.btnPayTax.UseVisualStyleBackColor = true;
            this.btnPayTax.Click += new System.EventHandler(this.btnPayTax_Click);
            // 
            // TaxControl
            // 
            this.Controls.Add(this.lblTaxStatus);
            this.Controls.Add(this.lblMonthProfit);
            this.Controls.Add(this.lblTaxAmount);
            this.Controls.Add(this.btnPayTax);
            this.Name = "TaxControl";
            this.Size = new System.Drawing.Size(400, 200);
            this.Load += new System.EventHandler(this.TaxControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void TaxControl_Load(object sender, EventArgs e)
        {

        }
    }
}