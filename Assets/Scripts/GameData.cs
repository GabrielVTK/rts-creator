using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData {

	public string tiles;
	public string baseMaterials;
	public string mapComponents;
	public string buildings;
	public string units;

	private static string dataPath = Application.dataPath + "/StreamingAssets/data.json";

	public GameData() {}

	public static void Save(GameData gameData) {
		File.WriteAllText(dataPath, JsonUtility.ToJson(gameData));
	}

	public static GameData Load() {
		return JsonUtility.FromJson<GameData>(File.ReadAllText(Application.dataPath + "/StreamingAssets/data.json"));
	}

}
