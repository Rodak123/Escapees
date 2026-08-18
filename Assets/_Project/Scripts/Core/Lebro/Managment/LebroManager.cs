using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class LebroManager : MonoBehaviour
    {
        [SerializeField] private Lebro lebroPrefab;

        [Header("Spawning")]
        [SerializeField] private Transform lebroContainer;

        private readonly HashSet<Lebro> lebros = new();
        private Lebro hoveredLebro;

        public bool IsPaused { get; private set; }
        public int TotalLebros { get; private set; }

        public event Action<Lebro> OnLebroSpawned;
        public event Action<Lebro> OnLebroDied;
        public event Action<Lebro> OnLebroRemoved;

        public IReadOnlyList<Lebro> Lebros => new List<Lebro>(lebros);

        public Lebro HoveredLebro => hoveredLebro;

        private void Update()
        {
            UpdateHoveredLebro();
        }

        private void UpdateHoveredLebro()
        {
            if (hoveredLebro != null) return;
            foreach (Lebro lebro in lebros)
            {
                if (lebro.IsHovered)
                {
                    hoveredLebro = lebro;
                    hoveredLebro.OnHoveredChanged += HoveredLebro_OnHoveredChanged;
                    return;
                }
            }
        }

        private void Lebro_OnDeath(Lebro lebro)
        {
            lebros.Remove(lebro);
            lebro.OnDeath -= Lebro_OnDeath;

            if (lebro == hoveredLebro)
            {
                lebro.OnHoveredChanged -= HoveredLebro_OnHoveredChanged;
                hoveredLebro = null;
            }

            OnLebroDied?.Invoke(lebro);
        }

        private void HoveredLebro_OnHoveredChanged(Lebro lebro)
        {
            if (lebro.IsHovered) return;
            lebro.OnHoveredChanged -= HoveredLebro_OnHoveredChanged;

            hoveredLebro = null;
        }

        public Lebro SpawnLebro(Vector2Int spawn)
        {
            Lebro lebro = Instantiate(lebroPrefab, lebroContainer);
            lebro.transform.position = new(spawn.x, spawn.y);

            lebros.Add(lebro);
            lebro.OnDeath += Lebro_OnDeath;

            OnLebroSpawned?.Invoke(lebro);

            TotalLebros++;

            return lebro;
        }

        public bool TryRemoveLebro(Lebro lebro)
        {
            if (!lebros.Remove(lebro))
                return false;

            lebro.DestroySelf();
            OnLebroRemoved?.Invoke(lebro);
            return true;
        }

        public void PauseLebros()
        {
            IsPaused = true;
            UpdateLebrosPaused();
        }

        public void UnpauseLebros()
        {
            IsPaused = false;
            UpdateLebrosPaused();
        }

        private void UpdateLebrosPaused()
        {
            foreach (Lebro lebro in GameContext.Instance.LebroManager.Lebros)
            {
                if (IsPaused)
                {
                    lebro.Pause();
                }
                else
                {
                    lebro.Unpause();
                }
            }
        }

    }
}
