using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Title
{
    public class TitleMain : MonoBehaviour
    {
        public enum TitleState
        {
            None,           //タイトル画面
            StageSelect,    //ステージ選択画面
            Option,         //オプション画面
        }
        public static TitleState sTitleState = TitleState.None;

        [Header("TitleManager")]
        [SerializeField]
        TitleManager mTitleManager;
        public static TitleManager TitleMgr { get; private set; }

        void Start()
        {
            TitleMgr = mTitleManager;
            TitleMgr.Initialize();
        }
    }
}
