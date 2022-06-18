using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class SupportItemsAnimationTrigger : MonoBehaviour
    {
        SupportItemsManager mStItemsMgr;
        void Start()
        {
            mStItemsMgr = sPuzzleMain.GetSupportItemsManager();
        }

        /// <summary>
        /// ƒAƒqƒ‹‚Ì‰‡Œì
        /// </summary>
        /// <param name="_supportColumn">‰‡Œì—ñ”Ô†</param>
        void DuckSupportTrigger(int _supportColumn)
        {
            mStItemsMgr.DuckSupport(_supportColumn);
        }
    }
}
