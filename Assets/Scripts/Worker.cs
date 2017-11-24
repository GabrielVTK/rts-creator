using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit {

	public int capacity;
	public int capacityTotal;
	public float collectSpeed;
    public Building returnBuilding;

    // CanBuild
    public List<BuildingDao> buildings;

    // Build
    //public Building targetBuilding;

    // Collect
    public float collectTimeCounter;
    public BaseMaterial materialType;
    //public MaterialSource targetSource;

    public Worker(string name, Sprite icon, GameObject model, Dictionary<BaseMaterial, int> cost, float attack, float defense, float walkSpeed, float lifeTotal, int capacityTotal, float collectSpeed, List<BuildingDao> buildings, float range, float attackSpeed, float trainingTime, int visionField) : base(name, icon, model, cost, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField) {
		this.capacity = 0;
		this.capacityTotal = capacityTotal;
		this.collectSpeed = collectSpeed;
        this.requiredBuilding = null;
        this.buildings = buildings;
        //this.targetBuilding = null;		
		this.collectTimeCounter = 0.0f;
        this.materialType = null;
       //this.targetSource = null;
    }
    
    public bool Build(Building building) {

        if(!building.constructed && building.life < building.lifeTotal) {
            
            building.life += building.lifeTotal / building.developTime;

            if (building.life > building.lifeTotal) {
                building.constructed = true;
                building.life = building.lifeTotal;
            }
            
            GameController.instance.DrawViewInfo();

            return true;
        }
        
        return false;
    }

}
