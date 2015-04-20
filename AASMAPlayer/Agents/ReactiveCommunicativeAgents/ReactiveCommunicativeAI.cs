using System;
using System.Collections.Generic;
using System.Text;


using System.Drawing;
using PH.Common;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using AASMAHoshimi.ReactiveCommunicativeAgents;



namespace AASMAHoshimi.ReactiveCommunicativeAgents
{
    public class ReactiveCommunicativeAI : AASMAAI
    {
        private List<Point> hoshimiesPoint = new List<Point>();
        private List<Point> hoshimiesNeedle = new List<Point>();
        private bool _distributeNavPoint = false;
        private Point _pointToDistribute;
        private bool _distributeNavPointMov = false;
        private Point _pointToDistributeMov;
        private string _pointToDistributeMovName;
        private bool _containersCreated = false;
        //private bool _hasObj = false;
        //private Object _objPoint;

        /*public Point ObjPoint
        {
            get { return (Point)_objPoint; }
            set { _objPoint = value; }
        }*/

        public ReactiveCommunicativeAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }

        //Sends msg to all nanobots of type s (E, C, P...)
       public void sendToAll(AASMAMessage msg, string s)
        {
            foreach(NanoBot n in getAASMAFramework().NanoBots)
            {
                if (n.InternalName.StartsWith(s))
                {
                    getAASMAFramework().sendMessage(msg, n.InternalName);
                    getAASMAFramework().logData(this._nanoAI, "sending msg to " + msg.Receiver + " : " + msg.Content);
                }
            }
            
        }
        
        public override void DoActions()
        {
            if (_containersCreated)
            {
                AASMAMessage msg = new AASMAMessage("AI", "I've created all containers");
                sendToAll(msg, "E");
                _containersCreated = false;
            }
            
            if (_distributeNavPoint)
            {
                    AASMAMessage msg = new AASMAMessage("AI", "I've visited a NAVPOINT");
                    msg.Tag = _pointToDistribute;

                    sendToAll(msg, "E");
                     _distributeNavPoint = false;

                }
               
            

            if (_distributeNavPointMov)
            {
                
                    AASMAMessage msg = new AASMAMessage(_pointToDistributeMovName, "I've found a NAVPOINT! moving...");
                    msg.Tag = _pointToDistributeMov;

                    sendToAll(msg, "E");
                    _distributeNavPointMov = false;
            }


            
            if (getAASMAFramework().explorersAlive() < 5 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building EXPLORER " + this._explorerNumber);
                this._nanoAI.Build(typeof(ReactiveCommunicativeExplorer), "E" + this._explorerNumber++);
            }

            if (getAASMAFramework().protectorsAlive() < 5 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building PROTECTOR " + this._protectorNumber);
                this._nanoAI.Build(typeof(ReactiveCommunicativeProtector), "P" + this._protectorNumber++);
                
                
            }


            

            //builds one nanobot of the type Container
            if (getAASMAFramework().containersAlive() < 3 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building CONTAINER " + this._containerNumber);
                this._nanoAI.Build(typeof(ReactiveCommunicativeContainer), "C" + this._containerNumber++);
                if (getAASMAFramework().containersAlive() == 2) { _containersCreated = true; }

            }


            //BUILD NEEDLE ON HOSHIMI POINT
            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders && getNanoBot().Location.Equals(hoshimiesPoint[0]))
            {
                getAASMAFramework().logData(this._nanoAI, "Building NEEDLE ");
                this._nanoAI.Build(typeof(ReactiveCommunicativeNeedle), "N" + this._needleNumber++);
                hoshimiesNeedle.Add(hoshimiesPoint[0]);

                //Sends message to container with needle position
                for (int i = 0; i < this.getAASMAFramework().NanoBots.Count; i++)
                {
                    if (this.getAASMAFramework().NanoBots[i].InternalName.StartsWith("C"))
                    {
                        AASMAMessage msg = new AASMAMessage("AI", "Needle Position");
                        msg.Tag = hoshimiesPoint[0];
                        getAASMAFramework().sendMessage(msg, this.getAASMAFramework().NanoBots[i].InternalName);
                    }
                }       
                hoshimiesPoint.RemoveAt(0);
            }

            //GO TO HOSHIMI POINT
            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders)
            {
                foreach (NanoBot n in getAASMAFramework().NanoBots)
                {
                    getAASMAFramework().logData(this._nanoAI, "Name: " + n.InternalName);
                }
                
                Point p = hoshimiesPoint[0];
                //getAASMAFramework().logData(this._nanoAI, "I wanna move " );
                getNanoBot().MoveTo(p);
            }

            //IF HE SEES AN HOSHIMI POINT ADD IT TO THE LIST
            List<Point> points = getAASMAFramework().visibleHoshimies(getNanoBot());
            if (points.Count > 0)
            {
                if (!hoshimiesNeedle.Contains(points[0]) && !hoshimiesPoint.Contains(points[0]))
                {
                    hoshimiesPoint.Add(points[0]);
                }
            }

            //MOVE RANDOMLY
            if (getNanoBot().State == NanoBotState.WaitingOrders)
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
                //this AI also handles messages, writing them in the debug log file
                //the logData method is very usefull for debbuging purposes
                //it will write the turn number and name of the agent who wrote in the log
                getAASMAFramework().logData(this._nanoAI, "received message from " + msg.Sender + " : " + msg.Content);

                if (msg.Content.Equals("I've found an hoshimi point! Go there man!"))
                {
                    Point p = (Point)msg.Tag;
                    // _hasObj = true;
                    //ObjPoint = p;
                    if (!hoshimiesNeedle.Contains(p) && !hoshimiesPoint.Contains(p))
                    {
                        hoshimiesPoint.Add(p);
                    }


                    //getAASMAFramework().logData(this._nanoAI, "I have to go to this point " + p.X + " , " + p.Y);
                }

                
                if (msg.Content.Equals("I've visited a NAVPOINT"))
                {
                    Point p = (Point) msg.Tag;
                    _distributeNavPoint = true;
                    _pointToDistribute = p;
                }

                if (msg.Content.Equals("I've found a NAVPOINT! moving...") )
                {
                    Point p = (Point)msg.Tag;
                    _distributeNavPointMov = true;
                    _pointToDistributeMov = p;
                    _pointToDistributeMovName = msg.Sender;
                }

        }
    }
}
