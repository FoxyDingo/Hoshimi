﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.ISCommunicativeAgents
{
    //this protector does not move (u should write the code for it), however he will shoot any incoming pierre's that he sees
    //however, it is frequent that pierre's neurocontrollers kill the protector before he sees it
    //note that the shooting range is greater than the scan range
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 28)]
    public class ISCommunicativeProtector : AASMAProtector
    {
        public override void DoActions()
        {

            //Shoot at enemy
            List<Point> enemies = getAASMAFramework().visiblePierres(this);
            Point enemyPosition;
            if (enemies.Count > 0)
            {

                enemyPosition = enemies[0];
                int sqrDefenceDistance = this.DefenseDistance * this.DefenseDistance;
                int sqrDistanceToEnemy = Utils.SquareDistance(this.Location, enemyPosition);
                //we need to test if the enemy is within firing distance.
                if (sqrDistanceToEnemy <= sqrDefenceDistance)
                {
                    //the defendTo commands fires to the specified position for a number of specified turns. 1 is the recommended number of turns.

                    if (!this.DefendTo(enemyPosition, 1))
                    {
                        Point[] pointBetween = new Point[2];
                        pointBetween[0] = Location;
                        pointBetween[1] = enemyPosition;
                        MoveTo(Utils.getValidPoint(this.PlayerOwner.Tissue, Utils.getMiddlePoint(pointBetween)));
                    }
                    
                }
            }

            //MOVE RANDOMLY
            if (this.State == NanoBotState.WaitingOrders)
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
