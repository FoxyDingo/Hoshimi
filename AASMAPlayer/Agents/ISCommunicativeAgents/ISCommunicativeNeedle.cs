using System;
using PH.Common;
using PH.Map;
using System.Collections.Generic;
using System.Drawing;
using AASMAHoshimi.Examples;
using AASMAHoshimi.ReactiveAgents;

using AASMAHoshimi;

namespace AASMAHoshimi.ISCommunicativeAgents
{
    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public class ISCommunicativeNeedle : AASMANeedle
    {
        ReactiveAgent agent = new ReactiveAgent();
        public ISCommunicativeNeedle()
        {
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
                            Point p = Utils.getNearestPoint(this.Location, points);
                            agent.storeTemp(p);
                        }
                        return hasTarget;
                    }
                    return false;

                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    Point p = (Point)agent.getTemp();
                    this.DefendTo(p, 1);
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
