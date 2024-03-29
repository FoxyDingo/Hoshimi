﻿using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.HybridAgents
{
    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public class HybridNeedle : AASMANeedle
    {
        protected HybridAgent agent;
        private List<PlanCheckPoint> planCheckPoints = new List<PlanCheckPoint>();

        private bool planIsFinished = true;

        //TODO
        private bool planImpossible = false;

        PlanCheckPoint currentInstruction = null;

        public HybridNeedle()
            : base()
        {
            //I'm only interested in EnemyBot perceptions!!
            int[] interests = new int[1];
            interests[0] = (int)PerceptionType.EnemyBot;
            agent = new HybridAgent(interests);
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
            //Shoot PIERRES
            List<Point> points = getAASMAFramework().visiblePierres(this);
            if (points.Count > 0 && !hasReacted)
            {
               
                Point p = Utils.getNearestPoint(this.Location, points);
                if (canShoot(p))
                {
                    this.DefendTo(p, 1);
                    hasReacted = true;
                }


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
                case PlanCheckPoint.Actions.Attack:
                    if (this.State.Equals(NanoBotState.WaitingOrders))
                    {
                        this.DefendTo(currentInstruction.location, 1);
                    }
                    break;

            }
        }

        public void plan(KeyValuePair<Desires, Point> intention)
        {
            planCheckPoints.Clear();

            switch (intention.Key)
            {
                case Desires.Attack:
                    if (canShoot(intention.Value))
                    {
                        planCheckPoints.Add(new PlanCheckPoint(intention.Value, PlanCheckPoint.Actions.Attack));
                        planIsFinished = false;
                    }
                    break;
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

        public override void receiveMessage(AASMAMessage msg) { }

    }
}
