using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.DeliberativeAgents
{
    
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class DeliberativeExplorer : AASMAExplorer
    {
        protected DeliberativeAgent agent;
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private List<Point> navPointsVisited = new List<Point>();
        private bool planIsFinished = true;
        
        //TODO
        private bool planImpossible = false;

        PlanCheckPoint currentInstruction = null;

        public DeliberativeExplorer()
            : base()
        {
            //I'm only interested in NavPoint and EnemyBot perceptions!!
            int[] interests = new int[2];
            interests[0] = (int)PerceptionType.NavPoint;
            interests[1] = (int)PerceptionType.EnemyBot;
            agent = new DeliberativeAgent(interests);
        }

       
        public override void DoActions()
        {

           
            List<KeyValuePair<Desires, Point>> desires = getDesires();
            if (planIsFinished || planImpossible)
            {
                
                KeyValuePair<Desires, Point> intention = deliberate(desires);
                plan(intention);
                execute();
            }
            else
            {
                
                execute();
            }
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
                case PlanCheckPoint.Actions.Run:
                    Point p = currentInstruction.location;
                    int awayVectorX = this.Location.X - p.X;
                    int awayVectorY = this.Location.Y - p.Y;
                    Point awayPoint = new Point(this.Location.X + awayVectorX / 2, this.Location.Y + awayVectorY / 2);
                    Point validAwayPoint = Utils.getValidPoint(this.PlayerOwner.Tissue, awayPoint);
                    this.StopMoving();
                    this.MoveTo(validAwayPoint);
                    this.planCheckPoints.Clear();
                    currentInstruction = null;
                    planIsFinished = true;

                    break;
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
                case Desires.Run:
                    planCheckPoints.Clear();
                    this.StopMoving();
                    currentInstruction = null;
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Run));
                    planIsFinished = false;
                    break;
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
                if (desire.Key.Equals(Desires.Run))
                {
                    return new KeyValuePair<Desires, Point>(Desires.Run, desire.Value);
                }

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
                if (per.isType(PerceptionType.EnemyBot))
                {
                    EnemyBotPerception p = (EnemyBotPerception)per;
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Run, p.getPoint()));
                }
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