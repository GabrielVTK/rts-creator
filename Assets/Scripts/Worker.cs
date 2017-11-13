using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit {

	public int capacity;
	public int capacityTotal;
	public float collectSpeed;
    public Building returnBuilding;

    // CanBuild
    public List<BuildingDao> buildings;

    // Build
    //public Building targetBuilding;

    // Collect
    public float collectTimeCounter;
    public BaseMaterial materialType;
    //public MaterialSource targetSource;

    public Worker(string name, Sprite icon, GameObject model, Dictionary<BaseMaterial, int> cost, float attack, float defense, float walkSpeed, float lifeTotal, int capacityTotal, float collectSpeed, List<BuildingDao> buildings, float range, float attackSpeed, float trainingTime, int visionField) : base(name, icon, model, cost, attack, defense, walkSpeed, lifeTotal, range, attackSpeed, trainingTime, visionField) {
		this.capacity = 0;
		this.capacityTotal = capacityTotal;
		this.collectSpeed = collectSpeed;
        this.requiredBuilding = null;
        this.buildings = buildings;
        //this.targetBuilding = null;		
		this.collectTimeCounter = 0.0f;
        this.materialType = null;
       //this.targetSource = null;
    }

	/**public void CollectMaterial() {

		if (this.materialType != null) {

			if(this.targetSource != null) {

				if(this.materialType != this.targetSource.baseMaterial) {
                    
					if (this.isWalking) { return; }

					GameController.players [this.idPlayer - 1].baseMaterials [this.materialType] += this.capacity;
					GameObject.Find ("GameController").GetComponent<GameController> ().DrawInfoMaterials ();

					this.collectTimeCounter = 0;
					this.capacity = 0;

					this.materialType = this.targetSource.baseMaterial;
				}
			}

			if (this.capacity >= this.capacityTotal || this.targetSource == null || this.targetSource.quantity == 0) {

				if (this.isWalking) { return; }

				GameController.players [this.idPlayer].baseMaterials [this.materialType] += this.capacity;
				GameObject.Find ("GameController").GetComponent<GameController> ().DrawInfoMaterials ();

				this.capacity = 0;

				if (this.targetSource == null) {
					this.materialType = null;
					this.collectTimeCounter = 0;
				} else if (this.targetSource.quantity == 0) {
					this.targetSource = null;
					this.materialType = null;
					this.collectTimeCounter = 0;
				}

			} else {

				if (this.isWalking) { return; }

				if (this.collectTimeCounter < GameController.unitTime) {
					this.collectTimeCounter += Time.deltaTime * collectSpeed;
				} else {
					this.targetSource.quantity--;
					this.capacity++;
					this.collectTimeCounter -= GameController.unitTime;
				}

				if (this.targetSource.quantity == 0) {
					GameObject.Find ("GameController").GetComponent<GameController> ().DestroySource (this.targetSource);
					this.targetSource = null;
				}

			}

		} else  if(this.targetSource != null) {
			this.materialType = this.targetSource.baseMaterial;
		}

	}/**/

	/**public BaseMaterial ReturnMaterial(Building target) {
		return materialType;
	}/**/

    public bool Build(Building building) {

        if(!building.constructed && building.life < building.lifeTotal) {
            
            building.life += building.lifeTotal / building.developTime;

            if (building.life > building.lifeTotal) {
                building.constructed = true;
                building.life = building.lifeTotal;
            }

            GameObject.Find("GameController").GetComponent<GameController>().DrawViewInfo();
            
            return true;
        }
        
        return false;
    }

    /**public void Build() {

		if (!this.targetBuilding.constructed) {

			bool needWalk = true;

			for (int i = (int)this.targetBuilding.position.y - 1; i <= (int)this.targetBuilding.position.y + (int)this.targetBuilding.size.y; i++) {

				for (int j = (int)this.targetBuilding.position.x - 1; j <= (int)this.targetBuilding.position.x + (int)this.targetBuilding.size.x; j++) {
                    if ((int)this.position.y == i && (int)this.position.x == j) {
						needWalk = false;
						break;
					}
				}
			}

			if (needWalk) {
                //this.Walk(targetBuilding.position, targetBuilding);
			} else {

				float unitTime = GameController.unitTime;

				if (this.timeCounter < unitTime) {
					this.timeCounter += Time.deltaTime;
				} else {

					this.timeCounter -= unitTime;

					//if (this.targetBuilding.constructionTime > this.targetBuilding.timeCounter) {
					//this.targetBuilding.timeCounter += Time.deltaTime;
					if(!this.targetBuilding.constructed && this.targetBuilding.life < this.targetBuilding.lifeTotal) {
						this.targetBuilding.life += this.targetBuilding.lifeTotal / this.targetBuilding.developTime;

						if(this.targetBuilding.life > this.targetBuilding.lifeTotal) {
							this.targetBuilding.life = this.targetBuilding.lifeTotal;
						}

						GameObject.Find("GameController").GetComponent<GameController>().DrawViewInfo();

					} else {

						this.targetBuilding.RemoveFog(true);

						this.targetBuilding.constructed = true;
						this.targetBuilding = null;

						GameObject.Find("GameController").GetComponent<GameController>().DrawViewContent();
						GameObject.Find("GameController").GetComponent<GameController>().DrawViewInfo();
					}

				}

			}

		} else if (this.targetBuilding.life < this.targetBuilding.lifeTotal) {
			this.targetBuilding.life++;
		} else {
			this.targetBuilding = null;
		}

	}/**/
}
