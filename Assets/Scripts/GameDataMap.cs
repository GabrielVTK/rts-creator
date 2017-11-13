using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameDataMap {

	public string map;
	public string mapTile;
	public string mapComponent;

	private static string dataPath = Application.dataPath + "/StreamingAssets/";

	public GameDataMap() {}

	public static void Save(GameDataMap gameDataMap, string mapName) {

        string[] ext = mapName.Split('.');

        if(ext.Length == 1 || !ext[ext.Length - 1].ToLower().Equals("map")) {
            mapName += ".map";
        }

		File.WriteAllText(dataPath + mapName, JsonUtility.ToJson(gameDataMap));
	}
    
    public static GameDataMap Load(string mapName) {

        string[] ext = mapName.Split('.');

        if(ext.Length == 1 || !ext[ext.Length - 1].ToLower().Equals("map")) {
            mapName += ".map";
        }

        return JsonUtility.FromJson<GameDataMap>(File.ReadAllText(dataPath + mapName));
    }

}
