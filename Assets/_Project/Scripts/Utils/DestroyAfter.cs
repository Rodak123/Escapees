using System;
using UnityEngine;

namespace GameJam
{
    public class DestroyAfter : MonoBehaviour
    {
        [SerializeField] public float Duration;

        private float timer;

        public Action<DestroyAfter> OnFinished;

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= Duration)
            {
                OnFinished?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
