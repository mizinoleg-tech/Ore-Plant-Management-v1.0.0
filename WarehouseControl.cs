using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public class WarehouseControl : UserControl
    {
        private DataGridView dgv;
        private Label lblSummary;
        private WarehouseItem item;   // один объект склада
        private GameState gameState;  // ссылка на состояние игры

        public WarehouseItem RawOre { get; private set; }

        // Конструктор: только GameState
        public WarehouseControl(GameState state)
        {
            gameState = state ?? throw new ArgumentNullException(nameof(state));
            item = state.RawOre ?? new WarehouseItem("Сырая руда", 0, 0.0); // ✅ присваиваем item
            InitUI();
            LoadData();
            gameState.DayChanged += OnDayChanged;
        }


        // Конструктор: GameState + WarehouseItem
        public WarehouseControl(GameState state, WarehouseItem rawOre)
        {
            gameState = state ?? throw new ArgumentNullException(nameof(state));
            RawOre = rawOre ?? state.RawOre ?? new WarehouseItem("Сырая руда", 0, 0.0);
            InitUI();
            LoadData();
            gameState.DayChanged += OnDayChanged;
        }

        // Перегрузка с обратным порядком аргументов
        public WarehouseControl(WarehouseItem rawOre, GameState state)
            : this(state, rawOre) { }

        public WarehouseControl(WarehouseItem rawOre)
        {
            RawOre = rawOre;
        }

        private double GetOreQuantityFromState(GameState state)
        {
            return state.RawOre.Quantity; // заменил Count на Quantity
        }

        private void InitUI()
        {
            this.BackColor = Color.FromArgb(40, 40, 45);
            this.Padding = new Padding(10);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 85));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

            // === Таблица склада ===
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(45, 45, 48)
            };

            dgv.Columns.Add("Name", "📦 Ресурс");
            dgv.Columns.Add("Quantity", "⚖️ Количество");
            dgv.Columns.Add("Price", "💰 Цена за тонну");
            dgv.Columns.Add("Total", "💵 Общая стоимость");

            dgv.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 65);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkSlateBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 35);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            // === Сводка ===
            lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 55)
            };

            layout.Controls.Add(dgv, 0, 0);
            layout.Controls.Add(lblSummary, 0, 1);

            this.Controls.Add(layout);
        }

        private void LoadData()
        {
            if (item == null)
            {
                lblSummary.Text = "Ошибка: склад не инициализирован.";
                return;
            }

            dgv.Rows.Clear();
            dgv.Rows.Add(
                item.Name,
                $"{item.Quantity:F2}",           // количество
                $"{item.PricePerTon:F2} грн",    // цена
                $"{item.TotalValue():F2} грн"    // общая стоимость
            );

            lblSummary.Text = $"На складе: {item.Quantity:F2} тонн | " +
                              $"Цена: {item.PricePerTon:F2} грн/т | " +
                              $"Общая стоимость: {item.TotalValue():F2} грн";
        }


        private void UpdateSummary()
        {
            lblSummary.Text = $"Всего: {item.Quantity:F2} тонн | Общая стоимость: {item.TotalValue():F2} грн";
        }

        public WarehouseItem GetItem()
        {
            return item;
        }

        // 👉 метод обновления склада
        public void UpdateData(WarehouseItem item)
        {
            item.Quantity = gameState.RawOre.Quantity;
            item.PricePerTon = gameState.RawOre.PricePerTon; // 👉 обновляем цену

            // если RawOre — список, то суммируем
            //double totalOre = gameState.RawOre.Sum(r => r.Quantity);
            // item.Quantity = totalOre;

            LoadData();
        }


        // 👉 обработчик события смены дня
        public void OnDayChanged()
        {
            UpdateData(GetItem());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && gameState != null)
            {
                gameState.DayChanged -= OnDayChanged;
            }
            base.Dispose(disposing);
        }
    }
}