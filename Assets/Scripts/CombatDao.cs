using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDao : UnitDao {

	public CombatType combatType;

    public CombatDao() {}

	public CombatDao(string name, string icon, string model, float attack, float defense, float walkSpeed, float lifeTotal, float range, float attackSpeed, CombatType combatType, float trainingTime, int visionField) : base(name, icon, model, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField) {
        this.name = name;
		this.icon = icon;
		this.model = model;
		this.cost = new Dictionary<int, int>();
		this.attack = attack;
		this.defense = defense;
		this.walkSpeed = walkSpeed;
		this.lifeTotal = lifeTotal;
		this.combatType = combatType;
	}

	public override Unit Instantiate() {
		return new Combat(id, name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, cost, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, combatType, trainingTime, visionField);
	}

}
