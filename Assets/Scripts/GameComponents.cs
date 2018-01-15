using System.Collections.Generic;
using UnityEngine;
using GameDevWare.Serialization;

public class GameComponents {

	public Map map;
	public int[,] mapTile;
	public int[,] mapComponent;

	public Dictionary<int, BaseMaterialDao> baseMaterials;
	public Dictionary<int, TileDao> tiles;
	public Dictionary<int, UnitDao> units;
	public Dictionary<int, BuildingDao> buildings;
	public Dictionary<int, MapComponentDao> mapComponents;

	public GameComponents() {

		this.tiles = new Dictionary<int, TileDao>();
		this.baseMaterials = new Dictionary<int, BaseMaterialDao>();
		this.units = new Dictionary<int, UnitDao>();
		this.buildings = new Dictionary<int, BuildingDao>();
		this.mapComponents = new Dictionary<int, MapComponentDao>();
                
		//CreateGame();
		LoadGame();
	}

	public void CreateGame() {
		
		// Tiles
		TileDao tile_Grass = new TileDao("Grass", "tile_Grass", true, true, 1.0f);
		TileDao tile_Water = new TileDao("Water", "tile_Water", false, false, 1.0f);
		TileDao tile_Bridge = new TileDao("Bridge", "tile_Bridge", true, false, 1.0f);

		// Base Material
		BaseMaterialDao baseMaterialGold = new BaseMaterialDao("Gold", "Images/material_Gold");
		BaseMaterialDao baseMaterialMeat = new BaseMaterialDao("Meat", "Images/material_Meat");

		// Material Source
		MaterialSourceDao mats_Gold = new MaterialSourceDao("Gold", "mats_Gold", 999999999999999999, baseMaterialGold);

		// Map Component
		MapComponentDao mc_Tree = new MapComponentDao("Tree", "mc_Tree");

		// Buildings
		BuildingDao build_base = new BuildingDao("Base", "Images/build_Base", "build_Base", new Vector2(2.0f, 2.0f), 3000.0f, 0.0f, true, 3);

		BuildingDao build_quarter = new BuildingDao("Quarter", "Images/build_Quarter", "build_Quarter", new Vector2 (1.0f, 1.0f), 1200.0f, 50.0f, false, 1);
		build_quarter.cost.Add(baseMaterialGold.id, 200);

		BuildingDao build_farm = new BuildingDao("Farm", "Images/material_Meat", "build_Farm", new Vector2(1.0f, 1.0f), 500.0f, 30.0f, false, 1, baseMaterialMeat, 1);
		build_farm.cost.Add(baseMaterialGold.id, 75);

		BuildingDao build_mine = new BuildingDao("Mine", "Images/build_Mine", "build_Mine", new Vector2(1.0f, 1.0f), 500.0f, 30.0f, false, 1, baseMaterialGold, 1);
		build_mine.cost.Add(baseMaterialMeat.id, 100);
		build_mine.requireds.Add(mats_Gold.name);

		// Warrior Unit
		CombatDao infantry_unit = new CombatDao("Infantry", "Images/unit_Warrior", "unit_Warrior", 3.0f, 0.0f, 1.0f, 50.0f, 1.0f, 1.0f, new CombatType(), 22.0f, 2);
		infantry_unit.cost.Add(baseMaterialMeat.id, 35);
		infantry_unit.cost.Add(baseMaterialGold.id, 25);

		// Archer Unit
		CombatDao archer_unit = new CombatDao("Archer", "Images/unit_Archer", "unit_Archer", 4.0f, 0.0f, 0.9f, 30.0f, 4.0f, 1.0f, new CombatType(), 30.0f, 2);
		archer_unit.cost.Add(baseMaterialMeat.id, 25);
		archer_unit.cost.Add(baseMaterialGold.id, 45);

		// Archer Unit
		CombatDao cavalry_unit = new CombatDao("Cavalry", "Images/unit_Knight", "unit_Knight", 10.0f, 0.0f, 1.5f, 100.0f, 1.0f, 1.0f, new CombatType(), 35.0f, 2);
		cavalry_unit.cost.Add(baseMaterialMeat.id, 60);
		cavalry_unit.cost.Add(baseMaterialGold.id, 75);

		// Worker Unit
		WorkerDao unit_worker = new WorkerDao("Worker", "Images/unit_Worker", "unit_Worker", 1.0f, 0.0f, 1.0f, 45.0f, 5, 1.0f, 1.0f, 1.0f, 10.0f, 2);
		unit_worker.cost.Add(baseMaterialMeat.id, 10);

		unit_worker.buildings.Add(build_quarter.id);
		unit_worker.buildings.Add(build_farm.id);
		unit_worker.buildings.Add(build_mine.id);


		// Add in tiles list
		tiles.Add(tile_Grass.id, tile_Grass);
		tiles.Add(tile_Water.id, tile_Water);
		tiles.Add(tile_Bridge.id, tile_Bridge);

		// Add in baseMaterial list
		baseMaterials.Add(baseMaterialGold.id, baseMaterialGold);
		baseMaterials.Add(baseMaterialMeat.id, baseMaterialMeat);

		// Add in materialSource list
		mapComponents.Add(mats_Gold.id, mats_Gold);
		mapComponents.Add(mc_Tree.id, mc_Tree);

		// Add in buildings list
		buildings.Add(build_base.id, build_base);
		buildings.Add(build_quarter.id, build_quarter);
		buildings.Add(build_farm.id, build_farm);
		buildings.Add(build_mine.id, build_mine);

		// Add in unit list
		units.Add(infantry_unit.id, infantry_unit);
		units.Add(archer_unit.id, archer_unit);
		units.Add(cavalry_unit.id, cavalry_unit);
		units.Add(unit_worker.id, unit_worker);
        
		// Add in unit list of build_base
		build_base.units.Add(unit_worker.id);
		build_quarter.units.Add(infantry_unit.id);
		build_quarter.units.Add(archer_unit.id);
		build_quarter.units.Add(cavalry_unit.id);
        
		SaveGame();
	}

	public void SaveGame() {
		GameData gameData = new GameData();
        
		// Tiles
		gameData.tiles = Json.SerializeToString<Dictionary<int, TileDao>> (tiles);
        
        // Base Materials
        gameData.baseMaterials = Json.SerializeToString<Dictionary<int, BaseMaterialDao>> (baseMaterials);

        // Material Sources
        gameData.mapComponents = Json.SerializeToString<Dictionary<int, MapComponentDao>>(mapComponents);
        
        // Units
        gameData.units = Json.SerializeToString<Dictionary<int, UnitDao>> (units);
        
        // Buildings
        gameData.buildings = Json.SerializeToString<Dictionary<int, BuildingDao>>(buildings);
        
        GameData.Save(gameData);
	}
    
    public void LoadGame() {
		GameData gameData = GameData.Load();
		GameDataMap gameDataMap = GameDataMap.Load("map");

        this.baseMaterials.Clear();
        this.tiles.Clear();
        this.mapComponents.Clear();
        this.buildings.Clear();
        this.units.Clear();

        // Tiles
        this.tiles = Json.Deserialize<Dictionary<int, TileDao>>(gameData.tiles);

        // Base Materials
        this.baseMaterials = Json.Deserialize<Dictionary<int, BaseMaterialDao>>(gameData.baseMaterials);

		foreach(BaseMaterialDao baseMaterial in baseMaterials.Values) {
			baseMaterial.Instantiate();
		}

        // Material Sources
        this.mapComponents = Json.Deserialize<Dictionary<int, MapComponentDao>>(gameData.mapComponents);

        // Units
        this.units = Json.Deserialize<Dictionary<int, UnitDao>>(gameData.units);

        foreach(UnitDao unit in this.units.Values) {
            Debug.Log(unit.name + " => " + unit.id);
        }

        // Buildings
        this.buildings = Json.Deserialize<Dictionary<int, BuildingDao>>(gameData.buildings);

		// Map
		//map = Json.Deserialize<Map>(gameData.map);
		this.map = JsonUtility.FromJson<Map>(gameDataMap.map);

        this.mapTile = new int[map.height, map.width];
        this.mapComponent = new int[map.height, map.width];

        // Map Tiles
        //mapTile = Json.Deserialize<int[,]>(gameData.mapTile);
        int[] tilesList = Json.Deserialize<int[]>(gameDataMap.mapTile);

		//Map Components
		//mapComponent = Json.Deserialize<int[,]>(gameData.mapComponent);
		int[] componentsList = Json.Deserialize<int[]>(gameDataMap.mapComponent);

		int height = (this.map.isReflected && (this.map.directionReflected == 'N' || this.map.directionReflected == 'S')) ? this.map.height / 2 : this.map.height;
		int width = (this.map.isReflected && (this.map.directionReflected == 'E' || this.map.directionReflected == 'W')) ? this.map.width / 2 : this.map.width;

        int i, j;

        for(i = 0; i < height; i++) {
			for(j = 0; j < width; j++) {
                this.mapTile[i, j] = tilesList[i * width + j];
                this.mapComponent[i, j] = componentsList[i * width + j];
			}
		}

	}

}
