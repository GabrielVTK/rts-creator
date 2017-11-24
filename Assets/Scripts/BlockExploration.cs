using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockExploration {
    
    public bool unknown;
    public float potential;

    private Fog fog;
    public int line;
    public int column;
    public int count;
    private const int MDMAX = 20;

    public BlockExploration(Fog fog, int i, int j) {
        this.unknown = true;
        this.potential = 0.0f;
        this.fog = fog;
        this.line = i;
        this.column = j;
        this.count = 10;
    }

    public void DecreaseCount() {
        this.count--;

        if(this.count <= 0) {
            this.fog.calculatePotential = true;
            this.unknown = false;
        }

    }

    public void CalculatePotential() {
        
        int i, j, md;

        this.potential = 0.0f;

        if(!this.unknown) {
            return;
        }

        int iLimit = this.fog.blocks.GetLength(0);
        int jLimit = this.fog.blocks.GetLength(1);
        int line, column;

        for (i = this.line - MDMAX / 4; i <= this.line + MDMAX / 4; i++) {
            for (j = this.column - MDMAX / 4; j <= this.column + MDMAX / 4; j++) {

                if(i >= 0 && i < iLimit && j >= 0 && j < jLimit) {

                    line = i - this.line;
                    column = j - this.column;

                    if (line < 0) {
                        line *= -1;
                    }

                    if (column < 0) {
                        column *= -1;
                    }

                    md = line + column;

                    if(md <= MDMAX) {
                        this.potential += 0.25f - md / 80;
                    }

                }

            }
        }

    }

    public Vector2 GetPosition() {
        return new Vector2(this.column*4 + 1, this.line*4 + 1);
    }
    
}
