using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GameDevWare.Serialization;
using System;

public class AG : MonoBehaviour {

    public static IndividuoAG[] populacao;
    //public IndividuoAG[] Npopulacao;

    public static bool divideJogagas;
    public static bool fazCruzamento;
    public static bool proxGeracao;

    public static int numGeracao;

    private int geracaoIni;

    void Start() {
        
        String[] args = Environment.GetCommandLineArgs();

        if (!PlayerPrefs.HasKey("GameCount")) {
            PlayerPrefs.SetInt("GameCount", 0);
        }

        //PlayerPrefs.SetInt("GameCount", PlayerPrefs.GetInt("GameCount") + 1);

        Debug.Log("GameCount " + PlayerPrefs.GetInt("GameCount"));

        string path = Application.dataPath + "/StreamingAssets/game" + PlayerPrefs.GetInt("GameCount");

        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }


        if ((args.Length == 5 && args[4].Equals("production")) || args.Length == 1) {
            AG.populacao = new IndividuoAG[16];
        } else { 
            AG.populacao = new IndividuoAG[Int32.Parse(args[1])];
        }
        
        inicializaPopulacao();  //Geração 0
        
        AG.numGeracao = 0;

        this.geracaoIni = AG.numGeracao;

        Time.timeScale = 50;
        Debug.Log("TimeScale: " + Time.timeScale);

        StartCoroutine(CriaGeracoes(10000));
    }

    public IEnumerator CriaGeracoes(int numeroGeracoes) {

        //IndividuoAG[] Npopulacao = new IndividuoAG[populacao.Length];

        int i, p;

        for(i = this.geracaoIni; i < numeroGeracoes; i++) {

            this.salvarGeracao(i);

            AG.numGeracao++;
            AG.divideJogagas = true;
            AG.fazCruzamento = false;
            AG.proxGeracao = false;

            IndividuoAG[] pais = new IndividuoAG[AG.populacao.Length / 2];
            Debug.Log("selecaoPais");
            StartCoroutine(selecaoPais(pais));
            
            while (!AG.fazCruzamento) {
                yield return null;
            }

            Dictionary<int, float> aptidao = new Dictionary<int, float>();

            for(p = 0; p < AG.populacao.Length; p++) {
                aptidao.Add(p, AG.populacao[p].getPontuacao());
            }

            File.WriteAllText(Application.dataPath + "/StreamingAssets/game" + PlayerPrefs.GetInt("GameCount")  + "/geracao" + AG.numGeracao + "_resultado.json", Json.SerializeToString<Dictionary<int, float>>(aptidao));

            Debug.Log("Cruzamento");
            cruzamento(pais);

            Debug.Log("Volta do cruzamento");
            
            while (!AG.proxGeracao) {
                yield return null;
            }

            Debug.Log("ProxGeracao");
        }
        
    }

    struct Params {
        public float comidaInicio;
        public float ouroInicio;
        public float qntTrabalhadores;
        public float qntRecursosMin;
        public float comidaMeio;
        public float ouroMeio;
        public float infantaria;
        public float arqueiro;
        public float cavalaria;
        public float qntTropaParaAtaque;
    };

    //inicializa população  
    //Cromossomo
    public void inicializaPopulacao() {

        /*
           * nº Parametro  -   Nome  -  Range
             0 - % comida inicio jogo  0-100%
             1 - % ouro inicio jogo    0-100%
             2 - Quantidade de trabalhadores  4 - 100
             3 - Quantidade de recursos obtido por minuto para mudar estado de jogo  100 - 1000
             4 - % comida meio jogo  0 - 100%
             5 - % ouro meio jogo   0 - 100%
             6 - % Infantaria   0 - 100%
             7 - % Arqueiro   0 - 100%
             8 - % Cavalaria   0 - 100%
             9 - Quantidade de tropa antes de iniciar ataque   10 - 500
         */

        int i;

        Debug.Log("Tamanho população: " + AG.populacao.Length);

        adicionaIndividuos();

        for (i = 16; i < AG.populacao.Length; i++) {

            Debug.Log("Cria individuo");

            float[] cromossomo = new float[10];
   
            cromossomo[0] = UnityEngine.Random.Range(0, 101);
            cromossomo[1] = UnityEngine.Random.Range(0, 101);
            cromossomo[2] = UnityEngine.Random.Range(3, 51);
            cromossomo[3] = UnityEngine.Random.Range(100, 1001);
            cromossomo[4] = UnityEngine.Random.Range(0, 101);
            cromossomo[5] = UnityEngine.Random.Range(0, 101);
            cromossomo[6] = UnityEngine.Random.Range(0, 101);
            cromossomo[7] = UnityEngine.Random.Range(0, 101);
            cromossomo[8] = UnityEngine.Random.Range(0, 101);
            cromossomo[9] = UnityEngine.Random.Range(10, 151);

            AG.populacao[i] = new IndividuoAG();
            AG.populacao[i].setCromossomo(cromossomo);
            AG.populacao[i].setGeracao(0);
        }

        normalizaCromossomo();
    }    

    public IEnumerator selecaoPais(IndividuoAG[] pais) {

        //calcula a aptidão dos indivíduos através do torneio no formato suiço
        TorneioTabelaLiga torneio = new TorneioTabelaLiga();

        //inicializa pontuacao
        torneio.iniciaPontuacao();

        Debug.Log("StartCoroutine");
        Debug.Log("Rodadas: "+ (AG.populacao.Length - 1));
        StartCoroutine(torneio.DivideJogadas(AG.populacao.Length - 1));

        int i;
        
        while(AG.divideJogagas) {
            yield return null;
        }

        Debug.Log("After StartCoroutine");

        //após todos os jogadores terem suas pontuações do torneio suiço
        //calcula a pontuação total e "roda a roleta"

        
        float pontuacaoTotal = 0f;

        for (i = 0; i < AG.populacao.Length; i++){
            pontuacaoTotal += AG.populacao[i].getPontuacao();
        }

        IndividuoAG ind;

        Debug.Log(" -- Resultado -- ");
        for (i = 0; i < AG.populacao.Length; i++) {
            ind = AG.populacao[i];
            Debug.Log("Player " + i + " -> " + ind.getPontuacao() + " pontos.");
            Debug.Log("Inimigos");
            foreach (int op in ind.getOponentes()) {
                Debug.Log(" - " + op);
            }
        }

        //IndividuoAG[] pai = new IndividuoAG[populacao.Length/2];


        //com a pontuacao total, é sorteado um numero aleatorio e identifica qual indivíduo que está na posição do numero sorteado;  
        //Roleta beaseada no link
        //https://sofiaia.wordpress.com/2008/07/15/algoritmos-geneticos-parte-3/

        int j, numeroSorteado;
        float somaPontos;

        for (i = 0; i < (AG.populacao.Length/2); i++) {
            numeroSorteado = UnityEngine.Random.Range(0, (int) pontuacaoTotal + 1);
            somaPontos = 0f;
            
            for(j = 0; j < AG.populacao.Length; j++) {
                somaPontos += AG.populacao[j].getPontuacao();
                if (somaPontos >= numeroSorteado) {
                    pais[i] = AG.populacao[j];
                    break;
                }
            }
            
        }

        AG.fazCruzamento = true;

    }
    
    public void cruzamento(IndividuoAG[] pais) {
        
        // Cruzamento com média ponderada usando peso aleatório
        // http://www2.peq.coppe.ufrj.br/Pessoal/Professores/Arge/COQ897/Naturais/aulas_piloto/aula4.pdf

        int geracaoAtual = pais[0].getGeracao(), count = 0, par, i;
        float mediaPonderada;
        float[] filho1, filho2;

        IndividuoAG[] novaGeracao = new IndividuoAG[pais.Length*2];
        do {

            par = UnityEngine.Random.Range(0, pais.Length);

            if (par != count) {
                mediaPonderada = UnityEngine.Random.Range(1, 100) / 100f;
                filho1 = new float[pais[0].getCromossomos().Length];
                filho2 = new float[pais[0].getCromossomos().Length];

                for (i = 0; i < pais[0].getCromossomos().Length; i++) {
                    filho1[i] = ( (1-mediaPonderada) * pais[count].getCromossomos()[i]) + (mediaPonderada * pais[par].getCromossomos()[i]);
                    filho2[i] = ( mediaPonderada  * pais[count].getCromossomos()[i]) + ((1 - mediaPonderada) * pais[par].getCromossomos()[i]);
                }

                //cria filho 1 
                novaGeracao[(count * 2)] = new IndividuoAG();
                novaGeracao[(count * 2)].setGeracao(geracaoAtual + 1);
                novaGeracao[(count * 2)].setCromossomo(filho1);

                //e filho 2
                novaGeracao[(count * 2) + 1] = new IndividuoAG();
                novaGeracao[(count * 2) + 1].setGeracao(geracaoAtual + 1);
                novaGeracao[(count * 2) + 1].setCromossomo(filho2);
                count++;

            }

        } while (count < pais.Length);

        AG.populacao = novaGeracao;

        Debug.Log("Proxima Geração = true");
        AG.proxGeracao = true;

        mutacao();
    }
    
    public void mutacao() {

        /*
            Mutação Indutiva (somente para codificação usando números reais)
            Semelhante à mutação aleatória, só que ao invés de sortear um novo valor para
            o gene, sorteia-se um valor a ser somado ao valor corrente do gene         
         */

        bool teste = false;
        int i, cromossomoEscolhido, valor;
        float[] cromossomo;

        for(i = 0; i < AG.populacao.Length; i++) {

            for(int g = 0; g < 10; g++) {

                //   Chance de aplicar mutação de 2% para cada indivíduo da população
                if (UnityEngine.Random.Range(0, 101) < 3) {

                    teste = true;

                    //Mutação ocorrerá de forma a alterar o gene aleatório do cromossomo escolhido
                    cromossomo = AG.populacao[i].getCromossomos();
                    cromossomoEscolhido = g;

                    //Aplicando um valor maior ou menor de -10 a 10  (caso negativo transforma o valor em zero)

                    switch (cromossomoEscolhido) {
                        case 2:
                            valor = UnityEngine.Random.Range(3, 51);
                            break;
                        case 3:
                            valor = UnityEngine.Random.Range(100, 1001);
                            break;
                        case 9:
                            valor = UnityEngine.Random.Range(10, 151);
                            break;
                        default:
                            valor = UnityEngine.Random.Range(0, 101);
                            break;
                    }

                    cromossomo[cromossomoEscolhido] = valor;
                    //atualiza população
                    AG.populacao[i].setCromossomo(cromossomo);
                }

            }
            
        }

        //se houve mutação, então normaliza novamente os valores;
        if (teste) {
            normalizaCromossomo();
        }
    }

    //Função para normalizar os dados do cromossomo, usada ao inicializar a população onde os dados são aleatórios e após os crusamentos
    void normalizaCromossomo() {
        int i;
        float valorBase;
        float[] cromossomo;

        for (i = 0; i < AG.populacao.Length; i++) {

            cromossomo = AG.populacao[i].getCromossomos();
            /*
               Soma do parametro 0 e 1 tem que dar 100%    (Comida e ouro inicial)
            */
            valorBase = 100f / (AG.populacao[i].getCromossomos()[0] + AG.populacao[i].getCromossomos()[1]);
            cromossomo[0] = AG.populacao[i].getCromossomos()[0] * valorBase;
            cromossomo[1] = AG.populacao[i].getCromossomos()[1] * valorBase;

           /*
              Soma do parametro 4 e 5 tem que dar 100%    (Comida e ouro meio jogo)
           */
            valorBase = 100f / (AG.populacao[i].getCromossomos()[4] + AG.populacao[i].getCromossomos()[5]);
            cromossomo[4] = AG.populacao[i].getCromossomos()[4] * valorBase;
            cromossomo[5] = AG.populacao[i].getCromossomos()[5] * valorBase;

            /*
             Soma do parametro 6 ,7 e 8 tem que dar 100%    (Infantaria , Arqueiros e Cavaleiros)
            */
            valorBase = 100f / (AG.populacao[i].getCromossomos()[6] + AG.populacao[i].getCromossomos()[7] + AG.populacao[i].getCromossomos()[8]);
            cromossomo[6] = AG.populacao[i].getCromossomos()[6] * valorBase;
            cromossomo[7] = AG.populacao[i].getCromossomos()[7] * valorBase;
            cromossomo[8] = AG.populacao[i].getCromossomos()[8] * valorBase;

            //Atualizando cromossomo da populacao normalizado
            AG.populacao[i].setCromossomo(cromossomo);
        }
    }
    
    private void salvarGeracao(int geracao) {
        Params[] popParams = new Params[AG.populacao.Length];
        int i;

        for(i = 0; i < AG.populacao.Length; i++) {
            popParams[i].comidaInicio = AG.populacao[i].getCromossomos()[0];
            popParams[i].ouroInicio = AG.populacao[i].getCromossomos()[1];
            popParams[i].qntTrabalhadores = AG.populacao[i].getCromossomos()[2];
            popParams[i].qntRecursosMin = AG.populacao[i].getCromossomos()[3];
            popParams[i].comidaMeio = AG.populacao[i].getCromossomos()[4];
            popParams[i].ouroMeio = AG.populacao[i].getCromossomos()[5];
            popParams[i].infantaria = AG.populacao[i].getCromossomos()[6];
            popParams[i].arqueiro = AG.populacao[i].getCromossomos()[7];
            popParams[i].cavalaria = AG.populacao[i].getCromossomos()[8];
            popParams[i].qntTropaParaAtaque = AG.populacao[i].getCromossomos()[9];
        }

        File.WriteAllText(Application.dataPath + "/StreamingAssets/game" + PlayerPrefs.GetInt("GameCount") + "/geracao" + geracao + ".json", Json.SerializeToString<Params[]>(popParams));
    }

    private void adicionaIndividuos() {
        float[] cromossomo;

        //População 16 => Geração 313
            // Individuo 0 (15.2999992)
            cromossomo = new float[10];
            cromossomo[0] = 73.00305f;
            cromossomo[1] = 26.9969521f;
            cromossomo[2] = 31.2349644f;
            cromossomo[3] = 707.428833f;
            cromossomo[4] = 52.9034042f;
            cromossomo[5] = 47.0965958f;
            cromossomo[6] = 47.9906158f;
            cromossomo[7] = 7.0988636f;
            cromossomo[8] = 44.91052f;
            cromossomo[9] = 40.849575f;
            AG.populacao[0] = new IndividuoAG();
            AG.populacao[0].descricao = "Individuo 0 da Geração 313 - População 16 (3% mutação)";
            AG.populacao[0].setCromossomo(cromossomo);
            AG.populacao[0].setGeracao(0);

            // Individuo 1 (12.000001)
            cromossomo = new float[10];
            cromossomo[0] = 73.27227f;
            cromossomo[1] = 26.7277317f;
            cromossomo[2] = 31.2349644f;
            cromossomo[3] = 707.428833f;
            cromossomo[4] = 52.9034042f;
            cromossomo[5] = 47.0965958f;
            cromossomo[6] = 47.990593f;
            cromossomo[7] = 7.09891033f;
            cromossomo[8] = 44.9104958f;
            cromossomo[9] = 40.849575f;
            AG.populacao[1] = new IndividuoAG();
            AG.populacao[1].descricao = "Individuo 1 da Geração 313 - População 16 (3% mutação)";
            AG.populacao[1].setCromossomo(cromossomo);
            AG.populacao[1].setGeracao(0);

        //População 16(6 % mutação) => Geração 292
            // Individuo 15 (15.2999992)
            cromossomo = new float[10];
            cromossomo[0] = 41.12878f;
            cromossomo[1] = 58.87122f;
            cromossomo[2] = 31.6382656f;
            cromossomo[3] = 548.054f;
            cromossomo[4] = 73.21813f;
            cromossomo[5] = 26.7818737f;
            cromossomo[6] = 24.905159f;
            cromossomo[7] = 34.2094f;
            cromossomo[8] = 40.88544f;
            cromossomo[9] = 91.67584f;
            AG.populacao[2] = new IndividuoAG();
            AG.populacao[2].descricao = "Individuo 15 da Geração 292 - População 16 (6% mutação)";
            AG.populacao[2].setCromossomo(cromossomo);
            AG.populacao[2].setGeracao(0);

            // Individuo 11 (12.3)
            cromossomo = new float[10];
            cromossomo[0] = 41.1283035f;
            cromossomo[1] = 58.8716965f;
            cromossomo[2] = 31.6382656f;
            cromossomo[3] = 547.9157f;
            cromossomo[4] = 73.21807f;
            cromossomo[5] = 26.7819328f;
            cromossomo[6] = 24.90525f;
            cromossomo[7] = 34.209156f;
            cromossomo[8] = 40.88559f;
            cromossomo[9] = 91.67577f;
            AG.populacao[3] = new IndividuoAG();
            AG.populacao[3].descricao = "Individuo 11 da Geração 292 - População 16 (6% mutação)";
            AG.populacao[3].setCromossomo(cromossomo);
            AG.populacao[3].setGeracao(0);

        //População 16(Mutação modificada) => Geração 127
            // Individuo 11 (14.999999)
            cromossomo = new float[10];
            cromossomo[0] = 25.5498333f;
            cromossomo[1] = 74.4501648f;
            cromossomo[2] = 36.30123f;
            cromossomo[3] = 687.898865f;
            cromossomo[4] = 55.9507f;
            cromossomo[5] = 44.0492973f;
            cromossomo[6] = 30.4719658f;
            cromossomo[7] = 30.3561325f;
            cromossomo[8] = 39.1719f;
            cromossomo[9] = 70.57033f;
            AG.populacao[4] = new IndividuoAG();
            AG.populacao[4].descricao = "Individuo 11 da Geração 127 - População 16 (Mutação modificada)";
            AG.populacao[4].setCromossomo(cromossomo);
            AG.populacao[4].setGeracao(0);

            // Individuo 0 (11.7000008)
            cromossomo = new float[10];
            cromossomo[0] = 27.8090153f;
            cromossomo[1] = 72.19099f;
            cromossomo[2] = 36.304966f;
            cromossomo[3] = 658.7697f;
            cromossomo[4] = 56.15996f;
            cromossomo[5] = 43.84004f;
            cromossomo[6] = 33.8104973f;
            cromossomo[7] = 33.72156f;
            cromossomo[8] = 32.4679375f;
            cromossomo[9] = 70.0304642f;
            AG.populacao[5] = new IndividuoAG();
            AG.populacao[5].descricao = "Individuo 0 da Geração 127 - População 16 (Mutação modificada)";
            AG.populacao[5].setCromossomo(cromossomo);
            AG.populacao[5].setGeracao(0);


        //População 32 => Geração 123
            // Individuo 18 (19.5000019)
            cromossomo = new float[10];
            cromossomo[0] = 56.3948364f;
            cromossomo[1] = 43.60516f;
            cromossomo[2] = 36.3893356f;
            cromossomo[3] = 834.7185f;
            cromossomo[4] = 51.74155f;
            cromossomo[5] = 48.25845f;
            cromossomo[6] = 48.6214371f;
            cromossomo[7] = 18.1605568f;
            cromossomo[8] = 33.2180061f;
            cromossomo[9] = 42.36512f;
            AG.populacao[6] = new IndividuoAG();
            AG.populacao[6].descricao = "Individuo 18 da Geração 123 - População 32 (3% mutacao)";
            AG.populacao[6].setCromossomo(cromossomo);
            AG.populacao[6].setGeracao(0);

            // Individuo 11 (16.8)
            cromossomo = new float[10];
            cromossomo[0] = 56.50211f;
            cromossomo[1] = 43.4978867f;
            cromossomo[2] = 36.37354f;
            cromossomo[3] = 834.7185f;
            cromossomo[4] = 51.7415352f;
            cromossomo[5] = 48.2584648f;
            cromossomo[6] = 48.6214256f;
            cromossomo[7] = 18.160574f;
            cromossomo[8] = 33.218f;
            cromossomo[9] = 42.3651848f;
            AG.populacao[7] = new IndividuoAG();
            AG.populacao[7].descricao = "Individuo 11 da Geração 123 - População 32 (3% mutacao)";
            AG.populacao[7].setCromossomo(cromossomo);
            AG.populacao[7].setGeracao(0);


        //População 32(6 % mutação) => Geração 124
            // Individuo 0 (20.7000027)
            cromossomo = new float[10];
            cromossomo[0] = 32.88713f;
            cromossomo[1] = 67.11287f;
            cromossomo[2] = 21.51574f;
            cromossomo[3] = 519.16925f;
            cromossomo[4] = 51.5309029f;
            cromossomo[5] = 48.4690971f;
            cromossomo[6] = 22.985096f;
            cromossomo[7] = 37.16888f;
            cromossomo[8] = 39.8460236f;
            cromossomo[9] = 52.7137566f;
            AG.populacao[8] = new IndividuoAG();
            AG.populacao[8].descricao = "Individuo 0 da Geração 124 - População 32 (6% mutação)";
            AG.populacao[8].setCromossomo(cromossomo);
            AG.populacao[8].setGeracao(0);

            // Individuo 4 (17.4)
            cromossomo = new float[10];
            cromossomo[0] = 32.88713f;
            cromossomo[1] = 67.11287f;
            cromossomo[2] = 20.9839516f;
            cromossomo[3] = 518.899536f;
            cromossomo[4] = 51.61385f;
            cromossomo[5] = 48.38615f;
            cromossomo[6] = 23.824213f;
            cromossomo[7] = 38.5611534f;
            cromossomo[8] = 37.61463f;
            cromossomo[9] = 52.74516f;
            AG.populacao[9] = new IndividuoAG();
            AG.populacao[9].descricao = "Individuo 4 da Geração 124 - População 32 (6% mutação)";
            AG.populacao[9].setCromossomo(cromossomo);
            AG.populacao[9].setGeracao(0);


        //População 32(Mutação modificada) => Geração 100
            // Individuo 16 (20.4000015)
            cromossomo = new float[10];
            cromossomo[0] = 42.3528328f;
            cromossomo[1] = 57.6471634f;
            cromossomo[2] = 27.0611382f;
            cromossomo[3] = 496.0652f;
            cromossomo[4] = 55.80641f;
            cromossomo[5] = 44.1935844f;
            cromossomo[6] = 41.52936f;
            cromossomo[7] = 34.8130951f;
            cromossomo[8] = 23.6575413f;
            cromossomo[9] = 34.57573f;
            AG.populacao[10] = new IndividuoAG();
            AG.populacao[10].descricao = "Individuo 16 da Geração 100 - População 32 (Mutação modificada)";
            AG.populacao[10].setCromossomo(cromossomo);
            AG.populacao[10].setGeracao(0);

            // Individuo 17 (17.0999985)
            cromossomo = new float[10];
            cromossomo[0] = 42.49127f;
            cromossomo[1] = 57.508728f;
            cromossomo[2] = 27.0619335f;
            cromossomo[3] = 494.0583f;
            cromossomo[4] = 50.8509979f;
            cromossomo[5] = 49.1490059f;
            cromossomo[6] = 41.7180672f;
            cromossomo[7] = 34.97281f;
            cromossomo[8] = 23.3091183f;
            cromossomo[9] = 34.57575f;
            AG.populacao[11] = new IndividuoAG();
            AG.populacao[11].descricao = "Individuo 17 da Geração 100 - População 32 (Mutação modificada)";
            AG.populacao[11].setCromossomo(cromossomo);
            AG.populacao[11].setGeracao(0);

        // Aleatórios
            cromossomo = new float[10];
            cromossomo[0] = 62.0689659f;
            cromossomo[1] = 37.9310341f;
            cromossomo[2] = 35f;
            cromossomo[3] = 784f;
            cromossomo[4] = 21.2962952f;
            cromossomo[5] = 78.7037048f;
            cromossomo[6] = 21.4912281f;
            cromossomo[7] = 42.5438576f;
            cromossomo[8] = 35.9649124f;
            cromossomo[9] = 145f;
            AG.populacao[12] = new IndividuoAG();
            AG.populacao[12].descricao = "Individuo Aleatório";
            AG.populacao[12].setCromossomo(cromossomo);
            AG.populacao[12].setGeracao(0);

            cromossomo = new float[10];
            cromossomo[0] = 74.60317f;
            cromossomo[1] = 25.3968258f;
            cromossomo[2] = 36f;
            cromossomo[3] = 171;
            cromossomo[4] = 90.90909f;
            cromossomo[5] = 9.090909f;
            cromossomo[6] = 53.6000023f;
            cromossomo[7] = 0.8f;
            cromossomo[8] = 45.6000023f;
            cromossomo[9] = 150;
            AG.populacao[13] = new IndividuoAG();
            AG.populacao[13].descricao = "Individuo Aleatório";
            AG.populacao[13].setCromossomo(cromossomo);
            AG.populacao[13].setGeracao(0);

            cromossomo = new float[10];
            cromossomo[0] = 58.6826324f;
            cromossomo[1] = 41.3173637f;
            cromossomo[2] = 29;
            cromossomo[3] = 297;
            cromossomo[4] = 75;
            cromossomo[5] = 25;
            cromossomo[6] = 17.7083321f;
            cromossomo[7] = 30.729166f;
            cromossomo[8] = 51.5624962f;
            cromossomo[9] = 58;
            AG.populacao[14] = new IndividuoAG();
            AG.populacao[14].descricao = "Individuo Aleatório";
            AG.populacao[14].setCromossomo(cromossomo);
            AG.populacao[14].setGeracao(0);

            cromossomo = new float[10];
            cromossomo[0] = 32.3308258f;
            cromossomo[1] = 67.6691742f;
            cromossomo[2] = 21;
            cromossomo[3] = 299;
            cromossomo[4] = 93.93939f;
            cromossomo[5] = 6.060606f;
            cromossomo[6] = 7.69230747f;
            cromossomo[7] = 92.30769f;
            cromossomo[8] = 0;
            cromossomo[9] = 137;
            AG.populacao[15] = new IndividuoAG();
            AG.populacao[15].descricao = "Individuo Aleatório";
            AG.populacao[15].setCromossomo(cromossomo);
            AG.populacao[15].setGeracao(0);
    }

}