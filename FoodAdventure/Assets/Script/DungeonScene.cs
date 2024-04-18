using Scene;
using UnityEngine;

namespace Dungeon
{
    public class DungeonScene : SceneBase
    {

        #region SerializeField
        [SerializeField] private ReversiController reversiController;
        #endregion

        #region UnityEvent
        public override void Start()
        {
            base.Start();

            reversiController.Init();
        }
        #endregion
    }
}