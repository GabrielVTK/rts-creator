using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUnitOrder : AttackOrder {

    public List<Unit> targets;
    private bool isConcentrated;
    private int targetIndex;

    private AttackUnitOrder() : base(new List<Unit>()) {}

    public AttackUnitOrder(List<Unit> units, List<Unit> targets, bool isConcentrated) : base(units) {
        this.targets = new List<Unit>();
        this.isConcentrated = isConcentrated;
        this.targetIndex = -1;
        
        foreach (Unit target in targets) {
            this.targets.Add(target);
        }

        if(units.Count == 0 || targets.Count == 0) {
            return;
        }

        GameController.players[this.idPlayer].orderAttackCount++;
    }

    private bool CheckDistance() {

        if (this.targets.Count == 0){
            GameController.players[(this.units[0].idPlayer == 0 ? 1 : 0)].enemyAttackOrders.Remove(this);
            this.isActive = false;
            return false;
        }

        int i, j, dist;

        for (i = (int)(units[0].position.y - units[0].range); i <= (int)(units[0].position.y + units[0].range); i++) {
            for (j = (int)(units[0].position.x - units[0].range); j <= (int)(units[0].position.x + units[0].range); j++) {

                dist = (int)(Mathf.Abs(i - units[0].position.y) + Mathf.Abs(j - units[0].position.x));

                if (dist <= units[0].range && targets[0].position.y == i && targets[0].position.x == j) {
                    return true;
                }
            }
        }

        return false;
    }

    private Unit GetTarget() {

        if(!this.isConcentrated) {

            Unit target = null;
            int i = 0;

            do {

                this.targetIndex++;

                if (this.targetIndex >= this.targets.Count) {
                    this.targetIndex = 0;
                }

                if (this.targets[this.targetIndex].life > 0) {
                    target = this.targets[this.targetIndex];
                }

                i++;
            } while (target == null && i < this.targets.Count);

            return target;
        }

        return (this.targets[0].life > 0) ? this.targets[0] : null;
    }

    public override bool Cooldown() {

        if (this.units.Count == 0 || this.targets.Count == 0) {
            GameController.players[(this.idPlayer == 0 ? 1 : 0)].enemyAttackOrders.Remove(this);
            this.isActive = false;

            foreach (Unit unit in this.targets) {
                unit.isAttacked = false;
            }

            if (!this.isActive && units.Count > 0 && units[0].position != units[0].positionInitial) {
                GameController.players[this.idPlayer].standbyOrders.Add(new MovementOrder(this.idPlayer, units, units[0].positionInitial, false, true, false));
            }

            return true;
        }

        if (this.timeCounter >= GameController.newUnitTime) {
            this.timeCounter -= GameController.newUnitTime;
            return false;
        }

        this.timeCounter += GameController.newUnitTime * this.units[0].attackSpeed;

        return true;
    }

    public override void Execute() {

        if (!this.isActive) {
            return;
        }

        if(this.CheckDistance()) {

            if(this.movementOrder != null) {
                foreach (Unit unit in this.units) {
                    unit.isWalking = false;
                }
                this.movementOrder = null;
            }

            Unit target;

            foreach(Unit unit in this.units) {

                target = this.GetTarget();

                if(target == null) {
                    GameController.players[(this.units[0].idPlayer == 0 ? 1 : 0)].enemyAttackOrders.Remove(this);
                    this.isActive = false;
                    break;
                }

                target.isAttacked = true;

                unit.Attack(target);

                if(target.life <= 0) {
                    this.targets.Remove(target);
                    GameController.players[target.idPlayer].propertiesDestroied.Add(target);
                    target = null;
                }

                if(this.targets.Count == 0) {
                    GameController.players[(this.units[0].idPlayer == 0 ? 1 : 0)].enemyAttackOrders.Remove(this);
                    this.isActive = false;
                    break;
                }

            }

            if (!this.isActive && units.Count > 0 && units[0].position != units[0].positionInitial) {
                GameController.players[this.idPlayer].standbyOrders.Add(new MovementOrder(this.idPlayer, units, units[0].positionInitial, false, true, false));
            }

        } else {
            
            if(this.movementOrder == null || this.movementOrder.destiny != this.targets[0].position) {
                this.movementOrder = new MovementOrder(this.idPlayer, this.units, targets[0].position, true, false);
            }

            if(!this.movementOrder.Cooldown()) {
                this.movementOrder.Execute();
            }

        }

    }

    public override Order Clone() {

        AttackUnitOrder AUO = new AttackUnitOrder();

        AUO.idPlayer = this.idPlayer;
        AUO.isActive = this.isActive;
        AUO.units = this.units;
        AUO.targets = this.targets;
        AUO.isConcentrated = this.isConcentrated;
        AUO.targetIndex = this.targetIndex;
        AUO.movementOrder = this.movementOrder;
        AUO.timeCounter = this.timeCounter;

        return AUO;
    }

}
