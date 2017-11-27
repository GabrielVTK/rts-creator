using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script {

	public Player player;

	public float timeActionCounter;
        
    private bool showDebug;

    private float penaltyCounter;
    private int penaltyIntensity;
    private bool penalty;

    private bool searchBase;

    private List<Building> myBase;
    private Building buildingHelp;
    private List<Unit> workersAvailable;

    private Unit enemyWorker;
    
    public Script(Player player) {
		this.player = player;
		this.timeActionCounter = 0.0f;

        this.showDebug = true;
        this.penaltyCounter = 0;
        this.penaltyIntensity = 3;
        this.penalty = false;
        this.searchBase = true;
        this.enemyWorker = null;
    }
    
    public void Main() {
        
        this.myBase = player.action.GetBuilding("Base");
        this.workersAvailable = player.action.GetUnitAvailable("Worker");

        this.buildingHelp = null;
        
        foreach (Building buildingItem in player.buildings.Values) {
            buildingHelp = buildingItem;
            break;
        }
        
        // Está sendo atacado?
        if(this.IsAttacked()) { return; }

        // Qtd. Recurso/min < Parâmetro 4?
        if(this.CheckResources()) { return; }

        // Tem quartel?
        if(this.HaveQuarter()) { return; }

        // % de comida < Parâmetro 5?
        if(this.CheckRateMidGame()) { return; }

        // Tropa > Parâmetro 10?
        if(this.CheckTroop()) { return; }

        // Já localizou inimigo?
        if(this.FindEnemy()) { return; }

        // Explora Mapa
        this.ExploreMap();

    }
    
    private bool IsAttacked() {

        if(player.action.VerifyBuildingsIsAttacked()) {

            this.ShowMessage("Está sendo atacado!");
            
            List<Unit> units = new List<Unit>();

            // Tem Tropa?
            if (player.action.GetAmountTroop() > 0) {
                this.ShowMessage("Defende com tropa!");

                List<Unit> troop = player.action.GetTroop();

                for (int i = 0; i < troop.Count / 2; i++) {
                    units.Add(troop[i]);
                }
                
            } else if(player.action.GetAmountUnit("Worker") > 0) {
                this.ShowMessage("Defende com trabalhadores!");

                List<Unit> workers = player.action.GetUnits("Worker");

                for (int i = 0; i < workers.Count / 2; i++) {
                    units.Add(workers[i]);
                }
                
            } else {
                this.ShowMessage("Continua o fluxograma!");
                // Cria tropa
                return false; // Continua o fluxograma
            }

            player.action.DefenseBuilding(units, player.action.GetBuildingsAttacked()[0], true);

            return true;
        }

        return false;
    }
        
    private bool CheckResources() {

        if((player.action.GetProductionRate("Meat") + player.action.GetProductionRate("Gold")) < player.scriptAttributes.GetAttribute("MATERIALPERMIN") / 60) {

            this.ShowMessage("Recursos baixos (inicio)");

            // Tem trabalhadores ociosos?
            if(this.workersAvailable.Count > 0) {

                // % de comida < Parâmetro 1?
                if(player.action.GetProductionRate("Meat") < ((player.scriptAttributes.GetAttribute("EARLYMEAT") / 100) * (player.scriptAttributes.GetAttribute("MATERIALPERMIN") / 60))) {

                    this.ShowMessage("Comida baixa (inicio)");

                    Vector3 pos = player.action.GetNearTile(this.myBase.Count > 0 ? this.myBase[0] : this.buildingHelp);

                    if (pos.x != -1.0f && pos.y != -1.0f) {
                        player.action.Build((Worker)this.workersAvailable[0], pos, "Farm");
                    } else {
                        this.ExploreMap();
                    }                    

                } else {

                    this.ShowMessage("Ouro baixo (inicio)");

                    List<MaterialSource> goldMine = player.action.GetMaterialSourceAvailable("Gold");

                    // Tem mina de ouro disponível?
                    if(goldMine.Count > 0) {
                        this.ShowMessage("Constroí (mina) => " + goldMine[0].position);
                        this.penalty = false;
                        this.penaltyCounter = 0;
                        player.action.Build((Worker)this.workersAvailable[0], goldMine[0].position, "Mine");
                    } else if(!this.penalty) {

                        this.ShowMessage("Explora (mina)");

                        this.penaltyCounter++;
                        this.ExploreMap();

                        if (this.penaltyCounter == 10) {
                            this.penalty = true;
                            this.penaltyCounter *= penaltyIntensity;
                            this.penaltyIntensity++;
                        }

                    } else {

                        this.ShowMessage("Desconta penalidade (mina)");

                        this.penaltyCounter -= 1;

                        if(this.penaltyCounter <= 0) {
                            this.penaltyCounter = 0;
                            this.penalty = false;
                        }

                        return false;
                    }

                }

            } else {

                this.ShowMessage("Sem trabalhadores ociosos!");

                //Qtd. Trabalhadores < Parâmetro 3?
                if (player.action.GetAmountUnit("Worker") < player.scriptAttributes.GetAttribute("WORKERS") && this.myBase.Count > 0) {
                    player.action.AddUnit(this.myBase[0], "Worker");
                } else {
                    this.ShowMessage("Não é possível criar trabalhadores ("+ player.action.GetAmountUnit("Worker") + "). Aguarde...");

                    if(player.action.GetAmountUnit("Worker") == 0) {
                        this.Suicide();
                    }

                }

            }

            return true;
        }

        return false;
    }

    private bool HaveQuarter() {

        if(!player.action.VerifyBuildingExists("Quarter")) {

            Vector3 pos = player.action.GetNearTile(this.myBase.Count > 0 ? this.myBase[0] : this.buildingHelp);

            if (pos.x != -1.0f && pos.y != -1.0f && this.workersAvailable.Count > 0) {
                player.action.Build((Worker)this.workersAvailable[0], pos, "Quarter");
            } else {
                this.ExploreMap();
            }

            return true;
        }

        return false;
    }

    private bool CheckRateMidGame() {

        if(this.workersAvailable.Count > 0 && player.action.GetProductionRate("Meat") < ((player.scriptAttributes.GetAttribute("MIDMEAT") / 100) * (player.scriptAttributes.GetAttribute("MATERIALPERMIN") / 60))) {

            this.ShowMessage("Comida baixa (meio)!");

            Vector3 pos = player.action.GetNearTile(this.myBase.Count > 0? this.myBase[0] : this.buildingHelp);

            if (pos.x != -1.0f && pos.y != -1.0f) {
                player.action.Build((Worker)this.workersAvailable[0], pos, "Farm");
            } else {
                this.ExploreMap();
            }
            
            return true;
        }

        return false;
    }

    private bool CheckTroop() {

        if(player.action.GetAmountTroop() < player.scriptAttributes.GetAttribute("TROOP")) {

            this.ShowMessage("Pouca tropa!");
            
            if (player.action.GetAmountUnit("Infantry") < (player.scriptAttributes.GetAttribute("INFANTRY") * player.scriptAttributes.GetAttribute("TROOP"))) {
                // Cria infantaria
                player.action.AddUnit(player.action.GetBuilding("Quarter")[0], "Infantry");
            } else if (player.action.GetAmountUnit("Archer") < (player.scriptAttributes.GetAttribute("ARCHER") * player.scriptAttributes.GetAttribute("TROOP"))) {
                // Cria arqueiro
                player.action.AddUnit(player.action.GetBuilding("Quarter")[0], "Archer");
            } else {
                // Cria cavalaria
                player.action.AddUnit(player.action.GetBuilding("Quarter")[0], "Cavalry");
            }

            if(player.action.GetProductionRate("Gold") <= 2) {
                this.ShowMessage("Permite a exploração de ouro!");
                this.penaltyCounter = 0;
                this.penalty = false;
            }

            if (((player.action.GetProductionRate("Meat") + player.action.GetProductionRate("Gold")) >= player.scriptAttributes.GetAttribute("MATERIALPERMIN") / 60) &&
                (player.action.GetProductionRate("Gold") == 0 || player.action.GetProductionRate("Meat") == 0)) {
                this.Suicide();
            }

            return true;
        }

        return false;
    }

    private bool FindEnemy() {

        List<Building> enemyBase = player.action.GetEnemyBuildings("Base");

        if(this.searchBase || enemyBase.Count > 0) {

            this.ShowMessage("Procura base inimiga!");

            if(enemyBase.Count > 0) {
                this.ShowMessage("Ataca base inimiga!");
                this.searchBase = false;
                player.action.Attack(player.action.GetTroop(), enemyBase[0]);
                return true;
            } else {
                this.ShowMessage("Base ainda não encontrada!");
            }

        } else {

            this.ShowMessage("Procura trabalhadores e buildings!");

            List<Unit> enemyWorkers = player.action.GetVisibleEnemyUnits("Worker");
            List<Building> enemyBuildings = player.action.GetVisibleEnemyBuildings();

            if(enemyWorkers.Count > 0) {

                this.ShowMessage("Ataca trabalhadores inimigos!");
                
                if(this.enemyWorker == null || !enemyWorkers.Contains(this.enemyWorker)) {
                    this.ShowMessage(player.action.GetTroop()[0].model.name + " ataca " + enemyWorkers[0].model.name);
                    this.enemyWorker = enemyWorkers[0];
                }

                player.action.Attack(player.action.GetTroop(), this.enemyWorker, true);
                
                return true;

            } else if(enemyBuildings.Count > 0) {

                this.ShowMessage("Ataca building(" + enemyBuildings[0].name + ") inimiga!");
                player.action.Attack(player.action.GetTroop(), enemyBuildings[0]);
                return true;
            }

        }

        return false;
    }

    private void ExploreMap() {

        this.ShowMessage("ExploreMap()!");

        if (this.workersAvailable.Count > 0) {
            player.action.ExploreMap(this.workersAvailable[0]);
        } else if (this.myBase.Count > 0) {
            player.action.AddUnit(this.myBase[0], "Worker");
        } else if (player.action.GetTroop().Count > 0) {
            player.action.ExploreMap(player.action.GetTroop()[0]);
        } else if (player.action.VerifyBuildingExists("Quarter")) {
            player.action.AddUnit(player.action.GetBuilding("Quarter")[0], "Infantary");
        } else {
            this.Suicide();
        }

    }

    private void Suicide() {
        
        this.ShowMessage("Impossível continuar jogo! P" + this.player.id + " perdeu!");

        foreach (Building buildingItem in this.player.buildings.Values) {
            this.player.propertiesDestroied.Add(buildingItem);
        }        
    }

    private float messageTimeCounter = 0;

    private void ShowMessage(string message) {
        if (this.showDebug) {
            if(messageTimeCounter >= 1) {
                Debug.Log(this.player.name + ": " + message); 
                messageTimeCounter -= 1;
            } else {
                this.messageTimeCounter += Time.deltaTime;
            }            
        }
    }

}
