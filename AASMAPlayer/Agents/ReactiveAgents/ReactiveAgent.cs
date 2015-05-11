using System;
using System.Collections.Generic;

using PH.Common;
using PH.Map;
using System.Drawing;


using AASMAHoshimi;

namespace AASMAHoshimi.ReactiveAgents
{
    public class ReactiveAgent : Agent
    {
        protected List<KeyValuePair<RuleConditionDelegate, RuleActionDelegate>> rules = new List<KeyValuePair<RuleConditionDelegate, RuleActionDelegate>>();
        
       
        public ReactiveAgent()
            :base()
        {
            
        }

        public ReactiveAgent(int[] interests)
            : base(interests)
        {
            
        }

        public delegate bool RuleConditionDelegate(List<Perception> perceptions);
        public delegate void RuleActionDelegate(ReactiveAgent agent, List<Perception> perceptions);

        public void AddRule(RuleConditionDelegate condition, RuleActionDelegate action)
        {
            rules.Add(new KeyValuePair<RuleConditionDelegate, RuleActionDelegate>(condition, action));
        }

        public void React(List<Perception> perceptions)
        {
            foreach (var rule in rules)
            {
                var condtion = rule.Key;
                var action = rule.Value;
                if (condtion(perceptions))
                {
                    action(this, perceptions);
                    return;
                }
            }
        }
        
        

        
    }
}