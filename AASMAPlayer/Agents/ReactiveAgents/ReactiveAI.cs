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
        private List<Point> hoshimiesPoint = new List<Point>();
        private List<Point> hoshimiesNeedle = new List<Point>();
             
        public ReactiveAI(NanoAI nanoAI)
            : base(nanoAI)
        {
        }

        public override void DoActions()
        {
                       
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
            if (getAASMAFramework().containersAlive() < 3 && this._nanoAI.State == NanoBotState.WaitingOrders)
            {
                getAASMAFramework().logData(this._nanoAI, "Building CONTAINER " + this._containerNumber);
                this._nanoAI.Build(typeof(ReactiveContainer), "C" + this._containerNumber++);
               

            }


            //BUILD NEEDLE ON HOSHIMI POINT
            if (hoshimiesPoint.Count > 0 && getNanoBot().State == NanoBotState.WaitingOrders && getNanoBot().Location.Equals(hoshimiesPoint[0]))
            {
                getAASMAFramework().logData(this._nanoAI, "Building NEEDLE ");
                this._nanoAI.Build(typeof(ReactiveNeedle), "N" + this._needleNumber++);
                hoshimiesNeedle.Add(hoshimiesPoint[0]);
                                
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
                      

        }
    }
}
