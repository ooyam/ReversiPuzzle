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
        void Start()
        {
            mGimmickInfo = this.GetComponent<GimmickInformation>();
        }

        /// <summary>
        /// �����̍U��
        /// </summary>
        /// <param name="attackNum">�U���ԍ�</param>
        void TornadoAttackTrigger(int attackNum)
        {
            GimmicksMgr.TornadoAttackPieceChange(mGimmickInfo, attackNum);
        }
    }
}