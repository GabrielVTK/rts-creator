using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order {

    public int idPlayer;

    public bool isActive = true;
    
    public abstract void Execute();

    protected void RemoveAnotherOrder(List<Unit> units) {

        Unit unit = units[0];

        foreach (Order order in GameController.players[unit.idPlayer].orders) {

            if (order != this) {

                if (!unit.isBusy && order.GetType() == typeof(MovementOrder)) {

                    MovementOrder MO = (MovementOrder)order;

                    if (MO.units.Contains(unit)) {

                        if (MO.units.Count != units.Count) {

                            foreach (Unit unitItem in units) {
                                MO.units.Remove(unitItem);
                            }

                            if (MO.units.Count > 0) {
                                GameController.players[unit.idPlayer].standbyOrders.Add(MO.Clone());
                            }
                        }

                        MO.isActive = false;

                        break;
                    }

                } else {

                    if (order.GetType().BaseType == typeof(AttackOrder)) {

                        AttackOrder AO = (AttackOrder)order;

                        if (AO.units.Contains(unit)) {

                            if (AO.units.Count != units.Count) {

                                foreach (Unit unitItem in units) {
                                    AO.units.Remove(unitItem);
                                }

                                if (AO.units.Count > 0) {
                                    GameController.players[unit.idPlayer].standbyOrders.Add(AO.Clone());
                                }
                            }

                            AO.isActive = false;

                            break;
                        }

                    } else if (order.GetType() == typeof(BuildOrder) && unit.GetType().BaseType == typeof(Worker)) {

                        BuildOrder BO = (BuildOrder)order;
                        
                        if (BO.workers.Contains((Worker)unit)) {

                            if (BO.workers.Count != units.Count) {

                                foreach (Worker unitItem in units) {
                                    BO.workers.Remove(unitItem);
                                }

                                if (BO.workers.Count > 0) {
                                    GameController.players[unit.idPlayer].standbyOrders.Add(BO.Clone());
                                }
                            }

                            BO.isActive = false;

                            break;
                        }

                    } else if(order.GetType() == typeof(DefenseOrder)) {
                        DefenseOrder DO = (DefenseOrder)order;

                        if(DO.units.Contains(unit)) {

                            if(DO.units.Count == units.Count) {

                                foreach (Unit unitItem in units) {
                                    DO.units.Remove(unitItem);
                                }

                                if (DO.units.Count > 0) {
                                    GameController.players[unit.idPlayer].standbyOrders.Add(DO.Clone());
                                }

                            }

                            DO.isActive = false;
                            break;
                        }

                    } else if(order.GetType() == typeof(FindAndAttackOrder)) {
                        FindAndAttackOrder FAO = (FindAndAttackOrder)order;

                        if (FAO.units.Contains(unit)) {

                            if (FAO.units.Count == units.Count) {

                                foreach (Worker unitItem in units) {
                                    FAO.units.Remove(unitItem);
                                }

                                if (FAO.units.Count > 0) {
                                    GameController.players[unit.idPlayer].standbyOrders.Add(FAO.Clone());
                                }

                            }

                            FAO.isActive = false;
                            break;
                        }
                    }

                }

            }

        }

    }

}
