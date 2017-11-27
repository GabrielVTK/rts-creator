using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackOrder : Order {

    public List<Unit> units;
    public MovementOrder movementOrder;

    protected bool needAlertAttack;

    protected AttackOrder(List<Unit> units) {
        this.needAlertAttack = true;

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

        RemoveAnotherOrder(this.units);
    }
   
    public abstract Order Clone();

}
