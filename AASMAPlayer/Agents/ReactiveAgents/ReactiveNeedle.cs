using System;
using PH.Common;
using PH.Map;
using System.Collections.Generic;
using System.Drawing;
using AASMAHoshimi.Examples;


namespace AASMAHoshimi.ReactiveAgents
{
    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public class ReactiveNeedle : AASMANeedle
    {
        ReactiveAgent agent = new ReactiveAgent();
        public ReactiveNeedle()
        {
            // Shoots a pierre nanobot
            // it shoots one of them, may not be the closest one
            agent.AddRule(
                delegate(List<Perception> perceptions)
                {
                    foreach (var perception in perceptions)
                    {
                        if (perception.isType(PerceptionType.EnemyBot))
                        {
                            EnemyBotPerception p = (EnemyBotPerception)perception;
                            NanoBotInfo bot = p.getBot();
                            if (canShoot(bot.Location))
                            {
                                agent.storeTemp(bot);
                                return true;
                            }
                        }
                    }
                    return false;
                },
                delegate(ReactiveAgent a, List<Perception> perceptions)
                {
                    // TODO get the closest one!
                    NanoBotInfo bot = (NanoBotInfo)agent.getTemp();
                    this.DefendTo(bot.Location, 1);
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
            agent.React(agent.getPerceptions(this));
        }

        public override void receiveMessage(AASMAMessage msg)
        {
        }
    }
}
