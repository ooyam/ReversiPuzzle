using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ui;
using Option;

namespace Title
{
    public class TitleMain : MonoBehaviour
    {
        //タイトル画面表示状態
        public enum TitleState
        {
            None,           //タイトル画面
            StageSelect,    //ステージ選択画面
        }

        [Header("TitleManager")]
        [SerializeField]
        TitleManager mTitleManager;

        [Header("OptionManager")]
        [SerializeField]
        OptionManager mOptionManager;

        [Header("CanvasManager")]
        [SerializeField]
        CanvasManager mCanvasManager;

        //----スタティック変数----//
        public static TitleManager  TitleMgr    { get; private set; }
        public static OptionManager OptionMgr   { get; private set; }
        public static CanvasManager CanvasMgr   { get; private set; }
        public static StagesData AllStagesData  { get; private set; }
        public static TitleState sTitleState;
        //-----------------------//

        /// <summary>
        /// タイトル開始
        /// </summary>
        void Awake()
        {
            //セーブデータ読み込み
            SaveDataManager.DataLoad();
            sTitleState = TitleState.None;
            TitleMgr  = mTitleManager;
            OptionMgr = mOptionManager;
            CanvasMgr = mCanvasManager;

            //タイトル初期化
            TitleMgr.Initialize();

            //オプション初期化
            OptionMgr.Initialize();

            //ステージデータ取得
            AllStagesData = Resources.Load("StagesData") as StagesData;
        }
    }
}
