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
        private List<Point> aznPoints = new List<Point>();
        private List<Point> needlePoints = new List<Point>();
        

        public override void DoActions()
        {
            List<Point> points;
            //DELETE FULL NEEDLE FROM LIST
            points = getAASMAFramework().visibleFullNeedles(this);
            if (points.Count > 0)
            {
                foreach (Point p in points)
                {
                    if (needlePoints.Contains(p))
                    {
                        needlePoints.Remove(p);
                    }
                }
            }

            //Collect AZN
            if (this.State == NanoBotState.WaitingOrders && getAASMAFramework().overAZN(this) && Stock < ContainerCapacity)
            {
                getAASMAFramework().logData(this, "collecting azn");
                this.collectAZN();
            }

            //transfer azn to needle
            if (this.State == NanoBotState.WaitingOrders && getAASMAFramework().overEmptyNeedle(this) && Stock > 0)
            {
                getAASMAFramework().logData(this, "transfer azn to needle");
                this.transferAZN();
                
            }

            //GO TO azn POINT
            points = getAASMAFramework().visibleAznPoints(this);
            if (Stock == 0 && (points.Count > 0 || aznPoints.Count > 0))
            {
                getAASMAFramework().logData(this, "new route");
                List<Point> temp = new List<Point>(points);
                temp.AddRange(aznPoints);
                Point nearest = Utils.getNearestPoint(this.Location, temp);
                this.MoveTo(nearest);
            }

            //Go to needle
            points = getAASMAFramework().visibleEmptyNeedles(this);
            if (this.State == NanoBotState.WaitingOrders && Stock > 0 && (points.Count > 0 || needlePoints.Count > 0))
            {
                getAASMAFramework().logData(this, "Going to needle");
                //We dont know if the needlePoints are still Empty so we prefer to go to the points location
                if (points.Count > 0)
                {
                    Point p = Utils.getNearestPoint(this.Location, points);
                    this.MoveTo(p);

                }
                else if (needlePoints.Count > 0)
                {
                    Point p = Utils.getNearestPoint(this.Location, needlePoints);
                    this.MoveTo(p);
                }

            }

            //SEE azn POINT   
            points = getAASMAFramework().visibleAznPoints(this);
            if (points.Count > 0)
            {
                foreach (Point p in points)
                {
                    if (!aznPoints.Contains(p)) { aznPoints.Add(p); }
                }

            }

            //SEE EMPTY NEEDLE  
            points = getAASMAFramework().visibleEmptyNeedles(this);
            if (points.Count > 0)
            {
                foreach (Point p in points)
                {
                    if (!needlePoints.Contains(p)) { needlePoints.Add(p); }
                }

            }

            //MOVE RANDOMLY
            if (this.State == NanoBotState.WaitingOrders)
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
        }

        public override void receiveMessage(AASMAMessage msg)
        {

            try
            {
                if (msg.Content.Equals("I've found an AZN POINT"))
                {
                    getAASMAFramework().logData(this, "somebody found an azn point");
                    Point p = (Point)msg.Tag;
                    if (!aznPoints.Equals(p))
                    {
                        aznPoints.Add(p);
                    }
                }

                if (msg.Content.Equals("Needle Position"))
                {
                    getAASMAFramework().logData(this, "needle position received from IA");
                    Point p = (Point)msg.Tag;
                    if (!needlePoints.Contains(p))
                    {
                        needlePoints.Add(p);
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
