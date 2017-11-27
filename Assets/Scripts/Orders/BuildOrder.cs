using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOrder : Order {

    public List<Worker> workers;
    public Building building;
    private MovementOrder movementOrder;

    private BuildOrder() {}

    public BuildOrder(int idPlayer, List<Worker> workers, Building building) {
        this.idPlayer = idPlayer;
        this.workers = new List<Worker>();
        this.building = building;
        this.movementOrder = null;
        this.timeCounter = 0;

        foreach(Worker worker in workers) {
            this.workers.Add(worker);
        }

        if (this.workers.Count == 0 || building == null) {
            Debug.Log("BuildOrder desativada por falta de trabalhador ou building inexistente.");
            this.isActive = false;
            return;
        }
        
        foreach (Worker worker in this.workers) {
            worker.isBusy = true;
        }

        List<Unit> units = new List<Unit>();
        foreach (Unit unit in this.workers) {
            units.Add(unit);
        }

        RemoveAnotherOrder(units);
    }

    private bool CheckDistance() {

        int i, j, bestDistance = -1, dist;
        Vector2 position = building.position;

        for (i = (int)this.building.position.y; i < (int)(this.building.position.y + this.building.size.y); i++) {
            for (j = (int)this.building.position.x; j < (int)(this.building.position.x + this.building.size.x); j++) {

                dist = (int)(Mathf.Abs(i - this.workers[0].position.y) + Mathf.Abs(j - this.workers[0].position.x));

                if (bestDistance == -1 || dist < bestDistance) {
                    bestDistance = dist;
                    position = new Vector2(j, i);
                }
            }
        }

        for (i = (int)(workers[0].position.y - workers[0].range); i <= (int)(workers[0].position.y + workers[0].range); i++) {
            for (j = (int)(workers[0].position.x - workers[0].range); j <= (int)(workers[0].position.x + workers[0].range); j++) {

                dist = (int)(Mathf.Abs(i - workers[0].position.y) + Mathf.Abs(j - workers[0].position.x));

                if (dist <= workers[0].range && position.y == i && position.x == j) {
                    return true;
                }
            }
        }

        return false;
    }

    public override bool Cooldown() {

        if(this.timeCounter >= GameController.newUnitTime) {
            this.timeCounter -= GameController.newUnitTime;
            return false;
        }

        this.timeCounter += GameController.newUnitTime;

        return true;
    }

    public override void Execute() {

        if (!this.isActive) {
            return;
        }

        if(this.CheckDistance()) {

            //Debug.Log("P" + this.idPlayer + ": Constroi building");

            if (this.movementOrder != null) {
                foreach(Worker worker in this.workers) {
                    worker.isWalking = false;
                }
                this.movementOrder = null;
            }
            
            foreach(Worker worker in this.workers) {
                if(!worker.Build(this.building)) {
                    break;
                }
            }
                
            if(this.building.constructed) {
                this.isActive = false;
                this.building.RemoveFog();

                List<Unit> units = new List<Unit>();
                foreach (Worker worker in this.workers) {
                    worker.isBusy = false;
                    units.Add((Unit)worker);
                }

                if (!this.isActive && units.Count > 0 && units[0].position != units[0].positionInitial) {
                    GameController.players[this.idPlayer].standbyOrders.Add(new MovementOrder(this.idPlayer, units, units[0].positionInitial, false, true, false));
                }

                if (this.building.producedMaterial != null) {
                    GameController.players[this.idPlayer].standbyOrders.Add(new ProduceMaterialOrder(this.idPlayer, this.building));
                }

            }                

            GameController.instance.GetComponent<GameController>().DrawViewContent();
            GameController.instance.GetComponent<GameController>().DrawViewInfo();

        } else {

            //Debug.Log("P"+this.idPlayer+": Move ate building");

            if(this.movementOrder == null || this.movementOrder.movements.Count == 0) {

                //Debug.Log("P" + this.idPlayer + ": Nova Order de movimento!");

                List<Unit> units = new List<Unit>();

                foreach(Worker worker in workers) {
                    units.Add(worker);
                }

                this.movementOrder = new MovementOrder(this.idPlayer, units, building.position, true, false, true, true);
            } 

            if (!this.movementOrder.Cooldown()) {
                this.movementOrder.Execute();
            }
        }

    }
    
    public Order Clone() {

        BuildOrder BO = new BuildOrder();

        BO.idPlayer = this.idPlayer;
        BO.isActive = this.isActive;
        BO.workers = this.workers;
        BO.building = this.building;
        BO.movementOrder = this.movementOrder;
        BO.timeCounter = this.timeCounter;

        return BO;
    }
    
}
