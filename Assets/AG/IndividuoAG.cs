
using System.Collections.Generic;

public class IndividuoAG  {

    public string descricao;
    int geracao, qtdVitoria;
    float[] cromossomos;
    float pontuacaoTorneio;
    List<int> oponentes;
    
    public IndividuoAG(string descricao = "Aleatório") {
        this.descricao = descricao;
        this.oponentes = new List<int> { };
        this.qtdVitoria = 0;
    }

    public int getGeracao() {
        return geracao;
    }

    public float[] getCromossomos() {
        return cromossomos;
    }

    public List<int> getOponentes() {
        return oponentes;
    }
    
    public void setGeracao(int g) {
        this.geracao = g;
    }

    public void setCromossomo(float[] c) {
        this.cromossomos = c;
    }
    
    public void setPontuacao(float p) {
        this.pontuacaoTorneio = p;
    }

    public float getPontuacao() {
        return pontuacaoTorneio;
    }

    public void setVitorias(int p) {
        this.qtdVitoria = p;
    }

    public int getVitorias() {
        return qtdVitoria;
    }

}
