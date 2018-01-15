using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using GameDevWare.Serialization;
using System;

public class MapEditorController : Editor {

    private GameComponents gameComponents;
    private Map map;
    private string mapName;
    private int width;
    private int height;
    
    // ScrollView
    private GameObject tilesView;
    private GameObject mapComponentsView;

    // Helper Insert
    private GameObject temp;
    private TileDao tile;
    private MapComponentDao mapComponent;
    private bool mouseDown;

    // Scales
    private float scale = 8.9f;
    private float spaceCell = 0.65f;
    public float cameraDelay = 0.01f;

    // Camera Counter
    private float timeCounter;

    // Matrix Delimitation
    private float xIni;
    private float xFin;
    private float yIni;
    private float yFin;
    private float kScale= 0;

    // Data
    private int[] mapTileData;
    private int[] mapComponentsData;

    // Buttons
    private GameObject buttonNew;
    private GameObject buttonLoad;
    private GameObject buttonSave;

    // NewMap
    private GameObject PanelNewMap;

    // Map
    private GameObject components;

    void Start () {
        
        // Buttons
        this.buttonNew = GameObject.Find("Canvas/Menu/ButtonNew");
        this.buttonLoad = GameObject.Find("Canvas/Menu/ButtonLoad");
        this.buttonSave = GameObject.Find("Canvas/Menu/ButtonSave");

        this.buttonSave.SetActive(false);
        
        this.mouseDown = false;

        this.tile = null;
        this.mapComponent = null;
        this.temp = null;

        this.timeCounter = 0;

        this.gameComponents = new GameComponents();
        this.gameComponents.map = null;
        this.gameComponents.mapTile = null;
        this.gameComponents.mapComponent = null;

        this.components = GameObject.Find("components"); ;

        // ScrollView
        this.tilesView = GameObject.Find("tilesView");
        this.mapComponentsView = GameObject.Find("mapComponentsView");
        
        GameObject button, model, tilesContent, mapComponentsContent;

        tilesContent = GameObject.Find("tilesView/Viewport/Content");
        mapComponentsContent = GameObject.Find("mapComponentsView/Viewport/Content");

        tilesContent.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + 120 * this.gameComponents.tiles.Count, 0.0f);
        mapComponentsContent.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + 120 * this.gameComponents.mapComponents.Count, 0.0f);

        int count = 0;

        List<string> options = new List<string>();
        
        foreach(TileDao tile in this.gameComponents.tiles.Values) {

            options.Add(tile.name);

            button = new GameObject();
            button.name = "Button" + tile.name;
            button.transform.SetParent(tilesContent.transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.5f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 0.5f);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 0.5f);
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            button.GetComponent<RectTransform>().localPosition = new Vector3(20.0f + 120*count, -10.0f, 0.0f);
            UnityEngine.Events.UnityAction action = () => { InsertTile(tile); };
            button.GetComponent<Button>().onClick.AddListener(action);
            
            model = GameObject.Instantiate(tile.Instantiate().model);
            model.transform.SetParent(button.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(95.0f, 95.0f, 1.0f);
            model.GetComponent<RectTransform>().localPosition = new Vector3(50.0f, 0.0f, 0.0f);
            
            count++;
        }

        count = 0;

        button = new GameObject();
        button.name = "ButtonEmpty";
        button.transform.SetParent(mapComponentsContent.transform);
        button.AddComponent<Button>();
        button.AddComponent<Image>();
        button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.5f);
        button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 0.5f);
        button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 0.5f);
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        button.GetComponent<RectTransform>().localPosition = new Vector3(20.0f + 120 * count, -10.0f, 0.0f);
        button.GetComponent<Button>().onClick.AddListener(DeleteMapComponent);

        GameObject text = new GameObject("empty", typeof(Text));
        text.gameObject.transform.SetParent(button.transform);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(320.0f, 200.0f);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.0f);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f);
        text.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0.0f, 0.0f);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0.0f, 0.0f);
        text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.GetComponent<Text>().fontSize = 30;
        text.GetComponent<Text>().fontStyle = FontStyle.Bold;
        text.GetComponent<Text>().color = Color.red;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        text.GetComponent<Text>().text = "X";

        count++;

        foreach (MapComponentDao mapComponent in this.gameComponents.mapComponents.Values) {

            button = new GameObject();
            button.name = "Button" + mapComponent.name;
            button.transform.SetParent(mapComponentsContent.transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 0.5f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 0.5f);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 0.5f);
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            button.GetComponent<RectTransform>().localPosition = new Vector3(20.0f + 120*count, -10.0f, 0.0f);
            UnityEngine.Events.UnityAction action = () => { InsertMapComponent(mapComponent); };
            button.GetComponent<Button>().onClick.AddListener(action);
        
            model = GameObject.Instantiate(mapComponent.Instantiate().model);
            model.transform.SetParent(button.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(95.0f, 95.0f, 1.0f);
            model.GetComponent<RectTransform>().localPosition = new Vector3(50.0f, -30.0f, 0.0f);
            
            count++;
        }
        
        GameObject.Find("Canvas/PanelNewMap/Form/TileDefault/DropdownTiles").GetComponent<Dropdown>().AddOptions(options);

        // NewMap
        this.PanelNewMap = GameObject.Find("Canvas/PanelNewMap");

        this.PanelNewMap.SetActive(false);

        this.mapComponentsView.SetActive(false);
	}
    
    void Update () {

        if (Input.GetAxis("Mouse ScrollWheel") > 0) {

            if(this.components.transform.position.z > 100) {
                this.components.transform.Translate(0.0f, 0.0f, -50.0f);
            }

        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) {

            if (this.components.transform.position.z < 300) {
                this.components.transform.Translate(0.0f, 0.0f, 50.0f);
            }

        }

        if (Input.GetMouseButtonDown(0)) {
            this.mouseDown = true;
        }

        if(Input.GetMouseButtonUp(0)) {
            this.mouseDown = false;
        }
    
        if (this.temp != null) {
        
            if((this.tile != null || this.mapComponent != null ||
                (this.tile == null && this.mapComponent == null && this.temp != null)) && 
                Input.mousePosition.x > 264 && Input.mousePosition.y > 204) {
                
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit)) {
                    
                    string objName = hit.transform.name;
                    string[] coord = objName.Split('_');

                    GameObject obj = GameObject.Find(objName);

                    this.temp.SetActive(true);
                    this.temp.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z - 0.1f);
                    
                    if(this.mouseDown) {
                        
                        GameObject newComponent = null;

                        if (this.tile != null || this.mapComponent != null) {
                            
                            newComponent = GameObject.Instantiate(this.temp);
                            newComponent.GetComponent<BoxCollider>().enabled = false;
                            newComponent.transform.SetParent(this.components.transform);
                            newComponent.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);

                        }

                        int width = ((this.map.directionReflected == 'E' || this.map.directionReflected == 'W') ? this.map.width / 2 : this.map.width);

                        if (this.tile != null) {                            

                            if(this.tile.isWalkable ||
                                (!this.tile.isWalkable && GameObject.Find("component_" + coord[1] + "_" + coord[2]) == null)) {
                                                                
                                GameObject.DestroyImmediate(GameObject.Find(objName));
                                newComponent.transform.name = objName;
                                newComponent.GetComponent<BoxCollider>().enabled = true;
                                this.mapTileData[int.Parse(coord[1]) * width + int.Parse(coord[2])] = this.tile.id;
                                
                            } else {
                                Destroy(newComponent);
                            }

                        } else if(this.mapComponent != null) {

                            TileDao tile = this.gameComponents.tiles[this.mapTileData[int.Parse(coord[1]) * this.map.width + int.Parse(coord[2])]];

                            if(tile.isWalkable) {
                                
                                //Debug.Log("Editando MapComponent");
                                GameObject.DestroyImmediate(GameObject.Find("component_" + coord[1] + "_" + coord[2]));
                                newComponent.transform.name = "component_" + coord[1] + "_" + coord[2];
                                newComponent.transform.position = new Vector3(newComponent.transform.position.x, newComponent.transform.position.y, newComponent.transform.position.z - 0.5f);
                                this.mapComponentsData[int.Parse(coord[1]) * width + int.Parse(coord[2])] = this.mapComponent.id + 1;

                            } else {
                                Destroy(newComponent);
                            }

                        } else {

                            //Debug.Log("Deleta MapComponent");
                            GameObject.DestroyImmediate(GameObject.Find("component_" + coord[1] + "_" + coord[2]));
                            this.mapComponentsData[int.Parse(coord[1]) * width + int.Parse(coord[2])] = 0;

                        }
                        
                    }

                }

            } else {

                this.temp.SetActive(false);
            }
        }

        timeCounter += Time.deltaTime;

        // Camera
        if(timeCounter >= cameraDelay) {
            
            if (Input.GetKey("left") && Camera.main.transform.position.x > this.xIni) {
                Camera.main.transform.Translate(new Vector3(-(this.scale + this.spaceCell), 0.0f, 0.0f));
            }

            if (Input.GetKey("right") && Camera.main.transform.position.x < this.xFin) {
                Camera.main.transform.Translate(new Vector3((this.scale + this.spaceCell), 0.0f, 0.0f));
            }

            if (Input.GetKey("up") && Camera.main.transform.position.y < this.yIni) {
                Camera.main.transform.Translate(new Vector3(0.0f, (this.scale + this.spaceCell), 0.0f));
            }

            if (Input.GetKey("down") && Camera.main.transform.position.y > this.yFin) {
                Camera.main.transform.Translate(new Vector3(0.0f, -(this.scale + this.spaceCell), 0.0f));
            }

            timeCounter -= cameraDelay;
        }

        if (timeCounter >= cameraDelay) {
            timeCounter -= cameraDelay;
        }

    }

    public void NewMap() {
        Debug.Log("NewMap()");

        this.CleanEditor();

        this.PanelNewMap.SetActive(true);
    }

    public void CleanEditor() {

        if (this.buttonSave.activeSelf) {

            GameObject components = GameObject.Find("components");

            while (components.transform.childCount > 0) {
                GameObject.DestroyImmediate(components.transform.GetChild(0).gameObject);
            }

            Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);

        }

        this.SelectButton();

    }

    public void LoadMap(string map = "") {

        Debug.Log("LoadMap()");

        string path = "";

        this.mapName = map;

        while(true && map == "") {

            path = EditorUtility.OpenFilePanel("Selecione um mapa", Application.dataPath + "/StreamingAssets/", "map");

            if(path.Equals("")) {
                return;
            }

            string[] extension = path.Split('.');
            
            string[] mapName = path.Split(new string[] { "/StreamingAssets/" }, StringSplitOptions.None);

            this.mapName = mapName[1];
                        
            if(extension[extension.Length - 1].ToLower().Equals("map")) {
                break;
            } else {
                bool option = EditorUtility.DisplayDialog("Arquivo inválido!", "Selecione um arquivo que possua a extenção .map", "Tentar novamente", "Cancelar");
                
                if(!option) {
                    return;
                }
            }

        }

        this.CleanEditor();

        GameDataMap gdm = GameDataMap.Load(this.mapName);
        
        // Map
        this.map = JsonUtility.FromJson<Map>(gdm.map);

        // Map Tiles
        this.mapTileData = Json.Deserialize<int[]>(gdm.mapTile);
        
        //Map Components
        this.mapComponentsData = Json.Deserialize<int[]>(gdm.mapComponent);
        
        this.map.height = (this.map.directionReflected.Equals('N') || this.map.directionReflected.Equals('S') ? this.map.height / 2 : this.map.height);
        this.map.width = (this.map.directionReflected.Equals('E') || this.map.directionReflected.Equals('W') ? this.map.width / 2 : this.map.width);

        this.height = this.map.height;
        this.width = this.map.width;

        this.gameComponents.mapTile = new int[this.map.height, this.map.width];
        this.gameComponents.mapComponent = new int[this.map.height, this.map.width];

        for (int i = 0; i < this.map.height; i++) {
            for (int j = 0; j < this.map.width; j++) {
                this.gameComponents.mapTile[i, j] = this.mapTileData[i * this.map.width + j];
                this.gameComponents.mapComponent[i, j] = this.mapComponentsData[i * this.map.width + j];
            }
        }

        bool isReflected = this.map.isReflected;

        this.map.isReflected = false;

        this.map.Build(gameComponents);
                
        this.DrawMap();

        if(isReflected) {
            this.map.isReflected = true;
            this.map.height = (this.map.directionReflected.Equals('N') || this.map.directionReflected.Equals('S') ? this.map.height * 2 : this.map.height);
            this.map.width = (this.map.directionReflected.Equals('E') || this.map.directionReflected.Equals('W') ? this.map.width * 2 : this.map.width); ;
        }        
        
        this.buttonSave.SetActive(true);
    }

    public void SaveMap() {

        Debug.Log("SaveMap()");

        GameDataMap gameDataMap = new GameDataMap();

        gameDataMap.map = JsonUtility.ToJson(this.map);
        gameDataMap.mapTile = Json.SerializeToString<int[]>(this.mapTileData);
        gameDataMap.mapComponent = Json.SerializeToString<int[]>(this.mapComponentsData);
        
        GameDataMap.Save(gameDataMap, mapName);
    }
    
    public void CreateNewMap() {

        Debug.Log("TileDefault: " + GameObject.Find("DropdownTiles").GetComponent<Dropdown>().value);

        // 0 => No
        // 1 => Yes, up
        // 2 => Yes, right
        // 3 => Yes, down
        // 4 => Yes, left
        Debug.Log("Reflected: " + GameObject.Find("DropdownReflected").GetComponent<Dropdown>().value);

        string mapName = GameObject.Find("Canvas/PanelNewMap/Form/Name/InputFieldName").GetComponent<InputField>().text.ToLower();
        string path = Application.dataPath + "/StreamingAssets/" + mapName + ".map";

        int width = int.Parse(GameObject.Find("Canvas/PanelNewMap/Form/Size/InputFieldWidth").GetComponent<InputField>().text);
        int height = int.Parse(GameObject.Find("Canvas/PanelNewMap/Form/Size/InputFieldHeight").GetComponent<InputField>().text);
        
        if(!File.Exists(path) && width <= 1000 && width > 0 && height <= 1000 && height > 0) {

            Debug.Log("Mapa Criado");

            this.mapName = mapName;

            this.map = new Map(width, height);

            this.mapTileData = new int[width * height];
            this.mapComponentsData = new int[width * height];

            switch(GameObject.Find("DropdownReflected").GetComponent<Dropdown>().value) {
                case 1:
                    this.map.isReflected = true;
                    this.map.directionReflected = 'N';
                    break;
                case 2:
                    this.map.isReflected = true;
                    this.map.directionReflected = 'E';
                    break;
                case 3:
                    this.map.isReflected = true;
                    this.map.directionReflected = 'S';
                    break;
                case 4:
                    this.map.isReflected = true;
                    this.map.directionReflected = 'W';
                    break;
                default:
                    this.map.isReflected = false;
                    break;
            }

            int tileIndex = GameObject.Find("DropdownTiles").GetComponent<Dropdown>().value;

            for (int i = 0; i < width * height; i++) {
                this.mapTileData[i] = tileIndex;
                this.mapComponentsData[i] = 0;
            }
            
            this.SaveMap();
            this.LoadMap(this.mapName);
            this.CloseForm();

        } else {

            string erro = (File.Exists(path) ? "Já existe um mapa com este nome!\n" : "");
            erro += ((width <= 1000 && width > 0 && height <= 1000 && height > 0) ? "" : "Tamanho inválido!");

            EditorUtility.DisplayDialog("Erro na criação do mapa!", erro, "Voltar");

        }

    }

    public void CloseForm() {

        this.CleanForm();

        this.PanelNewMap.SetActive(false);

        Debug.Log("CloseForm()");

    }

    private void CleanForm() {

        GameObject.Find("Canvas/PanelNewMap/Form/Name/InputFieldName").GetComponent<InputField>().text = "";
        GameObject.Find("Canvas/PanelNewMap/Form/Size/InputFieldWidth").GetComponent<InputField>().text = "";
        GameObject.Find("Canvas/PanelNewMap/Form/Size/InputFieldHeight").GetComponent<InputField>().text = "";        
        GameObject.Find("Canvas/PanelNewMap/Form/TileDefault/DropdownTiles").GetComponent<Dropdown>().value = 0;
        GameObject.Find("Canvas/PanelNewMap/Form/Reflected/DropdownReflected").GetComponent<Dropdown>().value = 0;

    }

    public void InsertTile(TileDao tile) {
                
        this.SelectButton(tile.name);

        this.tile = tile;
        this.mapComponent = null;

        this.PrepareTemp(tile.Instantiate().model);
        
    }

    public void InsertMapComponent(MapComponentDao mapComponent) {
        
        this.SelectButton(mapComponent.name);

        this.mapComponent = mapComponent;
        this.tile = null;

        this.PrepareTemp(mapComponent.Instantiate().model);
        
    }

    public void DeleteMapComponent() {

        this.SelectButton("Empty");

        this.mapComponent = null;
        this.tile = null;

        this.PrepareTemp(Resources.Load("tile_Help", typeof(GameObject)) as GameObject);

        this.temp.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;

    }

    private void PrepareTemp(GameObject model) {
        this.temp = GameObject.Instantiate(model);
        this.temp.name = "temp";
        this.temp.transform.position = new Vector3(0.0f, 0.0f, 89.9f);
        this.temp.transform.SetParent(GameObject.Find("components").transform);
        this.temp.transform.localScale = new Vector3(scale, scale, 0.1f);
        this.temp.transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        this.temp.GetComponent<BoxCollider>().enabled = false;
        this.temp.SetActive(false);
    }

    public void SelectButton(string componentName = "") {

        if (this.tile != null && this.tilesView.activeSelf == true) {
            GameObject.Find("Button" + this.tile.name).GetComponent<Image>().color = Color.white;
        }

        if (this.mapComponentsView.activeSelf == true){            

            if (this.mapComponent != null) {
                GameObject.Find("Button" + this.mapComponent.name).GetComponent<Image>().color = Color.white;
            }

            GameObject.Find("ButtonEmpty").GetComponent<Image>().color = Color.white;
        }        

        if(componentName != "") {
            GameObject.Find("Button" + componentName).GetComponent<Image>().color = Color.black;
        }
        
    }

    public void GetTabTilesList() {

        SelectButton();

        this.tilesView.SetActive(true);
        this.mapComponentsView.SetActive(false);
    }

    public void GetTabMapComponentsList() {

        SelectButton();

        this.mapComponentsView.SetActive(true);
        this.tilesView.SetActive(false);
    }

    public void DrawMap() {
        
        GameObject components = GameObject.Find("components");

        float xIni = (float)scale * (this.map.width - 1) / -2;
        float yIni = (float)scale * (this.map.height - 1) / 2;

        for (int i = 0; i < this.map.height; i++) {
            for (int j = 0; j < this.map.width; j++) {
                this.map.tiles[i, j].model = GameObject.Instantiate(this.map.tiles[i, j].model);
                this.map.tiles[i, j].model.name = "tile_" + i + "_" + j;
                this.map.tiles[i, j].model.tag = "Tile";
                this.map.tiles[i, j].model.transform.SetParent(components.transform);
                this.map.tiles[i, j].model.transform.localPosition = new Vector3(xIni + j * (scale + spaceCell), yIni - i * (scale + spaceCell), 0.0f);
                this.map.tiles[i, j].model.transform.localScale = new Vector3(scale, 0.1f, scale);
                this.map.tiles[i, j].model.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));

                if (this.map.tiles[i, j].mapComponent != null) {
                    
                    GameObject tileComponent = GameObject.Instantiate(this.map.tiles[i, j].mapComponent.model);
                    this.map.tiles[i, j].mapComponent.model = tileComponent;
                    tileComponent.transform.SetParent(components.transform);
                    tileComponent.name = "component_" + i + "_" + j;
                    tileComponent.tag = "MapComponent";
                    tileComponent.GetComponent<BoxCollider>().enabled = false;
                    tileComponent.transform.localScale = new Vector3(scale, scale, 0.1f);
                    tileComponent.transform.localPosition = new Vector3(xIni + j * (scale + spaceCell), yIni - i * (scale + spaceCell), -0.5f);
                }

            }
        }

        this.SetPositionCamera();

    }

    private void SetPositionCamera() {

        GameObject firstTile = GameObject.Find("tile_0_0");
        GameObject lastTile = GameObject.Find("tile_" + (this.height - 1) + "_" + (this.width - 1));
        
        this.xIni = firstTile.transform.position.x + (scale + spaceCell) * 3.4f;
        this.yIni = firstTile.transform.position.y - (scale + spaceCell) * 5.57f;
        this.xFin = lastTile.transform.position.x - (scale + spaceCell) * 8.0f;
        this.yFin = lastTile.transform.position.y + (scale + spaceCell) * 3.0f;

        float cameraX = firstTile.transform.position.x + (scale + spaceCell) * (3.4f + kScale*4.6f);
        float cameraY = firstTile.transform.position.y - (scale + spaceCell) * (5.57f + kScale * 4.6f);

        Camera.main.transform.position = new Vector3(cameraX, cameraY, Camera.main.transform.position.z);

    }

}
