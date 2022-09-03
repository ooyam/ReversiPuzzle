using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //���g�̃M�~�b�N���
        GimmickInformation mGimmickInfo;

        //�Đ�����AudioSource
        AudioSource mAudio;

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

        /// <summary>
        /// SE�Đ�
        /// </summary>
        void GimmickSePlay(SE_Type _seType)
        {
            switch (_seType)
            {
                //�A���Đ�
                case SE_Type.HamsterBurst:
                    mAudio = SE_ContinuousPlay(_seType);
                    break;

                //�P���Đ�
                default:
                    mAudio = SE_OneShot(_seType);
                    break;
            }
        }

        /// <summary>
        /// SE��~
        /// </summary>
        void GimmickSeStop()
        {
            SE_Stop(mAudio);
        }
    }
}