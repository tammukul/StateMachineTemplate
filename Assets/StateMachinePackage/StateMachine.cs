using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachinePackage
{
    /// <summary>
    /// StateMachine updates the current State.
    /// </summary>
    [Serializable]
    public class StateMachine
    {
        private readonly string uniqueStateMachinePrefix;
        
        internal State currentState { get; set; }
        internal State previousState { get; set; }

        protected readonly MasterStateMachine MasterStateMachine;
        protected readonly List<State> States = new List<State>();

        protected StateMachine(MasterStateMachine masterStateMachine)
        {
            this.MasterStateMachine = masterStateMachine;
            
            uniqueStateMachinePrefix = $"StateMachine{masterStateMachine.GetHashCode()}";
        }

        /// <summary>
        /// Exit current state.
        /// </summary>
        internal virtual void Exit()
        {
            this.currentState.ExitState();
        }

        /// <summary>
        /// Run Update.
        /// </summary>
        internal void Tick()
        {
            this.currentState?.Tick();
        }

        /// <summary>
        /// Run FixedUpdate.
        /// </summary>
        internal void FixedTick()
        {
            this.currentState?.FixedTick();
        }

        /// <summary>
        /// Run LateUpdate.
        /// </summary>
        internal void LateTick()
        {
            this.currentState?.LateTick();
        }

        public virtual bool ChangeState(Type stateType, IStateData args = null)
        {
            // Debug.Log("Changing state to: " + stateType);
            
            // Return if we try to change to the already active State.
            if (this.currentState != null && this.currentState.GetType() == stateType)
            {
                Debug.LogWarning("@StateMachine: Trying to change to State of type: " + stateType +
                                 " but it's already the active State.");
                return false;
            }

            // Find the State by type.
            for (int i = 0; i < this.States.Count; i++)
            {
                // Found the state we want to transition to.
                if (this.States[i].GetType() == stateType)
                {
                    // Exit previous State.
                    this.previousState = this.currentState;
                    this.previousState?.ExitState();

                    // Enter next State.
                    this.currentState = this.States[i];
                    this.currentState.EnterState(args);
                    return true;
                }
            }

            Debug.LogWarning("@StateMachine: Can't find State of type: " + stateType);
            return false;
        }

        /// <summary>
        /// Change the active StateMachine.
        /// </summary>
        /// <param name="stateMachineType"></param>
        /// <param name="stateType"></param>
        /// <param name="args"></param>
        internal void ChangeStateMachine(Type stateMachineType, Type stateType, IStateData args = null)
        {
            this.MasterStateMachine.ChangeStateMachine(stateMachineType, stateType, args);
        }
    }
}