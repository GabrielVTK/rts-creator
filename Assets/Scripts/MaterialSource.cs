using UnityEngine;

public class MaterialSource : MapComponent {

	public long quantityTotal;
	public long quantity;
	public BaseMaterial baseMaterial;
    public bool canBuild;
    public Building building;

	public MaterialSource(string name, GameObject model, BaseMaterial baseMaterial, long quantityTotal) : base(name, model) {
		this.baseMaterial = baseMaterial;
		this.quantityTotal = quantityTotal;
		this.quantity = quantityTotal;
        this.canBuild = true;
        this.building = null;
	}

}
