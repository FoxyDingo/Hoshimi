using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;

namespace AASMAHoshimi.DeliberativeAgents
{
    [Characteristics(ContainerCapacity = 100, CollectTransfertSpeed = 0, Scan = 10, MaxDamage = 5, DefenseDistance = 10, Constitution = 25)]
    public class DeliberativeNeedle : AASMANeedle
    {
        public DeliberativeNeedle() : base() { }

        public override void DoActions(){}
        public override void receiveMessage(AASMAMessage msg) { }
        
    }
}
