using System.Collections.Generic;
using UnityEngine;

public class Player {

	public int id;
	public string name;
	public bool isHuman;
	public Texture2D icon;
	public int population;
	public int populationLimit;
    public Dictionary<int, Building> buildings;
	public Dictionary<int, Unit> units;
	public Dictionary<BaseMaterial, int> baseMaterials;
    
    public Fog fog;

	public ScriptAttributes scriptAttributes;
	public Script script;
	public PlayerActions action;

    public List<Property> propertiesDestroied;
    
	// Auxiliar Visual
	public BuildingDao wantBuild;

    public List<Order> orders;
    public List<Order> standbyOrders;

    public List<AttackOrder> enemyAttackOrders;
    
    // Log
    public int unitsCount = 0;
    public int resourcesCount = 0;
    public int orderAttackCount = 0;

    public Player(int id, string name, int populationLimit, bool isHuman = true) {
		this.id = id;
		this.name = name;
		this.isHuman = isHuman;
		this.script = new Script(this);
		this.action = new PlayerActions(this);
        this.scriptAttributes = new ScriptAttributes();
                
        this.propertiesDestroied = new List<Property>();

		this.population = 0;
		this.populationLimit = populationLimit;
        
		this.baseMaterials = new Dictionary<BaseMaterial, int>();
		this.units = new Dictionary<int, Unit>();
		this.buildings = new Dictionary<int, Building> ();

        this.orders = new List<Order>();
        this.standbyOrders = new List<Order>();
        this.enemyAttackOrders = new List<AttackOrder>();

        this.fog = new Fog(this.id);

		this.wantBuild = null;
	}

    public void AddBaseMaterial(string baseMaterialName, int quantity) {

        BaseMaterial baseMaterial = BaseMaterial.GetInstance(baseMaterialName);

        if(baseMaterial != null) {
            this.baseMaterials.Add(baseMaterial, quantity);
        }        

    }

	public bool AddUnit(UnitDao unit, Building building) {

		if(building.constructed && this.CheckCosts(unit.cost) && unit.requiredBuilding.ContainsKey(building) && this.population < this.populationLimit) {

            this.population++;
			this.DiscountCosts(unit.cost);

            if(building.trainUnitOrder == null) {
                building.trainUnitOrder = new TrainUnitOrder(this, building);
                this.orders.Add(building.trainUnitOrder);
            }
            
            building.trainUnitOrder.AddUnit(unit.Instantiate());

			return true;
		}

		return false;
	}

	public bool AddBuilding(Map map, BuildingDao buildingDao, Vector2 position) {
        
		if(position.x > 0 && position.x < (map.width - 1) && position.y > 0 && position.y < (map.height - 1)) {
            
			int pos;
			int i = (int)position.y;
			int j = (int)position.x;

			if (!buildingDao.constructed && this.fog.tiles[i, j].unknown) {
				return false;
			}

			if (buildingDao.requireds.Count > 0) {
                                
				int requiredNumber = 0;

				foreach(string required in buildingDao.requireds) {

					if(map.tiles[i, j].mapComponent != null) {

                        if(map.tiles[i, j].mapComponent.GetType() == typeof(MaterialSource)) {
                            MaterialSource ms = (MaterialSource)map.tiles[i, j].mapComponent;

                            if(!ms.canBuild) {
                                return false;
                            }

                        }

						if(map.tiles[i, j].mapComponent.name.CompareTo(required) == 0) {
							requiredNumber++;
						}
					}
				}

				if(requiredNumber != buildingDao.requireds.Count) {
                    return false;
				}

			} else {

                // Check CanBuild Vertically
                for(pos = 0; pos < buildingDao.size.y; pos++) {
                    if (!map.tiles[(i + pos), j].canBuild) {
                        return false;
                    }
                }

                // Check CanBuild Horizontally
                for (pos = 0; pos < buildingDao.size.y; pos++) {
                    if (!map.tiles[i, (j + pos)].canBuild) {
                        return false;
                    }
                }

				int wSize = Mathf.Abs((int)buildingDao.size.x) + 2;
				int hSize = Mathf.Abs((int)buildingDao.size.y) + 2;

                // Check IsWalkable Vertically
                for (pos = 0; pos < hSize; pos++) {
                    if (!map.tiles[(pos + i - 1), (j - 1)].isWalkable || !map.tiles[(pos + i - 1), (j + (int)buildingDao.size.x)].isWalkable) {
                        return false;
                    }
                }

                // Check IsWalkable Horizontally
                for (pos = 0; pos < wSize; pos++) {
					if(!map.tiles[(i - 1), (pos + j - 1)].isWalkable || !map.tiles[(i + (int)buildingDao.size.y), (pos + j - 1)].isWalkable) {
						return false;
					}
				}				

			}

			if(CheckCosts(buildingDao.cost)) {

				DiscountCosts(buildingDao.cost);

				Building building = buildingDao.Instantiate();

                building.idPlayer = this.id;
                building.position = position;
                building.creationPoint = new Vector2(position.x - 1.0f, position.y - 1.0f);

				this.buildings.Add(building.id, building);

                building.ModifyTiles();
                building.RemoveFog();

                if (building.constructed) {
                    building.life = building.lifeTotal;
				}

                building.Draw();
				
				return true;
			}
		}

		return false;
	}

	public void CleanProperties() {

        List<Order> ordersClean = new List<Order>();

        foreach(Order order in this.orders) {
            if(!order.isActive) {
                ordersClean.Add(order);
            }
        }
        
        foreach(Order order in ordersClean) {
            this.orders.Remove(order);
            //Debug.Log(order.GetType());
            if(order.GetType().BaseType == typeof(AttackOrder)) {

                AttackOrder AO = (AttackOrder)order;

                //Debug.Log("P"+this.id + ": remove ordem de ataque do player " + (this.id == 0 ? 1 : 0));
                GameController.players[(this.id == 0 ? 1 : 0)].enemyAttackOrders.Remove((AttackOrder)order);
            }
        }

        foreach(Order order in this.standbyOrders) {
            this.orders.Add(order);
        }
		
        ordersClean.Clear();
        this.standbyOrders.Clear();
	}

	public void RemoveProperty(Property property) {
        
		if(property.GetType() == typeof(Building)) {
			this.buildings.Remove(property.id);	
		} else if(property.GetType().BaseType == typeof(Unit)) {
			this.units.Remove(property.id);	
		}

	}

	public bool CheckCosts(Dictionary<BaseMaterialDao, int> cost) {

		foreach(BaseMaterialDao baseMaterial in cost.Keys) {

			if(this.baseMaterials[baseMaterial.Instantiate()] < cost[baseMaterial]) {
				return false;
			}
		}

		return true;
	}

	public void DiscountCosts(Dictionary<BaseMaterialDao, int> cost) {
		
		foreach(BaseMaterialDao baseMaterial in cost.Keys) {
			this.baseMaterials[baseMaterial.Instantiate()] -= cost[baseMaterial];
		}

	}

    public bool valoresAbsurdos(string baseMaterialName) {

        BaseMaterial mat = BaseMaterial.GetInstance(baseMaterialName);
                     
        return this.baseMaterials[mat] >= 10000;
                
    }

}
