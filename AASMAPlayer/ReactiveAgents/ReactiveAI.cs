using System;
using System.Collections.Generic;
using System.Text;
using AASMAHoshimi;

using System.Drawing;
using PH.Common;
using AASMAHoshimi.ReactiveAgents;
using AASMAHoshimi.Examples;



namespace AASMAHoshimi.ReactiveAgents
{
    public class ReactiveAI : AASMAAI
    {
        private List<Point> hoshimiesPoint = new List<Point>();
        private List<Point> hoshimiesNeedle = new List<Point>();
        private bool _distributeNavPoint = false;
        private Point _pointToDistribute;
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


            //builds one nanobot of the type Explorer
            //the explorersAlive method gives us the number of nano explorers that are still alive (i.e. not destroyed)
            if (getAASMAFramework().explorersAlive() < 5)
                {
                //we want to identify explorers with the names E1,E2,E3,...
                //that's why we use the explorerNumber++;
                //if u don't want to give a name to the nanobot use the following constructor instead
                //this._nanoAi.Build(typeof(ForwardExplorer));
                //however, if you do this, you won't be able to send him personal messages
                this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
                
            }

            //builds one nanobot of the type Container
            if (getAASMAFramework().containersAlive() < 3)
            {
                getAASMAFramework().logData(this._nanoAI, "Building container");
                this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);

            }

            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders && getNanoBot().Location.Equals(hoshimiesPoint[0]))
            {
                getAASMAFramework().logData(this._nanoAI, "Building needle ");
                this._nanoAI.Build(typeof(PassiveNeedle), "N" + this._needleNumber++);
                hoshimiesNeedle.Add(hoshimiesPoint[0]);
                hoshimiesPoint.RemoveAt(0);
                
            }

            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders)
            {
                
                Point p = hoshimiesPoint[0];
                //getAASMAFramework().logData(this._nanoAI, "I wanna move " );
                getNanoBot().MoveTo(p);


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

                //TODO REMOVE msg.Sender.Equals("AI") when the broadcast problem is resolved
                if (msg.Content.Equals("I've visited a NAVPOINT") && !msg.Sender.Equals("AI"))
                {
                    Point p = (Point) msg.Tag;
                    _distributeNavPoint = true;
                    _pointToDistribute = p;
                }

        }
    }
}
