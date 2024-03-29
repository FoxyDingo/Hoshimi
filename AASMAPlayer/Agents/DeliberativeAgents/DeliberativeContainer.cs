﻿using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.DeliberativeAgents
{

    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class DeliberativeContainer : AASMAContainer
    {
        protected DeliberativeAgent agent ;
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();
        private List<Point> AZNPoints = new List<Point>();
        private bool planIsFinished = true;

        //TODO
        private bool planImpossible = false;

        PlanCheckPoint currentInstruction = null;


        public DeliberativeContainer()
            : base()
        {
            //I'm only interested in AZN and EmptyNeedle and EnemyBot perceptions!!
            int[] interests = new int[3];
            interests[0] = (int)PerceptionType.AZNPoint;
            interests[1] = (int)PerceptionType.EmptyNeedle;
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
                
                if (planCheckPoints.Count == 0)
                {
                    planIsFinished = true;
                    return;
                }
                
                currentInstruction = planCheckPoints[0];
                getAASMAFramework().logData(this, "I have an instruction " + currentInstruction.action);
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
                case PlanCheckPoint.Actions.Collect:
                    getAASMAFramework().logData(this, "I'm gonna colect");
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        int turns = (ContainerCapacity - Stock)/5;
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
                if (desire.Key.Equals(Desires.Run))
                {
                    return new KeyValuePair<Desires, Point>(desire.Key, desire.Value);
                }
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
                List<Point> temp = new List<Point>(pointsCollect);
                temp.AddRange(AZNPoints);
                Point p = Utils.getNearestPoint(this.Location, temp);
                return new KeyValuePair<Desires, Point>(Desires.Collect, p);
            }
            
            return new KeyValuePair<Desires, Point>(Desires.None, new Point());
        }

        public List<KeyValuePair<Desires, Point>> getDesires()
        {
            List<KeyValuePair<Desires, Point>> desires = new List<KeyValuePair<Desires, Point>>();
            List<Perception> perceptions = agent.getPerceptions(this, this.getAASMAFramework());

            foreach (Perception per in perceptions)
            {
                //Collect AZN
                if (per.getType().Equals(PerceptionType.AZNPoint) && Stock < ContainerCapacity)
                {
                    AZNPointPerception p = (AZNPointPerception)per;
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Collect, p.getPoint()));
                    if (!AZNPoints.Contains(p.getPoint()))
                    {
                        AZNPoints.Add(p.getPoint());
                    }
                }
                //Unload AZN
                if (per.getType().Equals(PerceptionType.EmptyNeedle) && this.Stock > 0)
                {
                    EmptyNeedlePerception p = (EmptyNeedlePerception)per;
                    desires.Add(new KeyValuePair<Desires, Point>(Desires.Unload, p.getPoint()));
                }
                //RUN AWAY
                if (per.isType(PerceptionType.EnemyBot))
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
        public override void receiveMessage(AASMAMessage msg) { }

    }
}
