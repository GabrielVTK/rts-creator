
using System.Collections.Generic;

public class IndividuoAG  {

    int geracao, qtdVitoria;
    float[] cromossomos;
    float pontuacaoTorneio;
    List<int> oponentes;
    
    public IndividuoAG() {
        oponentes = new List<int> { };
        qtdVitoria = 0;
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
