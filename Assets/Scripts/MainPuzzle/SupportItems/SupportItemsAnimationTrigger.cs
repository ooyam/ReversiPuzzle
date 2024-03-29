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
        /// AqÌì
        /// </summary>
        /// <param name="_column">ìñÔ</param>
        void DuckSupportTrigger(int _column)
        {
            SupportItemsMgr.DuckSupport(_column);
        }

        /// <summary>
        /// ÔÎÌì
        /// </summary>
        /// <param name="_place">ìê</param>
        void FireworkSupportTrigger(FireworkSupportPlace _place)
        {
            SupportItemsMgr.FireworkSupport(_place);
        }

        /// <summary>
        /// Pbg(¡)Ìì
        /// </summary>
        /// <param name="_supportNum">ìÔ(10ÌÊ:swè 1ÌÊ:ñwè)</param>
        void RocketLineSupportTrigger(int _supportNum)
        {
            SupportItemsMgr.RocketLineSupport(_supportNum);
        }

        /// <summary>
        /// Pbg(c)Ìì
        /// </summary>
        /// <param name="_supportNum">ìÔ(10ÌÊ:ñwè 1ÌÊ:swè)</param>
        void RocketColumnSupportTrigger(int _supportNum)
        {
            SupportItemsMgr.RocketColumnSupport(_supportNum);
        }

        /// <summary>
        /// SEÄ¶
        /// </summary>
        void SupportItemSePlay(SE_Type _seType)
        {
            switch (_seType)
            {
                //A±Ä¶
                case SE_Type.Duck:
                    SE_ContinuousPlay(_seType);
                    break;

                //P­Ä¶
                default:
                    SE_OneShot(_seType);
                    break;
            }
        }
    }
}
