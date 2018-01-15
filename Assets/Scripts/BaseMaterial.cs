using System.Collections.Generic;
using UnityEngine;

public class BaseMaterial {

    public int idType;

	public string name;
	public Sprite icon;
	private static Dictionary<int, BaseMaterial> instances;

	private BaseMaterial(int idType, string name, Sprite icon) {
        this.idType = idType;
		this.name = name;
		this.icon = icon;
	}

	public static BaseMaterial InstantiateBaseMaterial(int idType, string name, Sprite icon){
		
		if(instances == null) {
			instances = new Dictionary<int, BaseMaterial>();
		}

        if(!instances.ContainsKey(idType)) {
            instances.Add(idType, new BaseMaterial(idType, name, icon));
        }
        
        return instances[idType];
    }

	public static BaseMaterial GetInstance(int id){
        return instances[id];
    }

}