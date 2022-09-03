using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class SupportItemsAnimationTrigger : MonoBehaviour
    {
        /// <summary>
        /// アヒルの援護
        /// </summary>
        /// <param name="_column">援護列番号</param>
        void DuckSupportTrigger(int _column)
        {
            SupportItemsMgr.DuckSupport(_column);
        }

        /// <summary>
        /// 花火の援護
        /// </summary>
        /// <param name="_place">援護場所</param>
        void FireworkSupportTrigger(FireworkSupportPlace _place)
        {
            SupportItemsMgr.FireworkSupport(_place);
        }

        /// <summary>
        /// ロケット(横)の援護
        /// </summary>
        /// <param name="_supportNum">援護番号(10の位:行指定 1の位:列指定)</param>
        void RocketLineSupportTrigger(int _supportNum)
        {
            SupportItemsMgr.RocketLineSupport(_supportNum);
        }

        /// <summary>
        /// ロケット(縦)の援護
        /// </summary>
        /// <param name="_supportNum">援護番号(10の位:列指定 1の位:行指定)</param>
        void RocketColumnSupportTrigger(int _supportNum)
        {
            SupportItemsMgr.RocketColumnSupport(_supportNum);
        }

        /// <summary>
        /// SE再生
        /// </summary>
        void SupportItemSePlay(SE_Type _seType)
        {
            switch (_seType)
            {
                //連続再生
                case SE_Type.Duck:
                    SE_ContinuousPlay(_seType);
                    break;

                //単発再生
                default:
                    SE_OneShot(_seType);
                    break;
            }
        }
    }
}
