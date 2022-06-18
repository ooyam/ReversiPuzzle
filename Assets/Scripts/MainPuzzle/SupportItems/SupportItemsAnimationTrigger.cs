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
        /// �A�q���̉���
        /// </summary>
        /// <param name="_supportColumn">�����ԍ�</param>
        void DuckSupportTrigger(int _supportColumn)
        {
            mStItemsMgr.DuckSupport(_supportColumn);
        }
    }
}
