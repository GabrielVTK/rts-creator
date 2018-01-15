using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDao {

    public static int ID = 0;

    public int id;
    public string name;
	public string icon;
	public string model;
	public List<int> units;
	public List<int> technologies;
	public Dictionary<int, int> cost;
	public Vector2 size;
	public float lifeTotal;

	public int visionField;

	public List<string> requireds;

	public BaseMaterialDao producedMaterial;
	public int producedPerTime;

	public float developTime;
	public bool constructed;

	public BuildingDao() {}

	public BuildingDao(string name, string icon, string model, Vector2 size, float lifeTotal, float developTime, bool constructed, int visionField, BaseMaterialDao producedMaterial = null, int producedPerTime = 0) {
        this.id = BuildingDao.ID++;
        this.name = name;
		this.icon = icon;
		this.model = model;
		this.units = new List<int>();
		this.technologies = new List<int>();
		this.cost = new Dictionary<int, int>();
		this.size = size;
		this.lifeTotal = lifeTotal;
		this.developTime = developTime;
		this.constructed = constructed;
		this.visionField = visionField;
		this.producedMaterial = producedMaterial;
		this.producedPerTime = producedPerTime;
		this.requireds = new List<string>();
	}

	public Building Instantiate() {
        
		BaseMaterial producedBaseMaterial = null;

		if (producedMaterial != null) {
			producedBaseMaterial = producedMaterial.Instantiate();
		}

		return new Building(id, name, Resources.Load(icon, typeof(Sprite)) as Sprite, Resources.Load(model, typeof(GameObject)) as GameObject, units, technologies, cost, size, lifeTotal, developTime, constructed, visionField, requireds, producedBaseMaterial, producedPerTime);
	}

}
