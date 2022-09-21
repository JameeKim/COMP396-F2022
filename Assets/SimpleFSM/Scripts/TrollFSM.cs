using UnityEngine;

namespace SimpleFSM
{
    public class TrollFSM : MonoBehaviour
    {
        public Transform[] path;

        [Min(1f)]
        public float startChasingDistance = 5f;

        [Min(5f)]
        public float stopChasingDistance = 10f;

        public Color patrollingColor = Color.green;
        public Color chasingColor = Color.red;

        private GameObject player;
        private readonly FiniteStateMachine fsm = new();

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            MakeFSM();
        }

        private void Update()
        {
            fsm.Update();
        }

        private void MakeFSM()
        {
            FiniteStateMachine.State patrollingState = fsm.CreateState("Patrolling");

            patrollingState.onEnter = () =>
            {
                Debug.Log("Entering patrolling state");
                GetComponent<Renderer>().sharedMaterial.color = patrollingColor;
            };

            patrollingState.onFrame = () =>
            {
                if (Vector3.Distance(player.transform.position, transform.position) < startChasingDistance)
                {
                    fsm.TransitionTo("Chasing");
                }
            };

            patrollingState.onExit = () =>
            {
                Debug.Log("Exiting patrolling state");
            };

            FiniteStateMachine.State chasingState = fsm.CreateState("Chasing");

            chasingState.onEnter = () =>
            {
                Debug.Log("Entering chasing state");
                GetComponent<Renderer>().sharedMaterial.color = chasingColor;
            };

            chasingState.onFrame = () =>
            {
                if (Vector3.Distance(player.transform.position, transform.position) > stopChasingDistance)
                {
                    fsm.TransitionTo("Patrolling");
                }
            };

            chasingState.onExit = () =>
            {
                Debug.Log("Exiting chasing state");
            };
        }
    }
}
