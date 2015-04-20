using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.ReactiveAgents
{
    
    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class ReactiveContainer : AASMAContainer
    {
        //private List<Point> aznPointToVisit = new List<Point>();
        private List<Point> needlePoints = new List<Point>();
        private List<Point> aznPoints = new List<Point>();
        private bool _moving = false;
        private bool _onNeedle = false;



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
                    if(!aznPoints.Contains(p)) {aznPoints.Add(p);}
                }
                
            }

            //SEE EMPTY NEEDLE  
            points = getAASMAFramework().visibleEmptyNeedles(this);
            if (points.Count > 0)
            {
                foreach (Point p in points)
                {
                    if(!needlePoints.Contains(p)){needlePoints.Add(p);}
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


        }
    }
}
