using Miner;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

public class SaveManager
{
    private readonly string saveFile;

    public SaveManager()
    {
        // сохраняем в AppData, чтобы всегда был доступный путь
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OrePlantGame");
        Directory.CreateDirectory(folder);
        saveFile = Path.Combine(folder, "savegame.json");
    }

    public void Save(GameState state)
    {
        try
        {
            var json = JsonConvert.SerializeObject(state, Formatting.Indented);
            File.WriteAllText(saveFile, json);

            // временно для проверки
            MessageBox.Show("Сохранено:\n" + json.Substring(0, Math.Min(500, json.Length)));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
        }
    }


    public GameState Load()
    {
        try
        {
            if (!File.Exists(saveFile))
            {
                MessageBox.Show("Файл сохранения не найден.");
                return null;
            }

            var json = File.ReadAllText(saveFile);

            if (string.IsNullOrWhiteSpace(json))
            {
                MessageBox.Show("Файл сохранения пустой.");
                return null;
            }

            var state = JsonConvert.DeserializeObject<GameState>(json);

            if (state == null)
            {
                MessageBox.Show("Ошибка: JSON повреждён или структура изменилась.");
                return null;
            }

            return state;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            return null;
        }
    }


}