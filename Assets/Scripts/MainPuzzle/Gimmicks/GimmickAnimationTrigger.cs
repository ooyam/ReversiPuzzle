using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //���g�̃M�~�b�N���
        GimmickInformation gimmickInfo;
        GimmicksManager gimmicksMan;
        void Start()
        {
            gimmickInfo = this.GetComponent<GimmickInformation>();
            gimmicksMan = GameObject.FindWithTag("GimmicksManager").GetComponent<GimmicksManager>();
        }

        /// <summary>
        /// �����̍U��
        /// </summary>
        /// <param name="attackNum">�U���ԍ�</param>
        void TornadoAttackTrigger(int attackNum)
        {
            gimmicksMan.TornadoAttackPieceChange(gimmickInfo, attackNum);
        }
    }
}