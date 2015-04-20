using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.ISCommunicativeAgents
{
    //this is an explorer that does not move much (not much of a good explorer) but is a mouthfull
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class ISCommunicativeExplorer : AASMAExplorer
    {
        private List<Point> hoshimiesBroadcasted = new List<Point>();
        private List<Point> navPointVisited = new List<Point>();
        private List<Point> navPointBroadcasted = new List<Point>();
        private List<Point> aznPointBroadcasted = new List<Point>();
        private List<Point> aznPointsTemp = new List<Point>();
        //this is only used to stop the explorer from going to a previously explored navPoint
        private List<Point> _movingToNavPoint = new List<Point>();
        private bool _hasHoshimies = false;
        //Value from 0 to 20; The higher the value the more likely the agent is to not following the other agents
        private int percentageDefect = 15;
        private bool _containersCreated = false;


        public override void DoActions()
        {
            List<Point> points;
            


           //Tells containers the azn positions found 
            points = this.getAASMAFramework().visibleAznPoints(this);
            if (points.Count > 0 && _containersCreated)
            {
                foreach (Point p in points)
                {
                    if (!aznPointBroadcasted.Contains(p))
                    {
                        getAASMAFramework().logData(this, "AZN POINT FOUND");
                        aznPointBroadcasted.Add(p);
                        for (int i = 0; i < this.getAASMAFramework().NanoBots.Count; i++)
                        {
                            //getAASMAFramework().logData(this, "NANO NAME: " + this.getAASMAFramework().NanoBots[i].InternalName);
                            if (this.getAASMAFramework().NanoBots[i].InternalName.StartsWith("C"))
                            {
                                AASMAMessage msg = new AASMAMessage(this.InternalName, "I've found an AZN POINT");
                                msg.Tag = p;
                                getAASMAFramework().sendMessage(msg, this.getAASMAFramework().NanoBots[i].InternalName);
                            }
                        }
                        //We only want to send one point at a time
                        break;

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

                foreach (Point p in points)
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
                else
                {

                    AASMAMessage msg = new AASMAMessage(this.InternalName, "I still have hoshimies to broadcast");
                    getAASMAFramework().sendMessage(msg, "AI");
                }
                /// ****** ////




            }

            // GO TO NAV POINT
            points = getAASMAFramework().visibleNavigationPoints(this);
            if (points.Count > 0 && !_hasHoshimies && this.State == NanoBotState.WaitingOrders)
            {

                foreach (Point p in points)
                {
                    if (_movingToNavPoint.Count == 0 && !navPointVisited.Contains(p))
                    {
                        AASMAMessage msg = new AASMAMessage(this.InternalName, "I've found a NAVPOINT! moving...");

                        msg.Tag = p;

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
            if (msg.Content.Equals("I've visited a NAVPOINT"))
            {
                getAASMAFramework().logData(this, "somebody visited a navpoint");
                Point p = (Point)msg.Tag;
                navPointVisited.Add(p);
                if (_movingToNavPoint.Contains(p))
                {
                    this.StopMoving();
                    _movingToNavPoint.RemoveAt(0);

                }
            }

            if (msg.Content.Equals("I've found a NAVPOINT! moving...") && !msg.Sender.Equals(this.InternalName))
            {
                getAASMAFramework().logData(this, "somebody is moving to a navpoint");
                Point p = (Point)msg.Tag;
                if (!_movingToNavPoint.Contains(p)) 
                { 
                    int rnd = Utils.randomValue(20);
                    if (rnd < percentageDefect)
                    {
                        getAASMAFramework().logData(this, "I wont move here, too many people interested");
                        navPointVisited.Add(p);
                    }
                }
           }

            if (msg.Content.Equals("I've created all containers"))
            {
                _containersCreated = true;
            }

        }
    }
}
