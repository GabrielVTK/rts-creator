using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMaterialDao {

    public static int ID = 0;

    public int id;
    public string name;
	public string icon;

	public BaseMaterialDao() {}

	public BaseMaterialDao(string name, string icon) {
        this.id = BaseMaterialDao.ID++;
        this.name = name;
		this.icon = icon;
	}

	public BaseMaterial Instantiate() {
		return BaseMaterial.InstantiateBaseMaterial(id, name, Resources.Load(icon, typeof(Sprite)) as Sprite);
    }
}
