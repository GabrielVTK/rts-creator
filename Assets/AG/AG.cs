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
            AG.populacao = new IndividuoAG[8];
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

        for (i = 0; i < AG.populacao.Length; i++) {

            float[] cromossomo = new float[10];
   
            cromossomo[0] = UnityEngine.Random.Range(0, 101);
            cromossomo[1] = UnityEngine.Random.Range(0, 101);
            cromossomo[2] = UnityEngine.Random.Range(1, 11);
            cromossomo[3] = UnityEngine.Random.Range(100, 1001);
            cromossomo[4] = UnityEngine.Random.Range(0, 101);
            cromossomo[5] = UnityEngine.Random.Range(0, 101);
            cromossomo[6] = UnityEngine.Random.Range(0, 101);
            cromossomo[7] = UnityEngine.Random.Range(0, 101);
            cromossomo[8] = UnityEngine.Random.Range(0, 101);
            //cromossomo[9] = UnityEngine.Random.Range(10, 501);
            cromossomo[9] = UnityEngine.Random.Range(10, 51); // Baixado de 500 para 400 devido ao limite populacional

            AG.populacao[i] = new IndividuoAG();
            AG.populacao[i].setCromossomo(cromossomo);
            AG.populacao[i].setGeracao(0);
        }

        normalizaCromossomo();
    }    

    public IEnumerator selecaoPais(IndividuoAG[] pais) {
        
        //calcula a aptidão dos indivíduos através do torneio no formato suiço
        TorneioTabela torneio = new TorneioTabela(AG.populacao);

        //inicializa pontuacao
        torneio.iniciaPontuacao();

        Debug.Log("StartCoroutine");
        Debug.Log("Rodadas: "+ (int)Mathf.Ceil(Mathf.Log(AG.populacao.Length, 2)));
        StartCoroutine(torneio.DivideJogadas((int)Mathf.Ceil(Mathf.Log(AG.populacao.Length, 2))));
        
        while(AG.divideJogagas) {
            yield return null;
        }

        Debug.Log("After StartCoroutine");

        //após todos os jogadores terem suas pontuações do torneio suiço
        //calcula a pontuação total e "roda a roleta"

        int i;
        float pontuacaoTotal = 0f;

        for (i = 0; i < AG.populacao.Length; i++){
            pontuacaoTotal += AG.populacao[i].getPontuacao();
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

            //   Chance de aplicar mutação de 2% para cada indivíduo da população
            if (UnityEngine.Random.Range(0, 101) < 3) {

                teste = true;

                //Mutação ocorrerá de forma a alterar o gene aleatório do cromossomo escolhido
                cromossomo = AG.populacao[i].getCromossomos();
                cromossomoEscolhido = UnityEngine.Random.Range(0, 10);

                //Aplicando um valor maior ou menor de -10 a 10  (caso negativo transforma o valor em zero)
                valor = UnityEngine.Random.Range(-10, 11);
                if ((cromossomo[cromossomoEscolhido] + valor) <= 0) {
                    cromossomo[cromossomoEscolhido] = 0;
                } else {

                    //soma o valor positivo ou negativo ao gene do cromossomo
                    cromossomo[cromossomoEscolhido] = (cromossomo[cromossomoEscolhido] + valor);
                }

                //atualiza população
                AG.populacao[i].setCromossomo(cromossomo);
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

}