using UnityEngine;

namespace GameJam
{
    public class LebroSpawner : MonoBehaviour
    {
        [SerializeField] private LebroController.MovementGoal movementGoal = LebroController.MovementGoal.Standing;

        private void Start()
        {
            Vector2Int spawnPosition = transform.position.RoundToInt();
            Lebro lebro = GameContext.Instance.LebroManager.SpawnLebro(spawnPosition);
            lebro.Controller.CurrentMovementGoal = movementGoal;
        }

    }
}
