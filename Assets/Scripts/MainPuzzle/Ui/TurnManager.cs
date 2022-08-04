using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain.Ui
{
    public class TurnManager : MonoBehaviour
    {
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager
        TargetManager   mTargetMgr;     //TargetManager

        [Header("ターン数ウィンドウ(通常)のSprite取得")]
        [SerializeField]
        Sprite mTurnWindowNormalSpr;

        [Header("ターン数ウィンドウ(赤)のSprite取得")]
        [SerializeField]
        Sprite mTurnWindowRedSpr;

        [Header("ターン数ウィンドウImageの取得")]
        [SerializeField]
        Image mTurnWindowImg;

        [Header("ターン数Textの取得")]
        [SerializeField]
        Text mNumberText;

        const int RED_LINE = 5;     //ウィンドウを切り替えるターン
        const int TURN_MAX = 99;    //ターン最大数
        readonly WaitForSeconds RECOVERY_DELAY_TIME = new WaitForSeconds(0.3f);   //ターン回復の間隔

        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        /// <summary>
        /// TurnManagerの初期化
        /// </summary>
        public void Initialize()
        {
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();
            mTargetMgr   = sPuzzleMain.GetTargetManager();

            if (TURN_MAX < TURN_COUNT) TURN_COUNT = TURN_MAX;
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// ターン減少
        /// </summary>
        public void TurnDecrease()
        {
            TURN_COUNT--;
            if (0 > TURN_COUNT) TURN_COUNT = 0;
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// ターン回復(リワード広告視聴)
        /// </summary>
        public IEnumerator TurnRecovery_AdReward()
        {
            //フィルター解除
            sPuzzleMain.GetCanvasManager().SetFilter(false);

            //回復
            for (int i = 0; i < REWARD_RECOVERY_COUNT; i++)
            {
                TURN_COUNT++;
                if (TURN_MAX < TURN_COUNT) TURN_COUNT = TURN_MAX;
                mNumberText.text = TURN_COUNT.ToString();
                if (TURN_COUNT > RED_LINE) WindowChange(false);
                yield return RECOVERY_DELAY_TIME;
            }
        }
        const int REWARD_RECOVERY_COUNT = 5;

        /// <summary>
        /// ウィンドウSprite差し替え
        /// </summary>
        /// <param name="_red"></param>
        void WindowChange(bool _red)
        {
            mTurnWindowImg.sprite = (_red) ? mTurnWindowRedSpr : mTurnWindowNormalSpr;
        }


        //==========================================================//
        //-------------------ターン終了時の処理---------------------//
        //==========================================================//

        /// <summary>
        /// ターン数確認
        /// </summary>
        public void TurnCountCheck()
        {
            if (TURN_COUNT <= 0)
            {
                sPuzzleMain.GameOver();
            }
        }

        /// <summary>
        /// ターン終了
        /// </summary>
        /// <param name="supportItem">援護アイテム？</param>
        public IEnumerator TurnEnd(bool supportItem = false)
        {
            //ターン終了処理中フラグセット
            NOW_TURN_END_PROCESSING = true;

            //特定ギミック破壊判定開始
            yield return StartCoroutine(mGimmicksMgr.DestroyGimmicks_TurnEnd());

            //自由落下
            yield return StartCoroutine(mPiecesMgr.StratFallingPieces());

            //援護アイテム未使用時
            if (!supportItem)
            {
                //ギミック状態変化開始
                yield return StartCoroutine(mGimmicksMgr.ChangeGimmickState());
            }

            //特定ギミック破壊判定開始
            yield return StartCoroutine(mGimmicksMgr.DestroyGimmicks_TurnEnd());

            //ギミックターン終了処理
            mGimmicksMgr.ResetTurnInfo();

            //目標残数確認
            mTargetMgr.TargetCheck();

            //ターン数確認
            if (!GAME_CLEAR) TurnCountCheck();

            //ターン終了処理中フラグリセット
            NOW_TURN_END_PROCESSING = false;
        }
    }
}