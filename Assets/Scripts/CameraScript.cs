using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public string objectName;

	public GameObject tile;
	public bool wantBuild;

	private BuildingDao buildPlayer;
	private int buildIdPlayer;

	private int canBuildLine;
	private int canBuildColumn;


	void Start () {
		this.objectName = null;
		this.buildPlayer = null;
		this.wantBuild = false;

        if(GameController.draw) {
            this.tile = GameObject.Instantiate(Resources.Load("tile_Help", typeof(GameObject)) as GameObject);
            this.tile.name = "0_tile_0_0_CanBuild";
            this.tile.gameObject.SetActive(false);
        }

	}

	void Update () {

        if(GameController.draw) { 

		    if(wantBuild) {
			    for (int i = 0; i < 2; i++) {
				    if(GameController.players[i].wantBuild != null) {
					    buildIdPlayer = i;
					    buildPlayer = GameController.players[i].wantBuild;
					    break;
				    }
			    }
		    }

		    if(Input.GetMouseButtonDown(0) && Input.mousePosition.y > 200) {
			
			    RaycastHit hit;
			    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

			    if(!GameController.instance.GetComponent<GameController>().isActionButton) {
				
				    objectName = null;

				    if(Physics.Raycast(ray, out hit, 200.0f)) {
					    objectName = hit.transform.name;
				    }

				    if (tile.gameObject.activeSelf) {

					    wantBuild = false;

					    // Se posição válida
					    if(tile.GetComponent<MeshRenderer>().sharedMaterial.color == Color.green) {

						    if(buildPlayer != null) {

							    Player player = GameController.players[buildIdPlayer];

							    if(player.AddBuilding (GameController.map, buildPlayer, new Vector2 (canBuildColumn, canBuildLine))) {
                                
								    Building buildInstance = player.buildings[Property.nextId];
                                    List<Worker> workers = new List<Worker>();

                                    foreach(Unit unit in GameController.instance.GetComponent<GameController>().selectedUnits) {
                                        workers.Add((Worker)unit);
                                    }

                                    Debug.Log("Add BuildOrder");

                                    player.orders.Add(new BuildOrder(player.id, workers, buildInstance));
								    /**
								    foreach(Worker worker in GameController.instance.GetComponent<GameController>().selectedUnits) {
									    worker.targetBuilding = buildInstance;
									    worker.isWalking = false;
								    }
								    /**/

								    GameController.instance.GetComponent<GameController>().LeftClick (GameController.instance.GetComponent<GameController>().selectedUnits[0].model.name, hit.transform.position);
								    GameController.instance.GetComponent<GameController>().DrawInfoMaterials();
							    }

							    buildPlayer = null;
						    }

						    tile.gameObject.SetActive(false);
					    } else {
						    tile.gameObject.SetActive(false);
					    }

				    } else {

					    wantBuild = false;

					    tile.gameObject.SetActive(false);

					    if(hit.transform != null) {
						    GameController.instance.GetComponent<GameController>().LeftClick(objectName, hit.transform.position);
					    }

				    }

			    } else {
				
				    if(Physics.Raycast (ray, out hit, 200.0f) && objectName != null) {
					    GameController.instance.GetComponent<GameController>().RightClick(objectName, hit.transform.name, hit.transform.position.x, hit.transform.position.z);
				    }

				    GameController.instance.GetComponent<GameController> ().isActionButton = false;
			    }
		    }

		    if (wantBuild) {

			    RaycastHit hit;
			    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 

			    if (Physics.Raycast (ray, out hit, 200.0f)) {

				    string[] variables = hit.collider.name.Split ('_');

				    if (variables [1].CompareTo ("tile") == 0 || variables [1].CompareTo ("source") == 0) {
					
					    Map map = GameController.map;

					    float xIni = (float)map.GetTileSize () * (map.width - 1) / -2;
					    float yIni = (float)map.GetTileSize () * (map.height - 1) / 2;
                    
					    //Debug.Log("("+yIni+", "+xIni+")");
					    /**
					    float xAdd = 0.0f, zAdd = 0.0f;

					    if (xIni - Mathf.CeilToInt (xIni) > 0) {
						    xAdd = 0.5f;
					    } else if (xIni - Mathf.CeilToInt (xIni) < 0) {
						    xAdd = -0.5f;
					    }

					    if (yIni - Mathf.CeilToInt (yIni) > 0) {
						    zAdd = 0.5f;
					    } else if (yIni - Mathf.CeilToInt (yIni) < 0) {
						    zAdd = -0.5f;
					    }
                        /**/

					    int line = (map.height / 2) - Mathf.CeilToInt(hit.point.z);
					    int column = Mathf.CeilToInt(hit.point.x) + (map.width / 2) - 1;

					    if (buildPlayer.requireds.Count > 0 && line > 0 && line < map.height && column > 0 && column < map.width) {

						    int requiredNumber = 0;

						    foreach (string required in buildPlayer.requireds) {

							    if (map.tiles[line, column].mapComponent != null &&
                                    map.tiles[line, column].mapComponent.name.CompareTo(required) == 0) {
								    requiredNumber++;
							    }
						    }

						    if (requiredNumber == buildPlayer.requireds.Count) {
							    this.canBuildLine = line;
							    this.canBuildColumn = column;
							    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.green;
						    } else {
							    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.red;
						    }

					    } else if (map.tiles [line, column].canBuild && line > 0 && line < (map.height - 1) && column > 0 && column < (map.width - 1)) {

						    this.canBuildLine = line;
						    this.canBuildColumn = column;
						    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.green;

					    } else {
						    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.red;
					    }

					    /**
                        for (int i = 0; i < map.height; i++) {
						    for (int j = 0; j < map.width; j++) {

							    if (map.tiles [i, j].model.transform.position.x == (Mathf.CeilToInt (hit.point.x) + xAdd) &&
							        map.tiles [i, j].model.transform.position.z == (Mathf.CeilToInt (hit.point.z - 0.5f) + zAdd)) {

								    if (i > 0 && i < (map.height - 1) && j > 0 && j < (map.width - 1)) {

									    if (buildPlayer.requireds.Count > 0) {

										    int requiredNumber = 0;

										    foreach (string required in buildPlayer.requireds) {

											    if (map.tiles [i, j].mapComponent != null) {

												    if (map.tiles [i, j].mapComponent.name.CompareTo (required) == 0) {
													    requiredNumber++;
												    }

											    }

										    }

										    if (requiredNumber == buildPlayer.requireds.Count) {
											    this.canBuildLine = i;
											    this.canBuildColumn = j;
											    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.green;
										    } else {
											    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.red;
										    }
										
									    } else if (map.tiles [i, j].canBuild) {

										    this.canBuildLine = i;
										    this.canBuildColumn = j;
										    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.green;

									    } else {
										    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.red;
									    }
								    } else {

									    tile.GetComponent<MeshRenderer> ().sharedMaterial.color = Color.red;

								    }

								    break;
							    } else {							
								    tile.gameObject.SetActive (false);
							    }

						    }
					    }*/
                    
					    tile.gameObject.SetActive (true);
					    tile.transform.position = new Vector3 (xIni + column, 0.0f, yIni - line);

				    } else {
					    tile.gameObject.SetActive (false);
				    }
			    }
		    }

        }
    }

}
