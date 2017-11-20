using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

	public int width;
	public int height;
	public const int tileSize = 1;

	public bool isReflected;
	public char directionReflected;
    
	public Tile[,] tiles;
    public List<MapComponent> mapComponents;

	private int[,] mapTile;
	private int[,] mapComponent;

	public Map() {}

	public Map(int width, int height) {
		this.width = width;
		this.height = height;
		this.isReflected = false;
    }

	public Map(int width, int height, char directionReflected) {
		this.width = (directionReflected == 'E' || directionReflected == 'W') ? 2*width : width;
		this.height = (directionReflected == 'N' || directionReflected == 'S') ? 2*height : height;
		this.isReflected = true;
		this.directionReflected = directionReflected;

    }

	public int GetTileSize() {
		return tileSize;
	}

	public void Build(GameComponents gameComponents) {
        
		this.tiles = new Tile[this.height, this.width];
		this.mapTile = new int[this.height, this.width];
		this.mapComponent = new int[this.height, this.width];
        this.mapComponents = new List<MapComponent>();

        if (this.isReflected) {

			switch (this.directionReflected) {
			case 'N':
				ReflectNorth(gameComponents.mapTile, gameComponents.mapComponent);
				break;
			case 'S':
				ReflectSouth(gameComponents.mapTile, gameComponents.mapComponent);
				break;
			case 'E':
				ReflectEast(gameComponents.mapTile, gameComponents.mapComponent);
				break;
			case 'W':
				ReflectWest(gameComponents.mapTile, gameComponents.mapComponent);
				break;
			}

		} else {
			this.mapTile = gameComponents.mapTile;
			this.mapComponent = gameComponents.mapComponent;
		}

        int i, j;


        for (i = 0; i < this.height; i++) {
			for(j = 0; j < this.width; j++) {
				this.tiles[i, j] =  gameComponents.tiles[this.mapTile[i, j]].Instantiate ();

				if(this.mapComponent[i, j] != 0 && this.tiles[i, j].isWalkable && i > 1 && i < (this.height - 1) && j > 1 && j < (this.width - 1)) {
					this.tiles[i, j].mapComponent = gameComponents.mapComponents[this.mapComponent[i, j] - 1].Instantiate();
					this.tiles[i, j].mapComponent.position = new Vector2 (j, i);
					this.tiles[i, j].isWalkable = false;
					this.tiles[i, j].canBuild = false;
                    this.mapComponents.Add(this.tiles[i, j].mapComponent);
                }

			}

		}

        int x, y, lineIni, lineFin, columnIni, columnFin;

        // Set canBuild = false, where isWalkable = false
        for (i = 0; i < this.height; i++) {
			for(j = 0; j < this.width; j++) {

				if(!this.tiles[i, j].isWalkable) {
					lineIni = (i - 1) >= 0 ? (i - 1) : i;
					lineFin = (i + 1) < this.height ? (i + 1) : i;
					columnIni = (j - 1) >= 0 ? (j - 1) : j;
					columnFin = (j + 1) < this.width ? (j + 1) : j;

					for(y = lineIni; y <= lineFin; y++) {
						for(x = columnIni; x <= columnFin; x++) {
							this.tiles[y, x].canBuild = false;
						}
					}
				}

			}
		}

	}

	private void ReflectNorth(int[,] mapTile, int[,] mapComponent) {

		int line = 0, column = 0;
        int i, j;

        for (i = (this.height / 2) - 1; i >= 0; i--, line++) {
			for(j = this.width - 1; j >= 0; j--, column++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			column = 0;
		}

		for(i = 0; i < this.height / 2; i++, line++) {
			for(j = 0; j < this.width; j++, column++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			column = 0;
		}

	}

	private void ReflectSouth(int[,] mapTile, int[,] mapComponent) {

		int line = 0, column = 0;
        int i, j;

        for (i = 0; i < this.height / 2; i++, line++) {
			for(j = 0; j < this.width; j++, column++) {

				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			column = 0;
		}

		for(i = (this.height / 2) - 1; i >= 0; i--, line++) {
			for(j = this.width - 1; j >= 0; j--, column++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			column = 0;
		}

	}

	private void ReflectEast(int[,] mapTile, int[,] mapComponent) {

		int line = 0, column = 0;
        int i, j;

        for (j = 0; j < (this.width / 2); j++, column++) {
			for(i = 0; i < this.height; i++, line++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			line = 0;
		}

		for(j = (this.width / 2) - 1; j >= 0; j--, column++) {
			for(i = this.height - 1; i >= 0 ; i--, line++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			line = 0;
		}

	}

	private void ReflectWest(int[,] mapTile, int[,] mapComponent) {

		int line = 0, column = 0;
        int i, j;

        for (j = (this.width / 2) - 1; j >= 0; j--, column++) {
			for(i = this.height - 1; i >= 0 ; i--, line++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			line = 0;
		}

		for(j = 0; j < (this.width / 2); j++, column++) {
			for(i = 0; i < this.height; i++, line++) {
				this.mapTile[line, column] = mapTile[i, j];
				this.mapComponent[line, column] = mapComponent[i, j];
			}
			line = 0;
		}

	}

    public void Draw() {

        if(GameController.draw) {

            float xIni = (float)this.GetTileSize() * (this.width - 1) / -2;
            float yIni = (float)this.GetTileSize() * (this.height - 1) / 2;
            
            GameObject tile = new GameObject("tile", typeof(Canvas));
            int i, j;
            string name;
            GameObject tileComponent;

            for (i = 0; i < this.height; i++) {
                for(j = 0; j < this.width; j++) {
                    this.tiles[i, j].model = GameObject.Instantiate(this.tiles[i, j].model);
                    this.tiles[i, j].model.name = "0_tile_" + i + "_" + j;
                    this.tiles[i, j].model.transform.position = new Vector3(xIni + j * this.GetTileSize(), 0.0f, yIni - i * this.GetTileSize());
                    this.tiles[i, j].model.transform.SetParent(tile.transform);
                    
                    if (this.tiles[i, j].mapComponent != null) {

                        name = "obstacle";

                        if (this.tiles[i, j].mapComponent.GetType() == typeof(MaterialSource)) {
                            name = "source";
                        }

                        tileComponent = GameObject.Instantiate(this.tiles[i, j].mapComponent.model);
                        this.tiles[i, j].mapComponent.model = tileComponent;
                        tileComponent.transform.SetParent(tile.transform);
                        tileComponent.name = "0_" + name + "_" + i + "_" + j;
                        tileComponent.transform.position = new Vector3(xIni + j * this.GetTileSize(), 0.05f, yIni - i * this.GetTileSize());
                    }
                }
            }

        }

    }

    public void AddInMapTile(int line, int column, int value) {
        this.mapTile[line, column] = value;
    }

    public int[,] GetMapTile() {
        return this.mapTile;
    }

    public int[,] GetMapComponents() {
        return this.mapComponent;
    }

}
