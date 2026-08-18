using UnityEngine;

namespace GameJam
{
    public abstract class ALebroTrait : MonoBehaviour
    {
        private Lebro lebro;

        public Lebro Lebro => lebro;

        public void Init(Lebro lebro)
        {
            this.lebro = lebro;

            lebro.OnDeath += Lebro_OnDeath;
        }

        private void OnDestroy()
        {
            if (lebro == null) return;
            lebro.OnDeath -= Lebro_OnDeath;
        }

        private void Lebro_OnDeath(Lebro lebro)
        {
            gameObject.SetActive(false);
        }

        public abstract void Apply(Vector2 obtainPosition);
    }
}
