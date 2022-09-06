using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static CommonDefine;
using static PuzzleDefine;
using static Sound.SoundManager;
using static animation.AnimationManager;
using static ObjectMove.ObjectMove_UI;

namespace Option
{
    public class OptionManager : MonoBehaviour
    {
        //オプションタイプ
        enum OptionType
        {
            Title,
            Puzzle,

            Count
        }
        OptionType mOptionType;

        //オプション状態
        enum OptionState
        {
            None,           //通常
            Credit,         //クレジット
            Confirm,        //確認ウィンドウ
            TutorialSel,    //チュートリアル(選択)
            TutorialView,   //チュートリアル(説明)

            Count           //総数
        }
        OptionState mState = OptionState.None;
        
        //確認タイプ
        enum ConfirmType
        {
            Redo,           //やり直す
            ReturnTitle,    //タイトルに戻る
            QiteGame,       //ゲームを終了する
        }
        ConfirmType mConfirmType;

        //確認文の設定
        const string CONFIRM_TEXT = "\nよろしいですか？";
        readonly Dictionary<ConfirmType, string> mConfirmTextDic = new Dictionary<ConfirmType, string>
        {
            { ConfirmType.Redo,         "このステージをやり直します" + CONFIRM_TEXT },
            { ConfirmType.ReturnTitle,  "タイトルに戻ります" + CONFIRM_TEXT },
            { ConfirmType.QiteGame,     "ゲームを終了します" + CONFIRM_TEXT }
        };

        [Header("オプション画面")]
        [SerializeField]
        GameObject[] mOptionObjArr = new GameObject[(int)OptionState.Count];

        [Header("BGM切替ボタンImage")]
        [SerializeField]
        Image mBgmBtnImg;

        [Header("SE切替ボタンImage")]
        [SerializeField]
        Image mSeBtnImg;

        [Header("確認ウィンドウテキスト")]
        [SerializeField]
        Text mConfirmText;

        [Header("切り替えボタンスプライト")]
        [SerializeField]
        Sprite[] mSwitchBtnSpr;

        //スプライトタイプ
        enum SwitchBtnType
        { On, Off }

        [Header("チュートリアルボタン格納オブジェクト")]
        [SerializeField]
        RectTransform mTutorialBtnParentTra;

        [Header("チュートリアル格納オブジェクト")]
        [SerializeField]
        RectTransform mTutorialScreenParentTra;
        readonly GameObject[] mTutorialObjArr = new GameObject[(int)TutorialType.Count];
        readonly RectTransform[] mTutorialPagesTraArr = new RectTransform[(int)TutorialType.Count];
        readonly int[] mTutorialMaxPageArr = new int[(int)TutorialType.Count];

        //チュートリアルタイプ
        public enum TutorialType
        {
            None = -1,   //データベース無し指定用

            //---共通--//

            GameDescription,    //ゲームの説明
            BasicOperation,     //基本操作
            SupportItem,        //援護アイテム

            //---ギミック--//

            Balloon,    //風船
            Jewelry,    //宝石
            Wall,       //石壁
            Flower,     //種→蕾→お花
            Frame,      //色枠
            Hamster,    //オラフ
            Cage,       //鉄格子
            NumberTag,  //番号札
            Thief,      //泥棒
            Steel,      //鋼
            Tornado,    //竜巻

            Count   //総数
        }
        TutorialType mTutorialType;

        //強制表示チュートリアル
        public static TutorialType ForcedTutorialType { get; set; }

        [Header("矢印オブジェクト")]
        [SerializeField]
        GameObject[] mArrowObjArr = new GameObject[(int)ArrowType.Count];

        //矢印タイプ
        enum ArrowType
        {
            SelRight,   //選択右
            SelLeft,    //選択左
            ViewRight,  //表示右
            ViewLeft,   //表示左

            Count
        }

        //表示中のぺージ
        int mTutorialSelectPage;    //チュートリアル選択
        int mTutorialViewPage;      //チュートリアル表示
        Coroutine mPageMoveCor;

        const int TUTORIAL_SELECT_MAX_PAGE_INDEX = 1;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            //タイプ設定
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                //タイトルシーン
                case TITLE_SCENE_NAME:
                    mOptionType = OptionType.Title;
                    break;

                //パズルシーン
                case PUZZLE_SCENE_NAME:
                    mOptionType = OptionType.Puzzle;

                    for (int i = 0; i < (int)TutorialType.Count; i++)
                    {
                        //チュートリアル画面取得
                        Transform tra = mTutorialScreenParentTra.GetChild(i);
                        mTutorialObjArr[i] = tra.gameObject;
                        Transform pagesTra = tra.GetChild(0);
                        mTutorialPagesTraArr[i] = pagesTra.GetComponent<RectTransform>();
                        mTutorialMaxPageArr[i] = pagesTra.childCount;

                        //チュートリアルボタン設定
                        int childIndex = (i <= (int)TutorialType.SupportItem || SaveDataManager.ViewedTutorialNum >= i) ? 0 : 1; //0:解放状態,1:未開放
                        GameObject btnObj = mTutorialBtnParentTra.GetChild(i).GetChild(childIndex).gameObject;
                        btnObj.SetActive(true);
                        TutorialType type = (TutorialType)Enum.ToObject(typeof(TutorialType), i);
                        btnObj.GetComponent<Button>().onClick.AddListener(() => IsPushTutorialSelect(type));
                    }
                    break;
            }

            //BGM・SE表示切替
            mBgmBtnImg.sprite = mSwitchBtnSpr[Preferences.Bgm ? (int)SwitchBtnType.On : (int)SwitchBtnType.Off];
            mSeBtnImg.sprite = mSwitchBtnSpr[Preferences.Se ? (int)SwitchBtnType.On : (int)SwitchBtnType.Off];
        }


        //===============================================//
        //---------------チュートリアル関係--------------//
        //===============================================//

        /// <summary>
        /// 強制チュートリアル
        /// </summary>
        public void ForcedTutorial()
        {
            if (ForcedTutorialType == TutorialType.None) return;
            if ((int)ForcedTutorialType <= SaveDataManager.ViewedTutorialNum) return;

            //強制チュートリアル中フラグセット
            FlagOn(PuzzleFlag.NowForcedTutorial);

            //フィルター表示
            SetFilter(true);

            //チュートリアル表示
            TutorialDisplay(ForcedTutorialType);
        }

        /// <summary>
        /// 強制チュートリアル終了
        /// </summary>
        void ForcedTutorialEnd()
        {
            //データ保存
            SaveDataManager.SetViewedTutorialNum((int)ForcedTutorialType);
            SaveDataManager.DataSave();

            //ゲーム説明の場合
            if (ForcedTutorialType == TutorialType.GameDescription)
            {
                //続けて基本操作を表示
                ForcedTutorialType = TutorialType.BasicOperation;
                TutorialDisplay(ForcedTutorialType);
            }
            //その他
            else
            {
                //フィルター非表示
                SetFilter(false);

                //オプション画面全非表示
                ObjInactive();

                //強制チュートリアル中フラグセット
                FlagOff(PuzzleFlag.NowForcedTutorial);
            }
        }

        /// <summary>
        /// チュートリアル表示
        /// </summary>
        /// <param name="_type">チュートリアルタイプ</param>
        void TutorialDisplay(TutorialType _type)
        {
            SetTutorialView(_type);
            StartCoroutine(ObjectAppearance(OptionState.TutorialView));

            //ページ初期化
            mTutorialViewPage = 0;
            TutorialViewPageChange();
        }


        //==========================================================//
        //-----------------------ボタン判定-------------------------//
        //==========================================================//

        /// <summary>
        /// オプション
        /// </summary>
        public void IsPushOption()
        {
            //パズルシーン
            if (mOptionType == OptionType.Puzzle)
            {
                //操作禁止
                if (!IsOperable()) return;

                //オプション表示フラグセット
                FlagOn(PuzzleFlag.NowOptionView);
            }

            //フィルター表示
            SetFilter(true);

            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.None));
        }

        /// <summary>
        /// BGM_ON・OFF
        /// </summary>
        public void IsPushBGM()
        {
            //BGM切替
            Preferences.Bgm = !Preferences.Bgm;
            if (Preferences.Bgm)
            {
                //BGM開始
                SE_OneShot(SE_Type.BtnYes);
                BGM_FadeRestart();
                mBgmBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.On];
            }
            else
            {
                //BGMストップ
                SE_OneShot(SE_Type.BtnNo);
                BGM_Stop();
                mBgmBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.Off];
            }
        }

        /// <summary>
        /// SE_ON・OFF
        /// </summary>
        public void IsPushSE()
        {
            //SE切替
            Preferences.Se = !Preferences.Se;
            if (Preferences.Se)
            {
                //ON
                mSeBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.On];
                SE_OneShot(SE_Type.BtnYes);
            }
            else
            {
                //OFF
                mSeBtnImg.sprite = mSwitchBtnSpr[(int)SwitchBtnType.Off];
                SE_StopAll();
            }
        }

        /// <summary>
        /// クレジット
        /// </summary>
        public void IsPushCredit()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.Credit));
        }

        /// <summary>
        /// ゲームをやめる
        /// </summary>
        public void IsPushQuitGame()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            SetConfirmText(ConfirmType.QiteGame);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// はい
        /// </summary>
        public void IsPushYes()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            
            //シーン移管
            switch (mConfirmType)
            {
                //やり直し
                case ConfirmType.Redo:
                    StartCoroutine(BGM_FadeStop()); //BGMフェードアウト
                    SceneFader.SceneChangeFade(PUZZLE_SCENE_NAME);
                    break;

                //タイトルへ戻る
                case ConfirmType.ReturnTitle:
                    StartCoroutine(BGM_FadeStop()); //BGMフェードアウト
                    SceneFader.SceneChangeFade(TITLE_SCENE_NAME);
                    break;

                //ゲーム終了
                case ConfirmType.QiteGame:
                    GameManager.QuitGame();
                    break;
            }
        }

        /// <summary>
        /// いいえ
        /// </summary>
        public void IsPushNo()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnNo);
            StartCoroutine(ObjectAppearance(OptionState.None));
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void IsPushClose()
        {
            switch (mState)
            {
                //通常表示
                case OptionState.None:
                    OptionEnd();
                    break;

                //クレジット表示,チュートリアル(選択)
                case OptionState.Credit:
                case OptionState.TutorialSel:
                    StartCoroutine(ObjectAppearance(OptionState.None)); //通常状態へ
                    break;

                //チュートリアル(説明)
                case OptionState.TutorialView:
                    if (GetFlag(PuzzleFlag.NowForcedTutorial)) ForcedTutorialEnd();     //強制チュートリアル終了
                    else  StartCoroutine(ObjectAppearance(OptionState.TutorialSel));    //チュートリアル(選択)へ
                    break;

                //入ることはないはずだが念のため
                default: return;
            }

            //SE再生
            SE_OneShot(SE_Type.BtnNo);
        }

        /// <summary>
        /// チュートリアル
        /// </summary>
        public void IsPushTutorial()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.TutorialSel));

            //ページ初期化
            mTutorialSelectPage = 0;
            TutorialSelectPageChange();
        }

        /// <summary>
        /// やり直す
        /// </summary>
        public void IsPushRedo()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            SetConfirmText(ConfirmType.Redo);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// タイトルに戻る
        /// </summary>
        public void IsPushReturnTitle()
        {
            //SE再生
            SE_OneShot(SE_Type.BtnYes);
            SetConfirmText(ConfirmType.ReturnTitle);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// チュートリアル選択
        /// </summary>
        void IsPushTutorialSelect(TutorialType _type)
        {
            //未開放
            if (SaveDataManager.ViewedTutorialNum < (int)_type)
            {
                //SE再生
                SE_OneShot(SE_Type.BtnNo);
            }
            //解放済
            else
            {
                //SE再生
                SE_OneShot(SE_Type.BtnYes);
                TutorialDisplay(_type);
            }
        }

        /// <summary>
        /// 右矢印
        /// </summary>
        public void IsPushRightArrow()
        {
            switch (mState)
            {
                //チュートリアル選択画面
                case OptionState.TutorialSel:
                    mTutorialSelectPage++;
                    TutorialSelectPageChange();
                    break;

                //チュートリアル表示画面
                case OptionState.TutorialView:
                    mTutorialViewPage++;
                    TutorialViewPageChange();
                    break;
            }

            //SE再生
            SE_OneShot(SE_Type.BtnYes);
        }

        /// <summary>
        /// 左矢印
        /// </summary>
        public void IsPushLeftArrow()
        {
            switch (mState)
            {
                //チュートリアル選択画面
                case OptionState.TutorialSel:
                    mTutorialSelectPage--;
                    TutorialSelectPageChange();
                    break;

                //チュートリアル表示画面
                case OptionState.TutorialView:
                    mTutorialViewPage--;
                    TutorialViewPageChange();
                    break;
            }

            //SE再生
            SE_OneShot(SE_Type.BtnYes);
        }

        //==================================================//
        //---------------------表示関係---------------------//
        //==================================================//

        /// <summary>
        /// 指定画面の出力
        /// </summary>
        /// <param name="_anime">出現アニメの有無</param>
        IEnumerator ObjectAppearance(OptionState _state, bool _anime = true)
        {
            //状態設定
            mState = _state;

            //画面アクティブ状態切替
            ObjInactive();
            mOptionObjArr[(int)mState].SetActive(true);

            //出現アニメ再生
            if (_anime)
            {
                Animator ani = mOptionObjArr[(int)mState].GetComponent<Animator>();
                yield return StartCoroutine(AnimationStart(ani, STATE_NAME_APPEARANCE));
            }
        }

        /// <summary>z
        /// 全画面非アクティブ
        /// </summary>
        void ObjInactive()
        {
            foreach (GameObject obj in mOptionObjArr)
            {
                if (obj == null) continue;
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// 確認ウィンドウテキストの設定
        /// </summary>
        void SetConfirmText(ConfirmType _conType)
        {
            mConfirmType = _conType;
            mConfirmText.text = mConfirmTextDic[_conType];
        }

        /// <summary>
        /// チュートリアル表示の切り替え
        /// </summary>
        void SetTutorialView(TutorialType _tutType)
        {
            mTutorialObjArr[(int)mTutorialType].SetActive(false);
            mTutorialObjArr[(int)_tutType].SetActive(true);
            mTutorialType = _tutType;
        }

        /// <summary>
        /// フィルター切替
        /// </summary>
        void SetFilter(bool on)
        {
            switch (mOptionType)
            {
                //タイトルシーン
                case OptionType.Title:
                    StartCoroutine(Title.TitleMain.CanvasMgr.SetFilter(on));
                    break;

                //パズルシーン
                case OptionType.Puzzle:
                    StartCoroutine(PuzzleMain.PuzzleMain.CanvasMgr.SetFilter(on));
                    break;
            }
        }

        /// <summary>
        /// チュートリアル選択ページ変更
        /// </summary>
        void TutorialSelectPageChange()
        {
            mTutorialSelectPage = Mathf.Clamp(mTutorialSelectPage, 0, TUTORIAL_SELECT_MAX_PAGE_INDEX);

            //ページ移動
            Vector2 targetPos = new Vector2(mTutorialSelectPage * -PLAY_SCREEN_WIDTH, 0.0f);
            if (mPageMoveCor != null) StopCoroutine(mPageMoveCor);
            mPageMoveCor = StartCoroutine(DecelerationMovement(mTutorialBtnParentTra, PAGE_MOVE_SPEED, targetPos));

            //矢印表示状態変更
            mArrowObjArr[(int)ArrowType.SelRight].SetActive(mTutorialSelectPage < TUTORIAL_SELECT_MAX_PAGE_INDEX);
            mArrowObjArr[(int)ArrowType.SelLeft].SetActive(mTutorialSelectPage > 0);
        }

        /// <summary>
        /// チュートリアル表示ページ変更
        /// </summary>
        void TutorialViewPageChange()
        {
            mTutorialViewPage = Mathf.Clamp(mTutorialViewPage, 0, mTutorialMaxPageArr[(int)mTutorialType] - 1);

            //ページ移動
            Vector2 targetPos = new Vector2(mTutorialViewPage * -PLAY_SCREEN_WIDTH, 0.0f);
            if (mPageMoveCor != null) StopCoroutine(mPageMoveCor);
            mPageMoveCor = StartCoroutine(DecelerationMovement(mTutorialPagesTraArr[(int)mTutorialType], PAGE_MOVE_SPEED, targetPos));

            //矢印表示状態変更
            mArrowObjArr[(int)ArrowType.ViewRight].SetActive(mTutorialViewPage < mTutorialMaxPageArr[(int)mTutorialType] - 1);
            mArrowObjArr[(int)ArrowType.ViewLeft].SetActive(mTutorialViewPage > 0);
        }

        /// <summary>
        /// オプション終了
        /// </summary>
        void OptionEnd()
        {
            //フィルター解除
            SetFilter(false);

            //オプション画面全非表示
            ObjInactive();

            //パズルシーンの場合はオプション表示フラグリセット
            if (mOptionType == OptionType.Puzzle)
                FlagOff(PuzzleFlag.NowOptionView);
        }
    }
}