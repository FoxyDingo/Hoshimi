using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.DeliberativeAgents
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

    public class DeliberativeAgent : Agent
    {
       
        public DeliberativeAgent()
            : base()
        { }

        public DeliberativeAgent(int[] interests)
            : base(interests)
        { }

        

        


    
    
   
    public class Plan
    {
     
    }

   
   
}
