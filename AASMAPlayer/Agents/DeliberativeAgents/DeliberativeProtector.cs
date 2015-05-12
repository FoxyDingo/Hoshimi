using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.DeliberativeAgents
{

    //however, it is frequent that pierre's neurocontrollers kill the protector before he sees it
    //note that the shooting range is greater than the scan range
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 5, MaxDamage = 5, DefenseDistance = 12, Constitution = 28)]
    public class DeliberativeProtector : AASMAProtector
    {

        public DeliberativeProtector()
            : base()
        {
        }

        public override void DoActions() { }
        public override void receiveMessage(AASMAMessage msg) { }
    }
}