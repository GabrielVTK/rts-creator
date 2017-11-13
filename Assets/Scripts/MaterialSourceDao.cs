using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSourceDao : MapComponentDao {

	public long quantityTotal;
	public BaseMaterialDao baseMaterial;

	public MaterialSourceDao() {}

	public MaterialSourceDao(string name, string model, long quantityTotal, BaseMaterialDao baseMaterial) : base(name, model) {
		this.quantityTotal = quantityTotal;
		this.baseMaterial = baseMaterial;
	}

	public override MapComponent Instantiate() {
		return new MaterialSource(name, Resources.Load(model, typeof(GameObject)) as GameObject, baseMaterial.Instantiate(), quantityTotal);
	}

}
