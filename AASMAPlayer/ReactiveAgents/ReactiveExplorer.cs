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
        private List<Point> hoshimiesBroadcasted = new List<Point>();
        private List<Point> navPointVisited = new List<Point>();
        private List<Point> navPointBroadcasted = new List<Point>();
        //this is only used to stop the explorer from going to a previously explored navPoint
        private List<Point> _movingToNavPoint = new List<Point>();
        private bool _hasHoshimies = false;


        public override void DoActions()
        {
            List<Point> points;

         
           //Tells containers the azn positions found
            if (this.getAASMAFramework().overAZN(this))
            {
                getAASMAFramework().logData(this, "AZN POINT FOUND");
                for (int i = 0; i < this.getAASMAFramework().NanoBots.Count; i++)
                {
                    //getAASMAFramework().logData(this, "NANO NAME: " + this.getAASMAFramework().NanoBots[i].InternalName);
                    if( this.getAASMAFramework().NanoBots[i].InternalName.StartsWith("C"))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "I've visited a AZN POINT");
                        msg.Tag = Location;
                        getAASMAFramework().sendMessage(msg,this.getAASMAFramework().NanoBots[i].InternalName);
                    }
                }
            }
                       
           
            
            //TELL AI YOU VISITED A NAVPOINT
            if (_movingToNavPoint.Count > 0 && Location.Equals(_movingToNavPoint[0]))
            {
                AASMAMessage msg = new AASMAMessage(this.InternalName, "I've visited a NAVPOINT");

                msg.Tag = Location;

                getAASMAFramework().sendMessage(msg, "AI");
                //We are gonna receive our own message. We update the navPointVisited when we receive that message
                
            }

            points = getAASMAFramework().visibleHoshimies(this);
            // TELL AI ABOUT HOSHIMI POINT
            if (points.Count > 0)
            {

                //We reset the variable because we only want the explorer to move if all hoshimi points have been broadcasted
                _hasHoshimies = false;

                foreach(Point p in points)
                {
                    if (!hoshimiesBroadcasted.Contains(p))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "I've found an hoshimi point! Go there man!");

                        msg.Tag = p;

                        _hasHoshimies = true;

                        getAASMAFramework().sendMessage(msg, "AI");
                        hoshimiesBroadcasted.Add(p);
                        break;
                    }

                   
                }

                // **** DEBUG ONLY ****
                if (!_hasHoshimies)
                {
                    AASMAMessage msg = new AASMAMessage(this.InternalName, "I DONT HAVE MORE HOSHIMIES");
                    getAASMAFramework().sendMessage(msg, "AI");
                }
                else {

                    AASMAMessage msg = new AASMAMessage(this.InternalName, "I still have hoshimies to broadcast");
                    getAASMAFramework().sendMessage(msg, "AI");
                }
                /// ****** ////
               

                

            }
            
            // GO TO NAV POINT
            points = getAASMAFramework().visibleNavigationPoints(this);
            if (points.Count > 0 && !_hasHoshimies)
            {

                foreach (Point p in points)
                {
                    if (_movingToNavPoint.Count == 0 && !navPointVisited.Contains(p))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "I've found a NAVPOINT! moving...");

                        //msg.Tag = p;

                        getAASMAFramework().sendMessage(msg, "AI");
                       // navPointBroadcasted.Add(p);
                        this._movingToNavPoint.Add(p);
                        this.MoveTo(p);
                        break;
                    }


                }
           
            }

            //MOVE RANDOMLY
            if (!_hasHoshimies && this.State == NanoBotState.WaitingOrders)
            {
                RandomTurn();
                MoveForward();
            }
                
        }

        public override void receiveMessage(AASMAMessage msg)
        {
            if (msg.Content.Equals("I've visited a NAVPOINT"))
            {
                getAASMAFramework().logData(this, "somebody visited a navpoint");
                Point p = (Point)msg.Tag;
                navPointVisited.Add(p);
                try
                {
                    if (_movingToNavPoint.Contains(p))
                    {
                        this.StopMoving();
                        _movingToNavPoint.RemoveAt(0);

                    }
                }
                catch (Exception e)
                {
                    //TODO
                    getAASMAFramework().logData(this.getAASMAFramework().NanoBots[0], "EXCEPTIIION " + e.ToString());
                }
            }


        }

    }
}
