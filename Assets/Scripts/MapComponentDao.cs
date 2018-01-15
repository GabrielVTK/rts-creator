using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponentDao {

    public static int ID = 0;

    public int id;
    public string name;
	public string model;

	public MapComponentDao() {}

	public MapComponentDao(string name, string model) {
        this.id = MapComponentDao.ID++;
		this.name = name;
		this.model = model;
	}

	public virtual MapComponent Instantiate() {
		return new MapComponent(name, Resources.Load(model, typeof(GameObject)) as GameObject);
	}

}
