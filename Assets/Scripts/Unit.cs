using System.Collections.Generic;
using UnityEngine;

public class Unit : Property {

    public GameObject model;
    public Vector2 position;	
	public float lifeTotal;
	public float life;
    public float range;
    public float attack;
	public float defense;
    public int visionField;
    public float walkSpeed;
	public float attackSpeed;
	    
    // Flags
    public bool isBusy;
    public bool isWalking;
	public bool isAttacked;

    public Vector2 positionInitial;
    
	public Unit(string name, Sprite icon, GameObject model, Dictionary<BaseMaterial, int> cost, float attack, float defense, float walkSpeed, float lifeTotal, float range, float attackSpeed, float developTime, int visionField) : base(name, icon, developTime) {
		this.model = model;
		this.cost = cost;
		this.attack = attack;
		this.defense = defense;
		this.walkSpeed = walkSpeed;
		this.lifeTotal = lifeTotal;
		this.life = lifeTotal;
		this.range = range;
		this.visionField = visionField;
		this.attackSpeed = attackSpeed;
        this.isBusy = false;
        this.isWalking = false;
        this.isAttacked = false;
    }

    public bool Walk(Vector2 position) {
        
        if(GameController.map.tiles[(int)position.y, (int)position.x].isWalkable) {
            
            this.DrawMove(position);

            this.position = position;

            return true;
        }

        return false;
    }
    
    public void Attack(Unit target) {
        target.life -= this.attack - target.defense;        
    }

    public bool Attack(Building target) {

        target.life -= this.attack;

        if(target.life > 0) {
            return true;
        }

        return false;
    }

    public void Draw() {

        if(GameController.draw) {
            this.model = GameObject.Instantiate(this.model);
            this.model.name = this.idPlayer + "_unit_" + this.id;

            float column = ((GameController.map.width - 1) / -2.0f) + this.position.x;
            float line = ((GameController.map.height - 1) / 2.0f) - this.position.y;

            this.model.transform.position = new Vector3(column, 0.05f, line);

            if (GameController.draw) {
                int i, j, dist;

                for(i = (int)(this.position.y - this.range); i <= (int)(this.position.y + this.range); i++) {
                    for (j = (int)(this.position.x - this.range); j <= (int)(this.position.x + this.range); j++) {

                        if(i < 0 || i >= GameController.players[this.idPlayer].fog.tiles.GetLength(0) ||
                            j < 0 || j >= GameController.players[this.idPlayer].fog.tiles.GetLength(1)) {
                            continue;
                        }

                        dist = (int)(Mathf.Abs(i - this.position.y) + Mathf.Abs(j - this.position.x));
                        
                        if(dist <= this.range) {
                            GameController.players[this.idPlayer].fog.tiles[i, j].Destroy();
                        }

                    }
                }

            }

        }       

    }

    public void DrawMove(Vector2 position) {
        
        if(GameController.draw && this.model != null) {
            float column = Mathf.Ceil(position.x - this.position.x);
            float line = Mathf.Ceil(position.y - this.position.y);

            this.model.transform.Translate(column, 0, -line);
        }

    }

    public void Destroy() {
        GameController gameController = GameController.instance.GetComponent<GameController>();

        GameController.players[this.idPlayer].population--;
        gameController.RemoveSelectedUnit(this);
        GameController.players[this.idPlayer].RemoveProperty(this);
        gameController.DestroyGameObject(this.model);
        this.model = null;
        gameController.UpdateInfo();
    }
        
}
