using System;
using System.Collections.Generic;
using System.Text;
using PH.Common;
using System.Drawing;

namespace AASMAHoshimi.DeliberativeAgents
{
    //public delegate void AgentAction(object body, PerceptMemory perceptMemory);
    

    public class DeliberativeAgent : Agent
    {
        private Plan plan;
        protected List<DesiresDelegate> desiresDelegates = new List<DesiresDelegate>();
        public delegate List<KeyValuePair<int, Point>> DesiresDelegate(DeliberativeAgent agent, List<Perception> beliefs);
        protected List<IntentionsDelegate> intentionsDelegates = new List<IntentionsDelegate>();
        public delegate List<KeyValuePair<int, Point>> IntentionsDelegate(DeliberativeAgent agent, List<KeyValuePair<int,Point>> desires);
        protected List<ActionsDelegate> actionsDelegates = new List<ActionsDelegate>();
        public delegate ActionsDelegate ActionsDelegate(DeliberativeAgent agent, List<KeyValuePair<int, Point>> intentions);

        public DeliberativeAgent()
            : base()
        { }

        public DeliberativeAgent(int[] interests)
            : base(interests)
        { }

        public void AddDesire(DesiresDelegate desire)
        {
            desiresDelegates.Add(desire);
        }

        public void AddIntention(IntentionsDelegate intention)
        {
            intentionsDelegates.Add(intention);
        }

        public void Deliberate(List<Perception> beliefs, NanoBot bot_)
        {
            if (!(plan.isEmpty() || plan.succeeded() || plan.impossible()))
            {
                //see if we should reconsider plan
                //Execute plan step
                //review beliefs and generate desires
                
            }
            else
            {
                List<KeyValuePair<int, Point>> desires;
                List<KeyValuePair<int, Point>> intentions;
                List < ActionsDelegate > actions;
                //review beliefs and generate desires
                
                foreach (DesiresDelegate desire in desiresDelegates)
                {
                    desires = desire(this, beliefs);
                }
                //given a desire generate an intention
                foreach (IntentionsDelegate intention in intentionsDelegates)
                {
                   intentions = intention(this, desires);
                }
                //with an intention generate a plan
                foreach (ActionsDelegate action in actionsDelegates)
                {
                    actions = action(this, intentions);
                }
                //execute plan step
            }
        }

        


    }

    public class Plan
    {
        //This is the list of plan steps
        //protected List<KeyValuePair<Condition, AgentAction>> actions;

        public Plan()
        {
            //The obrigatory variable initialization, so that we neve get a null pointer exception, for this
           // this.actions = new List<KeyValuePair<Condition, AgentAction>>();
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
   /* #region Delegates

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

    #endregion*/
}
