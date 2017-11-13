using System.IO;
using UnityEngine;
using System.Collections.Generic;
using GameDevWare.Serialization;

public class Log {

    public float time;
    public ulong ciclos;

    public string winner;
    public string loser;
    public ScriptAttributes paramWinner;
    public int units;
    public int resources;
    public int ordersAttack;

    private int[] wins;

    private static string dataPath = Application.dataPath + "/StreamingAssets/game" + PlayerPrefs.GetInt("GameCount");
    
    public Log() {
        this.wins = Json.Deserialize<int[]>(File.ReadAllText(dataPath + "/geracao" + AG.numGeracao + "_rodada" + TorneioTabela.rodada + "_vitorias.json"));
    }

    public void Save() {

        if(!Directory.Exists(dataPath)) {
            Directory.CreateDirectory(dataPath);
        }

        File.WriteAllText(dataPath + "/geracao" + AG.numGeracao + "_rodada" + TorneioTabela.rodada + "_vitorias.json", Json.SerializeToString<int[]>(this.wins));
        File.WriteAllText(dataPath + "/geracao" + AG.numGeracao + "_rodada" + TorneioTabela.rodada + "_partida" + GameInitializer.rountCount + ".json", Json.SerializeToString<Log>(this));
        
    }

    public void AddWinner(Player player) {
        this.wins[GameInitializer.rountCount] = player.id;
        this.winner = player.name;
    }

    public void AddLoser(Player player)
    {
        this.loser = player.name;
    }

    public void AddParamWinner(ScriptAttributes script)
    {
        this.paramWinner = script;
    }

    public int[] GetWins() {
        return this.wins;
    }

}
