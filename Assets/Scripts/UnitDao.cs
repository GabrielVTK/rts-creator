using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDao {

	public string name;
	public string icon;
	public string model;
	public Dictionary<BaseMaterialDao, int> cost;
	public Dictionary<Building, int> requiredBuilding;
	public float attack;
	public float defense;
	public float walkSpeed;
	public float lifeTotal;

	public int visionField;

	public float trainingTime;

	public float range;
	public float attackSpeed;

	public UnitDao() {}

	public UnitDao(string name, string icon, string model, float attack, float defense, float walkSpeed, float lifeTotal, float range, float attackSpeed, float trainingTime, int visionField) {
		this.name = name;
		this.icon = icon;
		this.model = model;
		this.cost = new Dictionary<BaseMaterialDao, int>();
		this.requiredBuilding = new Dictionary<Building, int>();
		this.attack = attack;
		this.defense = defense;
		this.walkSpeed = walkSpeed;
		this.lifeTotal = lifeTotal;
		this.range = range;
		this.visionField = visionField;
		this.attackSpeed = attackSpeed;
		this.trainingTime = trainingTime;
	}

	public virtual Unit Instantiate() {
		
		Dictionary<BaseMaterial, int> costBaseMaterial = new Dictionary<BaseMaterial, int>();

		foreach(BaseMaterialDao baseMaterialDao in cost.Keys) {
			costBaseMaterial.Add(baseMaterialDao.Instantiate(), cost[baseMaterialDao]);
		}

		return new Unit(name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, costBaseMaterial, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField);
	}

}
