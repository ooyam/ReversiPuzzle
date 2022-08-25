using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Preferences;
using static CommonDefine;
using static Title.TitleMain;
using static Sound.SoundManager;
using static animation.AnimationManager;

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
        OptionType mType;

        //オプション状態
        enum OptionState
        {
            None,       //通常
            Credit,     //クレジット
            Confirm,    //確認ウィンドウ

            Count       //総数
        }
        OptionState mState = OptionState.None;

        [Header("オプション画面")]
        [SerializeField]
        GameObject[] mOptionObjArr;  //0:通常, 1:クレジット, 2:確認ウィンドウ

        [Header("BGM切替ボタンImage")]
        [SerializeField]
        Image mBgmBtnImg;

        [Header("SE切替ボタンImage")]
        [SerializeField]
        Image mSeBtnImg;

        [Header("切り替えボタンスプライト")]
        [SerializeField]
        Sprite[] mSwitchBtnSpr;

        //スプライトタイプ
        enum SwitchBtnType
        { On, Off }


        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            //タイプ設定
            switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                case TITLE_SCENE_NAME:  mType = OptionType.Title;  break;
                case PUZZLE_SCENE_NAME: mType = OptionType.Puzzle; break;
            }
        }


        //==========================================================//
        //-----------------------ボタン判定-------------------------//
        //==========================================================//

        /// <summary>
        /// オプション
        /// </summary>
        public void IsPushOption()
        {
            //フィルター表示
            StartCoroutine(CanvasMgr.SetFilter(true));

            //SE再生
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.None));
        }

        /// <summary>
        /// BGM_ON・OFF
        /// </summary>
        public void IsPushBGM()
        {
            //BGM切替
            Bgm = !Bgm;
            mBgmBtnImg.sprite = Bgm ? mSwitchBtnSpr[(int)SwitchBtnType.On] : mSwitchBtnSpr[(int)SwitchBtnType.Off];

            //切替SE
            SE_Onshot(Bgm ? SE_Type.BtnYes : SE_Type.BtnNo);
        }

        /// <summary>
        /// SE_ON・OFF
        /// </summary>
        public void IsPushSE()
        {
            //SE切替
            Se = !Se;
            mSeBtnImg.sprite = Se ? mSwitchBtnSpr[(int)SwitchBtnType.On] : mSwitchBtnSpr[(int)SwitchBtnType.Off];

            //切替SE
            if (Se) SE_Onshot(SE_Type.BtnYes);
        }

        /// <summary>
        /// クレジット
        /// </summary>
        public void IsPushCredit()
        {
            //SE再生
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.Credit));
        }

        /// <summary>
        /// ゲームをやめる
        /// </summary>
        public void IsPushQuitGame()
        {
            //SE再生
            SE_Onshot(SE_Type.BtnYes);
            StartCoroutine(ObjectAppearance(OptionState.Confirm, false));
        }

        /// <summary>
        /// はい
        /// </summary>
        public void IsPushYes()
        {
            //ゲーム終了
            SE_Onshot(SE_Type.BtnYes);
            GameManager.QuitGame();
        }

        /// <summary>
        /// いいえ
        /// </summary>
        public void IsPushNo()
        {
            //SE再生
            SE_Onshot(SE_Type.BtnYes);
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
                    StartCoroutine(CanvasMgr.SetFilter(false)); //フィルター解除
                    ObjInactive();  //オプション終了
                    break;

                //クレジット表示
                case OptionState.Credit:
                    StartCoroutine(ObjectAppearance(OptionState.None)); //通常状態へ
                    break;

                //入ることはないはず
                default: return;
            }

            //SE再生
            SE_Onshot(SE_Type.BtnYes);
        }


        //==========================================================//
        //---------------------オブジェクト生成---------------------//
        //==========================================================//

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
            { obj.SetActive(false); }
        }
    }
}