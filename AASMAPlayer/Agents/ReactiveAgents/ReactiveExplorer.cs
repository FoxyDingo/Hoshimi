using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.ReactiveAgents
{
    //this is an explorer that does not move much (not much of a good explorer) but is a mouthfull
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class ReactiveExplorer : AASMAExplorer
    {
        ReactiveAgent agent = new ReactiveAgent();

        public ReactiveExplorer() : base() 
        {
            //Move away from nav point
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    bool overNav = false;
                    
                    if (this.State == NanoBotState.WaitingOrders)
                    {
                        List<Point> points = new List<Point>();
                        foreach (var perception in perceptions)
                        {

                            if (perception.isType(PerceptionType.NavPoint))
                            {
                                
                                NavPointPerception per = (NavPointPerception)perception;
                                points.Add(per.getPoint());
                            }
                        }
                       
                        if (points.Count > 0)
                        {
                            Point p = Utils.getNearestPoint(this.Location, points);
                            if (p.Equals(this.Location))
                            {
                                overNav = true;
                            }
                        }
                        
                       
                        if (overNav)
                        {
                            getAASMAFramework().logData(this, "I want to move away from this nav point ");
                        }
                        return overNav;


                    }
                    return false;
                    
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this, "Moving away from nav point");
                    Point obj = this.Location;
                    obj = Utils.pointAway(obj, this.Scan, this.PlayerOwner.Tissue);
                    this.MoveTo(obj);
                }
            );

            //Go to Nav point
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    bool foundNav = false;
                    if (this.State == NanoBotState.WaitingOrders)
                    {
                        List<Point> points = new List<Point>();
                        foreach (var perception in perceptions)
                        {

                            if (perception.isType(PerceptionType.NavPoint))
                            {
                                foundNav = true;
                                NavPointPerception per = (NavPointPerception)perception;
                                points.Add(per.getPoint());
                            }
                        }
                        
                        if (foundNav)
                        {
                            Point p = Utils.getNearestPoint(this.Location, points);
                            agent.storeTemp(p);
                            getAASMAFramework().logData(this, "I want to go to a Nav point ");
                        }
                        return foundNav;


                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    Point p = (Point)agent.getTemp();
                    this.MoveTo(p);
                }
            );

            //MOVE RANDOMLY
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this, "I want to move randomly ");
                    if (this.State == NanoBotState.WaitingOrders)
                    {
                        return true;
                    }
                    return false;

                }, delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this, "I am moving randomly ");
                    if (frontClear())
                    {
                        if (Utils.randomValue(100) < 80)
                        {
                            this.MoveForward();
                        }
                        else
                        {
                            this.RandomTurn();
                        }
                    }
                    else
                    {
                        this.RandomTurn();
                    }
                }
            );

        }
        public override void DoActions()
        {
            agent.React(agent.getPerceptions(this, this.getAASMAFramework()));
        }



        public override void receiveMessage(AASMAMessage msg)
        {
            
        }
    }
}
