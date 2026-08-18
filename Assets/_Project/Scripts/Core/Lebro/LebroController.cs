using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    // inspired by https://www.youtube.com/watch?v=05eWA0TP3AA
    [RequireComponent(typeof(BoxCollider2D), typeof(Lebro))]
    public class LebroController : MonoBehaviour
    {
        [Serializable]
        public enum MovementGoal
        {
            Standing,
            WalkRight,
            WalkLeft
        }

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float maxStepHeight = 1f;
        [SerializeField] private float gravity = 35f;

        [SerializeField] private float normalFallVelocity = 6f;

        [SerializeField] public bool EnableLethalFalls = true;
        [SerializeField] private float lethalFallDistance = 15f;
        [SerializeField] private float lethalFallVelocity = 12f;
        [SerializeField] public float FallVelocityScale = 1;

        [field: SerializeField]
        public MovementGoal CurrentMovementGoal { get; set; } = MovementGoal.WalkRight;

        [Header("Collision")]
        [SerializeField] private float collisionOffset = 0.05f;
        [SerializeField] private float maxSlopeAngle = 60f;
        [SerializeField] private ContactFilter2D movementFilter;

        [Header("Movement Randomizer")]
        [SerializeField] private Vector2 movementSpeedMultiplierRange = new(1f, 1.5f);
        [SerializeField] private Vector2 fallSpeedMultiplierRange = new(1f, 1.5f);

        [Header("Debug")]
        [SerializeField] private bool drawDebug;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private bool isGrounded;
        [SerializeField] private int moveDirection;
        [SerializeField] private float fallDistance;

        private Lebro lebro;

        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;

        private readonly List<RaycastHit2D> castCollisions = new();
        private readonly List<Collider2D> overlapResults = new();

        private float randomMovementSpeedMultiplier;
        private float randomFallSpeedMultiplier;

        public Vector2 Velocity => velocity;
        public bool IsGrounded => isGrounded;
        public bool IsFallLethal => EnableLethalFalls && fallDistance >= lethalFallDistance;
        public Vector2Int PixelPosition { get; private set; }
        public Vector2Int Size => new(Mathf.CeilToInt(boxCollider.size.x), Mathf.CeilToInt(boxCollider.size.y));

        public float MoveSpeed => moveSpeed * randomMovementSpeedMultiplier;
        public float NormalFallVelocity => normalFallVelocity * randomFallSpeedMultiplier;

        public event Action<Lebro> OnStartedFalling;
        public event Action<Lebro> OnLanded;
        public event Action<Lebro> OnHitWall;

        private void Awake()
        {
            lebro = GetComponent<Lebro>();

            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.freezeRotation = true;

            boxCollider = GetComponent<BoxCollider2D>();

            movementFilter.useTriggers = false;

            isGrounded = true;
            fallDistance = 0;
            velocity = Vector2.zero;

            randomMovementSpeedMultiplier = UnityEngine.Random.Range(movementSpeedMultiplierRange.x, movementSpeedMultiplierRange.y);
            randomFallSpeedMultiplier = UnityEngine.Random.Range(fallSpeedMultiplierRange.x, fallSpeedMultiplierRange.y);
        }

        private void FixedUpdate()
        {
            if (lebro.IsPaused) return;

            EnsureUnstuck();

            // Apply gravity 
            float terminalFallVelocity = (IsFallLethal ? lethalFallVelocity : NormalFallVelocity) * FallVelocityScale;
            velocity.y = Math.Max(velocity.y - gravity * Time.fixedDeltaTime, -terminalFallVelocity);

            // Apply movement goal
            switch (CurrentMovementGoal)
            {
                case MovementGoal.Standing:
                    velocity.x = 0;
                    break;
                case MovementGoal.WalkRight:
                    velocity.x = MoveSpeed;
                    break;
                case MovementGoal.WalkLeft:
                    velocity.x = -MoveSpeed;
                    break;
            }

            if (isGrounded == false || lebro.IsDead)
            {
                velocity.x = 0; // fall until ground is hit
            }

            bool wasGrounded = isGrounded;

            // Update
            isGrounded = false;

            Vector2 moveDelta = velocity * Time.fixedDeltaTime;

            Vector2 position = rb.position;

            position = Move(position, new(0, moveDelta.y));
            position = Move(position, new(moveDelta.x, 0));

            if (wasGrounded && !isGrounded)
            {
                // started falling
                position = TryMoveToStepBelow(position);
                OnStartedFalling?.Invoke(lebro);
            }

            if (!isGrounded) fallDistance += Mathf.Abs(moveDelta.y);
            if (!wasGrounded && isGrounded)
            {
                // hit the ground
                OnLanded?.Invoke(lebro);
                if (IsFallLethal) lebro.Die();
            }
            if (isGrounded) fallDistance = 0;

            rb.MovePosition(position);
            PixelPosition = transform.position.RoundToInt();
        }

        private bool Cast(Vector2 castCenter, float characterBottomY, Vector2 direction, float distance, out RaycastHit2D validHit, bool useFlatCollider = false)
        {
            validHit = default;
            Vector2 start = castCenter + boxCollider.offset;
            Vector2 size = boxCollider.size;

            if (useFlatCollider)
            {
                float flatHeight = 1f;
                start.y = start.y - size.y / 2f + flatHeight / 2f;
                size.y = flatHeight;
            }

            if (drawDebug)
            {
                float duration = 0f;
                Color color = Color.orange;

                Vector2 end = start + direction * distance;
                Debug.DrawLine(start, end, color, duration);
                Debug.DrawLine(start - size / 2, end - size / 2, color, duration);
                Debug.DrawLine(start + size / 2, end + size / 2, color, duration);
                Debug.DrawLine(start + new Vector2(-size.x, size.y) / 2, end + new Vector2(-size.x, size.y) / 2, color, duration);
                Debug.DrawLine(start + new Vector2(size.x, -size.y) / 2, end + new Vector2(size.x, -size.y) / 2, color, duration);
            }

            int count = Physics2D.BoxCast(start, size, 0, direction, movementFilter, castCollisions, distance);

            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = castCollisions[i];
                if (drawDebug) Debug.Log($"Cast hit: {hit.collider}, {hit.point}, {hit.normal}");

                if (direction.y < -0.001f && hit.point.y > characterBottomY + 0.1f)
                {
                    // if casting down, ignore hits above
                    // this is when inside a collider
                    if (drawDebug) Debug.Log($"Ignored hit inside a collider");
                    continue;
                }

                // one-way platforms
                if (hit.collider.usedByEffector && hit.collider.TryGetComponent(out PlatformEffector2D effector) && effector.useOneWay)
                {
                    // 4 options here
                    // _ B _
                    // A / C
                    // X D X
                    // from A to C -> you are moving right (not down), and are above it = not ignored, moving up
                    // from C to A -> you are moving left (not down), and are below it = is ignored
                    // from B to D -> you are moving down, and are above it = not ignored, you will crash
                    // from D to B -> you are moving up (not down), and are below it = is ignored, you can climb up

                    // this is to account for when the hit and character
                    // are basically on the same y
                    // for flat platforms, there should be a bias towards passing through them from below (so below for longer)
                    // for sloped platforms, there should be a bias towards climbing them up (so below for shorter)
                    // I'm not sure if this is a good solution, but if i remove it, they either get stuck or ignore ramps :evaporate:
                    bool isPlatformFlat = hit.normal.y >= 0.99f;

                    bool isNotMovingDown = direction.y >= -0.001f;
                    bool isBelowSurface = characterBottomY < hit.point.y + (isPlatformFlat ? collisionOffset : -collisionOffset);
                    if (drawDebug) Debug.Log($"hit {effector}, isNotMovingDown:{isNotMovingDown} and isBelowSurface:{isBelowSurface}, {hit.normal}");
                    if (isNotMovingDown && isBelowSurface)
                    {
                        continue;
                    }
                }

                validHit = hit;
                return true;
            }

            return false;
        }

        private Vector2 Move(Vector2 position, Vector2 moveDelta)
        {
            if (drawDebug) Debug.Log($"Calculate move from {position} by {moveDelta}");
            if (moveDelta.magnitude < 0.00001f) return position;

            float characterBottomY = position.y + boxCollider.offset.y - (boxCollider.size.y / 2f);

            if (!Cast(position, characterBottomY, moveDelta.normalized, moveDelta.magnitude + collisionOffset, out RaycastHit2D hit))
            {
                return position + moveDelta;
            }

            if (moveDelta.y < 0)
            {
                // moving down
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (drawDebug) Debug.Log($"Moving down: {slopeAngle}, {hit.collider.gameObject}");
                if (slopeAngle <= maxSlopeAngle)
                {
                    if (drawDebug) Debug.Log("On the ground");
                    isGrounded = true;
                    velocity.y = 0;
                }
            }

            if (Math.Abs(moveDelta.x) > 0)
            {
                // moving sideways
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (drawDebug) Debug.Log($"Moving side: {slopeAngle}, {hit.collider.gameObject}");

                if (slopeAngle > 0 && slopeAngle <= maxSlopeAngle)
                {
                    // try stepping up
                    return TryMoveUpSteps(position, moveDelta.x);
                }

                if (slopeAngle > maxSlopeAngle)
                {
                    // turn around
                    HitWall();
                    return position;
                }
            }

            float nearestDistance = Math.Max(0, hit.distance - collisionOffset);
            return position + (moveDelta.normalized * nearestDistance);
        }

        // moves right/left and tries to step up
        private Vector2 TryMoveUpSteps(Vector2 position, float moveDeltaX)
        {
            if (drawDebug) Debug.Log("Moving up steps");

            if (Math.Abs(moveDeltaX) < 0.00001f) return position;

            Vector2 startAbove = position + new Vector2(moveDeltaX, maxStepHeight);
            float characterBottomY = position.y + boxCollider.offset.y - (boxCollider.size.y / 2f);

            if (!Cast(startAbove, characterBottomY, Vector2.down, maxStepHeight + collisionOffset, out RaycastHit2D hit, useFlatCollider: true))
            {
                HitWall();
                return position;
            }

            if (drawDebug)
            {
                Debug.DrawRay(hit.point, hit.normal, Color.orange, 10);
                Debug.DrawLine(position + new Vector2(moveDeltaX / 2, 0), hit.point, Color.purple, 10);
            }

            isGrounded = true;

            float halfHeight = boxCollider.size.y / 2f;
            float targetY = hit.point.y + halfHeight - boxCollider.offset.y + collisionOffset;

            return new Vector2(position.x + moveDeltaX, targetY);
        }

        // tries to step down if in reach, to prevent falling
        private Vector2 TryMoveToStepBelow(Vector2 position)
        {
            if (drawDebug) Debug.Log("Moving down steps");

            float characterBottomY = position.y + boxCollider.offset.y - (boxCollider.size.y / 2f);

            if (!Cast(position, characterBottomY, Vector2.down, maxStepHeight + collisionOffset, out RaycastHit2D hit))
                return position;

            isGrounded = true;

            float halfHeight = boxCollider.size.y / 2f;
            float targetY = hit.point.y + halfHeight - boxCollider.offset.y + collisionOffset;

            return new Vector2(position.x, targetY);
        }

        private void EnsureUnstuck()
        {
            if (lebro.IsDead) return;

            int count = boxCollider.Overlap(movementFilter, overlapResults);
            if (count > 0)
            {
                foreach (Collider2D otherCollider in overlapResults)
                {
                    // no need to unstuck one way platforms
                    if (otherCollider.usedByEffector && otherCollider.TryGetComponent(out PlatformEffector2D effector) && effector.useOneWay)
                        continue;

                    ColliderDistance2D distance = boxCollider.Distance(otherCollider);
                    if (distance.isOverlapped)
                    {
                        if (drawDebug) Debug.Log($"Unstucking from: {otherCollider.gameObject}, A:{distance.pointA}, B:{distance.pointB}");
                        rb.position += distance.normal * distance.distance;
                    }
                }
            }
        }

        private void HitWall()
        {
            TurnAround();
            OnHitWall?.Invoke(lebro);
        }

        public void TurnAround()
        {
            if (CurrentMovementGoal == MovementGoal.WalkRight) CurrentMovementGoal = MovementGoal.WalkLeft;
            else if (CurrentMovementGoal == MovementGoal.WalkLeft) CurrentMovementGoal = MovementGoal.WalkRight;
        }
    }
}