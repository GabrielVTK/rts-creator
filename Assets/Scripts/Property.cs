using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property {

    public static int nextId = 0;

    public int id;
	public int idPlayer;
	public string name;
    public Sprite icon;
    public float developTime;
    public List<Technology> requiredTechnology;
    public Dictionary<BaseMaterial, int> cost;
	public Dictionary<Building, int> requiredBuilding;
    
	public Property(string name, Sprite icon, float developTime) {
        this.id = Property.getId();
        this.name = name;
        this.icon = icon;
        this.developTime = developTime;
        this.requiredBuilding = new Dictionary<Building, int> ();
	}

	public static int getId(){
		nextId++;
		return nextId;
	}

}
