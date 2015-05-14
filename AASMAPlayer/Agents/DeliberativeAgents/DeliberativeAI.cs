using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.DeliberativeAgents
{

    class DeliberativeAI : AASMAAI
    {
        
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private bool planIsFinished = true;
        //TODO
        private bool planImpossible = false;
      
        PlanCheckPoint currentInstruction;
        

        protected DeliberativeAgent agent = new DeliberativeAgent();
        public DeliberativeAI(NanoAI nanoAI)
            : base(nanoAI)
        {
           
           
        }

        public override void DoActions()
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

        
        public void execute()
        {
            if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
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
                case PlanCheckPoint.Actions.Run:
                        Point p = currentInstruction.location;
                        int awayVectorX = this._nanoAI.Location.X - p.X;
                        int awayVectorY = this._nanoAI.Location.Y - p.Y;
                       // getAASMAFramework().logData(this._nanoAI, "Me " + _nanoAI.Location.X + "," + _nanoAI.Location.Y);
                       // getAASMAFramework().logData(this._nanoAI, "PIERRE " + p.X + "," + p.Y);
                        Point awayPoint = new Point(this._nanoAI.Location.X + (awayVectorX / 2), this._nanoAI.Location.Y + (awayVectorY / 2));
                        Point validAwayPoint = Utils.getValidPoint(this.getNanoBot().PlayerOwner.Tissue, awayPoint);
                        //getAASMAFramework().logData(this._nanoAI, "AwayPoint " + awayPoint.X + "," + awayPoint.Y);
                        //getAASMAFramework().logData(this._nanoAI, "VAwayPoint " + validAwayPoint.X + "," + validAwayPoint.Y);
                        this._nanoAI.StopMoving();
                        this._nanoAI.MoveTo(validAwayPoint);
                        //this._nanoAI.MoveTo(awayPoint);
                        currentInstruction = null;
                        this.planCheckPoints.Clear();
                        planIsFinished = true;
                  
                    break;

                case PlanCheckPoint.Actions.Move:
                    if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
                    {
                        getNanoBot().MoveTo(currentInstruction.location);
                        
                    }
                    
                    break;
                case PlanCheckPoint.Actions.MoveRandom:
                    if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
                    {
                        moveRandom();
                    }
                   
                    break;
                case PlanCheckPoint.Actions.BuildNeedle:
                    if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
                    {
                        this._nanoAI.Build(typeof(DeliberativeNeedle), "N" + this._needleNumber++);
                                                
                    }
                    else
                    {
                        if (this.getAASMAFramework().visibleEmptyNeedles(this.getNanoBot()).Contains(currentInstruction.location) || this.getAASMAFramework().visibleFullNeedles(this.getNanoBot()).Contains(currentInstruction.location))
                        {
                            planIsFinished = true;
                        }
                        
                    }
                    break;
                case PlanCheckPoint.Actions.BuildContainer:
                    if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
                    {
                        this._nanoAI.Build(typeof(DeliberativeContainer), "C" + this._containerNumber++);
                    }
                    
                    break;
                case PlanCheckPoint.Actions.BuildProtector:
                    if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
                    {
                        this._nanoAI.Build(typeof(DeliberativeProtector), "P" + this._protectorNumber++);
                    }
                   
                    break;
                case PlanCheckPoint.Actions.BuildExplorer:
                    if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders))
                    {
                        this._nanoAI.Build(typeof(DeliberativeExplorer), "E" + this._explorerNumber++);
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
                    this._nanoAI.StopMoving();
                    currentInstruction = null;
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Run));
                    planIsFinished = false;
                    break;
                case Desires.None:
                    planCheckPoints.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.MoveRandom));
                    planIsFinished = false;
                    break;
                case Desires.BuildNeedle:
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Move));
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.BuildNeedle));
                    planIsFinished = false;
                    break;
                case Desires.BuildContainer:
                    planCheckPoints.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.BuildContainer));
                    planIsFinished = false;
                    break;
                case Desires.BuildProtector:
                    planCheckPoints.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.BuildProtector));
                    planIsFinished = false;
                    break;
                case Desires.BuildExplorer:
                    planCheckPoints.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.BuildExplorer));
                    planIsFinished = false;
                    break;
            }
        }

        public KeyValuePair<Desires, Point> deliberate(List<KeyValuePair<Desires, Point>> desires) 
        {
            foreach (KeyValuePair<Desires, Point> desire in desires)
            {
                if (desire.Key.Equals(Desires.Run))
                {
                    return new KeyValuePair<Desires, Point>(desire.Key, desire.Value);
                }
            }
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
            if (hoshimiPoints.Count > 0)
            {
                Point p = Utils.getNearestPoint(this.getNanoBot().Location, hoshimiPoints);
                return new KeyValuePair<Desires, Point>(Desires.BuildNeedle, p);
            }
            
            return new KeyValuePair<Desires, Point>(Desires.None, new Point() );
            

           

        }


        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            List<Perception> perceptions = agent.getPerceptions(this.getNanoBot(), this.getAASMAFramework());
            if (getAASMAFramework().protectorsAlive() < 10)
            {
                desires.Add(new KeyValuePair<Desires, Point>(Desires.BuildProtector, new Point() ));
            }
            if (getAASMAFramework().explorersAlive() < 10)
            {
                desires.Add(new KeyValuePair<Desires, Point>(Desires.BuildExplorer, new Point()));
            }
            if (getAASMAFramework().containersAlive() < 10)
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
                if (per.getType().Equals(PerceptionType.EnemyBot))
                {
                    EnemyBotPerception p = (EnemyBotPerception)per;
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Run, p.getPoint()));
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
        }

        
    }
}
