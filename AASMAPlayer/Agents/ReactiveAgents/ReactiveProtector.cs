using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.ReactiveAgents
{
    //this protector does not move (u should write the code for it), however he will shoot any incoming pierre's that he sees
    //however, it is frequent that pierre's neurocontrollers kill the protector before he sees it
    //note that the shooting range is greater than the scan range
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 28)]
    public class ReactiveProtector : AASMAProtector
    {
         ReactiveAgent agent;

        public ReactiveProtector()
            : base()
        {

            //I'm only interested in EnemyBot perceptions!!
            int[] interests = new int[1];
            interests[0] = (int)PerceptionType.EnemyBot;
            agent = new ReactiveAgent(interests);

            // Shoots the closest pierre nanobot
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    List<Point> points = new List<Point>();
                    bool hasTarget = false;
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        foreach (var perception in perceptions)
                        {
                            if (perception.isType(PerceptionType.EnemyBot))
                            {
                                EnemyBotPerception per = (EnemyBotPerception)perception;
                                Point p = per.getPoint();
                                if (canShoot(p))
                                {
                                    hasTarget = true;
                                    points.Add(p);
                                }
                            }
                        }
                        if (hasTarget)
                        {
                            getAASMAFramework().logData(this, "I want to shoot someone");
                            Point p = Utils.getNearestPoint(this.Location, points);
                            agent.storeTemp(p);
                        }
                        return hasTarget;
                    }
                    return false;

                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    getAASMAFramework().logData(this, "I will shoot someone ");
                    Point p = (Point)agent.getTemp();
                    this.DefendTo(p, 1);
                }
            );

            //MOVE RANDOMLY
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    
                    if (this.State == NanoBotState.WaitingOrders)
                    {
                        getAASMAFramework().logData(this, "I want to move randomly ");
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
        
        public bool canShoot(Point p)
        {
            double d = this.DefenseDistance + PH.Common.Utils.ScanLength;
            return Utils.SquareDistance(p, this.Location) <= (d * d);
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
