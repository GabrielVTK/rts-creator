using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerDao : UnitDao {

	public int capacityTotal;
	public float collectSpeed;

	public List<string> buildingsName;

	public WorkerDao() {}

	public WorkerDao(string name, string icon, string model, float attack, float defense, float walkSpeed, float lifeTotal, int capacityTotal, float collectSpeed, float range, float attackSpeed, float trainingTime, int visionField) : base(name, icon, model, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField) {
		this.name = name;
		this.icon = icon;
		this.model = model;
		this.cost = new Dictionary<BaseMaterialDao, int>();
		this.attack = attack;
		this.defense = defense;
		this.walkSpeed = walkSpeed;
		this.lifeTotal = lifeTotal;
		this.capacityTotal = capacityTotal;
		this.collectSpeed = collectSpeed;
		this.buildingsName = new List<string>();
	}

	public override Unit Instantiate() {
		
		Dictionary<BaseMaterial, int> costBaseMaterial = new Dictionary<BaseMaterial, int>();
		List<BuildingDao> buildings = new List<BuildingDao>();

		foreach(BaseMaterialDao baseMaterialDao in cost.Keys) {
			costBaseMaterial.Add(baseMaterialDao.Instantiate(), cost[baseMaterialDao]);
		}

		foreach(BuildingDao build in GameController.gameComponents.buildings) {
			if(buildingsName.Contains(build.name)) {
				buildings.Add(build);
			}
		}

		return new Worker(name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, costBaseMaterial, attack, defense, walkSpeed, lifeTotal, capacityTotal, collectSpeed, buildings, range, attackSpeed, trainingTime, visionField);
	}

}
