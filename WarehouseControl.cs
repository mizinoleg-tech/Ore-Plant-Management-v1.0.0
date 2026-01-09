using Miner;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            RowCount = 3,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));       // таблица
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));      // панель кнопок фиксированно
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));       // сводка

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

        // === Панель кнопок ===
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(50, 50, 55)
        };

        var btnBuildWarehouse = new Button
        {
            Text = "🏗 Построить склады (650,000 грн)",
            Width = 280,
            Height = 40,
            BackColor = Color.SaddleBrown,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnBuildWarehouse.FlatAppearance.BorderSize = 0;
        btnBuildWarehouse.Click += BtnBuildWarehouse_Click;

        buttonPanel.Controls.Add(btnBuildWarehouse);

        // === Сводка ===
        lblSummary = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 55)
        };

        // === Добавляем всё в макет ===
        layout.Controls.Add(dgv, 0, 0);
        layout.Controls.Add(buttonPanel, 0, 1);
        layout.Controls.Add(lblSummary, 0, 2);

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
    private void BtnBuildWarehouse_Click(object sender, EventArgs e)
    {
        var game = ((Mine1)FindForm()).gameState;
        if (game.BuildWarehouse())
        {
            MessageBox.Show("Склады построены! Вместимость увеличена.",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ((Mine1)FindForm()).UpdateBalanceLabel();
            UpdateSummary(); // если есть сводка по складу
        }
        else
        {
            MessageBox.Show("Недостаточно средств для постройки!",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // WarehouseControl
            // 
            this.Name = "WarehouseControl";
            this.Load += new System.EventHandler(this.WarehouseControl_Load);
            this.ResumeLayout(false);

    }

    private void WarehouseControl_Load(object sender, EventArgs e)
    {

    }
}

