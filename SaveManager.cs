using Miner;
using Newtonsoft.Json;
using System.IO;

public class SaveManager
{
    private string saveFile = "savegame.json";

    public void Save(GameState state)
    {
        var json = JsonConvert.SerializeObject(state, Formatting.Indented);
        File.WriteAllText(saveFile, json);
    }

    public GameState Load()
    {
        if (!File.Exists(saveFile))
            return null;

        var json = File.ReadAllText(saveFile);
        return JsonConvert.DeserializeObject<GameState>(json);
    }
}
