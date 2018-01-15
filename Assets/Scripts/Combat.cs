using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : Unit {

	public CombatType combatType;

	public Combat(int idType, string name, Sprite icon, GameObject model, Dictionary<int, int> cost, float attack, float defense, float walkSpeed, float lifeTotal, float range, float attackSpeed, CombatType combatType, float trainingTime, int visionField) : base(idType, name, icon, model, cost, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField) {
		this.combatType = combatType;
	}

}