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

        [Header("ƒ^[ƒ“”Text‚Ìæ“¾")]
        [SerializeField]
        Text mNumberText;

        //==========================================================//
        //----------------------‰Šúİ’è,æ“¾-----------------------//
        //==========================================================//

        /// <summary>
        /// TurnManager‚Ì‰Šú‰»
        /// </summary>
        public void Initialize()
        {
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();
        }
    }
}