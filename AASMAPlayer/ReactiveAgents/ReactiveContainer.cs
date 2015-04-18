using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.ReactiveAgents
{
    //this is an example of a stupid collector that does not move, however if he is under an AZN point he will try to collect
    //something
    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class ReactiveContainer : AASMAContainer
    {
        private List<Point> aznPointToVisit = new List<Point>();
        Point aznPoint = new Point();
        bool _moving = false;
     


        public override void DoActions()
        {
            if (_moving == false && aznPointToVisit.Count > 0)
            {
                getAASMAFramework().logData(this, "new route");
                aznPoint = aznPointToVisit[0];
                aznPointToVisit.RemoveAt(0);
                this.MoveTo(aznPoint);
                _moving = true;
            }
            //Stops if reaches location of azn point
            if ( aznPoint != null  && Location.Equals(aznPoint) && _moving == true)
            {
                getAASMAFramework().logData(this, "stop at azn point");
                this.StopMoving();
                _moving = false;     
            }

            //stock is the ammount of azn the collector already has. If full, there is no point in collecting more azn.
            //the overAZN method checks if the received nanobot is over an AZN point
            if (Stock < ContainerCapacity && this.getAASMAFramework().overAZN(this) && _moving == false)
            {
                //Debbug
                getAASMAFramework().logData(this, "collecting azn...");
                this.collectAZN();
            }
           
            //Receive needle position, go to position if is full
            /*if( Stock > 0 && this.getAASMAFramework().overEmptyNeedle(this))
            {
                this.transferAZN();
            }*/
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

                if (msg.Content.Equals("I'm in a AZN POINT"))
                {
                    getAASMAFramework().logData(this, "somebody is in a azn point");
                    Point p = (Point)msg.Tag;
                    if (aznPointToVisit.Contains(p))
                    {
                        aznPointToVisit.Remove(p);
                    }
                    if(aznPoint.Equals(p) && _moving == true )
                    {
                        _moving= false;
                        this.StopMoving();
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
