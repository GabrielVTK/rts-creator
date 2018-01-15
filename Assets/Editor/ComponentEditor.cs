using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

struct TileView {
    public GameObject view;
    public InputField name;
    public InputField terraineType;
    public Toggle isWalkable;
    public Toggle canBuild;
}

struct BuildingView {
    public GameObject view;
    public InputField name;
}

struct UnitView {
    public GameObject view;
    public InputField name;
    public InputField attack;
    public InputField defense;
    public InputField walkSpeed;
    public InputField lifeTotal;
    public InputField visionField;
    public InputField trainingTime;
    public InputField range;
    public InputField attackSpeed;
    public GameObject requiredBuildings;
    public GameObject costs;
}

struct BaseMaterialView {
    public GameObject view;
    public InputField name;
}

struct MaterialSourceView {
    public GameObject view;
    public InputField name;
}

struct MapComponentView {
    public GameObject view;
    public InputField name;
}

public class ComponentEditor : Editor {

    private GameComponents gc;
    
    private GameObject tiles;
    private GameObject buildings;
    private GameObject units;
    private GameObject baseMaterials;
    private GameObject materialSources;
    private GameObject mapComponents;

    private GameObject buttonSave;
    private GameObject buttonDelete;
    private GameObject preview;
    private GameObject viewObject;

    private TileView tileForm;
    private BuildingView buildingForm;
    private UnitView unitForm;
    private BaseMaterialView baseMaterialForm;
    private MaterialSourceView materialSourcesForm;
    private MapComponentView mapComponentForm;

    private GameObject modalNewUnit;

    void Start() {
        
        this.gc = new GameComponents();
        
        this.tiles = GameObject.Find("Canvas/Panel/tilesView");
        this.buildings = GameObject.Find("Canvas/Panel/buildingsView");
        this.units = GameObject.Find("Canvas/Panel/unitsView");
        this.baseMaterials = GameObject.Find("Canvas/Panel/baseMaterialsView");
        this.materialSources = GameObject.Find("Canvas/Panel/materialSourcesView");
        this.mapComponents = GameObject.Find("Canvas/Panel/mapComponentsView");

        this.buttonSave = GameObject.Find("Canvas/Preview/ButtonSave");
        this.buttonDelete = GameObject.Find("Canvas/Preview/ButtonDelete");
        this.preview = GameObject.Find("Canvas/Preview/view");

        this.modalNewUnit = GameObject.Find("Canvas/PanelNewUnit");
        this.modalNewUnit.SetActive(false);

        this.buttonSave.SetActive(false);
        this.buttonDelete.SetActive(false);

        this.tileForm = new TileView();
        this.tileForm.view = GameObject.Find("Canvas/Preview/formTile");
        this.tileForm.view.SetActive(false);
        this.tileForm.name = GameObject.Find("Canvas/Preview/formTile/InputFieldName").GetComponent<InputField>();
        this.tileForm.terraineType = GameObject.Find("Canvas/Preview/formTile/InputFieldTerraineType").GetComponent<InputField>();
        this.tileForm.isWalkable = GameObject.Find("Canvas/Preview/formTile/ToggleIsWalkable").GetComponent<Toggle>();
        this.tileForm.canBuild = GameObject.Find("Canvas/Preview/formTile/ToggleCanBuild").GetComponent<Toggle>();

        this.buildingForm = new BuildingView();
        this.buildingForm.view = GameObject.Find("Canvas/Preview/formBuilding");
        this.buildingForm.view.SetActive(false);
        this.buildingForm.name = GameObject.Find("Canvas/Preview/formBuilding/InputFieldName").GetComponent<InputField>();

        this.unitForm = new UnitView();
        this.unitForm.view = GameObject.Find("Canvas/Preview/formUnit");
        this.unitForm.requiredBuildings = GameObject.Find("Canvas/Preview/formUnit/PanelRequiredBuilding");
        this.unitForm.requiredBuildings.SetActive(false);
        this.unitForm.costs = GameObject.Find("Canvas/Preview/formUnit/PanelCost");
        this.unitForm.costs.SetActive(false);
        this.unitForm.view.SetActive(false);
        this.unitForm.name = GameObject.Find("Canvas/Preview/formUnit/InputFieldName").GetComponent<InputField>();
        this.unitForm.attack = GameObject.Find("Canvas/Preview/formUnit/InputFieldAttack").GetComponent<InputField>();
        this.unitForm.defense = GameObject.Find("Canvas/Preview/formUnit/InputFieldDefense").GetComponent<InputField>();
        this.unitForm.walkSpeed = GameObject.Find("Canvas/Preview/formUnit/InputFieldWalkSpeed").GetComponent<InputField>();
        this.unitForm.lifeTotal = GameObject.Find("Canvas/Preview/formUnit/InputFieldLifeTotal").GetComponent<InputField>();
        this.unitForm.visionField = GameObject.Find("Canvas/Preview/formUnit/InputFieldVisionField").GetComponent<InputField>();
        this.unitForm.trainingTime = GameObject.Find("Canvas/Preview/formUnit/InputFieldTrainingTime").GetComponent<InputField>();
        this.unitForm.range = GameObject.Find("Canvas/Preview/formUnit/InputFieldRange").GetComponent<InputField>();
        this.unitForm.attackSpeed = GameObject.Find("Canvas/Preview/formUnit/InputFieldAttackSpeed").GetComponent<InputField>();

        this.baseMaterialForm = new BaseMaterialView();
        this.baseMaterialForm.view = GameObject.Find("Canvas/Preview/formBaseMaterial");
        this.baseMaterialForm.view.SetActive(false);
        this.baseMaterialForm.name = GameObject.Find("Canvas/Preview/formBaseMaterial/InputFieldName").GetComponent<InputField>();

        this.materialSourcesForm = new MaterialSourceView();
        this.materialSourcesForm.view = GameObject.Find("Canvas/Preview/formMaterialSource");
        this.materialSourcesForm.view.SetActive(false);

        this.mapComponentForm = new MapComponentView();
        this.mapComponentForm.view = GameObject.Find("Canvas/Preview/formMapComponent");
        this.mapComponentForm.view.SetActive(false);
        

        this.tiles.SetActive(false);
        this.buildings.SetActive(false);
        this.units.SetActive(false);
        this.baseMaterials.SetActive(false);
        this.materialSources.SetActive(false);
        this.mapComponents.SetActive(false);
    }

    public void CreateField(int index, string label, Transform parent, string value = "") {
        GameObject field = InputField.Instantiate(Resources.Load("Editor/Field") as GameObject);
        field.transform.SetParent(parent);

        field.name = label;
        field.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        field.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        field.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        field.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        field.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        field.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10 - (40 * index));
        field.GetComponent<RectTransform>().offsetMin = new Vector2(10, -40 - (40 * index));
        
        field.transform.GetChild(0).GetComponent<Text>().text = label + ":";
        field.GetComponent<InputField>().text = value;
    }

    public void ShowTiles() {
        this.CloseTabs();

        Color color;
        ColorUtility.TryParseHtmlString("#B2B2B2FF", out color);
        GameObject.Find("Canvas/Menu/ButtonTiles").GetComponent<Image>().color = color;
        
        this.ImportTiles();
        this.tiles.SetActive(true);
    }

    public void ShowBuildings() {
        this.CloseTabs();
        this.ImportBuildings();
        this.buildings.SetActive(true);
    }

    public void ShowUnits() {
        this.CloseTabs();
        this.ImportUnits();
        this.units.SetActive(true);
    }

    public void ShowBaseMaterials() {
        this.CloseTabs();
        this.ImportBaseMaterials();
        this.baseMaterials.SetActive(true);
    }

    public void ShowMaterialSources() {
        this.CloseTabs();
        this.ImportMaterialSources();
        this.materialSources.SetActive(true);
    }

    public void ShowMapComponents() {
        this.CloseTabs();
        this.ImportMapComponents();
        this.mapComponents.SetActive(true);
    }

    public void CloseTabs() {
        GameObject.Find("Canvas/Menu/ButtonTiles").GetComponent<Image>().color = Color.white;
        GameObject.Find("Canvas/Menu/ButtonBuildings").GetComponent<Image>().color = Color.white;
        GameObject.Find("Canvas/Menu/ButtonUnits").GetComponent<Image>().color = Color.white;
        GameObject.Find("Canvas/Menu/ButtonBaseMaterial").GetComponent<Image>().color = Color.white;
        GameObject.Find("Canvas/Menu/ButtonMaterialSource").GetComponent<Image>().color = Color.white;
        GameObject.Find("Canvas/Menu/ButtonMapComponent").GetComponent<Image>().color = Color.white;

        this.tiles.SetActive(false);
        this.tileForm.view.SetActive(false);

        this.buildings.SetActive(false);
        this.buildingForm.view.SetActive(false);

        this.units.SetActive(false);
        this.unitForm.view.SetActive(false);

        this.baseMaterials.SetActive(false);
        this.baseMaterialForm.view.SetActive(false);

        this.materialSources.SetActive(false);
        this.materialSourcesForm.view.SetActive(false);

        this.mapComponents.SetActive(false);
        this.mapComponentForm.view.SetActive(false);

        this.buttonSave.SetActive(false);
        this.buttonDelete.SetActive(false);

        if (this.viewObject != null) {
            Debug.Log("Destroi objeto");
            GameObject.Destroy(this.viewObject);
            this.viewObject = null;
        }
    }

    public void OpenModal(GameObject modal) {
        modal.SetActive(true);
    }

    public void CloseModal(GameObject modal) {
        modal.SetActive(false);
    }

    private void SetView(GameObject model) {

        if (this.viewObject != null) {
            GameObject.Destroy(this.viewObject);
            this.viewObject = null;
        }

        this.viewObject = model;
    }

    public void ShowTileDetails(TileDao tile) {
        Debug.Log("Mostra detalhes da tile => " + tile.name);

        GameObject model = null;

        if (tile.model != "") {
            model = GameObject.Instantiate(tile.Instantiate().model);
            model.transform.SetParent(this.preview.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(100.0f, 100.0f, 1.0f);
            model.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -75.0f, 0.0f);
        }        

        this.SetView(model);

        this.tileForm.name.text = tile.name;
        this.tileForm.terraineType.text = tile.terraineType.ToString();
        this.tileForm.isWalkable.isOn = tile.isWalkable;
        this.tileForm.canBuild.isOn = tile.canBuild;

        this.buttonSave.SetActive(true);
        this.buttonSave.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonSave.GetComponent<Button>().onClick.AddListener(() => { this.Save(tile); });

        this.buttonDelete.SetActive(true);
        this.buttonDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonDelete.GetComponent<Button>().onClick.AddListener(() => { this.Delete(tile); });

        this.tileForm.view.SetActive(true);
    }

    public void ShowBuildingDetails(BuildingDao building) {
        Debug.Log("Mostra detalhes da building => " + building.name);

        GameObject model = null;

        if (building.model != "") {
            model = GameObject.Instantiate(building.Instantiate().model);
            model.transform.SetParent(this.preview.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(100.0f, 100.0f, 1.0f);
            model.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -150.0f, 0.0f);
        }

        this.SetView(model);

        this.buildingForm.name.text = building.name;

        this.buttonSave.SetActive(true);
        this.buttonSave.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonSave.GetComponent<Button>().onClick.AddListener(() => { this.Save(building); });

        this.buttonDelete.SetActive(true);
        this.buttonDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonDelete.GetComponent<Button>().onClick.AddListener(() => { this.Delete(building); });

        this.buildingForm.view.SetActive(true);
    }

    public void ShowUnitDetails(UnitDao unit) {
        Debug.Log("Mostra detalhes da unit => " + unit.name);

        GameObject model = null;

        if(unit.model != "") {
            model = GameObject.Instantiate(unit.Instantiate().model);
            model.transform.SetParent(this.preview.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(50, 50, 1.0f);
            model.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -150.0f, 0.0f);
        }

        this.SetView(model);

        this.unitForm.name.text = unit.name;
        this.unitForm.attack.text = unit.attack.ToString();
        this.unitForm.defense.text = unit.defense.ToString();
        this.unitForm.walkSpeed.text = unit.walkSpeed.ToString();
        this.unitForm.lifeTotal.text = unit.lifeTotal.ToString();
        this.unitForm.visionField.text = unit.visionField.ToString();
        this.unitForm.trainingTime.text = unit.trainingTime.ToString();
        this.unitForm.range.text = unit.range.ToString();
        this.unitForm.attackSpeed.text = unit.attackSpeed.ToString();


        /** REQUIRED BUILDINGS **/
        GameObject buildingsAvailableContent = GameObject.Find("Canvas/Preview/formUnit/PanelRequiredBuilding/Form/Available/Viewport/Content");
        
        while (buildingsAvailableContent.transform.childCount > 0) {
            GameObject.DestroyImmediate(buildingsAvailableContent.transform.GetChild(0).gameObject);
        }

        GameObject button, text;

        int index = 0;

        foreach (BuildingDao building in this.gc.buildings.Values) {

            button = new GameObject();
            button.name = "Button" + building.name;
            button.transform.SetParent(GameObject.Find("Canvas/Preview/formUnit/PanelRequiredBuilding/Form/Available/Viewport/Content").transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            button.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            button.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10 - (40 * index));
            button.GetComponent<RectTransform>().offsetMin = new Vector2(10, -40 - (40 * index));

            text = new GameObject("Text", typeof(Text));
            text.transform.SetParent(button.transform);
            text.GetComponent<Text>().text = building.name;
            text.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
            text.GetComponent<Text>().color = Color.black;
            text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

            //UnityEngine.Events.UnityAction action = () => { Funcao(); };
            //button.GetComponent<Button>().onClick.AddListener(action);

            index++;
        }

        /** /REQUIRED BUILDINGS **/

        /** COST **/
        GameObject costContent = GameObject.Find("Canvas/Preview/formUnit/PanelCost/Form/Material/Viewport/Content");

        while (costContent.transform.childCount > 0) {
            GameObject.DestroyImmediate(costContent.transform.GetChild(0).gameObject);
        }
        
        index = 0;
        string value;

        foreach(int baseMaterialId in this.gc.baseMaterials.Keys) {

            try {
                value = unit.cost[baseMaterialId].ToString();
            } catch {
                value = "0";
            }

            this.CreateField(index, this.gc.baseMaterials[baseMaterialId].name, costContent.transform, value);
            index++;
        }
        /** /COST **/
        
        this.buttonSave.SetActive(true);
        this.buttonSave.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonSave.GetComponent<Button>().onClick.AddListener(() => { this.Save(unit); });

        this.buttonDelete.SetActive(true);
        this.buttonDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonDelete.GetComponent<Button>().onClick.AddListener(() => { this.Delete(unit); });

        this.unitForm.view.SetActive(true);
    }
    
    public void ShowBaseMaterialDetails(BaseMaterialDao baseMaterial) {
        Debug.Log("Mostra detalhes do baseMaterial => " + baseMaterial.name);
        
        this.baseMaterialForm.name.text = baseMaterial.name;

        this.SetView(null);

        this.buttonSave.SetActive(true);
        this.buttonSave.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonSave.GetComponent<Button>().onClick.AddListener(() => { this.Save(baseMaterial); });

        this.buttonDelete.SetActive(true);
        this.buttonDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        this.buttonDelete.GetComponent<Button>().onClick.AddListener(() => { this.Delete(baseMaterial); });

        this.baseMaterialForm.view.SetActive(true);
    }

    public void ShowMaterialSourceDetails(MaterialSourceDao materialSource) {
        Debug.Log("Mostra detalhes do materialSource => " + materialSource.name);

        GameObject model = GameObject.Instantiate(materialSource.Instantiate().model);
        model.transform.SetParent(this.preview.transform);
        model.AddComponent<RectTransform>();
        model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        model.GetComponent<RectTransform>().localScale = new Vector3(50, 50, 1.0f);
        model.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -200.0f, 0.0f);

        this.SetView(model);

        this.materialSourcesForm.view.SetActive(true);
    }

    public void ShowMapComponentDetails(MapComponentDao mapComponent) {
        Debug.Log("Mostra detalhes do mapComponent => " + mapComponent.name);

        GameObject model = GameObject.Instantiate(mapComponent.Instantiate().model);
        model.transform.SetParent(this.preview.transform);
        model.AddComponent<RectTransform>();
        model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        model.GetComponent<RectTransform>().localScale = new Vector3(50, 50, 1.0f);
        model.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -200.0f, 0.0f);

        this.SetView(model);

        this.mapComponentForm.view.SetActive(true);
    }

    public void ImportTiles() {

        GameObject tilesContent = GameObject.Find("Canvas/Panel/tilesView/Viewport/Content");

        while (tilesContent.transform.childCount > 0) {
            GameObject.DestroyImmediate(tilesContent.transform.GetChild(0).gameObject);
        }

        int count = 1, line = 1, lastId = -1, modelX;
        float dist;
        GameObject button, model = null;
        UnityEngine.Events.UnityAction action;

        foreach (TileDao tile in this.gc.tiles.Values) {

            if (tile.id > lastId) {
                lastId = tile.id;
            }

            button = new GameObject();
            button.name = "Button" + tile.name;
            button.transform.SetParent(tilesContent.transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            if (tile.model != "") {
                model = GameObject.Instantiate(tile.Instantiate().model);
                model.transform.SetParent(button.transform);
                model.AddComponent<RectTransform>();
                model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().localScale = new Vector3(95.0f, 95.0f, 1.0f);
            }            

            if (count % 2 == 1) {
                dist = 45;
                modelX = 50;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
            } else {
                dist = 296.33f;
                modelX = -50;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
            }

            if(tile.model != "") {
                model.GetComponent<RectTransform>().localPosition = new Vector3(modelX, -50.0f, 0.0f);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

            action = () => { ShowTileDetails(tile); };
            button.GetComponent<Button>().onClick.AddListener(action);

            if (count % 2 == 0) {
                line++;
            }

            count++;
        }

        TileDao.ID = lastId + 1;

        button = new GameObject();
        button.name = "ButtonNew";
        button.transform.SetParent(tilesContent.transform);
        button.AddComponent<Button>();
        button.AddComponent<Image>();
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        GameObject text = new GameObject("Text", typeof(Text));
        text.transform.SetParent(button.transform);
        text.GetComponent<Text>().text = "+";
        text.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.GetComponent<Text>().fontSize = 30;
        text.GetComponent<Text>().color = Color.black;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        if (count % 2 == 1) {
            dist = 45;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
        } else {
            dist = 296.33f;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        }

        button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

        Vector2 size = new Vector2(tilesContent.GetComponent<RectTransform>().sizeDelta.x, line * 130 + 20);
        tilesContent.GetComponent<RectTransform>().sizeDelta = size;

        action = () => { CreateNewTile(); };
        button.GetComponent<Button>().onClick.AddListener(action);
    }

    public void ImportBuildings() {

        GameObject buildingsContent = GameObject.Find("Canvas/Panel/buildingsView/Viewport/Content");

        while (buildingsContent.transform.childCount > 0) {
            GameObject.DestroyImmediate(buildingsContent.transform.GetChild(0).gameObject);
        }

        int count = 1, line = 1, lastId = -1, modelX;
        float dist;
        GameObject button, model = null;
        UnityEngine.Events.UnityAction action;

        foreach (BuildingDao building in this.gc.buildings.Values) {

            if (building.id > lastId) {
                lastId = building.id;
            }

            button = new GameObject();
            button.name = "Button" + building.name;
            button.transform.SetParent(buildingsContent.transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            if(building.model != "") {
                model = GameObject.Instantiate(building.Instantiate().model);
                model.transform.SetParent(button.transform);
                model.AddComponent<RectTransform>();
                model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().localScale = new Vector3(95.0f, 95.0f, 1.0f);
            }

            if (count % 2 == 1) {
                dist = 45;
                modelX = 50;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
            } else {
                dist = 296.33f;
                modelX = -50;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
            }

            if(building.model != "") {
                model.GetComponent<RectTransform>().localPosition = new Vector3(modelX, -50.0f, 0.0f);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

            action = () => { ShowBuildingDetails(building); };
            button.GetComponent<Button>().onClick.AddListener(action);

            if (count % 2 == 0) {
                line++;
            }

            count++;
        }

        BuildingDao.ID = lastId + 1;

        button = new GameObject();
        button.name = "ButtonNew";
        button.transform.SetParent(buildingsContent.transform);
        button.AddComponent<Button>();
        button.AddComponent<Image>();
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        GameObject text = new GameObject("Text", typeof(Text));
        text.transform.SetParent(button.transform);
        text.GetComponent<Text>().text = "+";
        text.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.GetComponent<Text>().fontSize = 30;
        text.GetComponent<Text>().color = Color.black;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        if (count % 2 == 1) {
            dist = 45;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
        } else {
            dist = 296.33f;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        }

        button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

        Vector2 size = new Vector2(buildingsContent.GetComponent<RectTransform>().sizeDelta.x, line * 130 + 20);
        buildingsContent.GetComponent<RectTransform>().sizeDelta = size;

        action = () => { CreateNewBuilding(); };
        button.GetComponent<Button>().onClick.AddListener(action);
    }

    public void ImportUnits() {

        GameObject unitsContent = GameObject.Find("Canvas/Panel/unitsView/Viewport/Content");

        while (unitsContent.transform.childCount > 0) {
            GameObject.DestroyImmediate(unitsContent.transform.GetChild(0).gameObject);
        }

        int count = 1, line = 1, lastId = -1, modelX;
        float dist;
        GameObject button, model = null;
        UnityEngine.Events.UnityAction action;
        
        foreach (UnitDao unit in this.gc.units.Values) {
            
            if(unit.id > lastId) {
                lastId = unit.id;
            }

            button = new GameObject();
            button.name = "Button" + unit.name;
            button.transform.SetParent(unitsContent.transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            if(unit.model != "") {
                model = GameObject.Instantiate(unit.Instantiate().model);
                model.transform.SetParent(button.transform);
                model.AddComponent<RectTransform>();
                model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                model.GetComponent<RectTransform>().localScale = new Vector3(30, 30, 1.0f);
            }            

            if (count % 2 == 1) {
                dist = 45;
                modelX = 50;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
            } else {
                dist = 296.33f;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                modelX = -50;
            }

            if(unit.model != "") {
                model.GetComponent<RectTransform>().localPosition = new Vector3(modelX, -90.0f, 0.0f);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

            action = () => { ShowUnitDetails(unit); };
            button.GetComponent<Button>().onClick.AddListener(action);

            if (count % 2 == 0) {
                line++;
            }

            count++;
        }

        UnitDao.ID = lastId + 1;

        button = new GameObject();
        button.name = "ButtonNew";
        button.transform.SetParent(unitsContent.transform);
        button.AddComponent<Button>();
        button.AddComponent<Image>();
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        GameObject text = new GameObject("Text", typeof(Text));
        text.transform.SetParent(button.transform);
        text.GetComponent<Text>().text = "+";
        text.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.GetComponent<Text>().fontSize = 30;
        text.GetComponent<Text>().color = Color.black;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        if (count % 2 == 1) {
            dist = 45;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
        } else {
            dist = 296.33f;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        }

        button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

        Vector2 size = new Vector2(unitsContent.GetComponent<RectTransform>().sizeDelta.x, line * 130 + 20);
        unitsContent.GetComponent<RectTransform>().sizeDelta = size;
        
        action = () => { OpenModal(this.modalNewUnit); };
        button.GetComponent<Button>().onClick.AddListener(action);
    }
    
    public void ImportBaseMaterials() {

        GameObject baseMateriasContent = GameObject.Find("Canvas/Panel/baseMaterialsView/Viewport/Content");

        while (baseMateriasContent.transform.childCount > 0) {
            GameObject.DestroyImmediate(baseMateriasContent.transform.GetChild(0).gameObject);
        }

        int count = 1, line = 1, lastId = -1;
        float dist;
        GameObject button;
        UnityEngine.Events.UnityAction action;

        foreach (BaseMaterialDao baseMaterial in this.gc.baseMaterials.Values) {

            if (baseMaterial.id > lastId) {
                lastId = baseMaterial.id;
            }

            button = new GameObject();
            button.name = "Button" + baseMaterial.name;
            button.transform.SetParent(baseMateriasContent.transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            
            if (count % 2 == 1) {
                dist = 45;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
            } else {
                dist = 296.33f;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

            action = () => { ShowBaseMaterialDetails(baseMaterial); };
            button.GetComponent<Button>().onClick.AddListener(action);

            if (count % 2 == 0) {
                line++;
            }

            count++;
        }

        BaseMaterialDao.ID = lastId + 1;

        button = new GameObject();
        button.name = "ButtonNew";
        button.transform.SetParent(baseMateriasContent.transform);
        button.AddComponent<Button>();
        button.AddComponent<Image>();
        button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        GameObject text = new GameObject("Text", typeof(Text));
        text.transform.SetParent(button.transform);
        text.GetComponent<Text>().text = "+";
        text.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        text.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        text.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        text.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        text.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        text.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        text.GetComponent<Text>().fontSize = 30;
        text.GetComponent<Text>().color = Color.black;
        text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        if (count % 2 == 1) {
            dist = 45;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
        } else {
            dist = 296.33f;
            button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        }

        button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

        Vector2 size = new Vector2(baseMateriasContent.GetComponent<RectTransform>().sizeDelta.x, line * 130 + 20);
        baseMateriasContent.GetComponent<RectTransform>().sizeDelta = size;

        action = () => { CreateNewBaseMaterial(); };
        button.GetComponent<Button>().onClick.AddListener(action);
    }

    public void ImportMaterialSources() {

        int count = 1, line = 1;
        float dist;
        GameObject button, model;

        foreach (MapComponentDao mapComponent in this.gc.mapComponents.Values) {

            if (mapComponent.GetType() != typeof(MaterialSourceDao)) {
                continue;
            }

            button = new GameObject();
            button.name = "Button" + mapComponent.name;
            button.transform.SetParent(GameObject.Find("Canvas/Panel/materialSourcesView/Viewport/Content").transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            model = GameObject.Instantiate(mapComponent.Instantiate().model);
            model.transform.SetParent(button.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(30, 30, 1.0f);

            if (count % 2 == 1) {
                dist = 45;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
                model.GetComponent<RectTransform>().localPosition = new Vector3(50.0f, -90.0f, 0.0f);
            } else {
                dist = 296.33f;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                model.GetComponent<RectTransform>().localPosition = new Vector3(-50.0f, -90.0f, 0.0f);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

            UnityEngine.Events.UnityAction action = () => { ShowMaterialSourceDetails((MaterialSourceDao)mapComponent); };
            button.GetComponent<Button>().onClick.AddListener(action);

            if (count % 2 == 0) {
                line++;
            }

            count++;
        }

    }

    public void ImportMapComponents() {

        int count = 1, line = 1;
        float dist;
        GameObject button, model;

        foreach (MapComponentDao mapComponent in this.gc.mapComponents.Values) {

            if (mapComponent.GetType() == typeof(MaterialSourceDao)) {
                continue;
            }

            button = new GameObject();
            button.name = "Button" + mapComponent.name;
            button.transform.SetParent(GameObject.Find("Canvas/Panel/mapComponentsView/Viewport/Content").transform);
            button.AddComponent<Button>();
            button.AddComponent<Image>();
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            model = GameObject.Instantiate(mapComponent.Instantiate().model);
            model.transform.SetParent(button.transform);
            model.AddComponent<RectTransform>();
            model.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            model.GetComponent<RectTransform>().localScale = new Vector3(30, 30, 1.0f);

            if (count % 2 == 1) {
                dist = 45;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1f);
                model.GetComponent<RectTransform>().localPosition = new Vector3(50.0f, -90.0f, 0.0f);
            } else {
                dist = 296.33f;
                button.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                button.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                model.GetComponent<RectTransform>().localPosition = new Vector3(-50.0f, -90.0f, 0.0f);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(dist, -20.0f * line - (110 * (line - 1)), 0.0f);

            UnityEngine.Events.UnityAction action = () => { ShowMapComponentDetails(mapComponent); };
            button.GetComponent<Button>().onClick.AddListener(action);

            if (count % 2 == 0) {
                line++;
            }

            count++;
        }

    }

    public void CreateNewTile() {
        
        Debug.Log("New Tile");
        TileDao tile = new TileDao("", "", true, true, 0);
        this.gc.tiles.Add(tile.id, tile);
        this.ShowTileDetails(tile);
        
        this.ImportTiles();
    }

    public void CreateNewBuilding() {

        Debug.Log("New Building");
        BuildingDao building = new BuildingDao("", "", "", new Vector2(), 0, 0, false, 0);
        this.gc.buildings.Add(building.id, building);
        this.ShowBuildingDetails(building);

        this.ImportBuildings();
    }

    public void CreateNewUnit(int type) {

        // type
        // 0 => Worker
        // 1 => Combat

        if (type == 1) {
            Debug.Log("New Combat");
            CombatDao unit = new CombatDao("", "", "", 0, 0, 0, 0, 0, 0, null, 0, 0);
            this.gc.units.Add(unit.id, unit);
            this.ShowUnitDetails(unit);
        } else {
            Debug.Log("New Worker");
            WorkerDao unit = new WorkerDao("", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this.gc.units.Add(unit.id, unit);
            this.ShowUnitDetails(unit);
        }

        CloseModal(this.modalNewUnit);
        this.ImportUnits();
    }

    public void CreateNewBaseMaterial() {

        Debug.Log("New Base Material");
        BaseMaterialDao baseMaterial = new BaseMaterialDao("", "");
        this.gc.baseMaterials.Add(baseMaterial.id, baseMaterial);
        this.ShowBaseMaterialDetails(baseMaterial);

        this.ImportBaseMaterials();
    }

    public void CreateNewMaterialSource() {

        Debug.Log("New Material Source");
        MaterialSourceDao materialSource = new MaterialSourceDao("", "", 0, null);
        this.gc.mapComponents.Add(materialSource.id, materialSource);
        this.ShowMaterialSourceDetails(materialSource);

        this.ImportMaterialSources();
    }

    public void CreateNewMapComponent() {

        Debug.Log("New Map Component");
        MapComponentDao mapComponent = new MapComponentDao("", "");
        this.gc.mapComponents.Add(mapComponent.id, mapComponent);
        this.ShowMapComponentDetails(mapComponent);

        this.ImportMapComponents();
    }

    public void Save(object obj) {
        Debug.Log("Save => " + obj + " - " + obj.GetType());

        if (obj.GetType() == typeof(TileDao)) {
            Debug.Log("SaveTileDao");

            TileDao tile = (TileDao)obj;
            tile.name = this.tileForm.name.text;
            tile.terraineType = float.Parse(this.tileForm.terraineType.text);
            tile.isWalkable = this.tileForm.isWalkable.isOn;
            tile.canBuild = this.tileForm.canBuild.isOn;

        } else if (obj.GetType() == typeof(BuildingDao)) {
            Debug.Log("SaveBuildingDao");

            BuildingDao building = (BuildingDao)obj;
            building.name = this.buildingForm.name.text;

        } else if (obj.GetType().BaseType == typeof(UnitDao)) {
            Debug.Log("SaveUnitDao (WorkerDao e CombatDao)");

            UnitDao unit = (UnitDao)obj;
            unit.name = this.unitForm.name.text;
            unit.attack = float.Parse(this.unitForm.attack.text);
            unit.defense = float.Parse(this.unitForm.defense.text);
            unit.walkSpeed = float.Parse(this.unitForm.walkSpeed.text);
            unit.lifeTotal = float.Parse(this.unitForm.lifeTotal.text);
            unit.visionField = int.Parse(this.unitForm.visionField.text);
            unit.trainingTime = float.Parse(this.unitForm.trainingTime.text);
            unit.range = float.Parse(this.unitForm.range.text);
            unit.attackSpeed = float.Parse(this.unitForm.attackSpeed.text);

        } else if(obj.GetType() == typeof(BaseMaterialDao)) {
            Debug.Log("SaveBaseMaterial");

            BaseMaterialDao baseMaterial = (BaseMaterialDao)obj;
            baseMaterial.name = this.baseMaterialForm.name.text;

        }
        
        this.gc.SaveGame();
    }

    public void Delete(object obj) {
        Debug.Log("Delete => " + obj + " - " + obj.GetType());

        if(obj.GetType() == typeof(TileDao)) {
            TileDao tile = (TileDao)obj;

            this.gc.tiles.Remove(tile.id);

            this.ImportTiles();
            this.tileForm.view.SetActive(false);

        } else if (obj.GetType() == typeof(BuildingDao)) {
            BuildingDao building = (BuildingDao)obj;

            this.gc.buildings.Remove(building.id);

            this.ImportBuildings();
            this.buildingForm.view.SetActive(false);

        } else if(obj.GetType().BaseType == typeof(UnitDao)) {
            UnitDao unit = (UnitDao)obj;

            this.gc.units.Remove(unit.id);

            this.ImportUnits();
            this.unitForm.view.SetActive(false);

        } else if(obj.GetType() == typeof(BaseMaterialDao)) {
            BaseMaterialDao baseMaterial = (BaseMaterialDao)obj;

            this.gc.baseMaterials.Remove(baseMaterial.id);

            this.ImportBaseMaterials();
            this.baseMaterialForm.view.SetActive(false);

        }

        this.buttonSave.SetActive(false);
        this.buttonDelete.SetActive(false);
        this.gc.SaveGame();
    }

}