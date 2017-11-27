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
            Debug.Log("Ordem de defesa invalida!");
            this.isActive = false;
            return;
        }

        foreach (Unit unit in units) {
            this.units.Add(unit);
        }

        Debug.Log("Defende " + this.building.name + "("+this.building.position+")");
        
        RemoveAnotherOrder(this.units);
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

    public override bool Cooldown() {

        if (this.units.Count == 0 || this.building == null) {
            this.isActive = false;
            return true;
        }

        return false;
    }

    public override void Execute() {

        /**/
        if (building.isAttacked) {
        /**/
            Debug.Log("P"+this.idPlayer+": DefenseOrder: Ataca inimigos!");
            
            if (this.AUO == null) {

                Debug.Log("Nova ordem de ataque!");

                List<Unit> targets = GameController.players[building.idPlayer].action.GetUnitsEnemyIsAttacking(this.building);

                if (targets.Count > 0) {
                    this.AUO = new AttackUnitOrder(this.units, targets, this.isConcentrated, true);
                } else {
                    Debug.Log("P" + this.idPlayer + ": DefenseOrder: Sem alvos!");
                    this.isActive = false;
                    return;
                }
                
            }

            if(!this.AUO.Cooldown()) {
                this.AUO.Execute();
            }
            
            if(!this.AUO.isActive) {
                Debug.Log("P" + this.idPlayer + ": DefenseOrder: Desativa ordem!");
                this.isActive = false;
            }
        /**/
        } else {

            Debug.Log("DefenseOrder: protege building!");

            this.AUO = null;

            if(CheckDistance()) {

                this.MO = null;

                foreach (Unit unit in this.units) {
                    unit.isBusy = false;
                    unit.isWalking = false;
                }

            } else {

                if (this.MO == null){
                    this.MO = new MovementOrder(this.idPlayer, this.units, this.building.position, false, false, true, true);
                }

                if (!this.MO.Cooldown()) {
                    this.MO.Execute();
                }

            }
            
        }
        /**/

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
