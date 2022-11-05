using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleMain.PuzzleMain;
using static Title.TitleMain;
using static CommonDefine;

namespace Ui
{
    public class ButtonController : MonoBehaviour
    {
        Option.OptionManager GetOptionMgr()
        {
            return GameManager.sSceneType switch
            {
                GameManager.SceneType.Title  => Title.TitleMain.OptionMgr,
                GameManager.SceneType.Puzzle => PuzzleMain.PuzzleMain.OptionMgr,
                _ => null,
            };
        }

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

        public void IsPushOption_Option()      => GetOptionMgr().IsPushOption();        //オプション
        public void IsPushBGM_Option()         => GetOptionMgr().IsPushBGM();           //BGM_ON・OFF
        public void IsPushSE_Option()          => GetOptionMgr().IsPushSE();            //SE_ON・OFF
        public void IsPushCredit_Option()      => GetOptionMgr().IsPushCredit();        //クレジット
        public void IsPushQuitGame_Option()    => GetOptionMgr().IsPushQuitGame();      //ゲームをやめる
        public void IsPushYes_Option()         => GetOptionMgr().IsPushYes();           //はい
        public void IsPushNo_Option()          => GetOptionMgr().IsPushNo();            //いいえ
        public void IsPushClose_Option()       => GetOptionMgr().IsPushClose();         //閉じる
        public void IsPushNext_Option()        => GetOptionMgr().IsPushNext();          //次へ
        public void IsPushTutorial_Option()    => GetOptionMgr().IsPushTutorial();      //チュートリアル
        public void IsPushRedo_Option()        => GetOptionMgr().IsPushRedo();          //やり直す
        public void IsPushReturnTitle_Option() => GetOptionMgr().IsPushReturnTitle();   //タイトルに戻る
        public void IsPushRightArrow_Option()  => GetOptionMgr().IsPushRightArrow();    //右矢印
        public void IsPushLeftArrow_Option()   => GetOptionMgr().IsPushLeftArrow();     //左矢印


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