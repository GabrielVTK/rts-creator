using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameDevWare.Serialization;

public class GameInitializer : MonoBehaviour {

    public static Script script;
    public static List<ScriptAttributes> p1ScriptAttributes;
    public static List<ScriptAttributes> p2ScriptAttributes;
    public static int rountCount = 0;
    public static bool incrementGameCount = true;
    public int rounds;
    
    void Start () {
        StartCoroutine(this.RunGame());
    }
        
    public IEnumerator RunGame() {
        
        string path = Application.dataPath + "/StreamingAssets/game" + PlayerPrefs.GetInt("GameCount");

        int i;
        int[] wins = new int[GameInitializer.p1ScriptAttributes.Count];
        for (i = 0; i < GameInitializer.p1ScriptAttributes.Count; i++) {
            wins[i] = -1;
        }

        File.WriteAllText(path + "/geracao" + AG.numGeracao + "_rodada" + TorneioTabela.rodada + "_vitorias.json", Json.SerializeToString<int[]>(wins));

        Scene game;

        for (i = 0; i < GameInitializer.p1ScriptAttributes.Count; i++) {

            GameInitializer.rountCount = i;
            
            GameController.scriptP1 = p1ScriptAttributes[i];
            GameController.scriptP2 = p2ScriptAttributes[i];
            SceneManager.LoadScene("game", LoadSceneMode.Additive);

            game = SceneManager.GetSceneByName("game");
            
            while(!game.isLoaded) {
                yield return null;
            }

            SceneManager.SetActiveScene(game);
            
            while (SceneManager.GetSceneByName("game").IsValid()) {
                //Debug.Log("Aguarda finalinar o game");
                yield return null;
            }

            System.GC.Collect();
            System.Threading.Thread.Sleep(3000);
        }

        Debug.Log("Todas as partidas do round foram executadas!");
        
        SceneManager.UnloadScene(SceneManager.GetSceneByName("gameInitializer"));

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("testeAg"));
    }

}
