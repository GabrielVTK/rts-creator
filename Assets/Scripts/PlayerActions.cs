﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerActions {

	private Map map;

	private Player player;

	public PlayerActions(Player player) {
		this.map = GameController.map;
		this.player = player;
	}

    public void Reform(List<Worker> workers, Building building) {

        if(workers.Count > 0 && building != null) {
            player.orders.Add(new BuildOrder(this.player.id, workers, building));
        }

    }

    public bool Build(Worker worker, Vector2 position, string buildingName) {

        List<Worker> workers = new List<Worker>();
        workers.Add(worker);

        return Build(workers, position, buildingName);
    }

    public bool Build(List<Worker> workers, Vector2 position, string buildingName) {

		if(workers.Count == 0) {
            Debug.Log("Trabalhadores não encontrado!");
			return false;
		}

		BuildingDao buildingDao = null;

		foreach(int buildingId in workers[0].buildings) {
			if(GameController.gameComponents.buildings[buildingId].name == buildingName) {
				buildingDao = GameController.gameComponents.buildings[buildingId];
				break;
			}
		}

		if(buildingDao == null) {
			Debug.Log("Building não encontrada!");
			return false;
		}

		if (this.player.AddBuilding(map, buildingDao, position)) {

            player.orders.Add(new BuildOrder(this.player.id, workers, this.player.buildings[Property.nextId]));

			return true;
		}

		return false;
	}

    public bool CheckCost(string buildingName) {

        foreach(BuildingDao buildingDao in GameController.gameComponents.buildings.Values) {
            if(buildingDao.name == buildingName) {
                return player.CheckCosts(buildingDao.cost);
            }
        }

        return false;
    }

	public bool AddUnit(Building building, string unitName) {
        player.unitsCount++;
		foreach(int unitId in building.units) {
			if(GameController.gameComponents.units[unitId].name == unitName) {
				return this.player.AddUnit(GameController.gameComponents.units[unitId], building);
			}
		}

		return false;
	}
    
    public void Walk(Unit unit, Vector2 position) {
        if(unit != null) {
            List<Unit> units = new List<Unit>();
            units.Add(unit);

            player.orders.Add(new MovementOrder(this.player.id, units, position, false));
        }
    }

    public void Walk(List<Unit> units, Vector2 position) {
        if(units.Count > 0) {
            player.orders.Add(new MovementOrder(this.player.id, units, position, false));
        }
    }

    public void Attack(Unit unit, Unit target, bool isConcentrated) {
        if(unit != null && target != null) {
            List<Unit> units = new List<Unit>();
            units.Add(unit);

            List<Unit> targets = new List<Unit>();
            targets.Add(target);

            this.AttackUnit(units, targets, isConcentrated);
        }
    }

    public void Attack(Unit unit, List<Unit> targets, bool isConcentrated) {
        if (unit != null && targets.Count > 0) {
            List<Unit> units = new List<Unit>();
            units.Add(unit);
            
            this.AttackUnit(units, targets, isConcentrated);
        }
    }

    public void Attack(List<Unit> units, Unit target, bool isConcentrated) {
        if (units.Count > 0 && target != null) {
            List<Unit> targets = new List<Unit>();
            targets.Add(target);

            this.AttackUnit(units, targets, isConcentrated);
        } else {
            Debug.Log("NoAttack -> ("+units.Count+") e ("+target+")");
        }
    }

    public void Attack(List<Unit> units, List<Unit> targets, bool isConcentrated) {
        if (units.Count > 0 && targets.Count > 0) {
            this.AttackUnit(units, targets, isConcentrated);
        }
    }
    
    public void Attack(Unit unit, Building building) { 
        if(unit != null && building != null) {
            List<Unit> units = new List<Unit>();

            if(!unit.isBusy && !unit.isWalking) {
                units.Add(unit);
            }

            this.AttackBuilding(units, building);
        }
    }

    public void Attack(List<Unit> units, Building building) {
        if(units.Count > 0 && building != null) {
            this.AttackBuilding(units, building);
        } else {
            Debug.Log("Erro ao atacar building. (Units: "+units.Count+", Building: "+building+")");
        }
    }

    private void AttackUnit(List<Unit> units, List<Unit> targets, bool isConcentrated) {
        if (units.Count > 0 && targets.Count > 0) {
            player.orders.Add(new AttackUnitOrder(units, targets, isConcentrated));
        }
    }

    private void AttackBuilding(List<Unit> units, Building building) {
        if(units.Count > 0 && building != null) {
             player.orders.Add(new AttackBuildingOrder(units, building));
        }
    }

    public void DefenseBuilding(List<Unit> units, Building building, bool isConcentrated) {
        player.orders.Add(new DefenseOrder(this.player.id, units, building, isConcentrated));
    }

    public Vector2 GetNearTile(Building building) {
        
        if(building != null) {

            Debug.Log("P"+this.player.id+": Building váilda");

            int range = 1, i, j;

            while (range < map.height * map.width) {

                int lineIni = (int)building.position.y - range;
                int lineFin = (int)(building.position.y + building.size.y) - 1 + range;
                int columnIni = (int)building.position.x - range;
                int columnFin = (int)(building.position.x + building.size.x) - 1 + range;

                for (i = lineIni; i <= lineFin; i++) {

                    if (i > 0 && i < (map.height - 1)) {
                        if (columnIni > 0 && !this.player.fog.tiles[i, columnIni].unknown && map.tiles[i, columnIni].canBuild) {
                            return new Vector2(columnIni, i);
                        } else if (columnFin < (map.width - 1) && !this.player.fog.tiles[i, columnFin].unknown && map.tiles[i, columnFin].canBuild) {
                            return new Vector2(columnFin, i);
                        }
                    }
                }

                lineIni--;
                lineFin--;

                for (j = columnIni; j <= columnFin; j++) {

                    if (j > 0 && j < (map.width - 1)) {
                        if (lineIni > 0 && !this.player.fog.tiles[lineIni, j].unknown && map.tiles[lineIni, j].canBuild) {
                            return new Vector2(j, lineIni);
                        } else if (lineFin < (map.height - 1) && !this.player.fog.tiles[lineFin, j].unknown && map.tiles[lineFin, j].canBuild) {
                            return new Vector2(j, lineFin);
                        }
                    }
                }

                range++;
            }

        } else {
            Debug.Log("P" + this.player.id + ": Building nula");
        }

        Debug.Log("P"+this.player.id+ ": GetNearTile(-1.0f, -1.0f)");
		return new Vector2(-1.0f, -1.0f);
	}

	public Vector2 GetNearSource(Unit unit, string materialName) {
        
		if(unit == null) {
            Debug.Log("sem unidade");
			return new Vector2(-1.0f, -1.0f);
		}

        int distance = -1, newDistance;

        Vector2 position = new Vector2(-1.0f, -1.0f);

        foreach(MaterialSource materialSource in GetMaterialSourceAvailable(materialName)) {
            
            newDistance = (int)(Mathf.Abs(materialSource.position.x - unit.position.x) + Mathf.Abs(materialSource.position.y - unit.position.y));

            if(distance == -1 || newDistance < distance) {
                distance = newDistance;
                position = materialSource.position;
            }
        }
        
		return position;
	}
    
    public bool Destroy(Property property) {

		if(property.GetType() == typeof(Building)) {
			Building building = (Building)property;
			building.Destroy();
			return true;
		} else if(property.GetType().BaseType == typeof(Unit)) {
			Unit unit = (Unit)property;
			unit.Destroy();
			return true;
		}

		return false;
	}

    public List<Unit> GetUnits(string name) {
        List<Unit> units = new List<Unit>();

        foreach (Unit unit in player.units.Values) {
            if (unit.name == name) {
                units.Add(unit);
            }
        }

        return units;
    }

    public List<Worker> GetWorkers() {

        Debug.Log("GetWorkers()");

        List<Worker> workers = new List<Worker>();

        int idWorker = -1;

        foreach(UnitDao unit in GameController.gameComponents.units.Values) {
            if(unit.GetType() == typeof(WorkerDao)) {
                idWorker = unit.id;
                break;
            }
        }

        Debug.Log("IdWorker => " + idWorker);

        if(idWorker != -1) {
            foreach (Unit unit in player.units.Values) {
                if (unit.idType == idWorker) {
                    workers.Add((Worker)unit);
                }
            }
        }

        Debug.Log("Workers => " + workers.Count);

        return workers;
    }

    public List<Worker> GetWorkersAvailable() {

        List<Worker> workers = GetWorkers();
        List<Worker> workersAvailable = new List<Worker>();
        
        if (workers.Count > 0) {
            foreach (Worker worker in workers) {
                if (!worker.isWalking && !worker.isBusy) {
                    workersAvailable.Add(worker);
                }
            }
        }

        return workersAvailable;
    }

    public List<Unit> GetTroop() {

        List<Unit> troop = new List<Unit>();

        foreach(Unit unit in player.units.Values) {
            if(unit.GetType() == typeof(Combat)) {
                troop.Add(unit);
            }
        }

        return troop;
    }

	public List<Unit> GetUnitAvailable(string unitName, int quantity = -1) {

        List<Unit> units = new List<Unit>();

        int count = 0;

		foreach(Unit unit in this.player.units.Values) {

			if(unit.name == unitName && !unit.isWalking && !unit.isBusy) {
                units.Add(unit);
                count++;
			}

            if(count == quantity) {
                break;
            }
		}

		return units;
	}

	public List<Building> GetBuilding(string buildingName) {

		List<Building> buildings = new List<Building>();

		foreach(Building building in this.player.buildings.Values) {

			if(building.name == buildingName) {
				buildings.Add(building);
			}

		}

		return buildings;
	}

	public List<MaterialSource> GetMaterialSourceAvailable(string sourceName) {

		List<MaterialSource> materialSources = new List<MaterialSource>();

        MaterialSource ms;


        foreach (MapComponent mapComponent in GameController.map.mapComponents) {
            
            if(mapComponent.GetType() == typeof(MaterialSource) &&
               mapComponent.name == sourceName &&
               !player.fog.tiles[(int)mapComponent.position.y, (int)mapComponent.position.x].unknown) {

                ms = (MaterialSource)mapComponent;

                if(ms.canBuild) {
                    materialSources.Add(ms);
                }
                
            }
        }

        return materialSources;
	}

	public int GetAmountUnit(string unitName) {

		int count = 0;

		foreach(Unit unit in this.player.units.Values) {
			if(unit.name == unitName) {
				count++;
			}
		}

		foreach(Building building in this.player.buildings.Values) {
			if(building.trainUnitOrder != null) {

				foreach(Unit unit in building.trainUnitOrder.units) {
					if(unit.name == unitName) {
						count++;
					}
				}

			}
		}

		return count;
	}

	public int GetAmountTroop() {

		int count = 0;

		foreach(Unit unit in this.player.units.Values) {
			if(unit.GetType() == typeof(Combat)) {
				count++;
			}
		}

        List<Building> quarters = GetBuilding("Quarter");

        foreach(Building quarter in quarters) {

            if(quarter.trainUnitOrder != null) {

                foreach (Unit unit in quarter.trainUnitOrder.units) {
                    if(unit.GetType() == typeof(Combat)) {
                        count++;
                    }
                }
            }

        }        
                
        return count;
	}

	public float GetProductionRate(string materialName) {

		float count = 0.0f;

		foreach(Building building in this.player.buildings.Values) {

			if(building.producedMaterial != null && building.producedMaterial.name == materialName) {
				count += building.producedPerTime;
			}

		}

		return count;
	}

	public bool VerifyPropertiesIsAttacked() {

		foreach(Building building in this.player.buildings.Values) {
			if(building.isAttacked) {
				return true;
			}
		}

		foreach(Unit unit in this.player.units.Values) {
			if(unit.isAttacked) {
				return true;
			}
		}

		return false;
	}

	public bool VerifyBuildingsIsAttacked() {

		foreach(Building building in this.player.buildings.Values) {
			if(building.isAttacked) {
				return true;
			}
		}

		return false;
	}

	public bool VerifyUnitsIsAttacked() {

		foreach(Unit unit in this.player.units.Values) {
			if(unit.isAttacked) {
				return true;
			}
		}

		return false;
	}

	public List<Unit> GetUnitsAttacked() {

		List<Unit> units = new List<Unit>();

		foreach(Unit unit in this.player.units.Values) {
			if(unit.isAttacked) {
				units.Add(unit);	
			}
		}

		return units;
	}

	public List<Building> GetBuildingsAttacked() {

		List<Building> buildings = new List<Building>();

		foreach(Building building in this.player.buildings.Values) {
			if(building.isAttacked) {
				buildings.Add(building);	
			}
		}

		return buildings;
	}

    public List<Unit> GetUnitsEnemyIsAttacking(Building building) {

        List<Unit> targets = new List<Unit>();

        AttackBuildingOrder ABO;

        foreach (AttackOrder AO in this.player.enemyAttackOrders) {
            if(AO.GetType() == typeof(AttackBuildingOrder)) {
                ABO = (AttackBuildingOrder)AO;
                if(ABO.target == building) {
                    if(ABO.units.Count > 0) {
                        return ABO.units;
                    } else {
                        ABO.isActive = false;
                    }
                }
            }
        }

        return targets;
    }

    public List<Unit> GetUnitsEnemyIsAttacking(Unit unit) {

        AttackUnitOrder AUO;

        foreach (AttackOrder AO in this.player.enemyAttackOrders) {
            if (AO.GetType() == typeof(AttackUnitOrder)) {
                AUO = (AttackUnitOrder)AO;
                if (AUO.targets.Contains(unit)) {
                    return AUO.units;
                }
            }
        }

        return null;
    }

    public bool VerifyBuildingExists(string buildingName) {

		foreach(Building building in this.player.buildings.Values) {
			if(building.name == buildingName) {
				return true;
			}
		}

		return false;
	}

	public bool VerifyBuildingsLife() {

		foreach(Building building in this.player.buildings.Values) {
			if(building.life < building.lifeTotal) {
				return true;
			}
		}

		return false;
	}

	public List<Building> GetBuildingsLostLife() {

		List<Building> buildings = new List<Building>();

		foreach(Building building in this.player.buildings.Values) {
			if(building.life < building.lifeTotal) {
				buildings.Add(building);
			}
		}

		return buildings;
	}

    public void ExploreMap(Unit unit) {
        List<Unit> units = new List<Unit>();
        units.Add(unit);

        this.ExploreMap(units);
    }

    public void ExploreMap(List<Unit> units) {

        if(units.Count == 0) {
            return;
        }
        
        BlockExploration block = this.player.fog.GetBetterBlockPotential();

        if(block != null) {
            block.unknown = false;
            player.orders.Add(new MovementOrder(this.player.id, units, block.GetPosition(), false));
        } else {
            Debug.Log(this.player.name + ": Todos os blocos foram descobertos.");
        }
    }

	public List<Building> GetVisibleEnemyBuildings() {

		Player enemy = GameController.players[(player.id == 0 ? 1 : 0)];
		List<Building> buildings = new List<Building>();
        
		foreach(Building building in enemy.buildings.Values) {

            int i, j;
            
            for (i = (int)building.position.y; i < (int)(building.position.y + building.size.y); i++) {
				for(j = (int)building.position.x; j < (int)(building.position.x + building.size.x); j++) {

					if(!player.fog.tiles[i, j].unknown) {

                        if(!buildings.Contains(building)) {
                            buildings.Add(building);
                        }
					}
				}
			}
		}

		return buildings;
	}

	public List<Unit> GetVisibleEnemyUnits(string unitName = "") {

		Player enemy = GameController.players[(player.id == 0 ? 1 : 0)];
		List<Unit> units = new List<Unit>();

		foreach(Unit unit in enemy.units.Values) {
			
			if(!player.fog.tiles[(int)unit.position.y, (int)unit.position.x].unknown && (unitName.Equals(unit.name) || unitName.Equals(""))) {
				units.Add(unit);
			}

		}

		return units;

	}

    public List<Building> GetEnemyBuildings(string buildingName) {

        List<Building> enemyBuildings = new List<Building>();

        foreach(Building building in GetVisibleEnemyBuildings()) {
            if(building.name == buildingName) {
                enemyBuildings.Add(building);
            }
        }

        return enemyBuildings;
    }

	public bool VerifyVisibleEnemyBuilding(string buildingName) {

		foreach(Building building in GetVisibleEnemyBuildings()) {

			if(building.name == buildingName) {
				return true;
			}

		}

		return false;
	}
    
}