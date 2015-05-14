using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.COMHybridAgents
{

    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class COMHybridContainer : AASMAContainer
    {
        protected COMHybridAgent agent;
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private bool planIsFinished = true;
        //TODO
        private bool planImpossible = false;
        PlanCheckPoint currentInstruction = null;

        private List<Point> aznPoints = new List<Point>();
        private List<Point> emptyNeedles = new List<Point>();
        private List<Point> aznPointsBroadcasted = new List<Point>();
        private List<Point> fullNeedlesBroadcasted = new List<Point>();
        private List<Point> emptyNeedlesBroadcasted = new List<Point>();


        public COMHybridContainer()
            : base()
        {
            //I'm only interested in AZN and EmptyNeedle and EnemyBot perceptions!!
            int[] interests = new int[4];
            interests[0] = (int)PerceptionType.AZNPoint;
            interests[1] = (int)PerceptionType.EmptyNeedle;
            interests[2] = (int)PerceptionType.FullNeedle;
            interests[3] = (int)PerceptionType.EnemyBot;
            agent = new COMHybridAgent(interests);
        }
        public override void DoActions()
        {
            if (!React())
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

        }

        public bool React()
        {
            bool hasReacted = false;
            //RUN AWAY FROM PIERRES
            List<Point> points = getAASMAFramework().visiblePierres(this);
            if (points.Count > 0 && !hasReacted)
            {
                Point p = Utils.getNearestPoint(this.Location, points);

                AASMAMessage msg = new AASMAMessage(this.InternalName, "PIERRE");
                msg.Tag = p;
                //sendToAll(msg, "P");
                getAASMAFramework().broadCastMessage(msg);

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

                if (planCheckPoints.Count == 0)
                {
                    planIsFinished = true;
                    return;
                }

                currentInstruction = planCheckPoints[0];
                //getAASMAFramework().logData(this, "I have an instruction " + currentInstruction.action);
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
                case PlanCheckPoint.Actions.Collect:
                    // getAASMAFramework().logData(this, "I'm gonna colect");
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        int turns = (ContainerCapacity - Stock) / 5;
                        this.CollectFrom(this.Location, turns);
                    }
                    break;
                case PlanCheckPoint.Actions.Unload:
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        this.TransferTo(this.Location, 5);
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
                case Desires.Collect:
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Move));
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Collect));
                    planIsFinished = false;
                    break;
                case Desires.Unload:
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Move));
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Unload));
                    planIsFinished = false;
                    break;
            }
        }

        public KeyValuePair<Desires, Point> deliberate(List<KeyValuePair<Desires, Point>> desires)
        {
            List<Point> pointsUnload = new List<Point>();
            List<Point> pointsCollect = new List<Point>();
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {

                if (desire.Key.Equals(Desires.Unload))
                {
                    pointsUnload.Add(desire.Value);
                }
                if (desire.Key.Equals(Desires.Collect))
                {
                    pointsCollect.Add(desire.Value);
                }
            }
            if (pointsUnload.Count > 0)
            {
                Point p = Utils.getNearestPoint(this.Location, pointsUnload);
                return new KeyValuePair<Desires, Point>(Desires.Unload, p);
            }
            if (pointsCollect.Count > 0)
            {
                //List<Point> temp = new List<Point>(pointsCollect);
               // temp.AddRange(aznPoints);
                Point p = Utils.getNearestPoint(this.Location, pointsCollect);
                return new KeyValuePair<Desires, Point>(Desires.Collect, p);
            }

            return new KeyValuePair<Desires, Point>(Desires.None, new Point());
        }

        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            if (emptyNeedles.Count > 0 && this.Stock > 0)
            {
                Point point = Utils.getNearestPoint(this.Location, emptyNeedles);
                desires.Add(new KeyValuePair<Desires, Point>(Desires.Unload, point));
            }

            if (aznPoints.Count > 0 && Stock < ContainerCapacity)
            {
                Point point = Utils.getNearestPoint(this.Location, aznPoints);
                desires.Add(new KeyValuePair<Desires, Point>(Desires.Collect, point));
            }
                       
            List<Perception> perceptions = agent.getPerceptions(this, this.getAASMAFramework());

            foreach (Perception per in perceptions)
            {
                //Collect AZN
                if (per.getType().Equals(PerceptionType.AZNPoint) && Stock < ContainerCapacity)
                {
                    AZNPointPerception p = (AZNPointPerception)per;
                    Point point = p.getPoint();
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Collect, point));
                                        
                    if (!aznPointsBroadcasted.Contains(point))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "AZN POINT");
                        msg.Tag = point;
                        aznPointsBroadcasted.Add(point);
                        //sendToAll(msg, "C");
                        getAASMAFramework().broadCastMessage(msg);
                    }

                    if (!aznPoints.Contains(p.getPoint()))
                    {
                        aznPoints.Add(p.getPoint());
                    }
                }
                //Unload AZN
                if (per.getType().Equals(PerceptionType.EmptyNeedle) && this.Stock > 0)
                {
                    EmptyNeedlePerception p = (EmptyNeedlePerception)per;
                    Point point = p.getPoint();
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Unload, point));

                    if (!emptyNeedlesBroadcasted.Contains(point))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "EMPTY NEEDLE");
                        msg.Tag = point;
                        emptyNeedlesBroadcasted.Add(point);
                        //sendToAll(msg, "C");
                        getAASMAFramework().broadCastMessage(msg);
                    }
                }

                if (per.isType(PerceptionType.FullNeedle))
                {
                    FullNeedlePerception p = (FullNeedlePerception)per;
                    Point point = p.getPoint();
                    emptyNeedles.Remove(point);
                    if (!fullNeedlesBroadcasted.Contains(point))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "FULL NEEDLE");
                        msg.Tag = point;
                        fullNeedlesBroadcasted.Add(point);
                        //sendToAll(msg, "C");
                        getAASMAFramework().broadCastMessage(msg);
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
        
        public override void receiveMessage(AASMAMessage msg) 
        {
           /* if(!msg.Sender.Equals(this.InternalName))
            {*/

            getAASMAFramework().logData(this, "received message from " + msg.Sender + " : " + msg.Content);

                if (msg.Content.Equals("AZN POINT"))
                {
                    Point p = (Point)msg.Tag;
                    if (!aznPoints.Contains(p))
                    {
                        aznPoints.Add(p);
                    }
                    if (!aznPointsBroadcasted.Contains(p))
                    {
                        aznPointsBroadcasted.Add(p);
                    }
                }
                if (msg.Content.Equals("FULL NEEDLE"))
                {
                    Point p = (Point)msg.Tag;
                    if (emptyNeedles.Contains(p))
                    {
                        emptyNeedles.Remove(p);
                    }
                    if (!fullNeedlesBroadcasted.Contains(p))
                    {
                        fullNeedlesBroadcasted.Add(p);
                    }
                }
                if (msg.Content.Equals("EMPTY NEEDLE"))
                {
                    Point p = (Point)msg.Tag;
                    if (!emptyNeedles.Contains(p))
                    {
                        emptyNeedles.Add(p);
                    }
                    if (!emptyNeedlesBroadcasted.Contains(p))
                    {
                        emptyNeedlesBroadcasted.Add(p);
                    }
                }
           // }
            
        }

        //Sends msg to all nanobots of type s (E, C, P...)
        public void sendToAll(AASMAMessage msg, string s)
        {
            foreach (NanoBot n in getAASMAFramework().NanoBots)
            {
                if (n.InternalName.StartsWith(s))
                {
                    getAASMAFramework().sendMessage(msg, n.InternalName);
                    getAASMAFramework().logData(this, "sending msg to " + msg.Receiver + " : " + msg.Content);
                }
            }

        }

    }
}

