using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementOrder : Order {

    public List<Unit> units;
    private List<Vector2> movements;
    public Vector2 destiny;
    private bool rangeConsidered;

    private MovementOrder() {}

    public MovementOrder(int idPlayer, List<Unit> units, Vector2 destiny, bool rangeConsidered, bool checkAnotherOrder = true) {
        this.idPlayer = idPlayer;
        this.units = new List<Unit>();
        this.destiny = destiny;
        this.timeCounter = 0;
        this.rangeConsidered = rangeConsidered;

        foreach (Unit unit in units) {
            unit.isWalking = true;
            this.units.Add(unit);
        }

        if(units.Count == 0) {
            this.isActive = false;
            return;
        }

        if(checkAnotherOrder && (this.units[0].isWalking || this.units[0].isBusy)) {
            RemoveAnotherOrder(this.units);
        }
        
        this.NewPath();
    }
    
    private void NewPath() {
        Node origin = new Node(units[0].position);
        Node destiny = new Node(this.destiny);
        
        Pathfinding pathfinding = new Pathfinding(origin, destiny, units[0], rangeConsidered);

        this.movements = pathfinding.GetPath();        
    }

    public override bool Cooldown() {

        if (this.units.Count == 0 || this.movements.Count == 0) {
            this.isActive = false;
            return true;
        }

        if (this.timeCounter >= GameController.newUnitTime) {
            this.timeCounter -= GameController.newUnitTime;
            return false;
        }

        this.timeCounter += GameController.newUnitTime * GameController.staticGameSpeed * this.units[0].walkSpeed / GameController.map.tiles[(int)this.movements[0].y, (int)this.movements[0].x].terraineType;
        return true;
    }

    public override void Execute() {
        
        if (!this.isActive || this.movements.Count == 0 || this.units.Count == 0) {

            foreach (Unit unit in this.units) {
                unit.isWalking = false;
            }

            this.isActive = false;
            return;
        }

        Map map = GameController.map;
        
        bool walk = true, discovered = false;

        foreach(Unit unit in this.units) {
            if(!unit.Walk(this.movements[0])) {
                walk = false;
                break;
            }
        }

        if(walk) {

            int x, y;

            for(x = (int)this.units[0].position.x - this.units[0].visionField; x <= (int)this.units[0].position.x + this.units[0].visionField; x++) {
                for(y = (int)this.units[0].position.y - this.units[0].visionField; y <= (int)this.units[0].position.y + this.units[0].visionField; y++) {

                    if (x >= 0 && x < map.width && y >= 0 && y < map.height) {
                        
                        int dist = Mathf.Abs(x - (int)this.units[0].position.x) + Mathf.Abs(y - (int)this.units[0].position.y);

                        if (GameController.players[this.units[0].idPlayer].fog.tiles[y, x].unknown && dist <= this.units[0].visionField) {
                            GameController.players[this.units[0].idPlayer].fog.tiles[y, x].Destroy();
                            discovered = true;
                        }
                    }
                }
            }

        }

        if (walk && !discovered) {
            this.movements.RemoveAt(0);
        } else {
            this.NewPath();
        }

    }
    
    public Order Clone() {

        MovementOrder MO = new MovementOrder();

        MO.idPlayer = this.idPlayer;
        MO.isActive = this.isActive;
        MO.units = this.units;
        MO.movements = this.movements;
        MO.destiny = this.destiny;
        MO.timeCounter = this.timeCounter;
        MO.rangeConsidered = this.rangeConsidered;

        return MO;
    }

}
