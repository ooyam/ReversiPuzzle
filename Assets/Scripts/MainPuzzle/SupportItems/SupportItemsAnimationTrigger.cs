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
        /// アヒルの援護
        /// </summary>
        /// <param name="_column">援護列番号</param>
        void DuckSupportTrigger(int _column)
        {
            mStItemsMgr.DuckSupport(_column);
        }

        /// <summary>
        /// ロケット(横)の援護
        /// </summary>
        /// <param name="_supportNum">援護番号(10の位:行指定 1の位:列番号指定)</param>
        void RocketLineSupportTrigger(int _supportNum)
        {
            mStItemsMgr.RocketLineSupport(_supportNum);
        }
    }
}
