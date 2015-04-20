using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.ReactiveAgents
{
    //this is an explorer that does not move much (not much of a good explorer) but is a mouthfull
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class ReactiveExplorer : AASMAExplorer
    {
       
        public override void DoActions()
        {
            List<Point> points;
            
            //Move away from nav point
            points = getAASMAFramework().visibleNavigationPoints(this);
            if(points.Count > 0 && this.Location.Equals(Utils.getNearestPoint(this.Location, points)) &&  this.State == NanoBotState.WaitingOrders)
            {
                Point obj = this.Location;
                getAASMAFramework().logData(this, "FINDING A NEW POINT");
                int rnd = Utils.randomValue(5);
                if(rnd == 0){
                    Point p = this.Location;
                    p.X = p.X + this.Scan + 10 + (this.Scan / 2);
                    obj = Utils.getValidPoint(getAASMAFramework().Tissue, p);
                }
                if(rnd == 1){
                    Point p = this.Location;
                    p.X = p.X - this.Scan + 10 + (this.Scan / 2);
                    obj = Utils.getValidPoint(getAASMAFramework().Tissue, p);
                }
                if(rnd == 2) {
                    Point p = this.Location;
                    p.Y = p.Y - this.Scan + 10 + (this.Scan / 2);
                    obj = Utils.getValidPoint(getAASMAFramework().Tissue, p);
                }
                if(rnd == 3){
                    Point p = this.Location;
                    p.Y = p.Y + this.Scan + 10 + (this.Scan / 2);
                    obj = Utils.getValidPoint(getAASMAFramework().Tissue, p);
                }
                if (rnd == ~4)
                {
                    Point p = this.Location;
                    p.Y = p.Y + this.Scan + 10 + (this.Scan / 2);
                    p.X = p.X + this.Scan + 10 + (this.Scan / 2);
                    obj = Utils.getValidPoint(getAASMAFramework().Tissue, p);
                }
                if (rnd == 5)
                {
                    Point p = this.Location;
                    p.Y = p.Y - this.Scan + 10 + (this.Scan / 2);
                    p.X = p.X - this.Scan + 10 + (this.Scan / 2);
                    obj = Utils.getValidPoint(getAASMAFramework().Tissue, p);
                }
                this.MoveTo(obj);
            }
            
       
            // GO TO NAV POINT
            points = getAASMAFramework().visibleNavigationPoints(this);
            if (points.Count > 0 && this.State == NanoBotState.WaitingOrders)
            {
                this.MoveTo(Utils.getNearestPoint(this.Location, points));
            }

            //MOVE RANDOMLY
            if( this.State == NanoBotState.WaitingOrders)
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
