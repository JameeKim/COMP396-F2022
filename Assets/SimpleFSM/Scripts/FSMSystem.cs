using System.Collections.Generic;
using UnityEngine;

namespace SimpleFSM
{
    /// <summary>
    /// Labels for the transitions
    /// </summary>
    public enum Transition
    {
        Null = 0,
        SawPlayer,
        LostPlayer,
    }

    /// <summary>
    /// Labels for the states
    /// </summary>
    public enum StateID
    {
        Null = 0,
        FollowingPath,
        ChasingPlayer,
    }

    public abstract class FSMState
    {
        public StateID ID { get; }

        protected Dictionary<Transition, StateID> TransitionToStateMap { get; } = new();

        protected FSMState(StateID id = StateID.Null)
        {
            ID = id;
        }

        public void AddTransition(Transition transition, StateID stateIDToAdd)
        {
            if (transition == Transition.Null)
            {
                Debug.LogError("Null transition not allowed to be added");
                return;
            }

            if (stateIDToAdd == StateID.Null)
            {
                Debug.LogError("Null state not allowed to be added");
                return;
            }

            if (TransitionToStateMap.ContainsKey(transition))
            {
                Debug.LogError($"Transition {transition} already exists in the map (value: {stateIDToAdd})");
                return;
            }

            TransitionToStateMap.Add(transition, stateIDToAdd);
        }

        public void RemoveTransition(Transition transition)
        {
            if (!TransitionToStateMap.Remove(transition))
            {
                Debug.LogError($"Transition {transition} cannot be removed from the map since it does not exist");
            }
        }

        public StateID GetOutputState(Transition transition)
        {
            return TransitionToStateMap.ContainsKey(transition) ? TransitionToStateMap[transition] : StateID.Null;
        }

        public virtual void BeforeEnter()
        {
        }

        public virtual void BeforeLeave()
        {
        }

        public abstract void Reason(GameObject player, GameObject npc);

        public abstract void Act(GameObject player, GameObject npc);
    }

    public class FSMSystem
    {
        private readonly List<FSMState> states = new();

        public FSMState CurrentState { get; private set; }

        public StateID CurrentStateID => CurrentState?.ID ?? StateID.Null;

        public void AddState(FSMState state)
        {
            if (state == null)
            {
                Debug.LogError("Null object passed in as a state to add to the FSM system");
                return;
            }

            if (states.Exists(item => item.ID == state.ID))
            {
                Debug.LogError($"State with ID {state.ID} already exists in the list of FSM system");
                return;
            }

            states.Add(state);

            if (states.Count == 1)
            {
                CurrentState = state;
            }
        }

        public void RemoveState(StateID id)
        {
            if (states.RemoveAll(item => item.ID == id) == 0)
            {
                Debug.LogError($"Cannot remove state with ID {id} since it does not exist in the list");
            }
        }

        public void PerformTransition(Transition transition)
        {
            if (transition == Transition.Null)
            {
                Debug.LogError("Null transition cannot be performed");
                return;
            }

            StateID newStateID = CurrentState.GetOutputState(transition);
            FSMState newState = states.Find(item => item.ID == newStateID);

            if (newState == null)
            {
                Debug.LogError(
                    $"Cannot perform transition {transition} to state with ID {newStateID} "
                    + "since the state does not exist in the list");
                return;
            }

            CurrentState.BeforeLeave();
            CurrentState = newState;
            CurrentState.BeforeEnter();
        }
    }
}
