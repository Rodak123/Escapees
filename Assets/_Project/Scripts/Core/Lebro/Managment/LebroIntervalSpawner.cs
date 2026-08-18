using UnityEngine;

namespace GameJam
{
    public class LebroIntervalSpawner : MonoBehaviour
    {
        [Header("Spawning")]
        [SerializeField] private bool startReady = true;
        [SerializeField, Min(0)] private float spawnInterval = 1f;

        [Header("Lebro")]
        [SerializeField] private LebroController.MovementGoal[] pickedMovementGoals;

        private Vector2Int spawnPosition;
        private float spawnTimer;

        private void Start()
        {
            spawnPosition = transform.position.RoundToInt();

            if (startReady) spawnTimer = spawnInterval;
        }

        private void Update()
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer -= spawnInterval;
                Lebro lebro = GameContext.Instance.LebroManager.SpawnLebro(spawnPosition);

                if (pickedMovementGoals.Length > 0)
                {
                    lebro.Controller.CurrentMovementGoal = pickedMovementGoals[Random.Range(0, pickedMovementGoals.Length)];
                }
            }
        }
    }
}
