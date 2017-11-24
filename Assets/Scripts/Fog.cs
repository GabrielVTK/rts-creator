using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog {

    public FogTile[,] tiles;
    public BlockExploration[,] blocks;
    public bool calculatePotential;

    private int idPlayer;

	public Fog(int idPlayer) {
        this.idPlayer = idPlayer;
        this.tiles = new FogTile[GameController.map.height, GameController.map.width];
        this.blocks = new BlockExploration[Mathf.CeilToInt(GameController.map.height / 4.0f), Mathf.CeilToInt(GameController.map.width / 4.0f)];
        this.calculatePotential = true;

        int i, j, iBlocks = this.blocks.GetLength(0), jBlocks = this.blocks.GetLength(1);

        for(i = 0; i < iBlocks; i++) {
            for (j = 0; j < jBlocks; j++) {
                this.blocks[i, j] = new BlockExploration(this, i, j);
            }
        }

        for (i = 0; i < GameController.map.height; i++) {
            for (j = 0; j < GameController.map.width; j++) {
                this.tiles[i, j] = new FogTile();
                this.tiles[i, j].unknown = true;
                this.tiles[i, j].blockExploration = this.blocks[(int)(i / 4), (int)(j / 4)];
            }
        }

    }
    
    public void CalculateBlocksPotential() {

        int i, j, iLimit = this.blocks.GetLength(0), jLimit = this.blocks.GetLength(1);

        for (i = 0; i < iLimit; i++) {
            for (j = 0; j < jLimit; j++) {
                this.blocks[i, j].CalculatePotential();
            }
        }

        this.calculatePotential = false;
    }

    public BlockExploration GetBetterBlockPotential() {

        int counterTilesUnknow = 0, i, j, iLimit = this.blocks.GetLength(0), jLimit = this.blocks.GetLength(1); ;
        BlockExploration bestBlock = null, blockMostUnknow = null;
        
        if(this.idPlayer == 0) {
            for (i = 0; i < iLimit; i++) {
                for (j = 0; j < jLimit; j++) {
                    this.blocks[i, j].CalculatePotential();

                    if (counterTilesUnknow < this.blocks[i, j].count) {
                        counterTilesUnknow = this.blocks[i, j].count;
                        blockMostUnknow = this.blocks[i, j];
                    }

                    if (this.blocks[i, j].unknown && (bestBlock == null || this.blocks[i, j].potential > bestBlock.potential)) {
                        bestBlock = this.blocks[i, j];
                    }
                }
            }
        } else {
            for (i = iLimit - 1; i >= 0; i--) {
                for (j = jLimit - 1; j >= 0; j--) {
                    this.blocks[i, j].CalculatePotential();

                    if (counterTilesUnknow < this.blocks[i, j].count) {
                        counterTilesUnknow = this.blocks[i, j].count;
                        blockMostUnknow = this.blocks[i, j];
                    }

                    if (this.blocks[i, j].unknown && (bestBlock == null || this.blocks[i, j].potential > bestBlock.potential)) {
                        bestBlock = this.blocks[i, j];
                    }
                }
            }
        }

        if (bestBlock == null) {
            bestBlock = blockMostUnknow;
        }

        return bestBlock;
    }

    public void Draw() {

        if (GameController.draw) {

            float xIni = (float)GameController.map.GetTileSize() * (GameController.map.width - 1) / -2;
            float yIni = (float)GameController.map.GetTileSize() * (GameController.map.height - 1) / 2;

            GameObject fog;

            fog = GameObject.Find("Fog");

            if(fog == null) {
                fog = new GameObject("Fog", typeof(Canvas));
            }            

            for (int i = 0; i < this.tiles.GetLength(0); i++) {
                for(int j = 0; j < this.tiles.GetLength(1); j++) {
                    if(this.tiles[i, j].unknown) {
                        this.tiles[i, j].model = GameObject.Instantiate(Resources.Load("Fog", typeof(GameObject)) as GameObject);
                        this.tiles[i, j].model.gameObject.transform.SetParent(fog.transform);
                        this.tiles[i, j].model.name = this.idPlayer + "_fog_" + i + "_" + j;
                        this.tiles[i, j].model.tag = "Fog P" + (this.idPlayer + 1);
                        this.tiles[i, j].model.transform.position = new Vector3(xIni + j * GameController.map.GetTileSize(), 2.0f, yIni - i * GameController.map.GetTileSize());
                    }
                }
            }

        }
    }

}
