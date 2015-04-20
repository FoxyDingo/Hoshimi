using System;
using System.Collections.Generic;
using System.Text;


using System.Drawing;
using PH.Common;
using AASMAHoshimi;
using AASMAHoshimi.Examples;




namespace AASMAHoshimi.ReactiveAgents
{
    public class ReactiveAI : AASMAAI
    {
        
             
        public ReactiveAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }

        public override void DoActions()
        {
            List<Point> points = new List<Point>();
                       
            if (getAASMAFramework().explorersAlive() < 5 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building EXPLORER " + this._explorerNumber);
                this._nanoAI.Build(typeof(ReactiveExplorer), "E" + this._explorerNumber++);
            }

            if (getAASMAFramework().protectorsAlive() < 5 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building PROTECTOR " + this._protectorNumber);
                this._nanoAI.Build(typeof(ReactiveProtector), "P" + this._protectorNumber++);


            }

            //builds one nanobot of the type Container
            if (getAASMAFramework().containersAlive() < 10 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building CONTAINER " + this._containerNumber);
                this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);
               

            }


            //BUILD NEEDLE ON HOSHIMI POINT
            if (getNanoBot().State == NanoBotState.WaitingOrders && getAASMAFramework().overHoshimiPoint(this._nanoAI) && !getAASMAFramework().overNeedle(this.getNanoBot()))
            {
                getAASMAFramework().logData(this._nanoAI, "Building NEEDLE ");
                this._nanoAI.Build(typeof(ReactiveNeedle), "N" + this._needleNumber++);
            }

            //GO TO HOSHIMI POINT
            points = getAASMAFramework().visibleHoshimies(this.getNanoBot());
            if (points.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders)
            {
                Point p = Utils.getNearestPoint(this.getNanoBot().Location, points);
                //getAASMAFramework().logData(this._nanoAI, "I wanna move " );
                getNanoBot().MoveTo(p);
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
                      

        }
    }
}
