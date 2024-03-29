using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //©gÌM~bNîñ
        GimmickInformation mGimmickInfo;

        //Ä¶ÌAudioSource
        AudioSource mAudio;

        void Start()
        {
            mGimmickInfo = this.GetComponent<GimmickInformation>();
        }

        /// <summary>
        /// ³ªÌU
        /// </summary>
        /// <param name="attackNum">UÔ</param>
        void TornadoAttackTrigger(int attackNum)
        {
            GimmicksMgr.TornadoAttackPieceChange(mGimmickInfo, attackNum);
        }

        /// <summary>
        /// SEÄ¶
        /// </summary>
        void GimmickSePlay(SE_Type _seType)
        {
            switch (_seType)
            {
                //A±Ä¶
                case SE_Type.HamsterBurst:
                    mAudio = SE_ContinuousPlay(_seType);
                    break;

                //P­Ä¶
                default:
                    mAudio = SE_OneShot(_seType);
                    break;
            }
        }

        /// <summary>
        /// SEâ~
        /// </summary>
        void GimmickSeStop()
        {
            SE_Stop(mAudio);
        }
    }
}