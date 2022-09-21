using UnityEngine;

namespace SimpleFSM
{
    public class TrollControl : MonoBehaviour
    {
        public GameObject player;
        public Transform[] path;

        public FSMSystem FSMSystem { get; } = new();

        public void SetTransition(Transition transition)
        {
            FSMSystem.PerformTransition(transition);
        }

        private void Start()
        {
            MakeFSM();
        }

        private void FixedUpdate()
        {
            FSMSystem.CurrentState.Reason(player, gameObject);
            FSMSystem.CurrentState.Act(player, gameObject);
        }

        private void MakeFSM()
        {
            FollowPathState followPathState = new(path);
            followPathState.AddTransition(Transition.SawPlayer, StateID.ChasingPlayer);

            ChasePlayerState chasePlayerState = new();
            chasePlayerState.AddTransition(Transition.LostPlayer, StateID.FollowingPath);

            FSMSystem.AddState(followPathState);
            FSMSystem.AddState(chasePlayerState);
        }
    }

    public class FollowPathState : FSMState
    {
        private Transform[] waypoints;
        private int currentWaypointIdx;
        private Transform CurrentWaypoint => waypoints[currentWaypointIdx];

        public FollowPathState(Transform[] path) : base(StateID.FollowingPath)
        {
            waypoints = path;
            currentWaypointIdx = 0;
        }

        public override void Reason(GameObject player, GameObject npc)
        {
            if (!Physics.Raycast(npc.transform.position, npc.transform.forward, out RaycastHit hit, 15f)) return;

            if (hit.transform.gameObject.CompareTag("Player"))
            {
                npc.GetComponent<TrollControl>().SetTransition(Transition.SawPlayer);
            }
        }

        public override void Act(GameObject player, GameObject npc)
        {
            Vector3 direction = CurrentWaypoint.position - npc.transform.position;

            if (direction.magnitude < 1f)
            {
                currentWaypointIdx = (currentWaypointIdx + 1) % waypoints.Length;
            }
            else
            {
                // Face the waypoint
                npc.transform.rotation = Quaternion.Slerp(
                    npc.transform.rotation,
                    Quaternion.LookRotation(direction),
                    5f * Time.deltaTime);
                npc.transform.eulerAngles = new Vector3(0f, npc.transform.eulerAngles.y, 0f);

                // Set the velocity
                npc.GetComponent<Rigidbody>().velocity = direction.normalized * 10f;
            }
        }
    }

    public class ChasePlayerState : FSMState
    {
        public ChasePlayerState() : base(StateID.ChasingPlayer)
        {
        }

        public override void Reason(GameObject player, GameObject npc)
        {
            if (Vector3.Distance(npc.transform.position, player.transform.position) > 30f)
            {
                npc.GetComponent<TrollControl>().SetTransition(Transition.LostPlayer);
            }
        }

        public override void Act(GameObject player, GameObject npc)
        {
            // Direction towards the player
            Vector3 direction = player.transform.position - npc.transform.position;

            // Face the player
            npc.transform.rotation = Quaternion.Slerp(
                npc.transform.rotation,
                Quaternion.LookRotation(direction),
                5f * Time.deltaTime);
            npc.transform.eulerAngles = new Vector3(0f, npc.transform.eulerAngles.y, 0f);

            // Set the velocity
            npc.GetComponent<Rigidbody>().velocity = direction.normalized * 10f;
        }
    }
}
