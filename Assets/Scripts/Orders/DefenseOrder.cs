using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseOrder : Order {

    public List<Unit> units;
    public Building building;
    public bool isConcentrated;
    private AttackUnitOrder AUO;
    private MovementOrder MO;

    private DefenseOrder() {}

    public DefenseOrder(int idPlayer, List<Unit> units, Building building, bool isConcentrated) {
        this.idPlayer = idPlayer;
        this.units = new List<Unit>();
        this.building = building;
        this.isConcentrated = isConcentrated;

        if(building == null || units == null || units.Count == 0) {
            this.isActive = false;
            return;
        }

        foreach (Unit unit in units) {
            this.units.Add(unit);
        }

        if (this.units[0].isWalking || this.units[0].isBusy) {
            RemoveAnotherOrder(this.units);
        }

        Debug.Log("DefenseOrder Ativada!");
    }

    private bool CheckDistance() {

        if(this.units.Count == 0) {
            this.isActive = false;
            return false;
        }

        int i, j, bestDistance = -1, dist;
        Vector2 position = building.position;

        for (i = (int)this.building.position.y; i < (int)(this.building.position.y + this.building.size.y); i++) {
            for (j = (int)this.building.position.x; j < (int)(this.building.position.x + this.building.size.x); j++) {

                dist = (int)(Mathf.Abs(i - this.units[0].position.y) + Mathf.Abs(j - this.units[0].position.x));

                if (bestDistance == -1 || dist < bestDistance) {
                    bestDistance = dist;
                    position = new Vector2(j, i);
                }
            }
        }

        for (i = (int)(units[0].position.y - units[0].range); i <= (int)(units[0].position.y + units[0].range); i++) {
            for (j = (int)(units[0].position.x - units[0].range); j <= (int)(units[0].position.x + units[0].range); j++) {

                dist = (int)(Mathf.Abs(i - units[0].position.y) + Mathf.Abs(j - units[0].position.x));

                if (dist <= units[0].range && position.y == i && position.x == j) {
                    return true;
                }
            }
        }

        return false;
    }

    public override void Execute() {

        if(building.isAttacked) {
            
            if(this.AUO == null) {
                this.AUO = new AttackUnitOrder(this.units, GameController.players[building.idPlayer].action.GetUnitsEnemyIsAttacking(this.building), this.isConcentrated);
            }

            this.AUO.Execute();
        } else {

            this.AUO = null;

            if(CheckDistance()) {

                this.MO = null;

                foreach (Unit unit in this.units) {
                    unit.isBusy = false;
                    unit.isWalking = false;
                }

            } else {

                if (this.MO == null){
                    this.MO = new MovementOrder(this.idPlayer, this.units, this.building.position, false, false);
                }

                this.MO.Execute();
            }
            
        }

    }

    public Order Clone() {

        DefenseOrder DO = new DefenseOrder();

        DO.idPlayer = this.idPlayer;
        DO.isActive = this.isActive;
        DO.units = this.units;
        DO.building = this.building;
        DO.isConcentrated = this.isConcentrated;
        DO.AUO = this.AUO;
        DO.MO = this.MO;

        return DO;
    }

}
