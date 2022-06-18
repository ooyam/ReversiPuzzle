using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //©g‚ÌƒMƒ~ƒbƒNî•ñ
        GimmickInformation mGimmickInfo;
        GimmicksManager mGimmicksMan;
        void Start()
        {
            mGimmickInfo = this.GetComponent<GimmickInformation>();
            mGimmicksMan = sPuzzleMain.GetGimmicksManager();
        }

        /// <summary>
        /// —³Šª‚ÌUŒ‚
        /// </summary>
        /// <param name="attackNum">UŒ‚”Ô†</param>
        void TornadoAttackTrigger(int attackNum)
        {
            mGimmicksMan.TornadoAttackPieceChange(mGimmickInfo, attackNum);
        }
    }
}