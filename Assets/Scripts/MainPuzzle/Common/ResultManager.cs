using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;
using static Sound.SoundManager;

namespace PuzzleMain
{
    public class ResultManager : MonoBehaviour
    {
        [Header("ゲームクリアプレハブ")]
        [SerializeField]
        GameObject mGameClearPre;

        [Header("ゲームオーバープレハブ")]
        [SerializeField]
        GameObject[] mGameOverPreArr;
        enum GameOverObjType
        { WithRecovery, NoRecovery, Count }   //0:ターン回復有, 1:ボタン無し

        [Header("確認ウィンドウプレハブ")]
        [SerializeField]
        GameObject mConfirmWinPre;

        [Header("リワード広告プレハブ")]
        [SerializeField]
        GameObject mAdRewardPre;

        [Header("インタースティシャル広告")]
        [SerializeField]
        GoogleAdmobInterstitial mAdInterstitial;

        [Header("メッセージウィンドウ(ボタン無し)プレハブ")]
        [SerializeField]
        GameObject mMessageWinNoBtnPre;

        [Header("メッセージウィンドウプレハブ")]
        [SerializeField]
        GameObject mMessageWinPre;

        [Header("Resultオブジェクト格納場所")]
        [SerializeField]
        RectTransform mResultTra;

        GameObject mDisplayObj;         //表示中オブジェクト
        GoogleAdmobReward mAdReward;    //リワード広告クラス
        Coroutine mAppearanceCor;       //出現アニメーションコルーチン

        //==========================================================//
        //------------------------リザルト画面----------------------//
        //==========================================================//

        /// <summary>
        /// 確認ウィンドウ表示状態
        /// </summary>
        public enum ConfirmWinStatus
        {
            TryAgain,       //再挑戦する？
            ReturnTitle     //タイトルに戻る？
        }
        [HideInInspector]
        public ConfirmWinStatus mConfirmWinState = ConfirmWinStatus.TryAgain;

        /// <summary>
        /// ゲームクリア
        /// </summary>
        public IEnumerator GenerateGameClearObj()
        {
            ObjectDestroy();
            mDisplayObj = Instantiate(mGameClearPre);
            yield return StartCoroutine(ObjectAppearance());
            yield return new WaitForSeconds(3.0f);

            //インタースティシャル広告表示
            StartCoroutine(CanvasMgr.SetFilter(true));
            yield return StartCoroutine(ShowAdInterstitial());

            //タイトルへ移管
            ReturnTitle();
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        public IEnumerator GenerateGameOverObj()
        {
            StartCoroutine(CanvasMgr.SetFilter(true));
            ObjectDestroy();

            //継続不可能の場合
#if UNITY_ANDROID
            mDisplayObj = Instantiate(mGameOverPreArr[
                !GetFlag(PuzzleFlag.Uncontinuable) && !GetFlag(PuzzleFlag.TurnRecovered) ?
                (int)GameOverObjType.WithRecovery : (int)GameOverObjType.NoRecovery]);
#else
            mDisplayObj = Instantiate(mGameOverPreArr[(int)GameOverObjType.NoRecovery]);
#endif
            yield return StartCoroutine(ObjectAppearance());
        }

        /// <summary>
        /// 確認ウィンドウ
        /// </summary>
        public void GenerateConfirmObj()
        {
            //確認内容設定
            string msg = "";
            switch (mConfirmWinState)
            {
                case ConfirmWinStatus.TryAgain:
                    msg = "もういちど挑戦しますか？";
                    break;
                case ConfirmWinStatus.ReturnTitle:
                    msg = "タイトル画面に戻ります\nよろしいですか？";
                    break;
            }

            //メンバ変数の参照が存在する(オブジェクトがある)場合は改め生成しない
            if (!mConfirmMsgText)
            {
                //ウィンドウ生成
                ObjectDestroy();
                mDisplayObj = Instantiate(mConfirmWinPre);
                mConfirmMsgText = mDisplayObj.transform.GetChild(0).GetChild(0).GetComponent<Text>();
                StartCoroutine(ObjectAppearance(false));
            }

            //テキスト反映
            mConfirmMsgText.text = msg;
        }
        Text mConfirmMsgText;   //確認ウィンドウメッセージテキスト

        /// <summary>
        /// オブジェクト出現
        /// </summary>
        /// <param name="_anime">出現アニメの有無</param>
        IEnumerator ObjectAppearance(bool _anime = true)
        {
            RectTransform tra = mDisplayObj.GetComponent<RectTransform>();
            tra.SetParent(mResultTra);
            tra.localPosition = Vector3.zero;
            tra.localScale = Vector3.one;
            if (_anime)
            {
                Animator ani = mDisplayObj.GetComponent<Animator>();
                mAppearanceCor = StartCoroutine(AnimationStart(ani, STATE_NAME_APPEARANCE));
                yield return mAppearanceCor;
            }
        }

        /// <summary>
        /// オブジェクト削除
        /// </summary>
        void ObjectDestroy()
        {
            if (mDisplayObj != null)
            {
                if (mAppearanceCor != null) StopCoroutine(mAppearanceCor);  //出現アニメーション中止
                Destroy(mDisplayObj);   //オブジェクト破壊
            }
        }


        //==========================================================//
        //------------------------広告表示--------------------------//
        //==========================================================//

        //---共通---//

        const float LOADING_MAX_TIME = 30.0f;   //広告読み込み最大時間
        GameObject mMessageWaitObj;             //待機中オブジェクト
        Text mMessageMsgText;                   //メッセージテキスト

        /// <summary>
        /// 読み込み中メッセージ表示
        /// </summary>
        void AdLoadingMessageDisplay()
        {
            //読み込み中メッセージ表示
            ObjectDestroy();
            mDisplayObj = Instantiate(mMessageWinNoBtnPre);
            StartCoroutine(ObjectAppearance(false));
            Transform winTra = mDisplayObj.transform.GetChild(0);
            mMessageMsgText = winTra.GetChild(0).GetComponent<Text>();
            mMessageMsgText.text = "広告を読み込んでいます\nしばらくお待ちください\n\n";

            //待機中オブジェクト表示
            mMessageWaitObj = winTra.GetChild(1).gameObject;
            mMessageWaitObj.SetActive(true);
        }


        //---インタースティシャル---//

        const int SHOW_STAGE_NUM = 5;                                               //5ステージごとに表示を行う
        readonly WaitForSeconds MESSAGE_DISPLAY_TIME = new WaitForSeconds(2.0f);    //メッセージ表示時間

        /// <summary>
        /// インタースティシャル広告表示
        /// </summary>
        IEnumerator ShowAdInterstitial()
        {
#if !UNITY_ANDROID
            yield break;
#endif
            if (STAGE_NUMBER % SHOW_STAGE_NUM != 0) yield break;

            //読み込み中メッセージ表示
            AdLoadingMessageDisplay();

            //読み込み開始
            mAdInterstitial.AdInterstitialStart();

            bool end = false;
            float waitTime = 0.0f;
            while (!end)
            {
                yield return FIXED_UPDATE;
                switch (mAdInterstitial.AdState)
                {
                    //広告表示
                    case GoogleAdmobInterstitial.State.Showing:
                        ObjectDestroy();
                        end = true;
                        break;

                    //読み込み失敗
                    case GoogleAdmobInterstitial.State.loadFailed:

                        //広告破壊
                        mAdInterstitial.OnDestroy();

                        //メッセージ切替
                        mMessageMsgText.text = "広告の取得に失敗しました\nタイトルに戻ります";
                        mMessageWaitObj.SetActive(false);
                        yield return MESSAGE_DISPLAY_TIME;

                        //処理終了
                        yield break;

                    //閉じた
                    case GoogleAdmobInterstitial.State.Closed:

                        //メッセージ破壊
                        ObjectDestroy();

                        //広告破壊
                        mAdInterstitial.OnDestroy();

                        //処理終了
                        yield break;
                }

                //待機時間の上限を超えた場合
                if (waitTime > LOADING_MAX_TIME)
                {
                    //広告破壊
                    mAdInterstitial.OnDestroy();

                    //メッセージ表示
                    mMessageMsgText.text = "広告の取得に失敗しました\nタイトルに戻ります";
                    mMessageWaitObj.SetActive(false);
                    yield return MESSAGE_DISPLAY_TIME;

                    //処理終了
                    yield break;
                }

                if (end) break;
                waitTime += ONE_FRAME_TIMES;
            }

            //広告表示が終了するまで待機
            yield return new WaitUntil(() => mAdInterstitial.AdState == GoogleAdmobInterstitial.State.Showing);
        }


        //---------リワード---------//

        /// <summary>
        /// リワード広告表示状態
        /// </summary>
        public enum AdRewardState
        {
            None,
            Loading,        //読み込み中
            Loaded,         //読み込み完了
            FailedToLoad,   //読み込み失敗
            AdOpen,         //広告表示
            FailedToOpen,   //広告表示失敗
            AdClosed,       //広告終了
            EarnedReward    //報酬獲得
        }
        [HideInInspector]
        public AdRewardState mRewardState = AdRewardState.None;

        /// <summary>
        /// リワード広告表示
        /// </summary>
        void GenerateAdRewardObj()
        {
            //表示中の場合は処理をしない
            if (mRewardState != AdRewardState.None) return;

            //SE
            SE_OneShot(SE_Type.BtnYes);

            //リザルトオブジェクトの削除
            ObjectDestroy();

            //読み込み開始表示
            AdLoadingMessageDisplay();

            //オブジェクト生成
            GameObject obj = Instantiate(mAdRewardPre);
            mAdReward = obj.GetComponent<GoogleAdmobReward>();

            //状態監視開始
            mRewardState = AdRewardState.Loading;
            StartCoroutine(StatusMonitoringRewardAds());
        }

        /// <summary>
        /// リワード広告の状態監視
        /// </summary>
        /// <returns></returns>
        IEnumerator StatusMonitoringRewardAds()
        {
            //状態監視
            float waitTime = 0.0f;
            bool end = false;
            while (true)
            {
                switch (mRewardState)
                {
                    //読み込み完了
                    case AdRewardState.Loaded:
                        //広告開始
                        mAdReward.ShowRewardAd();
                        break;

                    //報酬獲得失敗
                    case AdRewardState.FailedToLoad:    //読み込み失敗
                    case AdRewardState.FailedToOpen:    //広告表示失敗
                        end = true;
                        break;

                    //報酬獲得成功
                    case AdRewardState.EarnedReward:
                        StartCoroutine(TurnMgr.TurnRecovery_AdReward());
                        end = true;
                        break;
                }

                //待機時間の上限を超えた場合
                if (mRewardState != AdRewardState.AdOpen && waitTime > LOADING_MAX_TIME)
                {
                    //読み込み失敗
                    mRewardState = AdRewardState.FailedToLoad;
                    end = true;
                    waitTime += ONE_FRAME_TIMES;
                }

                //状態監視終了
                if (end) break;
                yield return FIXED_UPDATE;
            }

            //メッセージ表示
            GenerateMessageWindow();

            //広告オブジェクト破壊
            mAdReward.RewardOnDestroy();
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        public void GenerateMessageWindow()
        {
            //表示文字列指定
            string msg = "";

            //置く場所がない
            if (GetFlag(PuzzleFlag.Uncontinuable))
            {
                msg = "駒を置ける場所がありません\nゲームオーバーとなります";
            }
            //その他
            else
            {
                //リワード広告
                switch (mRewardState)
                {
                    //報酬獲得せずに広告を閉じた
                    case AdRewardState.AdClosed:
                        msg = "広告をキャンセルしました";
                        break;

                    //読み込み完了
                    case AdRewardState.Loaded:
                        msg = "広告を表示します";
                        break;

                    //表示失敗
                    case AdRewardState.FailedToLoad:    //読み込み失敗
                    case AdRewardState.FailedToOpen:    //広告表示失敗
                        msg = "広告の表示に失敗しました\nもう一度お試しください";
                        break;

                    //報酬獲得
                    case AdRewardState.EarnedReward:
                        msg = "5ターン回復しました\nゲームを再開します";
                        break;

                }
            }

            //待機中オブジェクトがある or メッセージウィンドウが出ていない
            if (mMessageWaitObj || !mMessageMsgText)
            {
                //オブジェクト表示
                ObjectDestroy();
                mDisplayObj = Instantiate(mMessageWinPre);
                StartCoroutine(ObjectAppearance(false));

                //テキスト取得
                Transform winTra = mDisplayObj.transform.GetChild(0);
                mMessageMsgText = winTra.GetChild(0).GetComponent<Text>();
            }

            //テキスト反映
            mMessageMsgText.text = msg;
        }

        /// <summary>
        /// ゲーム再開
        /// </summary>
        /// <returns></returns>
        IEnumerator GameRestart()
        {
            //フィルターの解除
            Coroutine cor = StartCoroutine(CanvasMgr.SetFilter(false));

            //フィルターの解除待機(閉じるボタンのタップに駒を反応させないためでもある)
            yield return null;  //1フレームは必ず待機
            yield return cor;

            //フラグのリセット
            FlagReset();
            FlagOn(PuzzleFlag.TurnRecovered);

            //BGM再開
            BGM_FadeRestart();
        }

        /// <summary>
        /// 再挑戦
        /// </summary>
        /// <returns></returns>
        void TryAgain()
        {
            SceneFader.SceneChangeFade(PUZZLE_SCENE_NAME);
        }

        /// <summary>
        /// タイトルへ
        /// </summary>
        /// <returns></returns>
        void ReturnTitle()
        {
            SceneFader.SceneChangeFade(TITLE_SCENE_NAME);
        }


        //==========================================================//
        //----------------------ボタン判定--------------------------//
        //==========================================================//

        /// <summary>
        /// はい
        /// </summary>
        public void IsPushYes()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            switch (mConfirmWinState)
            {
                //再挑戦する？
                case ConfirmWinStatus.TryAgain:
                    TryAgain();
                    break;

                //タイトルに戻る？
                case ConfirmWinStatus.ReturnTitle:
                    ReturnTitle();
                    break;
            }
        }

        /// <summary>
        /// いいえ
        /// </summary>
        public void IsPushNo()
        {
            //SE
            SE_OneShot(SE_Type.BtnNo);

            switch (mConfirmWinState)
            {
                //再挑戦する？
                case ConfirmWinStatus.TryAgain:
                    mConfirmWinState = ConfirmWinStatus.ReturnTitle;
                    GenerateConfirmObj();
                    break;

                //タイトルに戻る？
                case ConfirmWinStatus.ReturnTitle:
                    StartCoroutine(GenerateGameOverObj());
                    break;
            }
        }

        /// <summary>
        /// 広告を見る
        /// </summary>
        public void IsPushSeeAbs()
        {
            GenerateAdRewardObj();
        }

        /// <summary>
        /// あきらめる
        /// </summary>
        public void IsPushGiveUp()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            mConfirmWinState = ConfirmWinStatus.TryAgain;
            GenerateConfirmObj();
        }

        /// <summary>
        /// 閉じる・キャンセル
        /// </summary>
        public void IsPushClose()
        {
            //SE
            SE_OneShot(SE_Type.BtnNo);

            //置く場所がない
            if (GetFlag(PuzzleFlag.Uncontinuable))
            {
                //ゲームオーバー
                sPuzzleMain.GameOver();
            }
            //その他
            else
            {
                //リワード広告
                switch (mRewardState)
                {
                    case AdRewardState.FailedToLoad:    //読み込み失敗
                    case AdRewardState.FailedToOpen:    //広告表示失敗
                    case AdRewardState.AdClosed:        //報酬獲得せずに広告を閉じた

                        //ゲームーバーオブジェクト再生成
                        StartCoroutine(GenerateGameOverObj());
                        mRewardState = AdRewardState.None;
                        break;

                    //報酬獲得
                    case AdRewardState.EarnedReward:

                        //ゲーム再開
                        ObjectDestroy();
                        StartCoroutine(GameRestart());
                        mRewardState = AdRewardState.None;
                        break;

                    //その他(入ることはないはず)
                    default:

                        //ゲームオバー表示をやり直す
                        StartCoroutine(GenerateGameOverObj());
                        mRewardState = AdRewardState.None;
                        break;
                }
            }
        }

        /// <summary>
        /// 再挑戦する
        /// </summary>
        public void IsPushTryAgain()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            TryAgain();
        }

        /// <summary>
        /// タイトルに戻る
        /// </summary>
        public void IsPushReturnTitle()
        {
            //SE
            SE_OneShot(SE_Type.BtnYes);

            ReturnTitle();
        }
    }
}