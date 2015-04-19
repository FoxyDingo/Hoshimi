using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

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
        private bool _moving = false;
        private bool _onNeedle = false;
     


        public override void DoActions()
        {
            //Starts to move if stopped and an azn point to visit waiting
            if (_moving == false && aznPointToVisit.Count > 0 && _onNeedle == false)
            {
                getAASMAFramework().logData(this, "new route");
                aznPoint = aznPointToVisit[0];
                aznPointToVisit.RemoveAt(0);
                this.MoveTo(aznPoint);
                _moving = true;
            }
            //Stops if reaches location of azn point
            if ( aznPoint != null  && Location.Equals(aznPoint) && _moving == true && _onNeedle == false)
            {
                getAASMAFramework().logData(this, "stop at azn point");
                this.StopMoving();
                _moving = false;     
            }

            //stock is the ammount of azn the collector already has. If full, there is no point in collecting more azn.
            //the overAZN method checks if the received nanobot is over an AZN point
            if (Stock < ContainerCapacity && this.getAASMAFramework().overAZN(this) && _moving == false && _onNeedle == false)
            {
                //Debbug
                getAASMAFramework().logData(this, "collecting azn...");
                this.collectAZN();
            }

            //if full and has needle to tranfer, goes there
            if (Stock == 50 && needlesPoints.Count > 0 && _onNeedle == false )
            {
                getAASMAFramework().logData(this, "I'm full!! Going to needle");
                this.MoveTo(needlesPoints[0]);
                _moving = true;
                _onNeedle = true;
            }

            //transfer azn to needle
            if ( _onNeedle == true && Location.Equals(needlesPoints[0]))
            {
                this.transferAZN();
                _moving = false;
            }

            //ends needle transfer and sets container to new azn collection
            if (_onNeedle == true && Stock == 0 && _moving == false)
            {
                needlesPoints.RemoveAt(0);
                _onNeedle = false;
            }
           
        }

        public override void receiveMessage(AASMAMessage msg)
        {

            try
            {
                if (msg.Content.Equals("I've found an AZN POINT"))
                {
                    getAASMAFramework().logData(this, "somebody found a azn point");
                    Point p = (Point)msg.Tag;
                    if (!aznPoint.Equals(p) && !aznPointToVisit.Contains(p))
                    {
                        aznPointToVisit.Add(p);
                    }
                }

                if (msg.Content.Equals("I'm in a AZN POINT"))
                {
                    getAASMAFramework().logData(this, "somebody is in a azn point");
                    Point p = (Point)msg.Tag;
                    if (aznPointToVisit.Contains(p))
                    {
                        aznPointToVisit.Remove(p);
                    }
                    if(aznPoint.Equals(p) && _moving == true)
                    {
                        _moving= false;
                        this.StopMoving();
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

                
            }catch (Exception e)
            {
              //TODO
              getAASMAFramework().logData(this.getAASMAFramework().NanoBots[0], "EXCEPTION IN AZN RECEIVE MESSAGE" + e.ToString());
            }

          

            }
        }  
}
