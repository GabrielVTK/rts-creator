using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node {
	public Vector2 position;
	public float g;
	public float h;
	public float f;
	public Node previous;

	public Node(Vector2 position) {
		this.position = position;
        this.h = 0.0f;
        this.g = 0.0f;
        this.f = 0.0f;
	}

}

public class Pathfinding {

	private Node origin;
	private Node destiny;
	private Map map;
	private Unit unitCurrent;
    
    private Dictionary<int, Node> opened;
    private Dictionary<int, Node> closed;

	private List<Vector2> movements;

	private Node nextExplored;

	private Fog fog;
    
    private bool rangeConsidered;

    public Pathfinding(Node origin, Node destiny, Unit unit, bool rangeConsidered = true) {
		this.map = GameController.map;
		this.unitCurrent = unit;
        this.origin = origin;
        this.rangeConsidered = rangeConsidered;
        this.destiny = CheckDestiny(destiny);
        this.opened = new Dictionary<int, Node>();
        this.closed = new Dictionary<int, Node>();

		// Optimizations
		this.nextExplored = null; // Diminui de 78702 para 9280 comparações da lista aberta.
		this.movements = new List<Vector2>();

		this.fog = GameController.players[unit.idPlayer].fog;
        
        this.opened.Add((int)(origin.position.y * map.width + origin.position.x), origin);
    }

	public List<Vector2> GetPath() {
        
		Node node = null, current;
		bool bestWay, nodeBlocked;

        int i, j;
        float g, h;

        do {
            
			current = null;
            
            if (nextExplored != null) {

				current = nextExplored;
                nextExplored = null;

			} else {

                //List<Node> lista= this.opened.Values.OrderBy(o => o.f).ToList();

                /**/
                foreach (Node nodeOpen in this.opened.Values) {
                    if(current == null || nodeOpen.f < current.f) {
                        current = nodeOpen;
                    }
                }
                /**/

                //Debug.Log("(Dicionario) Menor nó: " + current.position);
                //Debug.Log("(ListaOrden) Menor nó: " + lista[0].position);
            }
            
            this.opened.Remove((int)(current.position.y * map.width + current.position.x));

            this.closed.Add((int)(current.position.y * map.width + current.position.x), current);
                        

            // Pega todos os nodos ao redor para calcular o custo heurístico de cada um
            for (i = (int)current.position.y - 1; i <= (int)current.position.y + 1; i++) {
				for(j = (int)current.position.x - 1; j <= (int)current.position.x + 1; j++) {

					if (i >= 0 && i < map.height && j >= 0 && j < map.width && !(i == current.position.y && j == current.position.x)) {
                        
						nodeBlocked = false;
                        
						// Verifica se tem fog, se a tile é andável e se a posição é uma tile ao redor da atual
						//if (
                        //    ((!fog.tiles[i, j].unknown && map.tiles [i, j].isWalkable) || fog.tiles[i, j].unknown) &&
                        //   !((int)current.position.x == j && (int)current.position.y == i)
                        //   ) {

                        if(!((int)current.position.x == j && (int)current.position.y == i) && map.tiles[i, j].isWalkable) { 
                            // Verifica se tem unidade no Tile
                            nodeBlocked = this.HaveUnit(i, j);

							if(nodeBlocked) { continue; }

                            nodeBlocked = this.closed.ContainsKey(i * map.width + j);

							if (!nodeBlocked) {

								node = null;
                                
                                if(this.opened.ContainsKey(i * map.width + j)) {
                                    node = this.opened[i * map.width + j];
                                }
                                
								g = current.g + (((int)current.position.x == j || (int)current.position.y == i) ? 10 : 14);
                                
								g *= map.tiles[i, j].terraineType;

                                if (node == null) {

									node = new Node(new Vector2(j, i));
                                    
									h = (Mathf.Abs(destiny.position.x - j) + Mathf.Abs(destiny.position.y - i)) * 10;
                                    
									node.g = g;
									node.h = h;
									node.f = node.g + node.h;
									node.previous = current;

									if(node.f <= current.f) {
										nextExplored = node;
									}
                                    
                                    this.opened.Add((int)(node.position.y * map.width + node.position.x), node);

                                } else if (node.g > g) {
									
									node.g = g;
									node.f = node.g + node.h;
									node.previous = current;

									if(node.f <= current.f) {
										nextExplored = node;
									}
								}
							}
						}
					}
				}	
			}
            
			bestWay = this.closed.ContainsKey((int)(destiny.position.y * map.width + destiny.position.x));
            
		} while(this.opened.Count > 0 && !bestWay);

        try {

            Node nav;
                        
            if(this.closed.ContainsKey((int)(destiny.position.y * map.width + destiny.position.x))) {
                nav = this.closed[(int)(destiny.position.y * map.width + destiny.position.x)];
            } else {
                nav = this.closed[(int)(current.position.y * map.width + current.position.x)];
            }

            while (nav.previous != null) {
                movements.Add(nav.position);
                nav = nav.previous;
            }

        } catch(Exception e) {
            Debug.Log("Player " + this.unitCurrent.idPlayer);
            Debug.Log("BestWay? " + bestWay + " -> " + destiny.position.y + "*" + map.width + "+" + destiny.position.x);
            throw new Exception(e.Message + " = " + origin.position + " -> " + destiny.position);
        }
        
		movements.Reverse();

        return movements;
	}
    
    private Node CheckDestiny(Node destiny) {

        //Debug.Log("Player " + this.unitCurrent.idPlayer);

        //Debug.Log("DestinyIn: " + destiny.position);

        if(!GameController.map.tiles[(int)destiny.position.y, (int)destiny.position.x].isWalkable || HaveUnit((int)destiny.position.y, (int)destiny.position.x)) {
            
            int i, j, range = 1, distCurrent = -1, dist, distRange;
            Vector2 position = destiny.position;
            
            while (distCurrent == -1) {

                for(i = (int)destiny.position.y - range; i <= (int)destiny.position.y + range; i++) {
                    if(i >= 0 && i < map.height) {

                        dist = (int)(Mathf.Abs(i - this.origin.position.y) + Mathf.Abs(destiny.position.x - range - this.origin.position.x));

                        if(destiny.position.x - range >= 0 && !this.HaveUnit(i, (int)destiny.position.x - range) &&
                           GameController.map.tiles[i, (int)(destiny.position.x - range)].isWalkable &&
                           (distCurrent == -1 || dist < distCurrent)) {
                            
                            if (this.rangeConsidered) {
                                
                                distRange = (int)(Mathf.Abs(destiny.position.x - range - destiny.position.x) + Mathf.Abs(i - destiny.position.y));

                                if(distRange <= this.unitCurrent.range) {
                                    distCurrent = dist;
                                    position = new Vector2((destiny.position.x - range), i);
                                }

                            } else {
                                distCurrent = dist;
                                position = new Vector2((destiny.position.x - range), i);
                            }
                            
                        }

                        dist = (int)(Mathf.Abs(i - this.origin.position.y) + Mathf.Abs(destiny.position.x + range - this.origin.position.x));

                        if(destiny.position.x + range < map.width && !this.HaveUnit(i, (int)destiny.position.x + range) &&
                           GameController.map.tiles[i, (int)(destiny.position.x + range)].isWalkable &&
                           (distCurrent == -1 || dist < distCurrent)) {
                            
                            if (this.rangeConsidered) {
                            
                                distRange = (int)(Mathf.Abs(destiny.position.x + range - destiny.position.x) + Mathf.Abs(i - destiny.position.y));

                                if (distRange <= this.unitCurrent.range) {
                                    distCurrent = dist;
                                    position = new Vector2((destiny.position.x + range), i);
                                }
                                
                            } else {
                                distCurrent = dist;
                                position = new Vector2((destiny.position.x + range), i);
                            }

                        }
                    }
                }

                for (j = (int)destiny.position.x - range; j <= (int)destiny.position.x + range; j++) {
                    if (j >= 0 && j < map.width) {

                        dist = (int)(Mathf.Abs(destiny.position.y - range - this.origin.position.y) + Mathf.Abs(j - this.origin.position.x));

                        if(destiny.position.y - range >= 0 && !this.HaveUnit((int)destiny.position.y - range, j) &&
                          GameController.map.tiles[(int)(destiny.position.y - range), j].isWalkable &&
                          (distCurrent == -1 || dist < distCurrent)) {
                            
                            if (this.rangeConsidered) {

                                distRange = (int)(Mathf.Abs(j - destiny.position.x) + Mathf.Abs(destiny.position.y - range - destiny.position.y));

                                if (distRange <= this.unitCurrent.range) {
                                    distCurrent = dist;
                                    position = new Vector2(j, destiny.position.y - range);
                                }

                            } else {
                                distCurrent = dist;
                                position = new Vector2(j, destiny.position.y - range);
                            }

                        }

                        dist = (int)(Mathf.Abs(destiny.position.y + range - this.origin.position.y) + Mathf.Abs(j - this.origin.position.x));

                        if (destiny.position.y + range < map.height && !this.HaveUnit((int)destiny.position.y + range, j) &&
                          GameController.map.tiles[(int)(destiny.position.y + range), j].isWalkable &&
                          (distCurrent == -1 || dist < distCurrent)) {
                            
                            if(this.rangeConsidered) {

                                distRange = (int)(Mathf.Abs(j - destiny.position.x) + Mathf.Abs(destiny.position.y + range - destiny.position.y));

                                if (distRange <= this.unitCurrent.range) {
                                    distCurrent = dist;
                                    position = new Vector2(j, destiny.position.y + range);
                                }

                            } else {
                                distCurrent = dist;
                                position = new Vector2(j, destiny.position.y + range);
                            }

                        }

                    }
                }

                if(this.rangeConsidered && range > this.unitCurrent.range) {
                    Debug.Log("Range considerado!");
                    break;
                }

                range++;
            }

            destiny.position = position;
        }

        //Debug.Log("DestinyOut: " + destiny.position);

        return destiny;
    }

    private bool HaveUnit(int i, int j) {

        List<Unit> unitsTile = this.map.tiles[i, j].units;

        if (unitsTile.Count == 0 ||
           (unitsTile[0].GetType() == this.unitCurrent.GetType() && unitsTile[0].idPlayer == this.unitCurrent.idPlayer)) {
            return false;
        }
        
        return true;
    }

}
