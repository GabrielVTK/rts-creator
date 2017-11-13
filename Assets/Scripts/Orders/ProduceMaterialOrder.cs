using UnityEngine;

public class ProduceMaterialOrder : Order {

    public Building building;
    public BaseMaterial baseMaterial;
    private float timeCounter;

    public ProduceMaterialOrder(int idPlayer, Building building) {
        this.idPlayer = idPlayer;
        this.building = building;
        this.baseMaterial = building.producedMaterial;
        this.timeCounter = 0;
    }

    public override void Execute() {
        
        if(!this.isActive || !this.building.constructed) {
            return;
        }

        if(this.timeCounter < GameController.unitTime) {
            this.timeCounter += Time.deltaTime;
        } else {

            Tile tile = GameController.map.tiles[(int)this.building.position.y, (int)this.building.position.x];

            if(tile.mapComponent != null && tile.mapComponent.GetType() == typeof(MaterialSource)) {

                MaterialSource materialSource = (MaterialSource)tile.mapComponent;

                materialSource.quantity -= this.building.producedPerTime;

                if (materialSource.quantity <= 0) {
                    
                    GameController.players[this.building.idPlayer].baseMaterials[this.baseMaterial] += (int)materialSource.quantity;
                    GameController.players[this.building.idPlayer].resourcesCount += (int)materialSource.quantity;

                    this.building.Destroy();
                    GameController.players[this.building.idPlayer].RemoveProperty(this.building);
                    GameObject.Find("GameController").GetComponent<GameController>().DestroySource(materialSource);

                    this.isActive = false;
                }
                    
            }

            GameController.players[this.building.idPlayer].baseMaterials[this.baseMaterial] += this.building.producedPerTime;
            GameController.players[this.building.idPlayer].resourcesCount += this.building.producedPerTime;

            GameObject.Find("GameController").GetComponent<GameController>().DrawInfoMaterials();

            this.timeCounter -= GameController.unitTime;
        }

    }

}
