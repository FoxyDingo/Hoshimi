using System;
using System.Collections.Generic;
using PH.Common;
using PH.Map;
using System.Drawing;

using AASMAHoshimi;

namespace AASMAHoshimi.Examples
{
    public class ReactiveAgent
    {
        List<KeyValuePair<RuleConditionDelegate, RuleActionDelegate>> rules = new List<KeyValuePair<RuleConditionDelegate, RuleActionDelegate>>();
        Object temp;
        public ReactiveAgent()
        {
        }

        public void storeTemp(Object obj)
        {
            temp = obj;
        }

        public Object getTemp()
        {
            return temp;
        }

        public delegate bool RuleConditionDelegate(List<Perception> perceptions);
        public delegate void RuleActionDelegate(ReactiveAgent agent, List<Perception> perceptions);

        public void AddRule(RuleConditionDelegate condition, RuleActionDelegate action)
        {
            rules.Add(new KeyValuePair<RuleConditionDelegate, RuleActionDelegate>(condition, action));
        }

        public void React(List<Perception> perceptions)
        {
            foreach (var rule in rules)
            {
                var condtion = rule.Key;
                var action = rule.Value;
                if (condtion(perceptions))
                {
                    action(this, perceptions);
                    return;
                }
            }
        }

        private bool withinDistance(double scanDistance, double distance)
        {
            return distance < (scanDistance * scanDistance);
        }

        /// <summary>
        /// Gets the perceptions of all map elements within a scan distance
        /// </summary>
        /// <param name="bot_">Bot.</param>
        public List<Perception> getPerceptions(NanoBot bot_ , AASMAPlayer aasmaframework)
        {
            var perceptions = new List<Perception>();
            double scanDistance = bot_.Scan + PH.Common.Utils.ScanLength;
            double sqScanDistance = scanDistance * scanDistance;

            // My team's bots
            foreach (NanoBot bot in bot_.PlayerOwner.NanoBots)
            {
                double distance = Utils.SquareDistance(bot_.Location, bot.Location);
                if (distance < sqScanDistance || bot_.NanoBotType == NanoBotType.NanoAI)
                {
                    perceptions.Add(new FriendlyBotPerception(bot, distance));
                }
            }

            // Other team's bots
            if (bot_.PlayerOwner.OtherNanoBotsInfo != null)
            {
                foreach (NanoBotInfo bot in bot_.PlayerOwner.OtherNanoBotsInfo)
                {
                    double distance = Utils.SquareDistance(bot_.Location, bot.Location);
                    if (distance < sqScanDistance)
                    {
                        perceptions.Add(new EnemyBotPerception(bot, distance));
                    }
                }
            }

            // AZN points
            foreach (Point p in aasmaframework.visibleAznPoints(bot_))
            {
                double distance = Utils.SquareDistance(bot_.Location, p);
                if (distance < sqScanDistance)
                {
                    perceptions.Add(new AZNPointPerception(p, distance));
                }
            }

            //Hoshimi points
            foreach (Point p in aasmaframework.visibleHoshimies(bot_))
            {
                double distance = Utils.SquareDistance(bot_.Location, p);
                if (distance < sqScanDistance)
                {
                    
                    perceptions.Add(new HoshimiPointPerception(p, distance));
                }
            }

            // Blood streams
            foreach (BloodStream bstream in bot_.PlayerOwner.Tissue.BloodStreams)
            {
                double distance = Utils.SquareDistance(bot_.Location, bstream.Location);
                if (distance < sqScanDistance)
                {
                    perceptions.Add(new BloodStreamPerception(bstream, distance));
                }
            }

            // TODO cells!!!

            return perceptions;
        }
    }
}