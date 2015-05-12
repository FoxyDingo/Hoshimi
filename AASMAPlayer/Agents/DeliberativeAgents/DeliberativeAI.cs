using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.DeliberativeAgents
{

    class DeliberativeAI : AASMAAI
    {
        Plan plan = new Plan();
        public delegate void ActionDelegate(NanoBot bot);

        bool planIsFinished = true;
        bool planImpossible = false;
        bool previousInstructionIsFinished = true;

        protected DeliberativeAgent agent = new DeliberativeAgent();
        public DeliberativeAI(NanoAI nanoAI)
            : base(nanoAI)
        {
           
           
        }

        public override void DoActions()
        {
            List<KeyValuePair<Desires, Point>> desires = getDesires();
            if (planIsFinished || planImpossible)
            {
                KeyValuePair<Desires, Point> intention = deliberate(desires);
                plan(intention);
                Execute();
            }
            else
            {
                Execute();
            }
        }

        public void plan(KeyValuePair<Desires, Point> intention)
        {
        }

        public KeyValuePair<Desires, Point> deliberate(List<KeyValuePair<Desires, Point>> desires) 
        { 
            foreach(KeyValuePair<Desires, Point> desire in desires)
            {
                if(desire.Key.Equals(Desires.BuildProtector))
                { 
                    return new KeyValuePair<Desires,Point>(desire.Key, desire.Value);
                }
            }
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {
                if (desire.Key.Equals(Desires.BuildExplorer))
                {
                    return new KeyValuePair<Desires, Point>(desire.Key, desire.Value);
                }
            }
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {
                if (desire.Key.Equals(Desires.BuildContainer))
                {
                    return new KeyValuePair<Desires, Point>(desire.Key, desire.Value);
                }
            }
            List<Point> hoshimiPoints = new List<Point>();
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {
                if (desire.Key.Equals(Desires.BuildNeedle))
                {
                    hoshimiPoints.Add(desire.Value);
                }
            }
            Point p = Utils.getNearestPoint(this.getNanoBot().Location, hoshimiPoints);
            return new KeyValuePair<Desires, Point>(Desires.BuildNeedle, p);

        }


        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            List<Perception> perceptions = agent.getPerceptions(this.getNanoBot(), this.getAASMAFramework());
            if (getAASMAFramework().protectorsAlive() < 5)
            {
                desires.Add(new KeyValuePair<Desires, Point>(Desires.BuildProtector, new Point() ));
            }
            if (getAASMAFramework().explorersAlive() < 5)
            {
                desires.Add(new KeyValuePair<Desires, Point>(Desires.BuildExplorer, new Point()));
            }
            if (getAASMAFramework().containersAlive() < 5)
            {
                desires.Add(new KeyValuePair<Desires, Point>(Desires.BuildContainer, new Point()));
            }
            foreach (Perception per in perceptions)
            {
                if (per.getType().Equals(PerceptionType.HoshimiPoint))
                {
                    HoshimiPointPerception p = (HoshimiPointPerception)per;
                    desires.Add(new KeyValuePair<Desires,Point>(Desires.BuildNeedle, p.getPoint()));
                }
            }

            return desires;
        }
       


        

        public override void receiveMessage(AASMAMessage msg)
        {
        }

        
    }
}
