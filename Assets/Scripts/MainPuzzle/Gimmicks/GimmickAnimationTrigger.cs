using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class GimmickAnimationTrigger : MonoBehaviour
    {
        //自身のギミック情報
        GimmickInformation mGimmickInfo;
        void Start()
        {
            mGimmickInfo = this.GetComponent<GimmickInformation>();
        }

        /// <summary>
        /// 竜巻の攻撃
        /// </summary>
        /// <param name="attackNum">攻撃番号</param>
        void TornadoAttackTrigger(int attackNum)
        {
            GimmicksMgr.TornadoAttackPieceChange(mGimmickInfo, attackNum);
        }

        /// <summary>
        /// SE再生
        /// </summary>
        void GimmickSePlay(SE_Type _seType)
        {
            switch (_seType)
            {
                //連続再生
                case SE_Type.HamsterBurst:
                    SE_ContinuousPlay(_seType);
                    break;

                //単発再生
                default:
                    SE_Onshot(_seType);
                    break;
            }
        }
    }
}