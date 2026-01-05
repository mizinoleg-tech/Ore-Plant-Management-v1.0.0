using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Miner
{
    partial class Mine1
    {
        private System.ComponentModel.IContainer components = null;

        private SplitContainer splitContainer1;
        private TreeView treeView1;
        private Panel panel1;
        private FlowLayoutPanel bottomPanel;
        private Panel topPanel;
        private Button BtnNextDay;
        private Button btnSellOre;
        private Label lblBalance;
        private Label lblDate;
        

       

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSellOre = new System.Windows.Forms.Button();
            this.BtnNextDay = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnSaveProgress = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblBalance = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 52);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Silver;
            this.splitContainer1.Panel1.Controls.Add(this.btnSellOre);
            this.splitContainer1.Panel1.Controls.Add(this.BtnNextDay);
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Silver;
            this.splitContainer1.Panel2.Controls.Add(this.btnSaveProgress);
            this.splitContainer1.Panel2.Controls.Add(this.lblDate);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(900, 548);
            this.splitContainer1.SplitterDistance = 407;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnSellOre
            // 
            this.btnSellOre.BackColor = System.Drawing.Color.Orange;
            this.btnSellOre.FlatAppearance.BorderSize = 0;
            this.btnSellOre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSellOre.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSellOre.ForeColor = System.Drawing.Color.Black;
            this.btnSellOre.Location = new System.Drawing.Point(15, 477);
            this.btnSellOre.Name = "btnSellOre";
            this.btnSellOre.Size = new System.Drawing.Size(182, 40);
            this.btnSellOre.TabIndex = 1;
            this.btnSellOre.Text = "💰 Продать руду";
            this.btnSellOre.UseVisualStyleBackColor = false;
            this.btnSellOre.Click += new System.EventHandler(this.BtnSellOre_Click);
            // 
            // BtnNextDay
            // 
            this.BtnNextDay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnNextDay.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.BtnNextDay.FlatAppearance.BorderSize = 0;
            this.BtnNextDay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNextDay.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.BtnNextDay.ForeColor = System.Drawing.Color.Black;
            this.BtnNextDay.Location = new System.Drawing.Point(215, 477);
            this.BtnNextDay.Name = "BtnNextDay";
            this.BtnNextDay.Size = new System.Drawing.Size(173, 40);
            this.BtnNextDay.TabIndex = 0;
            this.BtnNextDay.Text = "⏩ Следующий день";
            this.BtnNextDay.UseVisualStyleBackColor = false;
            this.BtnNextDay.Click += new System.EventHandler(this.BtnNextDay_Click1);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.treeView1.BackColor = System.Drawing.Color.IndianRed;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(407, 454);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterSelect);
            // 
            // btnSaveProgress
            // 
            this.btnSaveProgress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSaveProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveProgress.Location = new System.Drawing.Point(306, 497);
            this.btnSaveProgress.Name = "btnSaveProgress";
            this.btnSaveProgress.Size = new System.Drawing.Size(171, 23);
            this.btnSaveProgress.TabIndex = 2;
            this.btnSaveProgress.Text = "button1";
            this.btnSaveProgress.UseVisualStyleBackColor = true;
            this.btnSaveProgress.Click += new System.EventHandler(this.btnSaveProgress_Click);
            // 
            // lblDate
            // 
            this.lblDate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDate.AutoSize = true;
            this.lblDate.BackColor = System.Drawing.Color.Silver;
            this.lblDate.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.Black;
            this.lblDate.Location = new System.Drawing.Point(12, 496);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(122, 23);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "2.12.1986";
            this.lblDate.Click += new System.EventHandler(this.lblDate_Click_2);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 454);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // bottomPanel
            // 
            this.bottomPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bottomPanel.BackColor = System.Drawing.Color.Gainsboro;
            this.bottomPanel.Location = new System.Drawing.Point(0, 506);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(20);
            this.bottomPanel.Size = new System.Drawing.Size(900, 94);
            this.bottomPanel.TabIndex = 1;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.topPanel.Controls.Add(this.lblBalance);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(900, 52);
            this.topPanel.TabIndex = 2;
            // 
            // lblBalance
            // 
            this.lblBalance.AutoSize = true;
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblBalance.ForeColor = System.Drawing.Color.Black;
            this.lblBalance.Location = new System.Drawing.Point(10, 15);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(134, 25);
            this.lblBalance.TabIndex = 0;
            this.lblBalance.Text = "Баланс: 0 грн";
            // 
            // Mine1
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Name = "Mine1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "⛏ Управление комбинатом";
            this.Load += new System.EventHandler(this.LoadMine1);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnSaveProgress;
    }
}
