using System.Collections.Generic;
using UnityEngine;

public class BaseMaterial {

	public string name;
	public Sprite icon;
	private static List<BaseMaterial> instances;

	private BaseMaterial(string name, Sprite icon) {
		this.name = name;
		this.icon = icon;
	}

	public static void InstantiateBaseMaterial(string name, Sprite icon){
		
		if(instances == null) {
			instances = new List<BaseMaterial>();
		}

		instances.Add(new BaseMaterial(name, icon));
	}

	public static BaseMaterial GetInstance(string name){
		int i;

		for(i = 0; i < instances.Count; i++) {
			if(instances[i].name == name) {
                return instances[i];
            }
		}

        return null;
    }

}