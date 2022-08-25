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

        public void IsPushStart_Title()      => TitleMgr.IsPushStart();         //スタート
        public void IsPushBack_Title()       => TitleMgr.IsPushBack();          //もどる
        public void IsPushRightArrow_Title() => TitleMgr.IsPushRightArrow();    //右矢印
        public void IsPushLeftArrow_Title()  => TitleMgr.IsPushLeftArrow();     //左矢印


        //==========================================================//
        //---------------------オプション画面-----------------------//
        //==========================================================//

        public void IsPushOption_Option()   => OptionMgr.IsPushOption();    //オプション
        public void IsPushBGM_Option()      => OptionMgr.IsPushBGM();       //BGM_ON・OFF
        public void IsPushSE_Option()       => OptionMgr.IsPushSE();        //SE_ON・OFF
        public void IsPushCredit_Option()   => OptionMgr.IsPushCredit();    //クレジット
        public void IsPushQuitGame_Option() => OptionMgr.IsPushQuitGame();  //ゲームをやめる
        public void IsPushYes_Option()      => OptionMgr.IsPushYes();       //はい
        public void IsPushNo_Option()       => OptionMgr.IsPushNo();        //いいえ
        public void IsPushClose_Option()    => OptionMgr.IsPushClose();     //閉じる


        //==========================================================//
        //---------------------ゲームオーバー画面-------------------//
        //==========================================================//

        public void IsPushSeeAbs_GameOver()      => ResultMgr.IsPushSeeAbs();       //広告を見る
        public void IsPushGiveUp_GameOver()      => ResultMgr.IsPushGiveUp();       //あきらめる
        public void IsPushYes_GameOver()         => ResultMgr.IsPushYes();          //はい
        public void IsPushNo_GameOver()          => ResultMgr.IsPushNo();           //いいえ
        public void IsPushClose_GameOver()       => ResultMgr.IsPushClose();        //閉じる
        public void IsPushTryAgain_GameOver()    => ResultMgr.IsPushTryAgain();     //再挑戦する
        public void IsPushReturnTitle_GameOver() => ResultMgr.IsPushReturnTitle();  //タイトルに戻る
    }
}