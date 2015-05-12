using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;


namespace AASMAHoshimi.DeliberativeAgents
{

    [Characteristics(ContainerCapacity = 50, CollectTransfertSpeed = 5, Scan = 0, MaxDamage = 0, DefenseDistance = 0, Constitution = 15)]
    class DeliberativeContainer : AASMAContainer
    {


        public DeliberativeContainer()
            : base()
        {
        }
        public override void DoActions() { }
        public override void receiveMessage(AASMAMessage msg) { }

    }
}
