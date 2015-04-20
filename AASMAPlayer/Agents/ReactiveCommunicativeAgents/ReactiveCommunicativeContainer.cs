using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;
using AASMAHoshimi;

namespace AASMAHoshimi.ReactiveCommunicativeAgents
{
    //this is an example of a stupid collector that does not move, however if he is under an AZN point he will try to collect
    //something
    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class ReactiveCommunicativeContainer : AASMAContainer
    {
        private List<Point> aznPointToVisit = new List<Point>();
        private List<Point> needlesPoints = new List<Point>();
        private Point aznPoint = new Point();

        public override void DoActions()
        {
            List<Point> aznPoints;
            List<Point> emptyNeedlePoints;
            List<Point> fullNeedlePoints;

            //stock is the ammount of azn the collector already has. If full, there is no point in collecting more azn.
            //the overAZN method checks if the received nanobot is over an AZN point
            if (this.State == NanoBotState.WaitingOrders && Stock < ContainerCapacity && this.getAASMAFramework().overAZN(this))
            {
                getAASMAFramework().logData(this, "collecting azn...");
                this.collectAZN();
            }

            //transfer azn to needle
            if (this.State == NanoBotState.WaitingOrders && getAASMAFramework().overEmptyNeedle(this) && Stock > 0)
            {
                getAASMAFramework().logData(this, "transfer azn to needle");
                if (Location.Equals(needlesPoints[0]))
                {
                    this.transferAZN();
                }
            }

            //goes to an azn point to visit waiting
            if (Stock == 0 && aznPointToVisit.Count > 0)
            {
                getAASMAFramework().logData(this, "new route");
                aznPoint = aznPointToVisit[0];
                //aznPointToVisit.RemoveAt(0);
                this.MoveTo(aznPoint);
            }

            //has needle to tranfer, goes there
            if (Stock == ContainerCapacity && this.State == NanoBotState.WaitingOrders && Stock > 0 && aznPointToVisit.Count > 0)
            {
                if (needlesPoints.Count > 0)
                {
                    getAASMAFramework().logData(this, "I'm full!! Going to needle");
                    this.MoveTo(needlesPoints[0]);
                }
            }

            if (this.State == NanoBotState.WaitingOrders && Stock > 0 && aznPointToVisit.Count > 0)
            {
                aznPoints = this.getAASMAFramework().visibleAznPoints(this);
                emptyNeedlePoints = this.getAASMAFramework().visibleEmptyNeedles(this);
                fullNeedlePoints = this.getAASMAFramework().visibleFullNeedles(this);

                foreach (Point p in aznPoints)
                {
                    if (!aznPointToVisit.Contains(p))
                    {
                        aznPointToVisit.Add(p);
                    }
                }

                foreach (Point empty in emptyNeedlePoints)
                {
                    if (!needlesPoints.Contains(empty))
                    {
                        needlesPoints.Add(empty);
                    }
                }

                foreach (Point full in fullNeedlePoints)
                {
                    if (needlesPoints.Contains(full))
                    {
                        needlesPoints.Remove(full);
                    }
                }
            }

            if (this.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this, "moving randomly");
                if (frontClear())
                {
                    if (AASMAHoshimi.Utils.randomValue(100) < 90)
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
        }

        public override void receiveMessage(AASMAMessage msg)
        {

            try
            {
                if (msg.Content.Equals("I've visited a AZN POINT"))
                {
                    getAASMAFramework().logData(this, "somebody visited a azn point");
                    Point p = (Point)msg.Tag;
                    if (!aznPoint.Equals(p) && !aznPointToVisit.Contains(p))
                    {
                        aznPointToVisit.Add(p);
                    }
                }

                if (msg.Content.Equals("Needle Position"))
                {
                    getAASMAFramework().logData(this, "needle position received from IA");
                    Point p = (Point)msg.Tag;
                    if (!needlesPoints.Contains(p))
                    {
                        needlesPoints.Add(p);
                    }
                }


            }
            catch (Exception e)
            {
                //TODO
                getAASMAFramework().logData(this.getAASMAFramework().NanoBots[0], "EXCEPTION IN AZN RECEIVE MESSAGE" + e.ToString());
            }



        }
    }
}
