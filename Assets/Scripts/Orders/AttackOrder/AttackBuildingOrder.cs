using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuildingOrder : AttackOrder {

    public Building target;
    private Vector2 destiny;
    private bool subOrder;

    private AttackBuildingOrder() : base(new List<Unit>()) {}

	public AttackBuildingOrder(List<Unit> units, Building target, bool subOrder = false) : base(units) {
        this.target = target;
        this.destiny = this.GetBestPosition();
        this.subOrder = subOrder;

        if(units.Count == 0) {
            Debug.Log("AttackBuildingOrder => nenhuma unidade");
            return;
        }

        GameController.players[this.idPlayer].orderAttackCount++;
    }

    private Vector2 GetBestPosition() {

        Vector2 position = this.target.position;

        if (units.Count > 0) {
            
            int i, j, dist, bestDistance = -1;
            
            for (i = (int)target.position.y; i < (int)(target.position.y + target.size.y); i++) {
                for (j = (int)target.position.x; j < (int)(target.position.x + target.size.x); j++) {

                    dist = (int)(Mathf.Abs(i - units[0].position.y) + Mathf.Abs(j - units[0].position.x));

                    if (bestDistance == -1 || dist < bestDistance) {
                        bestDistance = dist;
                        position = new Vector2(j, i);
                    }
                }
        }

        }

        return position;
    }

    private bool CheckDistance() {

        int i, j, dist, bestDistance = -1;
        Vector2 position = target.position;

        for (i = (int)target.position.y; i < (int)(target.position.y + target.size.y); i++) {
            for (j = (int)target.position.x; j < (int)(target.position.x + target.size.x); j++) {

                dist = (int)(Mathf.Abs(i - units[0].position.y) + Mathf.Abs(j - units[0].position.x));

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

        /**/
        if (this.units.Count == 0 || this.target == null) {
            GameController.players[(this.idPlayer == 0 ? 1 : 0)].enemyAttackOrders.Remove(this);
            this.target.isAttacked = false;
            this.isActive = false;
            return true;
        }

        if (!this.isActive) {

            if (units.Count > 0 && units[0].position != units[0].positionInitial && !this.subOrder) {
                //GameController.players[this.idPlayer].standbyOrders.Add(new MovementOrder(this.idPlayer, units, units[0].positionInitial, false, true, false));
            }

            return true;
        }
        /**/

        if (this.timeCounter >= GameController.newUnitTime) {
            this.timeCounter -= GameController.newUnitTime;
            return false;
        }

        this.timeCounter += GameController.newUnitTime * this.units[0].attackSpeed;

        return true;
    }
    
    public override void Execute() {

        if(this.CheckDistance()) {

            if(this.movementOrder != null) {
                foreach (Unit unit in this.units) {
                    unit.isWalking = false;
                }
                this.movementOrder = null;
            }
            
            target.isAttacked = true;

            if(this.needAlertAttack) {
                this.needAlertAttack = false;
                GameController.players[(this.idPlayer == 0 ? 1 : 0)].enemyAttackOrders.Add(this);
            }

            foreach(Unit unit in this.units) {
                if(!unit.Attack(target)) {
                    GameController.players[this.target.idPlayer].enemyAttackOrders.Remove(this);
                    GameController.players[this.target.idPlayer].propertiesDestroied.Add(target);
                    this.isActive = false;
                    break;
                }
            }

            if (!this.isActive && units.Count > 0 && units[0].position != units[0].positionInitial && !this.subOrder) {
                GameController.players[this.idPlayer].standbyOrders.Add(new MovementOrder(this.idPlayer, units, units[0].positionInitial, false, true, false));
            }

        } else {

            if (this.movementOrder == null || this.movementOrder.movements.Count == 0) {
                this.movementOrder = new MovementOrder(this.idPlayer, this.units, this.destiny, true, false, true, true);
            }

            if(!movementOrder.Cooldown()) {
                this.movementOrder.Execute();
            }
            
        }

    }
    
    public override Order Clone() {

        AttackBuildingOrder ABO = new AttackBuildingOrder();

        ABO.idPlayer = this.idPlayer;
        ABO.isActive = this.isActive;
        ABO.units = this.units;
        ABO.movementOrder = this.movementOrder;
        ABO.timeCounter = this.timeCounter;
        ABO.target = this.target;
        ABO.destiny = this.destiny;

        return ABO;
    }

}
