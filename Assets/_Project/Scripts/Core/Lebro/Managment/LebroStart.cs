using UnityEngine;

namespace GameJam
{
    public class LebroStart : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private DestroyAfter lebroSpawnAnimation;

        [Header("Spawning")]
        [SerializeField] private bool startReady = true;
        [SerializeField, Min(0)] private float spawnInterval = 1f;

        [Header("Lebro")]
        [SerializeField, Min(0)] private int spawnUntil = 1;
        [SerializeField] private LebroController.MovementGoal[] pickedMovementGoals;

        private LebroManager lebroManager;

        private Vector2Int spawnPosition;
        private float spawnTimer;

        public int SpawnUntil
        {
            get => spawnUntil;
            set => spawnUntil = Mathf.Max(0, value);
        }

        private void Awake()
        {
            lebroManager = GameContext.Instance.LebroManager;
        }

        private void Start()
        {
            spawnPosition = transform.position.RoundToInt();

            if (startReady) spawnTimer = spawnInterval;
        }

        private void Update()
        {
            if (lebroManager.IsPaused) return;
            if (spawnTimer >= spawnInterval)
            {
                if (lebroManager.TotalLebros >= SpawnUntil) return;
                spawnTimer -= spawnInterval;
                SpawnLebro();
            }
            else
            {
                spawnTimer += Time.deltaTime;
            }
        }

        private void SpawnLebro()
        {
            DestroyAfter spawnAnimation = Instantiate(lebroSpawnAnimation, transform);
            spawnAnimation.gameObject.SetActive(true);
            spawnAnimation.OnFinished += SpawnAnimation_DestroyAfter_OnFinished;
        }

        private void SpawnAnimation_DestroyAfter_OnFinished(DestroyAfter destroyAfter)
        {
            Lebro lebro = lebroManager.SpawnLebro(spawnPosition);
            if (pickedMovementGoals.Length > 0)
            {
                lebro.Controller.CurrentMovementGoal = pickedMovementGoals[Random.Range(0, pickedMovementGoals.Length)];
            }
        }
    }
}
