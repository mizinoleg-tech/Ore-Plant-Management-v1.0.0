using Mine;
using Miner;
using Miner.Miner;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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


