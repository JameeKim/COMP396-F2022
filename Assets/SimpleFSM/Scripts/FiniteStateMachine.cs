using System.Collections.Generic;
using UnityEngine;

namespace SimpleFSM
{
    public class FiniteStateMachine
    {
        public class State
        {
            /// The name of this state
            public readonly string name;

            /// The logic to run every frame when the state is active
            public System.Action onFrame;

            /// The logic to run when the state becomes active
            public System.Action onEnter;

            /// The logic to run when the state becomes inactive
            public System.Action onExit;

            public State(string name)
            {
                this.name = name;
            }

            public override string ToString()
            {
                return name;
            }
        }

        private readonly Dictionary<string, State> states = new();

        public State CurrentState { get; private set; }
        public State InitialState { get; private set; }

        /// <summary>
        /// Create a new state and add it to the system
        /// </summary>
        /// <param name="name">ID/key for the new state</param>
        /// <returns>The created new state</returns>
        public State CreateState(string name)
        {
            State newState = new(name);

            if (states.Count == 0)
            {
                InitialState = newState;
            }

            states.Add(name, newState);
            return newState;
        }

        /// <summary>
        /// Update logic which is to be called in the game update loop
        /// </summary>
        public void Update()
        {
            if (states.Count == 0)
            {
                Debug.LogError("Cannot run `Update` of the finite state machine; no states are added yet");
                return;
            }

            // If current state is not set, transition to the initial state
            if (CurrentState == null)
            {
                TransitionTo(InitialState);
            }

            CurrentState?.onFrame?.Invoke();
        }

        public void TransitionTo(State newState)
        {
            if (newState == null)
            {
                Debug.LogError("Cannot transition to a null state");
                return;
            }

            CurrentState?.onExit?.Invoke();
            Debug.Log($"State transition: {CurrentState} -> {newState}");
            CurrentState = newState;
            CurrentState.onEnter?.Invoke();
        }

        public void TransitionTo(string stateName)
        {
            if (!states.ContainsKey(stateName))
            {
                Debug.LogError($"Cannot transition to state {stateName}; no such name for a state");
                return;
            }

            TransitionTo(states[stateName]);
        }
    }
}
