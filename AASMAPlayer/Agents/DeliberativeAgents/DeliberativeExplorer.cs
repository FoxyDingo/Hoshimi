using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using PH.Common;

namespace AASMAHoshimi.DeliberativeAgents
{
    
    [Characteristics(ContainerCapacity = 0, CollectTransfertSpeed = 0, Scan = 30, MaxDamage = 0, DefenseDistance = 0, Constitution = 10)]
    public class DeliberativeExplorer : AASMAExplorer
    {
        

        public DeliberativeExplorer()
            : base()
        {
        }
        public override void DoActions() { }
        public override void receiveMessage(AASMAMessage msg) { }
    }
}