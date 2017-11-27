using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAndAttackOrder : Order {

    public List<Unit> units;
    private AttackOrder AO;
    private bool executed;

    private FindAndAttackOrder() {}

    public FindAndAttackOrder(int idPlayer, List<Unit> units) {
        this.idPlayer = idPlayer;
        this.units = new List<Unit>();
        this.executed = false;

        foreach (Unit unit in units) {
            this.units.Add(unit);
        }

        if (this.units[0].isWalking || this.units[0].isBusy) {
            RemoveAnotherOrder(this.units);
        }

    }

    public Property GetEnemyProperty() {

        List<Building> buildingsEnemy = GameController.players[this.units[0].idPlayer].action.GetVisibleEnemyBuildings();
        List<Unit> unitsEnemy = GameController.players[this.units[0].idPlayer].action.GetVisibleEnemyUnits();

        Property property = null;
        int dist = -1, distTemp;

        if(buildingsEnemy.Count > 0) {
            foreach (Building building in buildingsEnemy) {

                distTemp = (int)(Mathf.Abs(building.position.y - this.units[0].position.y) + Mathf.Abs(building.position.x - this.units[0].position.x));

                if(distTemp < dist || dist == -1) {
                    dist = distTemp;
                    property = building;
                }

            }
        }

        if (unitsEnemy.Count > 0) {
            foreach(Unit unit in unitsEnemy) {

                distTemp = (int)(Mathf.Abs(unit.position.y - this.units[0].position.y) + Mathf.Abs(unit.position.x - this.units[0].position.x));

                if (distTemp < dist || dist == -1) {
                    dist = distTemp;
                    property = unit;
                }

            }
        }
        
        return property;
    }

    public override bool Cooldown() {

        if (this.units.Count == 0) {
            this.isActive = false;
            return true;
        }

        return false;
    }

    public override void Execute() {

        if (this.units.Count == 0) {
            this.isActive = false;
            return;
        }

        if(this.AO == null) {

            if(!this.executed) {

                this.executed = true;
                Property property = GetEnemyProperty();
                if(property != null) {

                    if(property.GetType().BaseType == typeof(Unit)) {
                        List<Unit> targets = new List<Unit>();
                        targets.Add((Unit)property);

                        this.AO = new AttackUnitOrder(this.units, targets, true, true);
                    } else {
                        this.AO = new AttackBuildingOrder(this.units, (Building)property, true);
                    }
                } else {
                    this.isActive = false;
                    return;
                }

            } else {
                this.isActive = false;
                return;
            }
        }
        
        if(this.AO.Cooldown()) {
            this.AO.Execute();
        }
    }

    public Order Clone() {

        FindAndAttackOrder FAO = new FindAndAttackOrder();

        FAO.units = this.units;
        FAO.isActive = this.isActive;
        FAO.executed = this.executed;
        FAO.AO = this.AO;

        return FAO;
    }

}
