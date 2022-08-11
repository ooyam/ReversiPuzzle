using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveDataManager;

namespace Title
{
    public class TitleMain : MonoBehaviour
    {
        //タイトル画面表示状態
        public enum TitleState
        {
            None,           //タイトル画面
            StageSelect,    //ステージ選択画面
            Option,         //オプション画面
        }

        [Header("TitleManager")]
        [SerializeField]
        TitleManager mTitleManager;

        //----スタティック変数----//
        public static TitleManager TitleMgr { get; private set; }
        public static TitleState sTitleState;
        //-----------------------//

        /// <summary>
        /// タイトル開始
        /// </summary>
        void Awake()
        {
            //セーブデータ読み込み
            DataLoad();
            sTitleState = TitleState.None;
            TitleMgr = mTitleManager;

            //タイトル初期化
            TitleMgr.Initialize();
        }
    }
}
