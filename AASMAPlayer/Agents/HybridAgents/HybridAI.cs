using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.HybridAgents
{

    class HybridAI : AASMAAI
    {

        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private bool planIsFinished = true;
        //TODO
        private bool planImpossible = false;

        PlanCheckPoint currentInstruction;


        protected HybridAgent agent = new HybridAgent();
        public HybridAI(NanoAI nanoAI)
            : base(nanoAI)
        {


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
            //TODO RUN AWAY FROM PIERRES
            if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders) && this._protectorNumber < 10)
            {
                this._nanoAI.Build(typeof(HybridProtector), "P" + this._protectorNumber++);

            }
            if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders) && this._explorerNumber < 10)
            {
                this._nanoAI.Build(typeof(HybridExplorer), "E" + this._explorerNumber++);

            }
            if (this.getNanoBot().State.Equals(NanoBotState.WaitingOrders) && this._containerNumber < 10)
            {
                this._nanoAI.Build(typeof(HybridContainer), "C" + this._containerNumber++);

            }
            return false;
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
                        this._nanoAI.Build(typeof(HybridNeedle), "N" + this._needleNumber++);

                    }
                    else
                    {
                        if (this.getAASMAFramework().visibleEmptyNeedles(this.getNanoBot()).Contains(currentInstruction.location) || this.getAASMAFramework().visibleFullNeedles(this.getNanoBot()).Contains(currentInstruction.location))
                        {
                            planIsFinished = true;
                        }

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
                    planCheckPoints.Add(new PlanCheckPoint(getNanoBot().Location, PlanCheckPoint.Actions.MoveRandom));
                    planIsFinished = false;
                    break;
                case Desires.BuildNeedle:
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Move));
                    planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.BuildNeedle));
                    planIsFinished = false;
                    break;
               
            }
        }

        public KeyValuePair<Desires, Point> deliberate(List<KeyValuePair<Desires, Point>> desires)
        {
            
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

            return new KeyValuePair<Desires, Point>(Desires.None, new Point());




        }


        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            List<Perception> perceptions = agent.getPerceptions(this.getNanoBot(), this.getAASMAFramework());
            
            foreach (Perception per in perceptions)
            {
                if (per.getType().Equals(PerceptionType.HoshimiPoint))
                {
                    HoshimiPointPerception p = (HoshimiPointPerception)per;
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.BuildNeedle, p.getPoint()));
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

