using System;
using System.Collections.Generic;

using PH.Common;
using PH.Map;
using System.Drawing;


namespace AASMAHoshimi
{
    public class Agent
    {
        //protected List<KeyValuePair<RuleConditionDelegate, RuleActionDelegate>> rules = new List<KeyValuePair<RuleConditionDelegate, RuleActionDelegate>>();
        protected Object  temp;
        protected bool[] perceptionsInterested;

        public Agent()
        {
            int length = Enum.GetNames(typeof(PerceptionType)).Length;
            perceptionsInterested = new bool[length];
            for (int i = 0; i < length; i++)
            {
                perceptionsInterested[i] = true;
            }
        }

        public Agent(int[] interests)
        {
            int length = Enum.GetNames(typeof(PerceptionType)).Length;
            perceptionsInterested = new bool[length];
            //populate perceptionsInterested with false
            for (int i = 0; i < length; i++)
            {
                perceptionsInterested[i] = false;
            }
            //put true in the perceptions the agent is interested
            for (int i = 0; i < interests.Length; i++)
            {
                perceptionsInterested[interests[i]] = true;
            }
        }

        public void storeTemp(Object obj)
        {
            temp = obj;
        }

        public Object getTemp()
        {
            return temp;
        }


       // public delegate bool RuleConditionDelegate(List<Perception> perceptions);
        //public delegate void RuleActionDelegate(Agent agent, List<Perception> perceptions);

        //public abstract void AddRule(RuleConditionDelegate condition, RuleActionDelegate action);
        

        
        
        protected bool withinDistance(double scanDistance, double distance)
        {
            return distance < (scanDistance * scanDistance);
        }

        /// <summary>
        /// Gets the perceptions of all map elements within a scan distance
        /// </summary>
        /// <param name="bot_">Bot.</param>
        public List<Perception> getPerceptions(NanoBot bot_, AASMAPlayer aasmaframework)
        {
            var perceptions = new List<Perception>();
            double scanDistance = bot_.Scan + PH.Common.Utils.ScanLength;
            double sqScanDistance = scanDistance * scanDistance;

            // My team's bots
            if (perceptionsInterested[(int)PerceptionType.FriendlyBot])
            {
                foreach (NanoBot bot in bot_.PlayerOwner.NanoBots)
                {
                    double distance = Utils.SquareDistance(bot_.Location, bot.Location);
                    if (distance < sqScanDistance || bot_.NanoBotType == NanoBotType.NanoAI)
                    {
                        perceptions.Add(new FriendlyBotPerception(bot, distance));
                    }
                }
            }


            // Other team's bots
            if (perceptionsInterested[(int)PerceptionType.EnemyBot])
            {
                if (bot_.PlayerOwner.OtherNanoBotsInfo != null)
                {

                    foreach (Point p in aasmaframework.visiblePierres(bot_))
                    {
                        double distance = Utils.SquareDistance(bot_.Location, p);

                        perceptions.Add(new EnemyBotPerception(p, distance));

                    }
                }
            }

            // AZN points
            if (perceptionsInterested[(int)PerceptionType.AZNPoint])
            {
                foreach (Point p in aasmaframework.visibleAznPoints(bot_))
                {
                    double distance = Utils.SquareDistance(bot_.Location, p);
                    perceptions.Add(new AZNPointPerception(p, distance));

                }
            }

            //Hoshimi points
            if (perceptionsInterested[(int)PerceptionType.HoshimiPoint])
            {
                foreach (Point p in aasmaframework.visibleHoshimies(bot_))
                {
                    double distance = Utils.SquareDistance(bot_.Location, p);
                    perceptions.Add(new HoshimiPointPerception(p, distance));

                }
            }

            //Nav Points
            if (perceptionsInterested[(int)PerceptionType.NavPoint])
            {
                foreach (Point p in aasmaframework.visibleNavigationPoints(bot_))
                {
                    double distance = Utils.SquareDistance(bot_.Location, p);
                    perceptions.Add(new NavPointPerception(p, distance));
                }
            }

            //Empty Needles
            if (perceptionsInterested[(int)PerceptionType.EmptyNeedle])
            {
                foreach (Point p in aasmaframework.visibleEmptyNeedles(bot_))
                {
                    double distance = Utils.SquareDistance(bot_.Location, p);
                    perceptions.Add(new EmptyNeedlePerception(p, distance));

                }
            }

            //Full Needles
            if (perceptionsInterested[(int)PerceptionType.FullNeedle])
            {
                foreach (Point p in aasmaframework.visibleFullNeedles(bot_))
                {
                    double distance = Utils.SquareDistance(bot_.Location, p);
                    perceptions.Add(new FullNeedlePerception(p, distance));

                }
            }

            // Blood streams
            if (perceptionsInterested[(int)PerceptionType.BloodStream])
            {
                foreach (BloodStream bstream in bot_.PlayerOwner.Tissue.BloodStreams)
                {
                    double distance = Utils.SquareDistance(bot_.Location, bstream.Location);
                    if (distance < sqScanDistance)
                    {
                        perceptions.Add(new BloodStreamPerception(bstream, distance));
                    }
                }
            }

            // TODO cells!!!

            return perceptions;
        }
    }
}
