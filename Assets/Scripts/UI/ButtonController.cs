using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Title.TitleMain;

namespace Ui
{
    public class ButtonController : MonoBehaviour
    {
        //==========================================================//
        //-----------------------タイトル画面-----------------------//
        //==========================================================//

        /// <summary>
        /// スタート
        /// </summary>
        public void IsPushStart_Title()
        {
            TitleMgr.IsPushStart();
        }

        /// <summary>
        /// もどる
        /// </summary>
        public void IsPushBack_Title()
        {
            TitleMgr.IsPushBack();
        }

        /// <summary>
        /// 右矢印
        /// </summary>
        public void IsPushRightArrow_Title()
        {
            TitleMgr.IsPushRightArrow();
        }

        /// <summary>
        /// 左矢印
        /// </summary>
        public void IsPushLeftArrow_Title()
        {
            TitleMgr.IsPushLeftArrow();
        }

        
        //==========================================================//
        //---------------------ゲームオーバー画面-------------------//
        //==========================================================//

        /// <summary>
        /// 広告を見る
        /// </summary>
        public void IsPushSeeAbs_GameOver()
        {
            ResultMgr.IsPushSeeAbs();
        }

        /// <summary>
        /// あきらめる
        /// </summary>
        public void IsPushGiveUp_GameOver()
        {
            ResultMgr.IsPushGiveUp();
        }

        /// <summary>
        /// はい
        /// </summary>
        public void IsPushYes_GameOver()
        {
            ResultMgr.IsPushYes();
        }

        /// <summary>
        /// いいえ
        /// </summary>
        public void IsPushNo_GameOver()
        {
            ResultMgr.IsPushNo();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void IsPushClose_GameOver()
        {
            ResultMgr.IsPushClose();
        }

        /// <summary>
        /// 再挑戦する
        /// </summary>
        public void IsPushTryAgain_GameOver()
        {
            ResultMgr.IsPushTryAgain();
        }

        /// <summary>
        /// タイトルに戻る
        /// </summary>
        public void IsPushReturnTitle_GameOver()
        {
            ResultMgr.IsPushReturnTitle();
        }
    }
}