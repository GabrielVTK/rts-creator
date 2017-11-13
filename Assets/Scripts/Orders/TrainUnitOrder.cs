using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainUnitOrder : Order {

    private Player player;
    public List<Unit> units;
    private Building building;
    private float timeCounter;

    public TrainUnitOrder(Player player, Building building) {
        this.idPlayer = player.id;
        this.player = player;
        this.building = building;
        this.units = new List<Unit>();
        this.timeCounter = 0;
    }

    public void AddUnit(Unit unit) {
        this.units.Add(unit);
    }

    public override void Execute() {
        
        if(this.units.Count == 0) {
            this.isActive = false;
            this.building.trainUnitOrder = null;
            GameObject.Find("GameController").GetComponent<GameController>().DrawInfoMaterials();
            return;
        }

        if(this.timeCounter < GameController.unitTime * this.units[0].developTime) {
            this.timeCounter += Time.deltaTime;
        } else {
            
            this.units[0].idPlayer = this.player.id;

            if (this.units[0].GetType() == typeof(Worker)) {
                Worker worker = (Worker)this.units[0];
                worker.returnBuilding = this.building;
            }

            this.units[0].position = new Vector2(this.building.position.x - 1.0f, this.building.position.y - 1.0f);
            this.units[0].Draw();

            this.player.units.Add(this.units[0].id, this.units[0]);

            this.units.RemoveAt(0);
            
            this.timeCounter = 0;
        }

    }

}
