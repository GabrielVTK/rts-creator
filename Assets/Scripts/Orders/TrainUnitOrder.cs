using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainUnitOrder : Order {

    private Player player;
    public List<Unit> units;
    private Building building;

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

    public override bool Cooldown() {

        if(this.units.Count == 0) {
            this.isActive = false;
            this.building.trainUnitOrder = null;
            GameController.instance.GetComponent<GameController>().DrawInfoMaterials();
            return true;
        }

        if (this.timeCounter >= GameController.newUnitTime * this.units[0].developTime) {
            this.timeCounter = 0;
            return false;
        }

        this.timeCounter += GameController.newUnitTime;

        return true;
    }

    public override void Execute() {
                
        this.units[0].idPlayer = this.player.id;

        if (this.units[0].GetType() == typeof(Worker)) {
            Worker worker = (Worker)this.units[0];
            worker.returnBuilding = this.building;
        }

        this.units[0].position = new Vector2(this.building.position.x - 1.0f, this.building.position.y - 1.0f);
        this.units[0].positionInitial = this.units[0].position;
        this.units[0].Draw();

        this.player.units.Add(this.units[0].id, this.units[0]);
        GameController.map.tiles[(int)this.units[0].position.y, (int)this.units[0].position.x].units.Add(this.units[0]);

        this.units.RemoveAt(0);
    }

}
