using System;
using System.Collections.Generic;
using System.Text;

namespace AASMAHoshimi.DeliberativeAgents
{
    public delegate void AgentAction(object body, PerceptMemory perceptMemory);

    public class DeliberativeAgent : Agent
    {
        private Plan plan;
        public DeliberativeAgent()
            : base()
        { }

        public DeliberativeAgent(int[] interests)
            : base(interests)
        { }


    }

    public class Plan
    {
        //This is the list of plan steps
        protected List<KeyValuePair<Condition, AgentAction>> actions;

        public Plan()
        {
            //The obrigatory variable initialization, so that we neve get a null pointer exception, for this
            this.actions = new List<KeyValuePair<Condition, AgentAction>>();
        }

        public bool Empty { get { return actions.Count == 0; } }

        public void AddStep(Condition preCondition, AgentAction action)
        {
            actions.Add(new KeyValuePair<Condition, AgentAction>(preCondition, action));
        }

        public AgentAction NextAction()
        {
            AgentAction action = actions[0].Value;
            actions.RemoveAt(0);
            return action;
        }
    }

    public class PerceptMemory
    {
    }
    #region Delegates

    /// <summary>
    /// This delegate is used to see if a condition holds against the current set
    /// of believes
    /// </summary>
    /// <param name="percepts">The current believes</param>
    /// <returns>True if the condition holds, false otherwise</returns>
    public delegate bool Condition(PerceptMemory percepts);

    /// <summary>
    /// This delegate is used to get a object in order to be used as a concrete entity for the corresponding
    /// desire template. If there isn't any entity that can be used it is returned null.
    /// </summary>
    /// <param name="percepts">The current believes</param>
    /// <returns>The object that can be used, or null if the desire template can't be applied</returns>
    public delegate Object PossibleEntity(PerceptMemory percepts);

    /// <summary>
    /// This is the template for a desire, and returns a concrete desire, for the object given in the input.
    /// </summary>
    /// <param name="percepts">The current believes.</param>
    /// <param name="targetEntity">The concrete object that is used to build a desire.</param>
    /// <returns>The desire that uses the object given as input.</returns>
    //public delegate Desire DesireTemplate(PerceptMemory percepts, Object targetEntity);

    /// <summary>
    /// This is a plan template, that returns a concrete plan given the current believes.
    /// </summary>
    /// <param name="percepts">The current believes.</param>
    /// <returns>The plan to use.</returns>
    public delegate Plan PlanTemplate(PerceptMemory percepts);

    #endregion
}
