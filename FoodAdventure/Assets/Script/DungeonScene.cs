using Scene;
using UnityEngine;

namespace Dungeon
{
    public class DungeonScene : SceneBase
    {
        #region SerializeField
        [SerializeField] private DungeonController dungeonController;
        #endregion

        #region UnityEvent
        public override void Start()
        {
            base.Start();

            dungeonController.Init();
        }
        #endregion
    }
}