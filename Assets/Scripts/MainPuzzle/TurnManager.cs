using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class TurnManager : MonoBehaviour
    {
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager

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

        const int RED_LINE = 5; //ウィンドウを切り替えるターン
        readonly WaitForSeconds RECOVERY_DELAY_TIME = new WaitForSeconds(0.2f);   //ターン回復の間隔

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
            mNumberText.text = TURN_COUNT.ToString();
        }

        /// <summary>
        /// ターン減少
        /// </summary>
        public void TurnDecrease()
        {
            TURN_COUNT--;
            mNumberText.text = TURN_COUNT.ToString();
            if (TURN_COUNT <= RED_LINE) WindowChange(true);
        }

        /// <summary>
        /// ターン回復
        /// </summary>
        /// <param name="_count">回復数</param>
        public IEnumerator TurnRecovery(int _count = 1)
        {
            for (int i = 0; i < _count; i++)
            {
                TURN_COUNT++;
                mNumberText.text = TURN_COUNT.ToString();
                if (TURN_COUNT > RED_LINE) WindowChange(false);
                yield return RECOVERY_DELAY_TIME;
            }
        }

        /// <summary>
        /// ウィンドウSprite差し替え
        /// </summary>
        /// <param name="_red"></param>
        void WindowChange(bool _red)
        {
            mTurnWindowImg.sprite = (_red) ? mTurnWindowRedSpr : mTurnWindowNormalSpr;
        }
    }
}