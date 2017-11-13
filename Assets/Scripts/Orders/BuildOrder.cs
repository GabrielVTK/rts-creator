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
            this.isActive = false;
            return;
        }

        if (this.workers[0].isWalking || this.workers[0].isBusy) {
            List<Unit> units = new List<Unit>();
            foreach(Unit unit in this.workers) {
                units.Add(unit);
            }
            
            RemoveAnotherOrder(units);
            
        }
        
        foreach (Worker worker in this.workers) {
            worker.isBusy = true;
        }

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

        this.timeCounter += GameController.newUnitTime * GameController.staticGameSpeed;

        return true;
    }

    public override void Execute() {
        
        if(this.isActive && this.CheckDistance()) {

            if(this.movementOrder != null) {
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

                foreach (Worker worker in this.workers) {
                    worker.isBusy = false;
                }

                if (this.building.producedMaterial != null) {
                    GameController.players[this.building.idPlayer].standbyOrders.Add(new ProduceMaterialOrder(this.idPlayer, this.building));
                }

            }                

            GameObject.Find("GameController").GetComponent<GameController>().DrawViewContent();
            GameObject.Find("GameController").GetComponent<GameController>().DrawViewInfo();

        } else {
            
            if(this.movementOrder == null) {

                List<Unit> units = new List<Unit>();

                foreach(Worker worker in workers) {
                    units.Add(worker);
                }

                this.movementOrder = new MovementOrder(this.idPlayer, units, building.position, true, false);
            }

            this.movementOrder.Execute();
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
