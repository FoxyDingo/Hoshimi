using System;
using System.Collections.Generic;
using System.Text;
using AASMAHoshimi;

using System.Drawing;
using PH.Common;



namespace AASMAHoshimi.Examples
{
    public class ReactiveAI : AASMAAI
    {
        private bool _hasObj = false;
        private Object _objPoint;

        public Point ObjPoint
        {
            get { return (Point)_objPoint; }
            set { _objPoint = value; }
        }

        public ReactiveAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }
        

        public override void DoActions()
        {
            //builds one nanobot of the type Explorer
            //the explorersAlive method gives us the number of nano explorers that are still alive (i.e. not destroyed)
            if (getAASMAFramework().explorersAlive() == 0)
                {
                //we want to identify explorers with the names E1,E2,E3,...
                //that's why we use the explorerNumber++;
                //if u don't want to give a name to the nanobot use the following constructor instead
                //this._nanoAi.Build(typeof(ForwardExplorer));
                //however, if you do this, you won't be able to send him personal messages
                this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
                
            }

            if (_hasObj && getNanoBot().State == NanoBotState.WaitingOrders)
            {
                
                Point p = ObjPoint;
                //getAASMAFramework().logData(this._nanoAI, "I wanna move " );
                getNanoBot().MoveTo(p);


            }

            if (_hasObj && getNanoBot().State == NanoBotState.WaitingOrders && getNanoBot().Location.Equals(ObjPoint))
            {
                getAASMAFramework().logData(this._nanoAI, "Building needle ");
                this._nanoAI.Build(typeof(PassiveNeedle), "N" + this._needleNumber++);
                _hasObj = false;
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
                _hasObj = true;
                ObjPoint = p;
                //getAASMAFramework().logData(this._nanoAI, "I have to go to this point " + p.X + " , " + p.Y);
            }
        }
    }
}
