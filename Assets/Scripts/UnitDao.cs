using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDao {

    public static int ID = 0;

    public int id;
	public string name;
	public string icon;
	public string model;
	public Dictionary<int, int> cost;
	public Dictionary<int, int> requiredBuilding;
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
        this.id = UnitDao.ID++;
        this.name = name;
		this.icon = icon;
		this.model = model;
		this.cost = new Dictionary<int, int>();
		this.requiredBuilding = new Dictionary<int, int>();
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
		return new Unit(id, name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, cost, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField);
	}

}
