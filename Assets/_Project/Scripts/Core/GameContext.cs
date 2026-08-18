using Rodak.Utils.Singleton;
using UnityEngine;

namespace GameJam
{
    [DefaultExecutionOrder(-50)]
    public class GameContext : SingletonMonoBehaviour<GameContext>
    {
        [SerializeField] private ToolBelt toolBelt;
        [SerializeField] private Map map;
        [SerializeField] private LebroManager lebroManager;

        public ToolBelt ToolBelt => toolBelt;
        public Map Map => map;
        public LebroManager LebroManager => lebroManager;
    }
}
