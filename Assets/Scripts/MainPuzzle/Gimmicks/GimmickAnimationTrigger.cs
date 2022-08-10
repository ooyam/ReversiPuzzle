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
        void Start()
        {
            mGimmickInfo = this.GetComponent<GimmickInformation>();
        }

        /// <summary>
        /// —³Šª‚ÌUŒ‚
        /// </summary>
        /// <param name="attackNum">UŒ‚”Ô†</param>
        void TornadoAttackTrigger(int attackNum)
        {
            GimmicksMgr.TornadoAttackPieceChange(mGimmickInfo, attackNum);
        }
    }
}