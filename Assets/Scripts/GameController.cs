using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using GameDevWare.Serialization;

struct Params {
    public float comidaInicio;
    public float ouroInicio;
    public float qntTrabalhadores;
    public float qntRecursosMin;
    public float comidaMeio;
    public float ouroMeio;
    public float infantaria;
    public float arqueiro;
    public float cavalaria;
    public float qntTropaParaAtaque;
};

public class GameController : MonoBehaviour {

    public float timeOut;

    public static Map map = new Map();
	public static Player[] players;

    public static bool draw = true;

	public static GameComponents gameComponents;

    public float unitTimeUnity = 0.0001f;
	public static float unitTime = 10.0f;
	public float cameraSpeed;

    private float counterTime;

	// Scroll View
	public float contentWidth = 350.0f;

	public int componentsPerLine = 4;
    
	public float componentHeight = 50.0f;
	public float componentMargin = 10.0f;

	private float widthLimit;
	private float heightLimit;

	public string objectName;

	public List<Unit> selectedUnits;

	public bool isActionButton = false;
	private string actionButton;

	public int currentPlayer = 1;

    private Vector3[] positionBase = new Vector3[2];

    public static ScriptAttributes scriptP1 = null;
    public static ScriptAttributes scriptP2 = null;

    private bool endGame;

    private bool playGame;

    public ulong ciclos;

    void Start() {

        this.ciclos = 0;

        this.timeOut = 500.0f;
        this.counterTime = 0.0f;

        this.playGame = false;

        StartCoroutine(RunGame());
	}

    public IEnumerator RunGame() {
        
        while (SceneManager.GetActiveScene().name != "game") {
            yield return null;
        }

        this.endGame = false;

        this.objectName = null;

        gameComponents = new GameComponents();

        this.selectedUnits = new List<Unit>();

        map = gameComponents.map;
        map.Build(gameComponents);

        this.positionBase[0] = new Vector2(14.0f, 6.0f);
        this.positionBase[1] = new Vector2(map.width - positionBase[0].x - gameComponents.buildings[0].size.x, map.height - positionBase[0].y - gameComponents.buildings[0].size.y);
        
        // Players
        players = new Player[2];

        for(int p = 0; p < 2; p++) {
            players[p] = new Player(p, "Player " + (p + 1), 500, false);
            players[p].AddBaseMaterial("Gold", 1000);
            players[p].AddBaseMaterial("Meat", 1000);
        }
        
        players[0].AddBuilding(map, gameComponents.buildings[0], positionBase[0]);
        players[1].AddBuilding(map, gameComponents.buildings[0], positionBase[1]);

        if (GameController.scriptP1 != null && GameController.scriptP2 != null) {
            players[0].scriptAttributes = scriptP1;
            players[1].scriptAttributes = scriptP2;

            Params[] playersParams = new Params[2];

            for(int i = 0; i < 2; i++) {
                playersParams[i].comidaInicio = players[i].scriptAttributes.GetAttribute("EARLYMEAT");
                playersParams[i].ouroInicio = players[i].scriptAttributes.GetAttribute("EARLYGOLD");
                playersParams[i].qntTrabalhadores = players[i].scriptAttributes.GetAttribute("WORKERS");
                playersParams[i].qntRecursosMin = players[i].scriptAttributes.GetAttribute("MATERIALPERMIN");
                playersParams[i].comidaMeio = players[i].scriptAttributes.GetAttribute("MIDMEAT");
                playersParams[i].ouroMeio = players[i].scriptAttributes.GetAttribute("MIDGOLD");
                playersParams[i].infantaria = players[i].scriptAttributes.GetAttribute("INFANTRY");
                playersParams[i].arqueiro = players[i].scriptAttributes.GetAttribute("ARCHER");
                playersParams[i].cavalaria = players[i].scriptAttributes.GetAttribute("CAVALRY");
                playersParams[i].qntTropaParaAtaque = players[i].scriptAttributes.GetAttribute("TROOP");
            }

            File.WriteAllText(Application.dataPath + "/StreamingAssets/game" + PlayerPrefs.GetInt("GameCount") + "/geracao" + AG.numGeracao + "_rodada" + TorneioTabela.rodada + "_partida" + GameInitializer.rountCount + "_players.json", Json.SerializeToString<Params[]>(playersParams));
        } else {

            players[0].scriptAttributes.AddAttribute("EARLYMEAT", 41.7582436f);
            players[0].scriptAttributes.AddAttribute("EARLYGOLD", 58.24176f);
            players[0].scriptAttributes.AddAttribute("WORKERS", 39);
            players[0].scriptAttributes.AddAttribute("MATERIALPERMIN", 290);
            players[0].scriptAttributes.AddAttribute("MIDMEAT", 50f);
            players[0].scriptAttributes.AddAttribute("MIDGOLD", 50f);
            players[0].scriptAttributes.AddAttribute("INFANTRY", 49.4736862f);
            players[0].scriptAttributes.AddAttribute("ARCHER", 21.578949f);
            players[0].scriptAttributes.AddAttribute("CAVALRY", 28.9473686f);
            players[0].scriptAttributes.AddAttribute("TROOP", 100);

            players[1].scriptAttributes.AddAttribute("EARLYMEAT", 34);
            players[1].scriptAttributes.AddAttribute("EARLYGOLD", 66);
            players[1].scriptAttributes.AddAttribute("WORKERS", 6);
            players[1].scriptAttributes.AddAttribute("MATERIALPERMIN", 980);
            players[1].scriptAttributes.AddAttribute("MIDMEAT", 66.99029f);
            players[1].scriptAttributes.AddAttribute("MIDGOLD", 33.00971f);
            players[1].scriptAttributes.AddAttribute("INFANTRY", 21.022728f);
            players[1].scriptAttributes.AddAttribute("ARCHER", 50);
            players[1].scriptAttributes.AddAttribute("CAVALRY", 28.977272f);
            players[1].scriptAttributes.AddAttribute("TROOP", 385);
            
        }

        positionBase[0] = players[0].action.GetBuilding("Base")[0].model.transform.position;
        positionBase[1] = players[1].action.GetBuilding("Base")[0].model.transform.position;

        gameComponents.buildings[0].constructed = false;
        
        widthLimit = ((float)map.GetTileSize() * (map.width) / 2);
        heightLimit = ((float)map.GetTileSize() * (map.height) / 2);
        
        Draw();
        ShowFog();
        CenterCamera();

        this.playGame = true;
    }
    
    void Update() {
        
        this.counterTime += Time.deltaTime;

        this.ciclos++;

        if (this.playGame) {

            unitTime = this.unitTimeUnity;

            // Informações
            if (Input.GetKeyDown("space")) {
                /**/
                Debug.Log("Player " + currentPlayer + " -> " + players[currentPlayer].orders.Count + " ordens.");
                Debug.Log("Ordens de ataque inimiga: " + players[currentPlayer].enemyAttackOrders.Count + " ordens.");
                Debug.Log("Tropas: " + players[currentPlayer].action.GetAmountTroop());
                Debug.Log("Trabalhadores: " + players[currentPlayer].action.GetAmountUnit("Worker"));
                Debug.Log("Comida: " + players[currentPlayer].action.GetProductionRate("Meat"));
                Debug.Log("Ouro: " + players[currentPlayer].action.GetProductionRate("Gold"));
                Debug.Log("Unidades Inimigas Visiveis: " + players[currentPlayer].action.GetVisibleEnemyUnits().Count);
                Debug.Log("Construções Inimigas Visiveis: " + players[currentPlayer].action.GetVisibleEnemyBuildings().Count);
                /**/
            }

            // Troca Jogador
            if (Input.GetKeyDown("tab")) {
                this.currentPlayer = (this.currentPlayer == 0) ? 1 : 0;
                this.objectName = null;
                this.actionButton = null;
                this.isActionButton = false;
                DrawInfoMaterials();
                DrawViewContent();
                DrawViewInfo();
                ShowFog();
                CenterCamera();
            }
            
            Camera camera = Camera.main;

            // Movement Horizontal
            Vector3 movementHorizontal = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * cameraSpeed, 0.0f, 0.0f);

            camera.transform.Translate(movementHorizontal, Space.World);

            if (camera.transform.position.x > widthLimit) {
                camera.transform.Translate(widthLimit - camera.transform.position.x, 0.0f, 0.0f, Space.World);
            } else if (camera.transform.position.x < -widthLimit) {
                camera.transform.Translate(-widthLimit - camera.transform.position.x, 0.0f, 0.0f, Space.World);
            }

            // Movement Vertical
            Vector3 movementVertical = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical") * Time.deltaTime * cameraSpeed);

            camera.transform.Translate(movementVertical, Space.World);

            if (camera.transform.position.z > heightLimit) {
                camera.transform.Translate(0.0f, 0.0f, heightLimit - camera.transform.position.z, Space.World);
            } else if (camera.transform.position.z < -heightLimit) {
                camera.transform.Translate(0.0f, 0.0f, -heightLimit - camera.transform.position.z, Space.World);
            }

            this.positionBase[currentPlayer] = camera.transform.position;

            // Zoom
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f && camera.orthographicSize > 3) {
                camera.orthographicSize -= 1;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0.0f && camera.orthographicSize < 10) {
                camera.orthographicSize += 1;
            }

        }

    }

    void FixedUpdate() {
    
        if(this.playGame) {

		    this.UpdateInfo();

            // IA Script
            for (int p = 0; p < 2; p++) {

                if(players[p].fog.calculatePotential) {
                    players[p].fog.CalculateBlocksPotential();
                }

                /**/
			    if(!players[p].isHuman) {

				    float timeAction = 10 * unitTime;

				    if (players[p].script.timeActionCounter >= timeAction) {
                        players[p].script.Main();
                        players[p].script.timeActionCounter -= timeAction;
				    } else {
					    players[p].script.timeActionCounter += Time.deltaTime;
				    }

			    }
                /**/
            
                foreach (Order order in players[p].orders) {
                    order.Execute();
                }

			    players[p].CleanProperties();
		    }

            if(this.CheckEndGame() && !this.endGame) {
                this.endGame = true;
                Debug.Log("Acabou o jogo!!! Tempo: " + Time.timeSinceLevelLoad);
                

                int propertiesPlayer1 = players[0].buildings.Count;
            
                Log log = new Log();
                log.time = this.counterTime;
                log.ciclos = this.ciclos;
                
                log.AddWinner((propertiesPlayer1 == 0) ? GameController.players[1] : GameController.players[0]);
                log.AddParamWinner((propertiesPlayer1 == 0) ? scriptP2 : scriptP1);
                log.AddLoser((propertiesPlayer1 == 0) ? GameController.players[0] : GameController.players[1]);
                log.units = GameController.players[0].unitsCount + GameController.players[1].unitsCount;
                log.resources = GameController.players[0].resourcesCount + GameController.players[1].resourcesCount;
                log.ordersAttack = GameController.players[0].orderAttackCount + GameController.players[1].orderAttackCount;
                
                log.Save();


                //GameInitializer.rountCount++;
                //SceneManager.LoadScene("GameInitializer");
                SceneManager.UnloadScene(SceneManager.GetSceneByName("game"));

                SceneManager.SetActiveScene(SceneManager.GetSceneByName("gameInitializer"));
            }

        }

    }

	public void UpdateInfo() {

		if(this.objectName != null) {

			string[] variables = this.objectName.Split ('_');

			if (variables.Length >= 1 && variables[1].CompareTo ("unit") == 0) {

			    if(selectedUnits.Count == 1) {
				    //GameObject.Find ("Info/unitLife").GetComponent<Text> ().text = "Vida: " + selectedUnits [0].life + " / " + selectedUnits [0].lifeTotal;	
			    }

				GameObject.Find ("Info/unitQuantity").GetComponent<Text> ().text = "" + selectedUnits.Count;

			} else if (variables [1].CompareTo ("build") == 0) {

				Building build = players[int.Parse (variables [0])].buildings[int.Parse(variables [2])];

				if (build.constructed) {
					GameObject.Find ("Info/buildLife").GetComponent<Text> ().text = "Vida: " + build.life + " / " + build.lifeTotal;
				}

			} else if (variables [1].CompareTo ("source") == 0) {

				MaterialSource materialSource = (MaterialSource)map.tiles [int.Parse (variables [2]), int.Parse (variables [3])].mapComponent;

				if (materialSource != null) {
					GameObject.Find ("Info/baseMaterialQuantity").GetComponent<Text> ().text = "Qtd. = " + materialSource.quantity;
				} else {
					this.objectName = null;
					DrawViewContent();
				}
			}
		}
	}

	public void RemoveSelectedUnit(Unit unit, bool destroy = false) {
		
		if(this.selectedUnits.Count > 0) {
			
			this.selectedUnits.Remove(unit);

			if(destroy) {
				unit.Destroy();
			}
            
			if(this.selectedUnits.Count >= 1) {
				//GameObject.Find("Info/unitQuantity").GetComponent<Text>().text = "" + selectedUnits.Count;
			} else if(this.selectedUnits.Count == 0) {
				GameObject.Find("Main Camera").GetComponent<CameraScript>().objectName = null;
				this.objectName = null;
				DrawViewContent();
			}

			if(this.selectedUnits.Count <= 1) {
				DrawViewInfo ();
			}
		}

	}

	public void AddBuilding(Player player, BuildingDao build) {

		player.wantBuild = build;

		GameObject.Find("Main Camera").GetComponent<CameraScript> ().wantBuild = true;

	}

	public void DestroySource(MaterialSource target) {
		map.tiles[(int)target.position.y, (int)target.position.x].mapComponent = null;
		map.tiles[(int)target.position.y, (int)target.position.x].isWalkable = true;
		map.tiles[(int)target.position.y, (int)target.position.x].canBuild = true;
        
        if(GameController.draw) {
            DestroyImmediate(target.model);
        }

	}

	public bool CheckCanBuild(int i, int j) {

		bool canBuild = true;

		int lineIni = (i - 1 >= 0) ? (i - 1) : i;
		int lineFin = (i + 1 < map.height) ? (i + 1) : i;

		int columnIni = (j - 1 >= 0) ? (j - 1) : j;
		int columnFin = (j + 1 < map.width) ? (j + 1) : j;

		for(i = lineIni; i <= lineFin; i++) {
			for(j = columnIni; j <= columnFin; j++) {
				if(!map.tiles[i, j].isWalkable) {
					canBuild = false;
				}
			}
		}

		return canBuild;
	}

	void Draw() {

        DrawPanel();
		DrawInfoMaterials();

		GameController.map.Draw();

        for (int p = 0; p < 2; p++) {
            GameController.players[p].fog.Draw();
        }
    }

	public void ShowFog() {
	
		GameObject[] peacesP1 = GameObject.FindGameObjectsWithTag("Fog P1");
		GameObject[] peacesP2 = GameObject.FindGameObjectsWithTag("Fog P2");

		bool isP1 = (this.currentPlayer == 0) ? true : false;

		for(int i = 0; i < peacesP1.Length; i++) {
			peacesP1[i].GetComponent<MeshRenderer> ().enabled = isP1;
		}

		for(int i = 0; i < peacesP2.Length; i++) {
			peacesP2[i].GetComponent<MeshRenderer> ().enabled = !isP1;
		}

	}

	public void DrawInfoMaterials() {

        if(GameController.draw) { 

		    GameObject text = GameObject.Find("playerInfo");

		    if (text == null) {
			    text = new GameObject ("playerInfo", typeof(Text));
			    text.gameObject.transform.SetParent (GameObject.Find ("taskbar").transform);
			    text.GetComponent<RectTransform> ().sizeDelta = new Vector2 (320.0f, 200.0f);
			    text.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0.0f, 0.0f, 0.0f);
			    Util.ResetGameObject (text, 1.0f, 1.0f);
			    text.GetComponent<Text> ().font = Font.CreateDynamicFontFromOSFont ("Arial", 14);
			    text.GetComponent<Text> ().fontSize = 25;
			    text.GetComponent<Text> ().color = Color.red;
		    } 

		    text.GetComponent<Text>().text = "Player " + (this.currentPlayer + 1) + "\n";
		    text.GetComponent<Text>().text += "     População: " + players[this.currentPlayer].population + " / " + players[this.currentPlayer].populationLimit + "\n";

		    foreach(BaseMaterial baseMaterial in players[this.currentPlayer].baseMaterials.Keys) {
			    text.GetComponent<Text>().text += "     " + baseMaterial.name + ": " + players[this.currentPlayer].baseMaterials[baseMaterial] + "\n";
		    }

        }

    }

	public void LeftClick(string objectName, Vector3 position) {

		int column = (int)(position.x - (float)0.5 + map.width/2);
		int line = -(int)(position.z + (float)0.5 - map.height/2);

		if(line >= 0 && line < map.height && column >= 0 && column < map.width) {

			if(players[this.currentPlayer].fog.tiles[line, column].unknown) {
				return;
			}

			if (!isActionButton) {
				this.objectName = objectName;
				DrawViewContent ();
				DrawViewInfo ();
			} else {
				isActionButton = false;
			}
		}

	}

	public void RightClick(string objectName, string targetName, float x, float y) {

		string[] variables = objectName.Split ('_');

		if(variables.Length > 3) {
			objectName = selectedUnits [0].model.name;
			variables = objectName.Split ('_');
		}

		int playerId = int.Parse(variables[0]);
		string type = variables[1];
		int index = int.Parse(variables [2]);
        
		if (type.CompareTo("build") == 0) {
			
			Building build = players[playerId].buildings[index];
			build.ChangeCreationPoint(x, y);

		} else if(type.CompareTo("unit") == 0) {

			string[] target = targetName.Split ('_');

			Unit unit = (Unit)selectedUnits[0];

			/**foreach (Unit unitItem in selectedUnits) {

				if(unitItem.target != null) {

					if(unitItem.target.GetType().BaseType == typeof(Unit)) {
						Unit unitTarget = (Unit)unitItem.target;
						unitTarget.isAttacked = false;
					} else if(unitItem.target.GetType() == typeof(Building)) {
						Building buildingTarget = (Building)unitItem.target;
						buildingTarget.isAttacked = false;
					}

				}

				unitItem.target = null;
				unitItem.timeCounter = 0;
			}/**/

			//if(unit.GetType() == typeof(Worker)) {
			//	foreach (Unit unitItem in selectedUnits) {
			//		Worker worker = (Worker)unitItem;

					//worker.targetSource = null;
					//worker.targetBuilding = null;
			//	}
			//}

			if (actionButton.CompareTo("walk") == 0
				//&& (target [1].CompareTo ("tile") == 0 || target [1].CompareTo ("fog") == 0 ||
				//target [1].CompareTo ("build") == 0 || target [1].CompareTo ("unit") == 0)
				)
				{

                Player player = GameController.players[this.selectedUnits[0].idPlayer];

				if (target [1].CompareTo ("build") == 0) {
					
					//Building buildTarget = (Building)players [int.Parse (target [0])].properties [int.Parse (target [2])];
					Building buildTarget = players [int.Parse (target [0])].buildings[int.Parse (target [2])];

                    //foreach (Unit unitItem in selectedUnits) {
                    //unitItem.Walk(buildTarget.position, buildTarget);
                    //}

                    player.orders.Add(new MovementOrder(player.id, this.selectedUnits, buildTarget.position, false));

                } else if(target [1].CompareTo ("unit") == 0) {

					//Unit unitTarget = (Unit)players [int.Parse (target [0])].properties [int.Parse (target [2])];
					Unit unitTarget = players [int.Parse (target [0])].units [int.Parse (target [2])];

                    //foreach (Unit unitItem in selectedUnits) {
                    //unitItem.Walk(unitTarget.position, unitTarget);
                    //}

                    player.orders.Add(new MovementOrder(player.id, this.selectedUnits, unitTarget.position, false));

                } else {

					int line = (map.height / 2) - Mathf.CeilToInt(y);
					int column = Mathf.CeilToInt(x) + (map.width / 2) - 1;

					//Debug.Log("Map: " + map.width + "x" + map.height);

					column = (column < 0) ? (column + 1) : column;
                    
                    player.orders.Add(new MovementOrder(player.id, this.selectedUnits, new Vector2(column, line), false));
                    
				}


			} else if((actionButton.CompareTo("attackC") == 0 || actionButton.CompareTo("attackD") == 0)) {

				if(target[1].CompareTo("unit") == 0 && int.Parse(target[0]) != unit.idPlayer) {

                    Unit unitTarget = (Unit)players[int.Parse(target[0])].units[int.Parse(target[2])];
                    
                    bool isConcentrated = false;

                    if(actionButton.CompareTo("attackC") == 0) {
                    	isConcentrated = true;
                    }

                    List<Unit> targets = new List<Unit>();
                    
                    foreach(Unit targetItem in GameController.players[unitTarget.idPlayer].units.Values) {
                        if(targetItem.position == unitTarget.position) {
                            targets.Add(targetItem);
                        }
                    }

                    Debug.Log("Unidades a serem atacadas: " + targets.Count);

                    GameController.players[unit.idPlayer].orders.Add(new AttackUnitOrder(selectedUnits, targets, isConcentrated));
                    
                } else if(target[1].CompareTo("build") == 0 && unit.idPlayer != int.Parse(target[0])) {

					Building buildingTarget = players[int.Parse(target[0])].buildings[int.Parse(target[2])];

                    GameController.players[unit.idPlayer].orders.Add(new AttackBuildingOrder(selectedUnits, buildingTarget));

					//foreach (Unit unitItem in selectedUnits) {
						//unitItem.Attack(buildTarget);
					//}

				}

			} else if(actionButton.CompareTo("collect") == 0 && target [1].CompareTo ("source") == 0 && unit.GetType() == typeof(Worker)){
				
				//foreach(Worker worker in selectedUnits) {
				//	worker.targetSource = (MaterialSource)map.tiles [int.Parse (target [2]), int.Parse (target [3])].mapComponent;
				//}

			} else if(actionButton.CompareTo("build") == 0 && target[1].CompareTo("build") == 0 && unit.idPlayer == int.Parse(target[0]) && unit.GetType() == typeof(Worker)) {

				//Building build = (Building)players[int.Parse(target[0])].properties[int.Parse(target[2])];
				Building build = players[int.Parse(target[0])].buildings[int.Parse(target[2])];

				//foreach(Worker worker in selectedUnits) {
				//	worker.targetBuilding = build;
				//}

			}

		}

	}

    void DrawPanel() {

        if(GameController.draw) {

            // Taskbar 
            GameObject canvas = new GameObject("taskbar", typeof(Canvas));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            canvas.GetComponent<Canvas>().worldCamera = Camera.main;
            canvas.GetComponent<Canvas>().planeDistance = 1;


            // Background
            GameObject background = new GameObject("background", typeof(Image));
            background.gameObject.transform.SetParent(canvas.transform);
            background.GetComponent<Image>().sprite = Resources.Load("Images/texture_Wood", typeof(Sprite)) as Sprite;
            background.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
            background.GetComponent<RectTransform>().eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
            background.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 0.0f);
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(1024.0f, 200.0f);
            Util.ResetGameObject(background, 0.0f, 0.0f);


            // Panel
            GameObject panel = new GameObject("panel", typeof(Canvas));
            panel.gameObject.transform.SetParent(canvas.transform);
            panel.AddComponent<GraphicRaycaster>();
            panel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, -575.0f, 0.0f);
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(1024.0f, 190.0f);
            Util.ResetGameObject(panel, 0.0f, 1.0f);


            // Scroll View
            GameObject scrollView = new GameObject("Scroll View", typeof(ScrollRect));
            scrollView.gameObject.transform.SetParent(panel.transform);
            scrollView.AddComponent<Image>().color = Color.white;
            scrollView.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(15.0f, -6.0f, 0.0f);
            scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(350.0f, 175.0f);
            scrollView.GetComponent<ScrollRect>().horizontal = false;
            scrollView.GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            scrollView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            Util.ResetGameObject(scrollView, 0.0f, 1.0f);


            // Viewport
            GameObject viewport = new GameObject("Viewport", typeof(Mask));
            viewport.gameObject.transform.SetParent(scrollView.transform);
            viewport.AddComponent<Image>().color = Color.white;
            viewport.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(350.0f, 175.0f);
            Util.ResetGameObject(viewport, 0.0f, 1.0f);


            // Content
            GameObject viewportContent = new GameObject("Content", typeof(RectTransform));
            viewportContent.gameObject.transform.SetParent(viewport.transform);
            viewportContent.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
            viewportContent.GetComponent<RectTransform>().sizeDelta = new Vector2(350.0f, 0.0f);
            Util.ResetGameObject(viewportContent, 0.0f, 1.0f);


            // Scrollbar Vertical
            GameObject scrollbarVertical = new GameObject("Scrollbar Vertical", typeof(Scrollbar));
            scrollbarVertical.gameObject.transform.SetParent(scrollView.transform);
            scrollbarVertical.AddComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            scrollbarVertical.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(20.0f, 0.0f, 0.0f);
            scrollbarVertical.GetComponent<RectTransform>().sizeDelta = new Vector2(20.0f, 175.0f);
            Util.ResetGameObject(scrollbarVertical, 1.0f, 1.0f);


            // Handle
            GameObject scrollbarHandle = new GameObject("Handle", typeof(Image));
            scrollbarHandle.gameObject.transform.SetParent(scrollbarVertical.transform);
            scrollbarHandle.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
            scrollbarHandle.GetComponent<RectTransform>().eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
            scrollbarHandle.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.7f, 0.0f);
            scrollbarHandle.GetComponent<RectTransform>().sizeDelta = new Vector2(10.0f, 20.0f);
            scrollbarHandle.GetComponent<RectTransform>().anchorMin = new Vector2(1.0f, 1.0f);
            scrollbarHandle.GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f);
            scrollbarHandle.GetComponent<RectTransform>().pivot = new Vector2(1.0f, 1.0f);

            // Scroll View Scripts
            scrollView.GetComponent<ScrollRect>().content = viewportContent.GetComponent<RectTransform>();
            scrollView.GetComponent<ScrollRect>().viewport = viewport.GetComponent<RectTransform>();
            scrollView.GetComponent<ScrollRect>().verticalScrollbar = scrollbarVertical.GetComponent<Scrollbar>();

            scrollbarVertical.GetComponent<Scrollbar>().targetGraphic = scrollbarHandle.GetComponent<Graphic>();
            scrollbarVertical.GetComponent<Scrollbar>().handleRect = scrollbarHandle.GetComponent<RectTransform>();
            scrollbarVertical.GetComponent<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;


            // Info
            GameObject info = new GameObject("Info", typeof(RectTransform));
            info.gameObject.transform.SetParent(panel.transform);
            info.AddComponent<Image>().color = Color.white;
            info.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(430.0f, -6.0f, 0.0f);
            info.GetComponent<RectTransform>().sizeDelta = new Vector2(350.0f, 175.0f);
            Util.ResetGameObject(info, 0.0f, 1.0f);


            // Minimap
            GameObject map = new GameObject("Minimap", typeof(RawImage));
            map.gameObject.transform.SetParent(panel.transform);
            map.GetComponent<RawImage>().texture = Resources.Load("minimap_Render", typeof(Texture)) as Texture;
            map.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(831.0f, -6.0f, 0.0f);
            map.GetComponent<RectTransform>().sizeDelta = new Vector2(175.0f, 175.0f);
            Util.ResetGameObject(map, 0.0f, 1.0f);

        }

	}

	public void DrawViewContent() {

        if(GameController.draw) {

		    GameObject content = GameObject.Find("Content");
		    GameObject info = GameObject.Find("Info");

		    while(content.transform.childCount > 0) {
			    GameObject.DestroyImmediate(content.transform.GetChild(0).gameObject);
		    }

		    while(info.transform.childCount > 0) {
			    GameObject.DestroyImmediate(info.transform.GetChild(0).gameObject);
		    }

		    if(objectName == null) { return; }

		    string[] variables = objectName.Split ('_');

		    int playerId = int.Parse (variables [0]);

		    if (this.objectName != null) {

			    string type = variables[1];
			    int propertiesId = int.Parse (variables [2]);

			    if (type.CompareTo ("build") == 0 && playerId == this.currentPlayer) {

                    Building build = players[playerId].buildings[propertiesId];

                    if (build.constructed) {
                        int technologiesNumber = build.technologies.Count;
					    int unitsNumber = build.units.Count;

					    int componentsNumber = technologiesNumber + unitsNumber;

					    int lineNumber = Mathf.CeilToInt ((float)componentsNumber / (float)componentsPerLine);

					    float componentWidth = (contentWidth - componentMargin) / (float)componentsPerLine - componentMargin;

					    GameObject.Find ("Content").GetComponent<RectTransform> ().sizeDelta = new Vector2 (GameObject.Find ("Content").GetComponent<RectTransform> ().sizeDelta.x, lineNumber * (componentHeight + componentMargin) + componentMargin);

					    int i = 0, j = 0;

					    while (technologiesNumber != 0) {

						    if (j == componentsPerLine) {
							    j = 0;
							    i++;
						    }

						    GameObject button = new GameObject ("button", typeof(Image));
						    GameObject icon = GameObject.Instantiate (button);

						    button.transform.SetParent (GameObject.Find ("Content").transform);
						    button.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentWidth, componentHeight);
						    button.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (componentMargin + (float)(j * (componentWidth + componentMargin)), -componentMargin - (float)(i * (componentHeight + componentMargin)), 0.0f);
						    Util.ResetGameObject (button, 0.0f, 1.0f);
						    button.GetComponent<Image> ().color = Color.red;
                            
						    icon.transform.SetParent (button.transform);
						    icon.name = "icon";
						    icon.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentWidth, componentHeight);
						    icon.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0.0f, 0.0f, 0.0f);
						    Util.ResetGameObject (icon, 0.0f, 1.0f);
						    icon.GetComponent<Image> ().sprite = Resources.Load ("Images/material_Wood", typeof(Sprite)) as Sprite;

						    technologiesNumber--;
						    j++;
					    }

					    int element = 0;

					    while (unitsNumber > element) {

						    if (j == componentsPerLine) {
							    j = 0;
							    i++;
						    }

						    UnitDao unit = build.units [element];

						    GameObject button = new GameObject ("button", typeof(Image));
						    GameObject icon = GameObject.Instantiate (button);

						    button.transform.SetParent (GameObject.Find ("Content").transform);
						    button.AddComponent<Button> ();
						    button.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentWidth, componentHeight);
						    button.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (componentMargin + (float)(j * (componentWidth + componentMargin)), -componentMargin - (float)(i * (componentHeight + componentMargin)), 0.0f);
						    Util.ResetGameObject (button, 0.0f, 1.0f);
						    button.GetComponent<Image> ().sprite = Resources.Load ("Images/background_icon", typeof(Sprite)) as Sprite;
						    UnityEngine.Events.UnityAction action = () => { players[playerId].AddUnit(unit, build);DrawInfoMaterials(); };
						    button.GetComponent<Button> ().onClick.AddListener (action);

						    icon.transform.SetParent (button.transform);
						    icon.name = "icon";
						    icon.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentWidth, componentHeight);
						    icon.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0.0f, 0.0f, 0.0f);
						    Util.ResetGameObject (icon, 0.0f, 1.0f);
						    icon.GetComponent<Image> ().sprite = Resources.Load (unit.icon, typeof(Sprite)) as Sprite;

						    element++;
						    j++;
					    }
				    }

			    } else if (type.CompareTo ("unit") == 0 && playerId == this.currentPlayer) {

                    selectedUnits.Clear ();
                    
				    Unit unit = players [playerId].units[propertiesId];

                    float componentWidth = (contentWidth - componentMargin) / (float)componentsPerLine - componentMargin;
                    
				    foreach (Unit item in players[playerId].units.Values) {

					    if (unit.position.x == item.position.x && unit.position.y == item.position.y) {
						    selectedUnits.Add (item);
					    }
				    }

				    if (unit.GetType () == typeof(Worker)) {

					    Worker worker = (Worker)unit;
                        
                        int i = 0, j = 0;

					    foreach (BuildingDao building in worker.buildings) {

						    if (j == componentsPerLine) {
							    i++;
							    j = 0;
						    }

						    GameObject button = new GameObject ("button", typeof(Image));
						    GameObject icon = GameObject.Instantiate (button);
						    //GameObject text = new GameObject ("newBuilding", typeof(Text));

						    button.transform.SetParent (GameObject.Find ("Content").transform);
						    button.AddComponent<Button> ();
						    button.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentWidth, componentHeight);
						    button.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (componentMargin + (float)(j * (componentWidth + componentMargin)), -componentMargin - (float)(i * (componentHeight + componentMargin)), 0.0f);
						    Util.ResetGameObject (button, 0.0f, 1.0f);
						    button.GetComponent<Image> ().sprite = Resources.Load ("Images/background_icon", typeof(Sprite)) as Sprite;
						    UnityEngine.Events.UnityAction action = () => { AddBuilding(players[playerId], building); };
						    button.GetComponent<Button>().onClick.AddListener(action);

						    icon.transform.SetParent (button.transform);
						    icon.name = "icon";
						    icon.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentWidth, componentHeight);
						    icon.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0.0f, 0.0f, 0.0f);
						    Util.ResetGameObject (icon, 0.0f, 1.0f);
						    icon.GetComponent<Image> ().sprite = Resources.Load (building.icon, typeof(Sprite)) as Sprite;

						    j++;
					    }
				    }
					
			    } else if (type.CompareTo ("tile") == 0) {

				    GameObject.Find("Main Camera").GetComponent<CameraScript>().objectName = null;

				    this.objectName = null;
			    }

			    if(type.CompareTo ("build") != 0) {
				    GameObject.Find("Main Camera").GetComponent<CameraScript>().tile.gameObject.SetActive(false);
			    }

		    }

        }

    }

	public void DrawViewInfo() {

        if(GameController.draw) { 

		    GameObject info = GameObject.Find("Info");

		    while(info.transform.childCount > 0) {
			    GameObject.DestroyImmediate(info.transform.GetChild(0).gameObject);
		    }

		    if(this.objectName != null) {

			    string[] variables = objectName.Split ('_');

			    string type = variables[1];

			    if(type.CompareTo("build") == 0) {

				    int playerId = int.Parse (variables [0]);

				    int propertiesId = int.Parse (variables [2]);
                    
				    Building build = players[playerId].buildings[propertiesId];

				    if(build.idPlayer == this.currentPlayer) {
					    // Destroy Building
					    GameObject buttonDestroy = new GameObject ("buttonDestroy", typeof(Image));

					    buttonDestroy.transform.SetParent(info.transform);
					    buttonDestroy.AddComponent<Button>();
					    buttonDestroy.GetComponent<RectTransform>().sizeDelta = new Vector2 (40.0f, 40.0f);
					    buttonDestroy.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (-10.0f, 10.0f, 0.0f);
					    Util.ResetGameObject(buttonDestroy, 1.0f, 0.0f);
					    buttonDestroy.GetComponent<Image>().color = Color.red;
					    UnityEngine.Events.UnityAction destroyBuilding = () => { build.Destroy(); };
					    buttonDestroy.GetComponent<Button>().onClick.AddListener(destroyBuilding);
				    }

				    // Icon Building
				    GameObject iconBuilding = new GameObject ("iconBuilding", typeof(Image));

				    iconBuilding.transform.SetParent(info.transform);
				    iconBuilding.GetComponent<RectTransform>().sizeDelta = new Vector2 (50.0f, 50.0f);
				    iconBuilding.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (10.0f, -10.0f, 0.0f);
				    Util.ResetGameObject(iconBuilding, 0.0f, 1.0f);
				    iconBuilding.GetComponent<Image>().sprite = build.icon;

				    // Life
				    GameObject textLife = new GameObject ("buildLife", typeof(Text));
				    textLife.transform.SetParent(info.transform);
				    textLife.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 50.0f);
				    textLife.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(25.0f , -20.0f, 0.0f);
				    Util.ResetGameObject(textLife, 1.0f, 1.0f);
				    textLife.GetComponent<Text>().text = "Vida: " + (build.life >= 0 ? build.life : 0) + " / " + build.lifeTotal;
				    textLife.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
				    textLife.GetComponent<Text>().fontSize = 20;
				    textLife.GetComponent<Text>().color = Color.black;

				    if(!build.constructed) {
					    GameObject text = new GameObject ("constructed", typeof(Text));
					    text.transform.SetParent(info.transform);
					    text.GetComponent<RectTransform>().sizeDelta = new Vector2(250.0f, componentHeight);
					    text.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(140, -60, 0.0f);
					    Util.ResetGameObject(text, 0.0f, 1.0f);
					    text.GetComponent<Text>().text = "Em construção";
					    text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
					    text.GetComponent<Text>().fontSize = 25;
					    text.GetComponent<Text>().color = Color.black;
				    }


			    } else if(type.CompareTo("source") == 0) {

				    int i = int.Parse(variables[2]);
				    int j = int.Parse(variables[3]);

				    MaterialSource materialSource = (MaterialSource)map.tiles [i, j].mapComponent;

				    if (materialSource != null) {
					
					    GameObject icon = new GameObject ("baseMaterial", typeof(Image));

					    icon.transform.SetParent (info.transform);
					    icon.GetComponent<RectTransform> ().sizeDelta = new Vector2 (componentHeight, componentHeight);
					    icon.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (componentMargin, -componentMargin, 0.0f);
					    Util.ResetGameObject (icon, 0.0f, 1.0f);
					    icon.GetComponent<Image> ().sprite = materialSource.baseMaterial.icon;

					    GameObject text = new GameObject ("baseMaterialQuantity", typeof(Text));
					    text.transform.SetParent(info.transform);
					    text.GetComponent<RectTransform>().sizeDelta = new Vector2(250.0f, componentHeight);
					    text.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(2 * componentMargin + componentHeight, -2 * componentMargin, 0.0f);
					    Util.ResetGameObject(text, 0.0f, 1.0f);
					    text.GetComponent<Text>().text = "Qtd. = " + materialSource.quantity;
					    text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
					    text.GetComponent<Text>().fontSize = 25;
					    text.GetComponent<Text>().color = Color.black;

				    } else {
					    this.objectName = null;
				    }

			    } else if(type.CompareTo("unit") == 0) {
                    
				    Unit unit = players[int.Parse (variables [0])].units[int.Parse (variables [2])];
				    //Unit unit = selectedUnits[0];

				    // Unit icon
				    GameObject unitIcon = new GameObject ("unitIcon", typeof(Image));

				    unitIcon.transform.SetParent(info.transform);
				    unitIcon.GetComponent<RectTransform>().sizeDelta = new Vector2 (50.0f, 50.0f);
				    unitIcon.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (10.0f, 10.0f, 0.0f);
				    Util.ResetGameObject(unitIcon, 0.0f, 0.0f);
				    unitIcon.GetComponent<Image>().sprite = unit.icon;

				    if(selectedUnits.Count == 1) {
					    GameObject textLife = new GameObject ("unitLife", typeof(Text));
					    textLife.transform.SetParent(info.transform);
					    textLife.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 50.0f);
					    textLife.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(10.0f , -10.0f, 0.0f);
					    Util.ResetGameObject(textLife, 0.0f, 1.0f);
					    textLife.GetComponent<Text>().text = "Vida: " + (unit.life >= 0 ? unit.life : 0) + " / " + unit.lifeTotal;
					    textLife.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
					    textLife.GetComponent<Text>().fontSize = 20;
					    textLife.GetComponent<Text>().color = Color.black;
				    }

				    GameObject text = new GameObject ("unitQuantity", typeof(Text));
				    text.transform.SetParent(info.transform);
				    text.GetComponent<RectTransform>().sizeDelta = new Vector2(200.0f, 50.0f);
				    text.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(55.0f , -15.0f, 0.0f);
				    Util.ResetGameObject(text, 0.0f, 0.0f);
				    text.GetComponent<Text>().text = "" + selectedUnits.Count;
				    text.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
				    text.GetComponent<Text>().fontSize = 25;
				    text.GetComponent<Text>().color = Color.red;

				    if(unit.idPlayer == this.currentPlayer) {

					    int textActionSize = 15;
					    float widthAction = 100;
					    float heightAction = 23;
				
					    UnityEngine.Events.UnityAction action;
					    int btn = 1;

					    GameObject action1 = new GameObject ("buttonAction", typeof(Image));
					    action1.transform.SetParent(info.transform);
					    action1.AddComponent<Button>();
					    action1.GetComponent<RectTransform>().sizeDelta = new Vector2 (widthAction, heightAction);
					    action1.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (-10.0f, -10.0f, 0.0f);
					    Util.ResetGameObject(action1, 1.0f, 1.0f);
					    action1.GetComponent<Image>().color = Color.blue;
					    action = () => { ActionButton("walk"); };
					    action1.GetComponent<Button>().onClick.AddListener(action);

					    GameObject textAction1 = new GameObject ("buttonActionText", typeof(Text));
					    textAction1.transform.SetParent(action1.transform);
					    textAction1.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAction, heightAction);
					    textAction1.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
					    Util.ResetGameObject(textAction1, 0.5f, 1.0f);
					    textAction1.GetComponent<Text>().text = "ANDAR";
					    textAction1.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
					    textAction1.GetComponent<Text>().fontSize = textActionSize;
					    textAction1.GetComponent<Text>().color = Color.white;
					    textAction1.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

					    if(unit.GetType() == typeof(Worker)) {

						    GameObject action2 = new GameObject ("buttonAction", typeof(Image));
						    action2.transform.SetParent(info.transform);
						    action2.AddComponent<Button>();
						    action2.GetComponent<RectTransform>().sizeDelta = new Vector2 (widthAction, heightAction);
						    action2.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (-10.0f, -10 -btn*(heightAction + 10), 0.0f);
						    Util.ResetGameObject(action2, 1.0f, 1.0f);
						    action2.GetComponent<Image>().color = Color.blue;
						    action = () => { ActionButton("collect"); };
						    action2.GetComponent<Button>().onClick.AddListener(action);

						    GameObject textAction2 = new GameObject ("buttonActionText", typeof(Text));
						    textAction2.transform.SetParent(action2.transform);
						    textAction2.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAction, heightAction);
						    textAction2.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
						    Util.ResetGameObject(textAction2, 0.5f, 1.0f);
						    textAction2.GetComponent<Text>().text = "COLETAR";
						    textAction2.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
						    textAction2.GetComponent<Text>().fontSize = textActionSize;
						    textAction2.GetComponent<Text>().color = Color.white;
						    textAction2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

						    btn++;

						    GameObject action3 = new GameObject ("buttonAction", typeof(Image));
						    action3.transform.SetParent(info.transform);
						    action3.AddComponent<Button>();
						    action3.GetComponent<RectTransform>().sizeDelta = new Vector2 (widthAction, heightAction);
						    action3.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (-10.0f, -10 -btn*(heightAction + 10), 0.0f);
						    Util.ResetGameObject(action3, 1.0f, 1.0f);
						    action3.GetComponent<Image>().color = Color.blue;
						    action = () => { ActionButton("build"); };
						    action3.GetComponent<Button>().onClick.AddListener(action);

						    GameObject textAction3 = new GameObject ("buttonActionText", typeof(Text));
						    textAction3.transform.SetParent(action3.transform);
						    textAction3.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAction, heightAction);
						    textAction3.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
						    Util.ResetGameObject(textAction3, 0.5f, 1.0f);
						    textAction3.GetComponent<Text>().text = "CONSTRUIR";
						    textAction3.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
						    textAction3.GetComponent<Text>().fontSize = textActionSize;
						    textAction3.GetComponent<Text>().color = Color.white;
						    textAction3.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

						    btn++;
					    }

					    GameObject action4 = new GameObject ("buttonAction", typeof(Image));
					    action4.transform.SetParent(info.transform);
					    action4.AddComponent<Button>();
					    action4.GetComponent<RectTransform>().sizeDelta = new Vector2 (widthAction, heightAction);
					    action4.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (-10.0f, -10 -btn*(heightAction + 10), 0.0f);
					    Util.ResetGameObject(action4, 1.0f, 1.0f);
					    action4.GetComponent<Image>().color = Color.blue;
					    action = () => { ActionButton("attackC"); };
					    action4.GetComponent<Button>().onClick.AddListener(action);

					    GameObject textAction4 = new GameObject ("buttonActionText", typeof(Text));
					    textAction4.transform.SetParent(action4.transform);
					    textAction4.GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f, heightAction);
					    textAction4.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
					    Util.ResetGameObject(textAction4, 0.5f, 1.0f);
					    textAction4.GetComponent<Text>().text = "ATAQUE C";
					    textAction4.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
					    textAction4.GetComponent<Text>().fontSize = textActionSize;
					    textAction4.GetComponent<Text>().color = Color.white;
					    textAction4.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

					    btn++;

					    GameObject action5 = new GameObject ("buttonAction", typeof(Image));
					    action5.transform.SetParent(info.transform);
					    action5.AddComponent<Button>();
					    action5.GetComponent<RectTransform>().sizeDelta = new Vector2 (widthAction, heightAction);
					    action5.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (-10.0f, -10 -btn*(heightAction + 10), 0.0f);
					    Util.ResetGameObject(action5, 1.0f, 1.0f);
					    action5.GetComponent<Image>().color = Color.blue;
					    action = () => { ActionButton("attackD"); };
					    action5.GetComponent<Button>().onClick.AddListener(action);

					    GameObject textAction5 = new GameObject ("buttonActionText", typeof(Text));
					    textAction5.transform.SetParent(action5.transform);
					    textAction5.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAction, heightAction);
					    textAction5.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
					    Util.ResetGameObject(textAction5, 0.5f, 1.0f);
					    textAction5.GetComponent<Text>().text = "ATAQUE D";
					    textAction5.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 14);
					    textAction5.GetComponent<Text>().fontSize = textActionSize;
					    textAction5.GetComponent<Text>().color = Color.white;
					    textAction5.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;


					    GameObject button = new GameObject ("buttonRemove", typeof(Image));
					    button.transform.SetParent(info.transform);
					    button.AddComponent<Button>();
					    button.GetComponent<RectTransform>().sizeDelta = new Vector2 (30.0f, 30.0f);
					    button.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (90.0f, 10.0f, 0.0f);
					    Util.ResetGameObject(button, 0.0f, 0.0f);
					    button.GetComponent<Image>().color = Color.red;
					    action = () => { RemoveSelectedUnit(selectedUnits[0]); };
					    button.GetComponent<Button>().onClick.AddListener(action);

					    GameObject textButtonRemove = new GameObject ("textButtonRemove", typeof(Text));
					    textButtonRemove.transform.SetParent(button.transform);
					    textButtonRemove.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAction, heightAction);
					    textButtonRemove.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
					    Util.ResetGameObject(textButtonRemove, 0.5f, 1.0f);
					    textButtonRemove.GetComponent<Text>().text = "-";
					    textButtonRemove.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 25);
					    textButtonRemove.GetComponent<Text>().fontSize = 25;
					    textButtonRemove.GetComponent<Text>().color = Color.white;
					    textButtonRemove.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

					    GameObject buttonDeleteUnit = new GameObject ("buttonDeleteUnit", typeof(Image));
					    buttonDeleteUnit.transform.SetParent(info.transform);
					    buttonDeleteUnit.AddComponent<Button>();
					    buttonDeleteUnit.GetComponent<RectTransform>().sizeDelta = new Vector2 (30.0f, 30.0f);
					    buttonDeleteUnit.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (175.0f, 10.0f, 0.0f);
					    Util.ResetGameObject(buttonDeleteUnit, 0.0f, 0.0f);
					    buttonDeleteUnit.GetComponent<Image>().color = Color.red;
					    action = () => { RemoveSelectedUnit(selectedUnits[0], true); };
					    buttonDeleteUnit.GetComponent<Button>().onClick.AddListener(action);

					    GameObject textButtonDeleteUnit = new GameObject ("textButtonDeleteUnit", typeof(Text));
					    textButtonDeleteUnit.transform.SetParent(buttonDeleteUnit.transform);
					    textButtonDeleteUnit.GetComponent<RectTransform>().sizeDelta = new Vector2(widthAction, heightAction);
					    textButtonDeleteUnit.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f , 0.0f, 0.0f);
					    Util.ResetGameObject(textButtonDeleteUnit, 0.5f, 1.0f);
					    textButtonDeleteUnit.GetComponent<Text>().text = "X";
					    textButtonDeleteUnit.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 25);
					    textButtonDeleteUnit.GetComponent<Text>().fontSize = 25;
					    textButtonDeleteUnit.GetComponent<Text>().color = Color.white;
					    textButtonDeleteUnit.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

					    if(unit.GetType() == typeof(Worker)) {

						    Worker worker = (Worker)unit;

						    if(worker.materialType != null) {
							    // Icon materialType
							    GameObject iconResourceWork = new GameObject ("iconResourceType", typeof(Image));

							    iconResourceWork.transform.SetParent(info.transform);
							    iconResourceWork.GetComponent<RectTransform>().sizeDelta = new Vector2 (50.0f, 50.0f);
							    iconResourceWork.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (10.0f, -10.0f, 0.0f);
							    Util.ResetGameObject(iconResourceWork, 0.0f, 1.0f);
							    iconResourceWork.GetComponent<Image>().sprite = worker.materialType.icon;
						    }
					    }

				    }

			    }

		    }

        }

    }

	public void ActionButton(string action) {

		GameObject.Find ("Main Camera").GetComponent<CameraScript> ().wantBuild = false;

		this.isActionButton = true;

		this.actionButton = action;
	}

	public void CenterCamera() {

		Camera camera = Camera.main;
        
        camera.transform.position = new Vector3(positionBase[currentPlayer].x, camera.transform.position.y, positionBase[currentPlayer].z);
       
	}

	public void DestroyGameObject(GameObject model) {
		
        if(GameController.draw) {
            Destroy(model);
        }

	}
    
    public bool CheckEndGame() {

        Unit unit;
        Building building;

        for(int i = 0; i <= 1; i++) {

            foreach(Property property in players[i].propertiesDestroied) {

                if(property.GetType().BaseType == typeof(Unit)) {
                    unit = (Unit)property;
                    unit.Destroy();
                } else if(property.GetType() == typeof(Building)) {
                    building = (Building)property;
                    building.Destroy();
                }

            }

            players[i].propertiesDestroied.Clear();
        }

        //int propertiesP1 = GameController.players[0].units.Count + GameController.players[0].buildings.Count;
        //int propertiesP2 = GameController.players[1].units.Count + GameController.players[1].buildings.Count;
        int buildingsP1 = GameController.players[0].buildings.Count;
        int buildingsP2 = GameController.players[1].buildings.Count;
        if (buildingsP1 == 0 || buildingsP2 == 0) {

            if(buildingsP1 > buildingsP2) {
                Debug.Log("P1 venceu o jogo!");
            } else if (buildingsP1 < buildingsP2) {
                Debug.Log("P2 venceu o jogo!");
            } else {
                Debug.Log("O jogo empatou!");
            }

            return true;
        }
        
        /**
        // Limite de tempo
        if(this.counterTime >= this.timeOut) {
            Debug.Log("Tempo de partida excedido!");

            int propertiesP1 = buildingsP1 + GameController.players[0].units.Count;
            int propertiesP2 = buildingsP2 + GameController.players[1].units.Count;

            if(propertiesP1 >= propertiesP2) {

                foreach (Building buildingItem in players[1].buildings.Values) {
                    buildingItem.Destroy();
                }

            } else {
                foreach (Building buildingItem in players[0].buildings.Values) {
                    buildingItem.Destroy();
                }
            }

            return true;
        }
        /**/

        return false;
    }

}