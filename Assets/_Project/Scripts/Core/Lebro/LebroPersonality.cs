using UnityEngine;

namespace GameJam
{
    [RequireComponent(typeof(Lebro))]
    public class LebroPersonality : MonoBehaviour
    {
        [SerializeField] private Transform traitContainer;

        private Lebro lebro;
        [SerializeField, ReadOnly] private ALebroTrait trait;

        public ALebroTrait CurrentTrait => trait;

        private void Awake()
        {
            lebro = GetComponent<Lebro>();
        }

        private void Start()
        {
            trait = traitContainer.GetComponentInChildren<ALebroTrait>();
            if (trait != null)
            {
                trait.Init(lebro);
                trait.Apply(transform.position);
            }
        }

        public bool TryGiveTrait(ALebroTrait traitPrefab, out ALebroTrait createdTrait)
        {
            if (lebro.IsDead || trait != null || traitPrefab == null)
            {
                createdTrait = null;
                return false;
            }

            trait = Instantiate(traitPrefab, traitContainer);
            trait.Init(lebro);
            createdTrait = trait;
            return true;
        }

        public void RemoveTrait(ALebroTrait removedTrait)
        {
            if (trait == null || lebro.IsDead || trait != removedTrait)
                return;

            Destroy(trait.gameObject);
            trait = null;
        }
    }
}
