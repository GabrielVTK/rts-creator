using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerDao : UnitDao {

	public int capacityTotal;
	public float collectSpeed;

	public List<int> buildings;

    public WorkerDao() {}

	public WorkerDao(string name, string icon, string model, float attack, float defense, float walkSpeed, float lifeTotal, int capacityTotal, float collectSpeed, float range, float attackSpeed, float trainingTime, int visionField) : base(name, icon, model, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField) {
		this.name = name;
		this.icon = icon;
		this.model = model;
		this.cost = new Dictionary<int, int>();
		this.attack = attack;
		this.defense = defense;
		this.walkSpeed = walkSpeed;
		this.lifeTotal = lifeTotal;
		this.capacityTotal = capacityTotal;
		this.collectSpeed = collectSpeed;
		this.buildings = new List<int>();
	}

	public override Unit Instantiate() {        
        return new Worker(id, name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, cost, attack, defense, walkSpeed, lifeTotal, capacityTotal, collectSpeed, buildings, range, attackSpeed, trainingTime, visionField);
	}

}
