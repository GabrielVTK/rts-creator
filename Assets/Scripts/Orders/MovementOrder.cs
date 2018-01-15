using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementOrder : Order {

    public List<Unit> units;
    public List<Vector2> movements;
    public Vector2 destiny;
    private bool rangeConsidered;
    private bool subOrder;

    private MovementOrder() {}

    public MovementOrder(int idPlayer, List<Unit> units, Vector2 destiny, bool rangeConsidered, bool checkAnotherOrder = true, bool setIsWalking = true, bool subOrder = false) {
        this.idPlayer = idPlayer;
        this.units = new List<Unit>();
        this.movements = new List<Vector2>();
        this.destiny = destiny;
        this.timeCounter = 0;
        this.rangeConsidered = rangeConsidered;
        this.subOrder = subOrder;
                
        if(units.Count == 0) {
            Debug.Log("Sem unidades para MovementOrder");
            this.isActive = false;
            return;
        }

        if(units[0].position == destiny) {
            this.isActive = false;
            return;
        }

        foreach (Unit unit in units) {
            unit.isWalking = setIsWalking;
            this.units.Add(unit);
        }

        if (checkAnotherOrder) {
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

        if (this.movements.Count == 0) {
            this.isActive = false;

            foreach (Unit unitItem in this.units) {
                unitItem.isWalking = false;
            }

            if(this.units.Count > 0) {
                //Debug.Log("Retorna para base!");
                GameController.players[this.idPlayer].standbyOrders.Add(new MovementOrder(this.idPlayer, units, units[0].positionInitial, false, true, false));
            }

            return true;
        }

        if(this.units.Count == 0) {
            this.isActive = false;
            return true;
        }
        /**
        if (!this.isActive || this.units.Count == 0 || this.movements.Count == 0) {
            //Debug.Log("Finaliza MovementOrder(" + this.isActive + ", " + this.units.Count + ", " + this.movements.Count +")");

            Unit unit = null;
            Player player = GameController.players[this.idPlayer];

            this.isActive = false;
            
            foreach (Unit unitItem in this.units) {
                unit = unitItem;
                unitItem.isWalking = false;
            }        

            if (!this.isActive && units.Count > 0 && unit.position != unit.positionInitial && !this.subOrder) {
                
            }

            return true;
        }
        /**/

        if (this.timeCounter >= GameController.newUnitTime) {
            this.timeCounter -= GameController.newUnitTime;
            return false;
        }

        this.timeCounter += GameController.newUnitTime * this.units[0].walkSpeed / GameController.map.tiles[(int)this.movements[0].y, (int)this.movements[0].x].terraineType;
        return true;
    }

    public override void Execute() {

        if (this.movements.Count == 0 || this.units.Count == 0) {
            this.isActive = false;
            return;
        }

        Map map = GameController.map;

        bool walk = true;
        Unit unit = this.units[0];
        Player player = GameController.players[unit.idPlayer];
        Vector2 positionOld = unit.position;

        foreach(Unit unitItem in this.units) {
            if(!unitItem.Walk(this.movements[0])) {
                walk = false;
                break;
            }
            map.tiles[(int)positionOld.y, (int)positionOld.x].units.Remove(unitItem);
            map.tiles[(int)this.movements[0].y, (int)this.movements[0].x].units.Add(unitItem);
        }
        
        if(walk) {

            int x, y, dist, visionField = unit.visionField;
            
            for (x = (int)unit.position.x - visionField; x <= (int)unit.position.x + visionField; x++) {
                for(y = (int)unit.position.y - visionField; y <= (int)unit.position.y + visionField; y++) {

                    if (x >= 0 && x < map.width && y >= 0 && y < map.height) {
                        
                        dist = Mathf.Abs(x - (int)unit.position.x) + Mathf.Abs(y - (int)unit.position.y);
                        
                        if (player.fog.tiles[y, x].unknown && dist <= visionField) {
                            player.fog.tiles[y, x].Destroy();
                        }
                        
                    }
                }
            }

        }

        if(this.movements.Count == 0) {
            Debug.Log("Sem movimentos.");
            this.isActive = false;

            foreach(Unit unitItem in this.units) {
                unitItem.isWalking = false;
            }

            if (!this.isActive && units.Count > 0 && unit.position != unit.positionInitial && !this.subOrder) {
                Debug.Log("Volta para base.");
                player.standbyOrders.Add(new MovementOrder(this.idPlayer, units, unit.positionInitial, false, true, false));
            }

            return;
        }

        if (walk) {
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
