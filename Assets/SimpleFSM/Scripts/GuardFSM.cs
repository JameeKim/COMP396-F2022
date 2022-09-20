using UnityEngine;

namespace SimpleFSM
{
    /// <summary>
    ///   A simple FSM for the AI logics of a guard
    /// </summary>
    public class GuardFSM : MonoBehaviour
    {
        /// States
        public enum GuardState
        {
            Patrol,
            Attack,
            Runaway,
        }

        private GuardState guardState = GuardState.Patrol;
        private GameObject guardEnemy;

        // Stubs: temporarily hard-coded combat stats
        [Header("Combat Stats (temporary)")]
        [Range(0f, 100f)]
        public float enemyStrength = 75f;

        [Range(0f, 100f)]
        public float strength = 90f;

        [Range(1f, 10f)]
        public float safeDistance = 5f;

        [Header("Debug Colors")]
        public Color threatenedColor = Color.red;
        public Color safeColor = Color.green;

        private void Start()
        {
            ChangeState(GuardState.Patrol);

            // Temporarily have the player as the enemy
            guardEnemy = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            switch (guardState)
            {
                case GuardState.Patrol:
                    UpdatePatrolState();
                    break;
                case GuardState.Attack:
                    UpdateAttackState();
                    break;
                case GuardState.Runaway:
                    UpdateRunawayState();
                    break;
                default:
                    Debug.LogError($"Invalid value for guardState: {guardState}");
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        private void UpdatePatrolState()
        {
            if (IsThreatened())
            {
                ChangeState(IsStrongerThanEnemy() ? GuardState.Attack : GuardState.Runaway);
            }
        }

        private void UpdateAttackState()
        {
            if (IsWeakerThanEnemy())
            {
                ChangeState(GuardState.Runaway);
            }
        }

        private void UpdateRunawayState()
        {
            if (IsSafe())
            {
                ChangeState(GuardState.Patrol);
            }
        }

        private void ChangeState(GuardState newState)
        {
            Debug.Log($"Changing state: {guardState} -> {newState}");

            // TODO: validate the new value
            guardState = newState;
        }

        private bool IsThreatened()
        {
            Vector3 position = transform.position;
            Vector3 enemyPosition = guardEnemy.transform.position;
            bool isThreatened = Vector3.Distance(position, enemyPosition) < safeDistance;
            GetComponent<Renderer>().sharedMaterial.color = isThreatened ? threatenedColor : safeColor;
            return isThreatened;
        }

        private bool IsSafe()
        {
            return !IsThreatened();
        }

        private bool IsStrongerThanEnemy()
        {
            return strength > enemyStrength;
        }

        private bool IsWeakerThanEnemy()
        {
            return !IsStrongerThanEnemy();
        }
    }
}
