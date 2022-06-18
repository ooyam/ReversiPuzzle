using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //���g�̃M�~�b�N���
        GimmickInformation mGimmickInfo;
        GimmicksManager mGimmicksMan;
        void Start()
        {
            mGimmickInfo = this.GetComponent<GimmickInformation>();
            mGimmicksMan = sPuzzleMain.GetGimmicksManager();
        }

        /// <summary>
        /// �����̍U��
        /// </summary>
        /// <param name="attackNum">�U���ԍ�</param>
        void TornadoAttackTrigger(int attackNum)
        {
            mGimmicksMan.TornadoAttackPieceChange(mGimmickInfo, attackNum);
        }
    }
}