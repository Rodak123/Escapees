using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameJam
{
    public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected string description;

        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private GameObject selectedHighlight;

        public event Action<ActionButtonUI> OnClick;
        public event Action<ActionButtonUI> OnMouseEntered;
        public event Action<ActionButtonUI> OnMouseExited;

        public string Description => description;

        protected virtual void OnEnable()
        {
            button.onClick.AddListener(() =>
            {
                OnClick?.Invoke(this);
            });
        }

        protected virtual void Awake()
        {
            Unselect();
        }

        public virtual void Select()
        {
            selectedHighlight.SetActive(true);
        }

        public virtual void Unselect()
        {
            selectedHighlight.SetActive(false);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseEntered?.Invoke(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            OnMouseExited?.Invoke(this);
        }
    }
}
