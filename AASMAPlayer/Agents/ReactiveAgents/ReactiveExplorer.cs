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
       
        private List<Point> navPointVisited = new List<Point>();
      
       
        //this is only used to stop the explorer from going to a previously explored navPoint
        private List<Point> _movingToNavPoint = new List<Point>();
        
       


        public override void DoActions()
        {
            List<Point> points;
                                           
            // GO TO NAV POINT
            points = getAASMAFramework().visibleNavigationPoints(this);
            if (points.Count > 0 )
            {

                foreach (Point p in points)
                {
                    if (_movingToNavPoint.Count == 0 && !navPointVisited.Contains(p))
                    {
                        
                        this._movingToNavPoint.Add(p);
                        this.MoveTo(p);
                        break;
                    }


                }

            }

            //MOVE RANDOMLY
            if( this.State == NanoBotState.WaitingOrders)
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
