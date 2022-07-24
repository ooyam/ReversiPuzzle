using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

namespace PuzzleMain
{
    public class TargetManager : MonoBehaviour
    {
        PiecesManager   mPiecesMgr;     //PiecesManager
        GimmicksManager mGimmicksMgr;   //GimmicksManager

        [Header("目標表示ボックスの取得")]
        [SerializeField]
        RectTransform mTexturesBoxTra;

        [Header("目標表示用ギミックプレハブの取得")]
        public TargetGimmicksArr[] mTargetGimmicksPrefabArr;
        [Serializable]
        public class TargetGimmicksArr
        { public GameObject[] prefab; }

        [Header("目標表示用駒プレハブの取得")]
        public TargetPiecesArr[] mTargetPiecesPrefabArr;
        [Serializable]
        public class TargetPiecesArr
        { public GameObject[] prefab; }

        RectTransform[] mTargetTraArr;
        Text[] mCountTextsArr;  //残り数量の表示Text

        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        /// <summary>
        /// TargetManagerの初期化
        /// </summary>
        public void Initialize()
        {
            mPiecesMgr   = sPuzzleMain.GetPiecesManager();
            mGimmicksMgr = sPuzzleMain.GetGimmicksManager();

            //目標の表示
            mTargetTraArr = new RectTransform[TARGETS_COUNT];
            mCountTextsArr = new Text[TARGETS_COUNT];
            for (int i = 0; i < TARGETS_COUNT; i++)
            {
                int colorId = (TARGETS_INFO_ARR[i][TARGET_INFO_COLOR] < 0) ? 0 : TARGETS_INFO_ARR[i][TARGET_INFO_COLOR];
                GameObject obj = Instantiate(mTargetGimmicksPrefabArr[TARGETS_INFO_ARR[i][TARGET_INFO_OBJ]].prefab[colorId]);
                mTargetTraArr[i] = obj.GetComponent<RectTransform>();
                mCountTextsArr[i] = mTargetTraArr[i].GetChild(0).GetComponent<Text>();
            }

        }
    }
}
