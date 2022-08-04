using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;

namespace Ui
{
    public class ButtonController : MonoBehaviour
    {
        //==========================================================//
        //---------------------ゲームオーバー画面-------------------//
        //==========================================================//

        /// <summary>
        /// 広告を見る
        /// </summary>
        public void IsPushSeeAbs_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushSeeAbs();
        }

        /// <summary>
        /// あきらめる
        /// </summary>
        public void IsPushGiveUp_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushGiveUp();
        }

        /// <summary>
        /// はい
        /// </summary>
        public void IsPushYes_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushYes();
        }

        /// <summary>
        /// いいえ
        /// </summary>
        public void IsPushNo_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushNo();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void IsPushClose_GameOver()
        {
            sPuzzleMain.GetResultManager().IsPushClose();
        }
    }
}