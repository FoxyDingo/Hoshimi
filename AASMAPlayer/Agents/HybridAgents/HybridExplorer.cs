using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.HybridAgents
{

    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class HybridExplorer : AASMAExplorer
    {
        protected HybridAgent agent;
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private bool planIsFinished = true;
        private List<Point> navPointsVisited = new List<Point>();
        //TODO
        private bool planImpossible = false;
        PlanCheckPoint currentInstruction = null;


        public HybridExplorer()
            : base()
        {
            //I'm only interested in NavPoint and EnemyBot perceptions!!
            int[] interests = new int[2];
            interests[0] = (int)PerceptionType.NavPoint;
            interests[1] = (int)PerceptionType.EnemyBot;
            agent = new HybridAgent(interests);
        }


        public override void DoActions()
        {

            if (!React())
            {

                if (planIsFinished || planImpossible)
                {
                    List<KeyValuePair<Desires, Point>> desires = getDesires();
                    KeyValuePair<Desires, Point> intention = deliberate(desires);
                    plan(intention);
                    execute();
                }
                else
                {

                    execute();
                }
            }
        }

        public bool React()
        {
            bool hasReacted = false;
            //RUN AWAY FROM PIERRES
            List<Point> points = getAASMAFramework().visiblePierres(this);
            if (points.Count > 0 && !hasReacted)
            {
                Point p = Utils.getNearestPoint(this.Location, points);
                int awayVectorX = Location.X - p.X;
                int awayVectorY = Location.Y - p.Y;
                Point awayPoint = new Point(Location.X + awayVectorX / 2, Location.Y + awayVectorY / 2);
                Point validAwayPoint = Utils.getValidPoint(getAASMAFramework().Tissue, awayPoint);

                StopMoving();
                MoveTo(validAwayPoint);

                hasReacted = true;
            }
            
            //We want the agent to follow the plan that he stopped when reacting
            if (hasReacted)
            {
                if (currentInstruction != null)
                {
                    planCheckPoints.Insert(0, currentInstruction);
                }
                return true;
            }

            return false;
        }



        public void execute()
        {
            if (this.State.Equals(NanoBotState.WaitingOrders))
            {
                //here currentInstruction is actually last instruction made
                if (currentInstruction != null)
                {
                    if (currentInstruction.action.Equals(PlanCheckPoint.Actions.Move) && currentInstruction.intention.Equals(Desires.Explore))
                    {
                        if (this.Location.Equals(currentInstruction.location))
                        {
                            navPointsVisited.Add(this.Location);
                        }
                    }
                }

                if (planCheckPoints.Count == 0)
                {
                    planIsFinished = true;
                    return;
                }
                currentInstruction = planCheckPoints[0];
                planCheckPoints.RemoveAt(0);
            }

            switch (currentInstruction.action)
            {
                
                case PlanCheckPoint.Actions.Move:
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        this.MoveTo(currentInstruction.location);
                    }

                    break;
                case PlanCheckPoint.Actions.MoveRandom:
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        moveRandom();
                    }

                    break;
            }
        }

        public void plan(KeyValuePair<Desires, Point> intention)
        {
            planCheckPoints.Clear();

            switch (intention.Key)
            {
                
                case Desires.None:
                    planCheckPoints.Add(new PlanCheckPoint(this.Location, PlanCheckPoint.Actions.MoveRandom));
                    planIsFinished = false;
                    break;
                case Desires.Explore:
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Move, Desires.Explore));
                    planIsFinished = false;
                    break;
            }
        }

        public KeyValuePair<Desires, Point> deliberate(List<KeyValuePair<Desires, Point>> desires)
        {
            List<Point> points = new List<Point>();
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {
                
                if (desire.Key.Equals(Desires.Explore))
                {
                    points.Add(desire.Value);
                }
            }
            if (points.Count > 0)
            {
                Point p = Utils.getNearestPoint(this.Location, points);
                return new KeyValuePair<Desires, Point>(Desires.Explore, p);
            }
            return new KeyValuePair<Desires, Point>(Desires.None, new Point());
        }

        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            List<Perception> perceptions = agent.getPerceptions(this, this.getAASMAFramework());

            foreach (Perception per in perceptions)
            {
               
                if (per.getType().Equals(PerceptionType.NavPoint))
                {
                    NavPointPerception p = (NavPointPerception)per;
                    if (!navPointsVisited.Contains(p.getPoint()))
                    {
                        desires.Add(new KeyValuePair<Desires, Point>(Desires.Explore, p.getPoint()));
                    }

                }
            }

            return desires;
        }

        public void moveRandom()
        {
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

        public override void receiveMessage(AASMAMessage msg) { }
    }
}