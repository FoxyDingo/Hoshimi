using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.DeliberativeAgents
{
    class DeliberativeAI : AASMAAI
    {
        protected DeliberativeAgent agent = new DeliberativeAgent();
        public DeliberativeAI(NanoAI nanoAI)
            : base(nanoAI)
        {
            agent.AddDesire(delegate(DeliberativeAgent a, List<Perception> beliefs)
                {
                    List<KeyValuePair<int, Point>> list = new List<KeyValuePair<int, Point>>();
                    foreach (Perception belief in beliefs)
                    {

                        if (belief.isType(PerceptionType.HoshimiPoint))
                        {
                            HoshimiPointPerception b = (HoshimiPointPerception)belief;
                            list.Add(new KeyValuePair<int, Point>((int)PerceptionType.HoshimiPoint, b.getPoint()));
                        }
                    }
                    return list;
                    
                }
            );

            agent.AddIntention(delegate(DeliberativeAgent a, List<KeyValuePair<int,Point>> desires)
            {
                List<Point> points = new List<Point>();
                foreach (KeyValuePair<int, Point> desire in desires)
                {
                    //Calculate the closest desire of going to hoshimi point
                    if (desire.Key == (int)PerceptionType.HoshimiPoint)
                        {
                            Point p = desire.Value;
                            points.Add(p);
                            Point nearest = Utils.getNearestPoint(this._nanoAI.Location, points);
                            List<KeyValuePair<int, Point>> ret = new List<KeyValuePair<int, Point>>();
                            ret.Add(new KeyValuePair<int, Point>((int)PerceptionType.HoshimiPoint, nearest));
                            return ret;
                            
                        }
                }
                return new List<KeyValuePair<int,Point>>();
            }
            );
        }

        
    }
}
