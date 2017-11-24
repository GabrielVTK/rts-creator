using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Order {

    public int idPlayer;

    public bool isActive = true;

    protected float timeCounter;
    
    public abstract void Execute();

    public abstract bool Cooldown();

    protected void RemoveAnotherOrder(List<Unit> units) {

        Player player = GameController.players[units[0].idPlayer];
        Unit unit = units[0];

        FindAndAttackOrder FAO;
        MovementOrder MO;
        DefenseOrder DO;
        AttackOrder AO;
        BuildOrder BO;

        foreach (Order order in GameController.players[unit.idPlayer].orders) {

            if (order != this) {

                if (!unit.isBusy && order.GetType() == typeof(MovementOrder)) {

                    MO = (MovementOrder)order;

                    if (MO.units.Contains(unit)) {

                        if (MO.units.Count != units.Count) {

                            foreach (Unit unitItem in units) {
                                MO.units.Remove(unitItem);
                            }

                            if (MO.units.Count > 0) {
                                player.standbyOrders.Add(MO.Clone());
                            }
                        }

                        MO.isActive = false;

                        break;
                    }

                } else {

                    if (order.GetType().BaseType == typeof(AttackOrder)) {

                        AO = (AttackOrder)order;

                        if (AO.units.Contains(unit)) {

                            if (AO.units.Count != units.Count) {

                                foreach (Unit unitItem in units) {
                                    AO.units.Remove(unitItem);
                                }

                                if (AO.units.Count > 0) {
                                    player.standbyOrders.Add(AO.Clone());
                                }
                            }

                            AO.isActive = false;

                            break;
                        }

                    } else if (order.GetType() == typeof(BuildOrder) && unit.GetType().BaseType == typeof(Worker)) {

                        BO = (BuildOrder)order;
                        
                        if (BO.workers.Contains((Worker)unit)) {

                            if (BO.workers.Count != units.Count) {

                                foreach (Worker unitItem in units) {
                                    BO.workers.Remove(unitItem);
                                }

                                if (BO.workers.Count > 0) {
                                    player.standbyOrders.Add(BO.Clone());
                                }
                            }

                            BO.isActive = false;

                            break;
                        }

                    } else if(order.GetType() == typeof(DefenseOrder)) {

                        DO = (DefenseOrder)order;

                        if(DO.units.Contains(unit)) {

                            if(DO.units.Count == units.Count) {

                                foreach (Unit unitItem in units) {
                                    DO.units.Remove(unitItem);
                                }

                                if (DO.units.Count > 0) {
                                    player.standbyOrders.Add(DO.Clone());
                                }

                            }

                            DO.isActive = false;
                            break;
                        }

                    } else if(order.GetType() == typeof(FindAndAttackOrder)) {
                        FAO = (FindAndAttackOrder)order;

                        if (FAO.units.Contains(unit)) {

                            if (FAO.units.Count == units.Count) {

                                foreach (Worker unitItem in units) {
                                    FAO.units.Remove(unitItem);
                                }

                                if (FAO.units.Count > 0) {
                                    player.standbyOrders.Add(FAO.Clone());
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
