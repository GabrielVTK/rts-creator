using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackOrder : Order {

    public List<Unit> units;
    public MovementOrder movementOrder;
    protected float timeCounter;

    protected AttackOrder(List<Unit> units) {
       
        if(units.Count == 0) {
            this.isActive = false;
            return;
        }

        this.idPlayer = units[0].idPlayer;
        this.units = new List<Unit>();
        this.movementOrder = null;
        this.timeCounter = 0;

        foreach (Unit unit in units) {
            this.units.Add(unit);
        }

        if (this.units[0].isWalking || this.units[0].isBusy) {
            RemoveAnotherOrder(this.units);
        }

        GameController.players[(this.units[0].id == 0 ? 1 : 0)].enemyAttackOrders.Add(this);
    }
   
    public abstract Order Clone();

}
