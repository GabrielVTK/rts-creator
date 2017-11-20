using System.Collections.Generic;
using UnityEngine;
using GameDevWare.Serialization;

public class GameComponents {

	public Map map;
	public int[,] mapTile;
	public int[,] mapComponent;

	public List<BaseMaterialDao> baseMaterials;
	public List<TileDao> tiles;
	public List<UnitDao> units;
	public List<BuildingDao> buildings;
	public List<MapComponentDao> mapComponents;

	public GameComponents() {

		this.tiles = new List<TileDao>();
		this.baseMaterials = new List<BaseMaterialDao>();
		this.units = new List<UnitDao>();
		this.buildings = new List<BuildingDao>();
		this.mapComponents = new List<MapComponentDao>();

		this.map = new Map(100, 50, 'N');
		this.mapTile = new int[map.height, map.width];
		this.mapComponent = new int[map.height, map.width];


		CreateGame();
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
		build_quarter.cost.Add(baseMaterialGold, 200);

		BuildingDao build_farm = new BuildingDao("Farm", "Images/material_Meat", "build_Farm", new Vector2(1.0f, 1.0f), 500.0f, 30.0f, false, 1, baseMaterialMeat, 1);
		build_farm.cost.Add(baseMaterialGold, 75);

		BuildingDao build_mine = new BuildingDao("Mine", "Images/build_Mine", "build_Mine", new Vector2(1.0f, 1.0f), 500.0f, 30.0f, false, 1, baseMaterialGold, 1);
		build_mine.cost.Add(baseMaterialMeat, 100);
		build_mine.requireds.Add(mats_Gold.name);

		// Warrior Unit
		CombatDao infantry_unit = new CombatDao("Infantry", "Images/unit_Warrior", "unit_Warrior", 3.0f, 0.0f, 1.0f, 50.0f, 1.0f, 1.0f, new CombatType(), 22.0f, 2);
		infantry_unit.cost.Add(baseMaterialMeat, 35);
		infantry_unit.cost.Add(baseMaterialGold, 25);

		// Archer Unit
		CombatDao archer_unit = new CombatDao("Archer", "Images/unit_Archer", "unit_Archer", 4.0f, 0.0f, 0.9f, 30.0f, 4.0f, 1.0f, new CombatType(), 30.0f, 2);
		archer_unit.cost.Add(baseMaterialMeat, 25);
		archer_unit.cost.Add(baseMaterialGold, 45);

		// Archer Unit
		CombatDao cavalry_unit = new CombatDao("Cavalry", "Images/unit_Knight", "unit_Knight", 10.0f, 0.0f, 1.5f, 100.0f, 1.0f, 1.0f, new CombatType(), 35.0f, 2);
		cavalry_unit.cost.Add(baseMaterialMeat, 60);
		cavalry_unit.cost.Add(baseMaterialGold, 75);

		// Worker Unit
		WorkerDao unit_worker = new WorkerDao("Worker", "Images/unit_Worker", "unit_Worker", 1.0f, 0.0f, 1.0f, 45.0f, 5, 1.0f, 1.0f, 1.0f, 10.0f, 2);
		unit_worker.cost.Add(baseMaterialMeat, 10);
		unit_worker.buildingsName.Add(build_quarter.name);
		unit_worker.buildingsName.Add(build_farm.name);
		unit_worker.buildingsName.Add(build_mine.name);


		// Add in tiles list
		tiles.Add(tile_Grass);
		tiles.Add(tile_Water);
		tiles.Add(tile_Bridge);

		// Add in baseMaterial list
		baseMaterials.Add(baseMaterialGold);
		baseMaterials.Add(baseMaterialMeat);

		// Add in materialSource list
		mapComponents.Add(mats_Gold);
		mapComponents.Add(mc_Tree);

		// Add in buildings list
		buildings.Add(build_base);
		buildings.Add(build_quarter);
		buildings.Add(build_farm);
		buildings.Add(build_mine);

		// Add in unit list
		units.Add(infantry_unit);
		units.Add(archer_unit);
		units.Add(cavalry_unit);
		units.Add(unit_worker);

		// Add in unit list of build_base
		build_base.units.Add(unit_worker);
		build_quarter.units.Add(infantry_unit);
		build_quarter.units.Add(archer_unit);
		build_quarter.units.Add(cavalry_unit);
        
		SaveGame();
	}

	public void SaveGame() {
		GameData gameData = new GameData ();

		// Tiles
		gameData.tiles = Json.SerializeToString<List<TileDao>> (tiles);

		// Base Materials
		gameData.baseMaterials = Json.SerializeToString<List<BaseMaterialDao>> (baseMaterials);

		// Material Sources
		gameData.mapComponents = Json.SerializeToString<List<MapComponentDao>>(mapComponents);

		// Units
		gameData.units = Json.SerializeToString<List<UnitDao>> (units);

		// Buildings
		gameData.buildings = Json.SerializeToString<List<BuildingDao>> (buildings);

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
        this.tiles = Json.Deserialize<List<TileDao>>(gameData.tiles);

        // Base Materials
        this.baseMaterials = Json.Deserialize<List<BaseMaterialDao>>(gameData.baseMaterials);

		foreach(BaseMaterialDao baseMaterial in baseMaterials) {
			baseMaterial.Instantiate();
		}

        // Material Sources
        this.mapComponents = Json.Deserialize<List<MapComponentDao>>(gameData.mapComponents);

        // Units
        this.units = Json.Deserialize<List<UnitDao>>(gameData.units);

        // Buildings
        this.buildings = Json.Deserialize<List<BuildingDao>>(gameData.buildings);

		// Map
		//map = Json.Deserialize<Map>(gameData.map);
		this.map = JsonUtility.FromJson<Map>(gameDataMap.map);

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
