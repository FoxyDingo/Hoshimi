using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.HybridAgents
{
    //public delegate void AgentAction(object body, PerceptMemory perceptMemory);

    public enum Desires
    {
        None,
        Unload,
        Collect,
        BuildContainer,
        BuildProtector,
        BuildExplorer,
        BuildNeedle,
        Explore,
        Defend,
        Attack
    }

    public class HybridAgent : Agent
    {
        

        public HybridAgent()
            : base()
        { }

        public HybridAgent(int[] interests)
            : base(interests)
        { }












    }
}
