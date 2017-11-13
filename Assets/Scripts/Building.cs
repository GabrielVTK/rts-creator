using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Property {

	public GameObject model;
	public int level;
	public List<UnitDao> units;
	public List<TechnologyDao> technologies;
	public Vector2 position;
	public Vector2 creationPoint;
	public float lifeTotal;
	public float life;
	public Vector2 size;

	public int visionField;

	public List<string> requireds;
	
	public int producedPerTime;
    public BaseMaterial producedMaterial;
    
	public bool constructed;

	public bool isAttacked;

    public TrainUnitOrder trainUnitOrder;

	public Building(string name, Sprite icon, GameObject model, List<UnitDao> units, List<TechnologyDao> technologies, Dictionary<BaseMaterial, int> cost, Vector2 size, float lifeTotal, float developTime, bool constructed, int visionField, List<string> requireds, BaseMaterial producedMaterial, int producedPerTime) : base(name, icon, developTime) {
        this.model = model;
		this.level = 1;
		this.units = units;
		this.technologies = technologies;
		this.cost = cost;
		this.size = size;
		this.lifeTotal = lifeTotal;
		this.life = 1.0f;
		//this.timeCounter = 0.0f;
		this.constructed = constructed;
		//this.trainingUnits = new List<Unit>();
		this.visionField = visionField;
		this.requireds = requireds;
		this.producedMaterial = producedMaterial;
		this.producedPerTime = producedPerTime;
		this.isAttacked = false;
        this.trainUnitOrder = null;


		foreach(UnitDao unit in this.units) {
			unit.requiredBuilding.Add(this, 1);
		}

	}

	public void DevelopTechnology(Technology technology) {
		
	}

	/**public Unit TrainUnit() {
		
		if (trainingUnits[0].timeCounter >= GameController.unitTime * trainingUnits[0].developTime) {

			Unit unit = trainingUnits[0];

			unit.idPlayer = this.idPlayer;
			unit.timeCounter = 0;

			if(unit.GetType() == typeof(Worker)) {

				Worker worker = (Worker)unit;

				worker.returnBuilding = this;
			}

			SetCreationPoint(unit);

            unit.Draw();

			trainingUnits.Remove(unit);
			return unit;

		} else {
			trainingUnits[0].timeCounter += Time.deltaTime;

			return null;
		}

	}/**/

    /** public void ProduceMaterial() {

		if (this.timeCounter < GameController.unitTime) {
			this.timeCounter += Time.deltaTime;
		} else {
            
			Map map = GameController.map;

			if(map.tiles[(int)this.position.y, (int)this.position.x].mapComponent != null) {

				if(map.tiles[(int)this.position.y, (int)this.position.x].mapComponent.GetType() == typeof(MaterialSource)) {

					MaterialSource materialSource = (MaterialSource)map.tiles [(int)this.position.y, (int)this.position.x].mapComponent;

					materialSource.quantity -= this.producedPerTime;

					// out sinc
					if(materialSource.quantity <= 0) {

						if(materialSource.quantity < 0) {
							GameController.players[this.idPlayer].baseMaterials [this.producedMaterial] += materialSource.quantity;
						}
                        
						GameController.players[this.idPlayer].uselessConstructions.Add (this);
						GameObject.Find("GameController").GetComponent<GameController> ().DestroySource(materialSource);
					}

				}

			}

			GameController.players[this.idPlayer].baseMaterials [this.producedMaterial] += this.producedPerTime;

			GameObject.Find("GameController").GetComponent<GameController>().DrawInfoMaterials();

			this.timeCounter -= GameController.unitTime;
		}

	}/**/

	public void ChangeCreationPoint(float x, float y) {
        
		Map map = GameController.map;

		int line = (map.height / 2) - Mathf.CeilToInt(y);
		int column = Mathf.CeilToInt(x) + (map.width / 2) - 1;

		if(map.tiles[line, column].isWalkable) {
			this.creationPoint = new Vector2((float)line, (float)column);
		}

	}

    /**public void SetCreationPoint(Unit unit) {

        unit.position = new Vector2(this.position.x - 1, this.position.y - 1);

        return;

        
		Map map = GameController.map;
		Player player = GameController.players[idPlayer];

		bool busy = false;

		float line = this.creationPoint.y;
		float column = this.creationPoint.x;

		foreach(Unit item in player.units.Values) {

			if(item.name.CompareTo(unit.name) != 0 && item.position.x == column && item.position.y == line) {
				busy = true;	
				break;
			}
		}

		if(busy) {

			int elements = 1, auxX = 1, auxY = 1;

			int yIni = (int)column, y = (int)column - 1;
			int xIni = (int)line, x = (int)line;
			int borderLimitXIni, borderLimitXFin, borderLimitYIni, borderLimitYFin, limit = 3;

			while(busy && elements < 50) {

				elements++;

				borderLimitXIni = xIni - (limit / 2);
				borderLimitXFin = xIni + (limit / 2);
				borderLimitYIni = yIni - (limit / 2);
				borderLimitYFin = yIni + (limit / 2);

				while(elements < 50) {

					elements++;

					if(busy) {

						while(x <= borderLimitXFin && x >= borderLimitXIni) {

							if(x < map.width && x >= 0 && y < map.height && y >= 0) {

								if(map.tiles[y, x].isWalkable == true) {

									busy = false;

									foreach(Unit item in player.units.Values) {

										if(item.name.CompareTo(unit.name) != 0 && item.position.x == y && item.position.y == x) {
											busy = true;
											break;
										}

									}

									if(!busy) {
										break;
									}
								}

							}

							if(!busy) {
								break;
							} else {
								x += auxX;
							}

						}
					}

					if(busy) {

						if (x >= borderLimitXFin) {
							x--;
						} else {
							x++;
						}

						if (y >= borderLimitYFin - 1) {
							y--;
						} else {
							y++;
						}

						while(y <= borderLimitYFin && y >= borderLimitYIni) {

							if(x < map.width && x >= 0 && y < map.height && y >= 0) {

								if(map.tiles[y,x].isWalkable == true) {

									busy = false;

									foreach(Unit item in player.units.Values) {	
										
										if(item.GetType() != unit.GetType() && item.position.x == y && item.position.y == x) {
											busy = true;
											break;
										}
									}

									if(!busy) {
										break;
									}
								}

							}

							if(!busy) {
								break;
							} else {
								elements++;
								y += auxY;
							}
						}

						if (busy) {
							if (elements != limit * limit) {
								if (x >= limit - 1) {
									x--;
								} else {
									x++;
								}

								if (y >= limit) {
									y--;
								} else {
									y++;
								}
							}

							auxX *= -1;
							auxY *= -1;
						} else {
							break;
						}
					}
				}

				limit++;

				if(x == borderLimitXIni && y == borderLimitYIni) {
					y--;
					x--;
				}

			}

			//column = y;
			//line = x;
		}

		unit.position = new Vector2(line, column);
        
	}/**/

    public void ModifyTiles() {

		int pos, posX, posY;
		int i = (int)position.y;
		int j = (int)position.x;
        
		Map map = GameController.map;

		for(posX = 0; posX < this.size.x; posX++) {
			for(posY = 0; posY < this.size.y; posY++) {
				map.tiles[(i + posX), (j + posY)].isWalkable = false;
				map.tiles[(i + posX), (j + posY)].canBuild = false;

                if(map.tiles[(i + posX), (j + posY)].mapComponent != null && map.tiles[(i + posX), (j + posY)].mapComponent.GetType() == typeof(MaterialSource)) {
                    MaterialSource ms = (MaterialSource)map.tiles[(i + posX), (j + posY)].mapComponent;

                    ms.canBuild = false;
                }
			}
		}

		for(pos = 0; pos <= this.size.x + 1; pos++) {
            map.tiles[(i - 1), (j + pos - 1)].canBuild = false;
			map.tiles[(i + (int)this.size.y), (j + pos - 1)].canBuild = false;
		}

		for(pos = 0; pos < this.size.y; pos++) {
			map.tiles[(i + pos), (j - 1)].canBuild = false;
			map.tiles[(i + pos), (j + (int)this.size.x)].canBuild = false;
		}
        
    }
		
	public void Destroy() {

		GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
		Map map = GameController.map;
        
		for(int i = (int)this.position.y; i < (int)(this.position.y + this.size.y); i++) {
			for(int j = (int)this.position.x; j < (int)(this.position.x + this.size.x); j++) {

				if(map.tiles[i, j].mapComponent == null) {
					map.tiles[i, j].isWalkable = true;
					map.tiles[i, j].canBuild = true;
				} else {
                    if (map.tiles[i, j].mapComponent.GetType() == typeof(MaterialSource)) {
                        MaterialSource ms = (MaterialSource)map.tiles[i, j].mapComponent;
                        ms.canBuild = true;
                    }
                }

			}
		}

		for(int i = (int)this.position.y - 1; i < ((int)this.position.y + (int)this.size.x + 1); i++) {
			for(int j = (int)this.position.x - 1; j < ((int)this.position.x + (int)this.size.y + 1); j++) {
				if(gameController.CheckCanBuild(i, j)) {
					map.tiles[i, j].isWalkable = true;
					map.tiles[i, j].canBuild = true;
				}
			}
		}

        foreach(Order order in GameController.players[this.idPlayer].orders) {
            if(order.GetType() == typeof(ProduceMaterialOrder)) {
                ProduceMaterialOrder PMO = (ProduceMaterialOrder)order;

                if(PMO.building == this) {
                    PMO.isActive = false;
                    break;
                }

            }
        }

		gameController.objectName = null;
		GameController.players[this.idPlayer].RemoveProperty(this);
		gameController.DestroyGameObject(this.model);
		gameController.DrawViewContent();
	}

	public void Draw() {
		
        if(GameController.draw) {
            Map map = GameController.map;

            float posX = ((map.width - 1) / -2.0f) + this.position.x + (this.size.x / 4.0f);
            float posZ = ((map.height - 1) / 2.0f) - this.position.y - (this.size.y / 4.0f);

            this.model = GameObject.Instantiate(this.model);
            this.model.name = this.idPlayer + "_build_" + this.id;
            this.model.tag = "Property";
            this.model.transform.position = new Vector3(posX, 0.05f, posZ);
        }

	}

	public void RemoveFog() {

		Player player = GameController.players [this.idPlayer];
		Map map = GameController.map;

		for(int i = (int)this.position.y; i < (int)this.position.y + (int)this.size.y; i++) {
			for(int j = (int)this.position.x; j < (int)this.position.x + (int)this.size.x; j++) {

				for(int iFog = (int)this.position.y - this.visionField; iFog < (int)this.position.y + (int)this.size.y + this.visionField; iFog++) {

					for(int jFog = (int)this.position.x - this.visionField; jFog < (int)this.position.x + (int)this.size.x + this.visionField; jFog++) {

						if (iFog >= 0 && iFog < map.height && jFog >= 0 && jFog < map.width) {
                            
							int dist = Mathf.Abs(jFog - j) + Mathf.Abs(iFog - i);

							if(dist <= this.visionField && player.fog.tiles[iFog, jFog].unknown) {
                                //Debug.Log("Destroi FOG");
                                GameController.players[this.idPlayer].fog.tiles[iFog, jFog].Destroy();
							}

						}

					}

				}

			}

		}

	}

}