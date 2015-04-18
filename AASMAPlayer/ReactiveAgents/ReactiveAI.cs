using System;
using System.Collections.Generic;
using System.Text;


using System.Drawing;
using PH.Common;
using AASMAHoshimi;
using AASMAHoshimi.Examples;
using AASMAHoshimi.ReactiveAgents;



namespace AASMAHoshimi.Examples
{
    public class ReactiveAI : AASMAAI
    {
        private List<Point> hoshimiesPoint = new List<Point>();
        private List<Point> hoshimiesNeedle = new List<Point>();
        private bool _distributeNavPoint = false;
        private Point _pointToDistribute;
        private bool _distributeNavPointMov = false;
        private Point _pointToDistributeMov;
        private string _pointToDistributeMovName;
        //private bool _hasObj = false;
        //private Object _objPoint;

        /*public Point ObjPoint
        {
            get { return (Point)_objPoint; }
            set { _objPoint = value; }
        }*/

        public ReactiveAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }
        
        public override void DoActions()
        {

            if (_distributeNavPoint)
            {
                for (int i = 1; i < this._explorerNumber; i++)
                {
                    AASMAMessage msg2 = new AASMAMessage("AI", "I've visited a NAVPOINT");
                    msg2.Tag = _pointToDistribute;

                    string e = "E" + i;


                    getAASMAFramework().sendMessage(msg2, e);
                    //TODO DONT KNOW WHY BUT THIS ONLY WORKS WITH BROADCAST
                    //getAASMAFramework().broadCastMessage(msg2);
                    getAASMAFramework().logData(this._nanoAI, "sending msg to " + msg2.Receiver + " : " + msg2.Content);

                }
                _distributeNavPoint = false;
            }

            if (_distributeNavPointMov)
            {
                for (int i = 1; i < this._explorerNumber; i++)
                {
                    AASMAMessage msg2 = new AASMAMessage(_pointToDistributeMovName, "I've found a NAVPOINT! moving...");
                    msg2.Tag = _pointToDistributeMov;

                    string e = "E" + i;


                    getAASMAFramework().sendMessage(msg2, e);
                    
                    getAASMAFramework().logData(this._nanoAI, "sending msg to " + msg2.Receiver + " : " + msg2.Content);

                }
                _distributeNavPointMov = false;
            }


            
            if (getAASMAFramework().explorersAlive() < 5)
            {
                this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
            }
            
            if (getAASMAFramework().protectorsAlive() < 5)
            {
                this._nanoAI.Build(typeof(ReactiveProtector), "P" + this._protectorNumber++);
            }


            

            //builds one nanobot of the type Container
            if (getAASMAFramework().containersAlive() < 3)
            {
                getAASMAFramework().logData(this._nanoAI, "Building container");
                this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);

            }


            //BUILD NEEDLE ON HOSHIMI POINT
            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders && getNanoBot().Location.Equals(hoshimiesPoint[0]))
            {
                getAASMAFramework().logData(this._nanoAI, "Building needle ");
                this._nanoAI.Build(typeof(ReactiveNeedle), "N" + this._needleNumber++);
                hoshimiesNeedle.Add(hoshimiesPoint[0]);
                hoshimiesPoint.RemoveAt(0);
                
            }

            //GO TO HOSHIMI POINT
            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders)
            {
                
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
