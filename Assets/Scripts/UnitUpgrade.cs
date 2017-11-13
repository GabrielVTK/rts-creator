using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUpgrade : Technology {

	private Unit unit;
	private string attribute;
	private float bonus;

	public UnitUpgrade(string name, Sprite icon, float developTime) : base(name, icon, developTime) {}
    
    public void SetUnit(Unit unit) {
		this.unit = unit;
	}

	public void SetAttribute(string attribute) {
		this.attribute = attribute;
	}

	public void SetBonus(float bonus) {
		this.bonus = bonus;
	}

	public Unit GetUnit() {
		return unit;
	}

	public string GetAttribute() {
		return attribute;
	}

	public float GetBonus() {
		return bonus;
	}

}