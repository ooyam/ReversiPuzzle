using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //©g‚ÌƒMƒ~ƒbƒNî•ñ
        GimmickInformation gimmickInfo;
        GimmicksManager gimmicksMan;
        void Start()
        {
            gimmickInfo = this.GetComponent<GimmickInformation>();
            gimmicksMan = GameObject.FindWithTag("GimmicksManager").GetComponent<GimmicksManager>();
        }

        /// <summary>
        /// —³Šª‚ÌUŒ‚
        /// </summary>
        /// <param name="attackNum">UŒ‚”Ô†</param>
        void TornadoAttackTrigger(int attackNum)
        {
            gimmicksMan.TornadoAttackPieceChange(gimmickInfo, attackNum);
        }
    }
}