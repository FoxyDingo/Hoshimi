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
       
        public override void DoActions()
        {
            List<Point> points;

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
            if (Stock == 0 && points.Count > 0)
            {
                getAASMAFramework().logData(this, "new route");
                Point nearest = Utils.getNearestPoint(this.Location, points);
                this.MoveTo(nearest);
            }
                                   
            //Go to needle
            points = getAASMAFramework().visibleEmptyNeedles(this);
            if (this.State == NanoBotState.WaitingOrders && Stock > 0 && points.Count > 0 )
            {
                getAASMAFramework().logData(this, "Going to needle");
                Point p = Utils.getNearestPoint(this.Location, points);
                this.MoveTo(p);
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
