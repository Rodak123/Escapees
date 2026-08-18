using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "LevelArea", menuName = "LevelArea")]
    public class LevelAreaSO : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private string areaName = "Area";
        [SerializeField, Min(0)] private int areaNumber = 0;

        public string AreaName => areaName;
        public int AreaNumber => areaNumber;

        public override string ToString()
        {
            return $"#{areaNumber} {areaName}";
        }
    }
}