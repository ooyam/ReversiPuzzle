using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class TurnManager : MonoBehaviour
    {
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager

        [Header("ターン数Textの取得")]
        [SerializeField]
        Text mNumberText;

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
        }
    }
}