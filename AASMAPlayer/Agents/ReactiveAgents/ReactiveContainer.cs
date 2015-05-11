using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.ReactiveAgents
{
    
    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class ReactiveContainer : AASMAContainer
    {
        ReactiveAgent agent = new ReactiveAgent();

        public ReactiveContainer()
            : base()
        {
            //Collect AZN
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (this.State == NanoBotState.WaitingOrders && getAASMAFramework().overAZN(this) && Stock < ContainerCapacity)
                    {
                        return true;
                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this, "collecting azn");
                    this.collectAZN();
                }
            );

            //transfer azn to needle
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (this.State == NanoBotState.WaitingOrders && getAASMAFramework().overEmptyNeedle(this) && Stock > 0)
                    {
                        return true;
                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    this.transferAZN();
                }
            );

            //GO TO azn POINT
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    try
                    {
                        
                        if (this.State == NanoBotState.WaitingOrders && Stock < ContainerCapacity)
                        {
                            List<Point> points = new List<Point>();
                            foreach (var perception in perceptions)
                            {

                                if (perception.isType(PerceptionType.AZNPoint))
                                {
                                    
                                    AZNPointPerception per = (AZNPointPerception)perception;
                                    points.Add(per.getPoint());
                                }
                            }
                            if (points.Count > 0)
                            {
                                Point p = Utils.getNearestPoint(this.Location, points);
                                agent.storeTemp(p);
                                getAASMAFramework().logData(this, "I want to go Azn point ");
                                return true;
                            }
                            return false;


                        }
                        return false;
                    }
                    catch (Exception e)
                    {
                        getAASMAFramework().logData(this, "EXC " + e.ToString());
                        return false;
                    }
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    Point p = (Point)agent.getTemp();
                    this.MoveTo(p);
                }
            );

            //Go to empty needle
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                   
                    if (this.State == NanoBotState.WaitingOrders && Stock > 0)
                    {
                        List<Point> points = new List<Point>();
                        foreach (var perception in perceptions)
                        {

                            if (perception.isType(PerceptionType.EmptyNeedle))
                            {
                                
                                EmptyNeedlePerception per = (EmptyNeedlePerception)perception;
                                points.Add(per.getPoint());
                            }
                        }
                        if (points.Count > 0)
                        {
                            Point p = Utils.getNearestPoint(this.Location, points);
                            agent.storeTemp(p);
                            getAASMAFramework().logData(this, "I want to go an empty needle ");
                            return true;
                        }
                        
                       


                    }
                    return false;
                   
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this, "Going to needle");
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
