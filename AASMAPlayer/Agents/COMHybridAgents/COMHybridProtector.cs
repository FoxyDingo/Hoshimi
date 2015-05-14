using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.COMHybridAgents
{

    //however, it is frequent that pierre's neurocontrollers kill the protector before he sees it
    //note that the shooting range is greater than the scan range
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 28)]
    public class COMHybridProtector : AASMAProtector
    {
        protected COMHybridAgent agent;
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private bool planIsFinished = true;
        //TODO
        private bool planImpossible = false;
        PlanCheckPoint currentInstruction = null;

        private List<Point>  pierresToShoot = new List<Point>();
        private bool protect = false;
        private Point protectPoint;

        public COMHybridProtector()
            : base()
        {
            //I'm only interested in EnemyBot perceptions!!
            int[] interests = new int[1];
            interests[0] = (int)PerceptionType.EnemyBot;
            agent = new COMHybridAgent(interests);
        }

        public override void DoActions()
        {
            try
            {
                if (!React())
                {
                    List<KeyValuePair<Desires, Point>> desires = getDesires();
                    if (planIsFinished || planImpossible)
                    {

                        KeyValuePair<Desires, Point> intention = deliberate(desires);
                        plan(intention);
                   //     getAASMAFramework().logData(this, "I am executing intention " + intention.Key);
                        execute();
                    }
                    else
                    {
                    //    getAASMAFramework().logData(this, "Doing a step of the plan");
                        execute();
                    }
                }
            }
            catch (Exception e)
            {
                getAASMAFramework().logData(this, "EXC " + e.Message);
            }




        }

        public bool React()
        {
            bool hasReacted = false;
            //Shoot PIERRES
            List<Point> points = getAASMAFramework().visiblePierres(this);
            if (points.Count > 0 && !hasReacted)
            {
                Point p = Utils.getNearestPoint(this.Location, points);
                AASMAMessage msg = new AASMAMessage(this.InternalName, "PIERRE");
                msg.Tag = p;
                sendToAll(msg, "P");

                if (canShoot(p))
                {
                  //  getAASMAFramework().logData(this, "REACT: I WANNA SHOOT " + p);
                    StopMoving();

                    if (!this.DefendTo(p, 1))
                    {
                        Point[] pointBetween = new Point[2];
                        pointBetween[0] = Location;
                        pointBetween[1] = p;
                        MoveTo(Utils.getValidPoint(this.PlayerOwner.Tissue, Utils.getMiddlePoint(pointBetween)));
                    }
                    hasReacted = true;
                }
            }

            if (pierresToShoot.Count > 0 && !hasReacted)
            {
                do
	            {
	                Point p = Utils.getNearestPoint(this.Location, pierresToShoot);
                    pierresToShoot.Remove(p);
                    if (canShoot(p))
                    {
                   //     getAASMAFramework().logData(this, "REACT: I WANNA SHOOT " + p);
                        StopMoving();

                        if (!this.DefendTo(p, 1))
                        {
                            Point[] pointBetween = new Point[2];
                            pointBetween[0] = Location;
                            pointBetween[1] = p;
                            MoveTo(Utils.getValidPoint(this.PlayerOwner.Tissue, Utils.getMiddlePoint(pointBetween)));
                        }
                        hasReacted = true;
                    }
	                
	            } while (pierresToShoot.Count > 0 && !hasReacted);
                    //pierresToShoot only stay here for one turn
                    pierresToShoot.Clear();
            }

            if (protect && !hasReacted)
            {
                this.StopMoving();
                this.MoveTo(protectPoint);
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
                case PlanCheckPoint.Actions.Attack:
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        this.DefendTo(currentInstruction.location, 3);
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
                case Desires.Attack:
                    if (canShoot(intention.Value))
                    {
                        planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Attack));
                        planIsFinished = false;
                        break;

                    }
                    else
                    {
                        //If we cant shoot it we just move to the halfway point
                        Point[] points = new Point[2];
                        points[0] = this.Location;
                        points[1] = intention.Value;
                        planCheckPoints.Add(new PlanCheckPoint(Utils.getValidPoint(this.getAASMAFramework().Tissue, Utils.getMiddlePoint(points)), PlanCheckPoint.Actions.Move));
                        planIsFinished = false;
                        break;
                    }

            }
        }

        public KeyValuePair<Desires, Point> deliberate(List<KeyValuePair<Desires, Point>> desires)
        {
            List<Point> points = new List<Point>();
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {
                if (desire.Key.Equals(Desires.Attack))
                {
                    points.Add(desire.Value);
                }
            }
            if (points.Count > 0)
            {
                Point p = Utils.getNearestPoint(this.Location, points);
                return new KeyValuePair<Desires, Point>(Desires.Attack, p);
            }
            return new KeyValuePair<Desires, Point>(Desires.None, new Point());
        }

        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            List<Perception> perceptions = agent.getPerceptions(this, this.getAASMAFramework());

            foreach (Perception per in perceptions)
            {
                if (per.getType().Equals(PerceptionType.EnemyBot))
                {
                    EnemyBotPerception p = (EnemyBotPerception)per;
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Attack, p.getPoint()));
                }
            }

            return desires;
        }

        public bool canShoot(Point p)
        {
            double d = this.DefenseDistance;
            return Utils.SquareDistance(p, this.Location) <= (d * d);
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
           /* if (!msg.Sender.Equals(this.InternalName))
            {*/
                if(msg.Content.Equals("PIERRE"))
                {
                    Point point = (Point) msg.Tag;
                    if (!pierresToShoot.Contains(point))
                    {
                        pierresToShoot.Add(point);
                    }
                    
                }

                if (msg.Content.Equals("PROTECT"))
                {
                    Point point = (Point)msg.Tag;
                    this.protect = true;
                    this.protectPoint = point;

                }
          //  }
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

