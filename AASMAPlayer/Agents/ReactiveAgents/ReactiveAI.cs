using System;
using System.Collections.Generic;
using System.Text;


using System.Drawing;
using PH.Common;
using AASMAHoshimi;
using AASMAHoshimi.Examples;




namespace AASMAHoshimi.ReactiveAgents
{
    public class ReactiveAI : AASMAAI
    {
        ReactiveAgent agent = new ReactiveAgent();
             
        public ReactiveAI(NanoAI nanoAI)
            : base(nanoAI)
        {
            //builds protectors
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (getAASMAFramework().protectorsAlive() < 3 && this._nanoAI.State == NanoBotState.WaitingOrders)
                    {
                        return true;
                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this._nanoAI, "Building PROTECTOR " + this._protectorNumber);
                    this._nanoAI.Build(typeof(ReactiveProtector), "P" + this._protectorNumber++);
                }
            );

            //builds explorers
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (getAASMAFramework().explorersAlive() < 3 && this._nanoAI.State == NanoBotState.WaitingOrders)
                    {
                        return true;
                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this._nanoAI, "Building EXPLORER " + this._explorerNumber);
                    this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
                }
            );

            //builds containers
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (getAASMAFramework().containersAlive() < 10 && this._nanoAI.State == NanoBotState.WaitingOrders)
                    {
                        return true;
                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this._nanoAI, "Building CONTAINER " + this._containerNumber);
                    this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);
                }
            );
                                      
            //GO AWAY FROM HOSHIMI WITH NEEDLE
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (getNanoBot().State == NanoBotState.WaitingOrders && getAASMAFramework().overHoshimiPoint(this._nanoAI) && getAASMAFramework().overNeedle(this.getNanoBot()))
                    {
                        return true;
                    }
                    return false;
                    
                }, delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    Point obj = this._nanoAI.Location;
                    getAASMAFramework().logData(this._nanoAI, "FINDING A NEW POINT");
                    obj = Utils.pointAway(obj, this._nanoAI.Scan, this._nanoAI.PlayerOwner.Tissue);
                    this._nanoAI.MoveTo(obj);
                }
            );

            //BUILD NEEDLE ON HOSHIMI POINT
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    if (getNanoBot().State == NanoBotState.WaitingOrders && getAASMAFramework().overHoshimiPoint(this._nanoAI) && !getAASMAFramework().overNeedle(this.getNanoBot()))
                    {
                        return true;
                    }
                    return false;

                }, delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this._nanoAI, "Building NEEDLE ");
                    this._nanoAI.Build(typeof(ReactiveNeedle), "N" + this._needleNumber++);
                }
            );

            //GO TO HOSHIMI POINT
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    
                   
                    if (getNanoBot().State == NanoBotState.WaitingOrders)
                    {
                        List<Point> points = new List<Point>();
                        foreach (var perception in perceptions)
                        {
                            
                            if (perception.isType(PerceptionType.HoshimiPoint))
                            {
                                
                                HoshimiPointPerception per = (HoshimiPointPerception)perception;
                                points.Add(per.getPoint());
                            }
                        }
                            bool onCicle = true;                     
                            while (onCicle)
                            {
                                if (points.Count > 0)
                                {
                                    Point p = Utils.getNearestPoint(this.getNanoBot().Location, points);
                                    if (needleOnHoshimi(p, perceptions))
                                    {
                                        points.Remove(p);
                                        continue;
                                    } else {
                                        agent.storeTemp(p);
                                        getAASMAFramework().logData(this._nanoAI, "I want to go to Hoshimi ");
                                        return true;
                                    }
                                    
                                } else{
                                    onCicle = false;
                                    return false;
                                }
                            }
                                
                            }
                            
                        
                        return false;

                       
                 
                   

                }, delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    Point p = (Point)agent.getTemp();
                    getAASMAFramework().logData(this._nanoAI, "I am going to hoshimi at " + p.ToString());

                    
                    getNanoBot().MoveTo(p);
                }
            );

            //MOVE RANDOMLY
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this._nanoAI, "I want to move randomly ");
                    if (getNanoBot().State == NanoBotState.WaitingOrders)
                    {
                        return true;
                    }
                    return false;

                }, delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this._nanoAI, "I am moving randomly ");
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
            agent.React(agent.getPerceptions(this._nanoAI, this.getAASMAFramework()));
        }

        public bool needleOnHoshimi(Point point, List<Perception> perceptions)
        {
            List<Point> points = new List<Point>();
            foreach (var perception in perceptions)
            {
                if (perception.isType(PerceptionType.EmptyNeedle) || perception.isType(PerceptionType.FullNeedle)) 
                {
                    EmptyNeedlePerception per = (EmptyNeedlePerception) perception;
                    points.Add(per.getPoint());
                }
            }
            if (points.Count > 0)
            {
                foreach (Point p in points)
                {
                    if (p.Equals(point))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void receiveMessage(AASMAMessage msg)
        {
                      

        }
    }
}
