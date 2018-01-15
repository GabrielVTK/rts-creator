using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TorneioTabelaLiga : MonoBehaviour {

    //public static IndividuoAG[] populacao;

    public static int rodada;

    public TorneioTabelaLiga() {
        TorneioTabela.rodada = 0;
        
        foreach(IndividuoAG ind in AG.populacao) {
            ind.getOponentes().Clear();
        }
    }
    
    //Divide jogadas através dos jogadores com maior ponto não visitado e segundo maior ponto não visitado
    public IEnumerator DivideJogadas(int rounds) {

        Debug.Log("DivideJogadas()");

        int round, i;
        int[] visitados;
        bool sair;

        List<ScriptAttributes> scriptsP1, scriptsP2;
        Dictionary<ScriptAttributes, int> jogadores;

        for (round = 0; round < rounds; round++) {
            
            sair = false;

            //cria array para controlar os jogadores que ja jogaram nessa rodada
            visitados = new int[AG.populacao.Length];
        
            //inicializa array
            visitados = reiniciaVisitados(visitados);

            scriptsP1 = new List<ScriptAttributes>();
            scriptsP2 = new List<ScriptAttributes>();
            
            jogadores = new Dictionary<ScriptAttributes, int>();
            
            int posMaior1, posMaior2;

            do {
                posMaior1 = -1;
                posMaior2 = -1;
            
                //busca jogador não visitado e guarda posição
                for (i = 0; i < AG.populacao.Length; i++) {
                    if(visitados[i] == 0) {
                        posMaior1 = i;
                    }
                }

                visitados[posMaior1] = 1;

                //busca segundo jogador não visitado e que ainda não batalhou contra o jogador 1 e guarda posicao
                for (i = 0; i < AG.populacao.Length; i++) {
                    if(visitados[i] == 0 && (!AG.populacao[posMaior1].getOponentes().Contains(i))) {
                        posMaior2 = i;
                    }
                }


                if (posMaior2 == -1) {
                    Debug.Log("Jogador dois não encontrado!");
                    /**
                    for (i = 0; i < AG.populacao.Length; i++) {                    
                        if (visitados[i] == 0 ) {
                            posMaior2 = i;
                        }
                    }
                    /**/
                }
                visitados[posMaior2] = 1;

                //Define jogadores como oponentes um do outro
                AG.populacao[posMaior1].getOponentes().Add(posMaior2);
                AG.populacao[posMaior2].getOponentes().Add(posMaior1);

                Debug.Log("Individuo " + posMaior1 + " vs Individuo " + posMaior2);

                // !---------------------------------------------!
                // Adiciona os jogadores nas listas
                ScriptAttributes scriptAttributesp1 = new ScriptAttributes();
                scriptAttributesp1.AddAttribute("EARLYMEAT", AG.populacao[posMaior1].getCromossomos()[0]);      // % de comida inicio de jogo
                scriptAttributesp1.AddAttribute("EARLYGOLD", AG.populacao[posMaior1].getCromossomos()[1]);      // % de ouro inicio de jogo
                scriptAttributesp1.AddAttribute("WORKERS", AG.populacao[posMaior1].getCromossomos()[2]);        // Qtd. de trabalhadores
                scriptAttributesp1.AddAttribute("MATERIALPERMIN", AG.populacao[posMaior1].getCromossomos()[3]); // Qtd. de recurso/min para mudar estado para meio de jogo
                scriptAttributesp1.AddAttribute("MIDMEAT", AG.populacao[posMaior1].getCromossomos()[4]);        // % de comida meio de jogo
                scriptAttributesp1.AddAttribute("MIDGOLD", AG.populacao[posMaior1].getCromossomos()[5]);        // % de ouro meio de jogo
                scriptAttributesp1.AddAttribute("INFANTRY", AG.populacao[posMaior1].getCromossomos()[6]);       // % de infantaria
                scriptAttributesp1.AddAttribute("ARCHER", AG.populacao[posMaior1].getCromossomos()[7]);         // % de arqueiro
                scriptAttributesp1.AddAttribute("CAVALRY", AG.populacao[posMaior1].getCromossomos()[8]);        // % de cavalaria
                scriptAttributesp1.AddAttribute("TROOP", AG.populacao[posMaior1].getCromossomos()[9]);          // Qtd. de tropas antes de inciar ataque

                ScriptAttributes scriptAttributesp2 = new ScriptAttributes();
                scriptAttributesp2.AddAttribute("EARLYMEAT", AG.populacao[posMaior2].getCromossomos()[0]);      // % de comida inicio de jogo
                scriptAttributesp2.AddAttribute("EARLYGOLD", AG.populacao[posMaior2].getCromossomos()[1]);      // % de ouro inicio de jogo
                scriptAttributesp2.AddAttribute("WORKERS", AG.populacao[posMaior2].getCromossomos()[2]);        // Qtd. de trabalhadores
                scriptAttributesp2.AddAttribute("MATERIALPERMIN", AG.populacao[posMaior2].getCromossomos()[3]); // Qtd. de recurso/min para mudar estado para meio de jogo
                scriptAttributesp2.AddAttribute("MIDMEAT", AG.populacao[posMaior2].getCromossomos()[4]);        // % de comida meio de jogo
                scriptAttributesp2.AddAttribute("MIDGOLD", AG.populacao[posMaior2].getCromossomos()[5]);        // % de ouro meio de jogo
                scriptAttributesp2.AddAttribute("INFANTRY", AG.populacao[posMaior2].getCromossomos()[6]);       // % de infantaria
                scriptAttributesp2.AddAttribute("ARCHER", AG.populacao[posMaior2].getCromossomos()[7]);         // % de arqueiro
                scriptAttributesp2.AddAttribute("CAVALRY", AG.populacao[posMaior2].getCromossomos()[8]);        // % de cavalaria
                scriptAttributesp2.AddAttribute("TROOP", AG.populacao[posMaior2].getCromossomos()[9]);          // Qtd. de tropas antes de inciar ataque

                scriptsP1.Add(scriptAttributesp1);
                scriptsP2.Add(scriptAttributesp2);

                jogadores.Add(scriptAttributesp1, posMaior1);
                jogadores.Add(scriptAttributesp2, posMaior2);

                // !---------------------------------------------!

                //se todos os jogadores jogaram nessa rodada, sai da partida
                sair = todosVisitados(visitados);
            
            } while (!sair);

            GameInitializer.p1ScriptAttributes = scriptsP1;
            GameInitializer.p2ScriptAttributes = scriptsP2;


            Debug.Log("Carrega gameInitializer. Jogo: " + round);
            SceneManager.LoadScene("gameInitializer", LoadSceneMode.Additive);

            Scene gameInitializer = SceneManager.GetSceneByName("gameInitializer");

            while (!gameInitializer.isLoaded) {
                yield return null;
            }

            SceneManager.SetActiveScene(gameInitializer);
            
            while (SceneManager.GetSceneByName("gameInitializer").IsValid()) {
                //Debug.Log("Aguarda finalinar o gameInitializer");
                yield return null;
            }
            
            Debug.Log("Fim do round " + round);

            Log log = new Log();

            int[] wins = log.GetWins();
            int indice;

            for (i = 0; i < wins.Length; i++) {
                
                indice = -1;

                if(wins[i] == 0) {
                    indice = jogadores[scriptsP1[i]];
                    Debug.Log("Player 1 ganhou ponto. (" + indice + ")");
                } else {
                    indice = jogadores[scriptsP2[i]];
                    Debug.Log("Player 2 ganhou ponto. (" + indice + ")");
                }

                if(indice != -1) {
                    Debug.Log("Individuo " + indice + ": " + AG.populacao[indice].getVitorias());
                    AG.populacao[indice].setVitorias(AG.populacao[indice].getVitorias() + 1);
                    Debug.Log("Individuo " + indice + ": " + AG.populacao[indice].getVitorias());
                } else {
                    break;
                }               
            }
            
            TorneioTabela.rodada++;

            //Ao final de todas as partidas, guarda as posicoes
            if (TorneioTabela.rodada == rounds) {
                AG.divideJogagas = false;
            }

            calculaPontuacaoTotal();

            System.GC.Collect();
            System.Threading.Thread.Sleep(3000);
        }

    }
    
    //Testa se todos os jogadores ja jogaram essa rodada
    bool todosVisitados(int[] visitados) {

        int i;

        for (i = 0; i < visitados.Length; i++) {
            if (visitados[i] == 0) {
                return false;
            }
		}

        return true;
	}

    //Inicializa array de visitado para controlar se todos os jogadores jogam a partida
    int[] reiniciaVisitados(int[] visitados) {

        int i;

        for(i = 0; i < visitados.Length; i++){
			visitados[i] = 0;
		}

		return visitados;
	}

    //inicializa array de pontuacao e vitorias
    public void iniciaPontuacao() {

        int i;

        for(i = 0; i < AG.populacao.Length; i++) {
            AG.populacao[i].setPontuacao(0f);
            AG.populacao[i].setVitorias(0);
        }

    }

    //Soma (total de vitoria dos jogadores  * 3 ) + (Cada vitoria dos oponentes do jogadores * 0,3)
    void calculaPontuacaoTotal() {

        int i;
        float totalPontos;

        for (i = 0; i < AG.populacao.Length; i++) {

            //quantidade de vitorias do jogador *3
            totalPontos = (AG.populacao[i].getVitorias() * 3.0f);
            
            foreach(int j in AG.populacao[i].getOponentes()) {
                //Quantidade de vitoria dos oponentes do jogador * 0.3
                totalPontos = totalPontos + (AG.populacao[j].getVitorias() * 0.3f);                
            }

            AG.populacao[i].setPontuacao(totalPontos);
        }

    }
    
}
