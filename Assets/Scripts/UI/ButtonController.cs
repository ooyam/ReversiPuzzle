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
        Option.OptionManager mOptionMgr;
        private void Start()
        {
            //シーン取得
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case TITLE_SCENE_NAME:  mOptionMgr = Title.TitleMain.OptionMgr; break;
                case PUZZLE_SCENE_NAME: mOptionMgr = PuzzleMain.PuzzleMain.OptionMgr; break;
            }
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

        public void IsPushOption_Option()           => mOptionMgr.IsPushOption();           //オプション
        public void IsPushBGM_Option()              => mOptionMgr.IsPushBGM();              //BGM_ON・OFF
        public void IsPushSE_Option()               => mOptionMgr.IsPushSE();               //SE_ON・OFF
        public void IsPushCredit_Option()           => mOptionMgr.IsPushCredit();           //クレジット
        public void IsPushQuitGame_Option()         => mOptionMgr.IsPushQuitGame();         //ゲームをやめる
        public void IsPushYes_Option()              => mOptionMgr.IsPushYes();              //はい
        public void IsPushNo_Option()               => mOptionMgr.IsPushNo();               //いいえ
        public void IsPushClose_Option()            => mOptionMgr.IsPushClose();            //閉じる
        public void IsPushTutorial_Option()         => mOptionMgr.IsPushTutorial();         //チュートリアル
        public void IsPushRedo_Option()             => mOptionMgr.IsPushRedo();             //やり直す
        public void IsPushReturnTitle_Option()      => mOptionMgr.IsPushReturnTitle();      //タイトルに戻る
        public void IsPushRightArrow_Option()       => mOptionMgr.IsPushRightArrow();       //右矢印
        public void IsPushLeftArrow_Option()        => mOptionMgr.IsPushLeftArrow();        //左矢印


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