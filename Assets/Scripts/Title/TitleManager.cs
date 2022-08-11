using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CommonDefine;
using static Title.TitleMain;
using static ObjectMove_UI.ObjectMove_UI;
using static animation.AnimationManager;
using static SaveDataManager;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        //タイトルに表示する駒の色
        enum PieceColor
        {
            Blue,   //青
            Red,    //赤
            Yellow, //黄
            Green,  //緑
            Violet, //紫
            Orange, //橙
            Brack,  //黒

            Count   //総数
        }

        [Header("BackGround")]
        [SerializeField]
        RectTransform mBackGroundTra;


        [Header("タイトル画面プレハブ")]
        [SerializeField]
        GameObject mTitleScreenPre;

        [Header("駒スプライト")]
        [SerializeField]
        Sprite[] mPiecesSpr;


        [Header("ステージ画面プレハブ")]
        [SerializeField]
        GameObject mStageSelScreenPre;

        [Header("ステージボタンプレハブ")]
        [SerializeField]
        GameObject mStageBtnPre;

        [Header("ステージボタンスプライト")]
        [SerializeField]
        Sprite[] mStageBtnSpr;

        //表示中のオブジェクト
        GameObject mTitleScreenObj;
        GameObject mStageSelScreenObj;

        //駒反転定数
        readonly Vector3 REVERSE_ROT_SPEED       = new Vector3(0.0f, 10.0f, 0.0f);          //駒反転速度
        readonly Vector3 REVERSE_SWITCH_ROT      = new Vector3(0.0f, 90.0f, 0.0f);          //反転中駒切り替え角度
        readonly Quaternion PIECE_REVERSE_QUEST  = Quaternion.Euler(0.0f, -90.0f, 0.0f);    //駒のスプライト差し替え時の角度
        readonly Vector3 REVERSE_FRONT_ROT       = Vector3.zero;                            //駒正面
        readonly WaitForSeconds WAIT_TIME        = new WaitForSeconds(0.5f);                //待機時間
        readonly WaitForSeconds REVERSE_INTERVAL = new WaitForSeconds(0.1f);                //反転間隔
        readonly WaitForSeconds BOTTOM_WAIT      = new WaitForSeconds(0.3f);                //下開始舞の間隔
        const float REVERSE_SCALING_SPEED        = 0.02f;                                   //駒の拡縮速度
        const float REVERSE_CHANGE_SCALE         = 1.2f;                                    //駒の拡大時のスケール
        const float PIECE_DEFAULT_SCALE          = 1.0f;                                    //駒の駒のスケール


        //==========================================================//
        //--------------------------初期化--------------------------//
        //==========================================================//

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            //タイトル画面生成
            TitleDisplay();

            //ステージ選択画面生成,非表示
            StageSelectDisplay();
            mStageSelScreenObj.SetActive(false);
        }


        //==========================================================//
        //--------------------タイトル画面--------------------------//
        //==========================================================//

        /// <summary>
        /// タイトル表示
        /// </summary>
        void TitleDisplay()
        {
            //タイトル画面生成
            mTitleScreenObj = Instantiate(mTitleScreenPre);

            //演出開始
            StartCoroutine(PiecesMove());
        }

        /// <summary>
        /// 駒演出
        /// </summary>
        IEnumerator PiecesMove()
        {
            //タイトル画面配置,コンポーネント取得
            Animator titleAni = mTitleScreenObj.GetComponent<Animator>();
            RectTransform tra = mTitleScreenObj.GetComponent<RectTransform>();
            tra.SetParent(mBackGroundTra, false);
            tra.localScale = Vector2.one;

            //上部の駒取得
            Transform topTra = tra.GetChild(0);
            int topChildCount = topTra.childCount;
            RectTransform[] topPiecesTraArr = new RectTransform[topChildCount];
            Image[] topPiecesImgArr = new Image[topChildCount];
            for (int i = 0; i < topChildCount; i++)
            {
                GameObject obj = topTra.GetChild(i).gameObject;
                topPiecesTraArr[i] = obj.GetComponent<RectTransform>();
                topPiecesImgArr[i] = obj.GetComponent<Image>();
            }

            //下部の駒取得
            Transform bottomTra = tra.GetChild(1);
            int bottomChildCount = topTra.childCount;
            RectTransform[] bottomPiecesTraArr = new RectTransform[bottomChildCount];
            Image[] bottomPiecesImgArr = new Image[bottomChildCount];
            for (int i = 0; i < bottomChildCount; i++)
            {
                GameObject obj = bottomTra.GetChild(i).gameObject;
                bottomPiecesTraArr[i] = obj.GetComponent<RectTransform>();
                bottomPiecesImgArr[i] = obj.GetComponent<Image>();
            }

            //初期駒色指定
            int topStartColorId = (int)PieceColor.Blue;
            int mBottomStartColorId = (int)PieceColor.Blue;

            //ループ開始
            while (sTitleState == TitleState.None)
            {
                yield return WAIT_TIME;

                //上部の駒反転
                topStartColorId--;
                if (topStartColorId < 0) topStartColorId = (int)PieceColor.Count - 1;
                for (int i = 0; i < topPiecesTraArr.Length; i++)
                {
                    int colorId = topStartColorId + i;
                    if (colorId >= (int)PieceColor.Count) colorId -= (int)PieceColor.Count;
                    StartCoroutine(ReversingPiece(topPiecesTraArr[i], topPiecesImgArr[i], mPiecesSpr[colorId]));
                    yield return REVERSE_INTERVAL;
                }

                yield return BOTTOM_WAIT;

                //下部の駒反転
                mBottomStartColorId++;
                if (mBottomStartColorId >= (int)PieceColor.Count) mBottomStartColorId = 0;
                for (int i = bottomPiecesTraArr.Length - 1; i >= 0 ; i--)
                {
                    int colorId = mBottomStartColorId + i;
                    if (colorId >= (int)PieceColor.Count) colorId -= (int)PieceColor.Count;
                    StartCoroutine(ReversingPiece(bottomPiecesTraArr[i], bottomPiecesImgArr[i], mPiecesSpr[colorId]));
                    yield return REVERSE_INTERVAL;
                }

                yield return WAIT_TIME;

                //タイトル拡縮
                AnimationPlay(titleAni, STATE_NAME_EFFECT);
            }
        }

        /// <summary>
        /// 駒反転
        /// </summary>
        /// <param name="_tra">反転駒のRectTransform</param>
        /// <param name="_img">反転駒のImage</param>
        /// <param name="_spr">反転後のSprite</param>
        /// <returns></returns>
        IEnumerator ReversingPiece(RectTransform _tra, Image _img, Sprite _spr)
        {
            //駒90°回転,拡大
            StartCoroutine(AllScaleChange(_tra, REVERSE_SCALING_SPEED, REVERSE_CHANGE_SCALE));
            yield return StartCoroutine(RotateMovement(_tra, REVERSE_ROT_SPEED, REVERSE_SWITCH_ROT));

            //スプライト切替
            _img.sprite = _spr;
            _tra.localRotation = PIECE_REVERSE_QUEST;

            //駒90°回転,縮小
            StartCoroutine(AllScaleChange(_tra, REVERSE_SCALING_SPEED, PIECE_DEFAULT_SCALE));
            yield return StartCoroutine(RotateMovement(_tra, REVERSE_ROT_SPEED, REVERSE_FRONT_ROT));
        }


        //==========================================================//
        //-------------------ステージ選択画面-----------------------//
        //==========================================================//

        //ステージ選択ボタンの色
        enum StageBtnColor
        {
            Blue,   //青
            Red,    //赤
            Yellow, //黄
            Green,  //緑
            Violet, //紫
            Orange, //橙

            Count   //総数
        }

        const int STAGE_BTN_WIDTH           = 130;
        const int STAGE_BTN_HEIGHT          = 130;
        const int STAGE_BTN_OFFSET_X        = 150;
        const int STAGE_BTN_OFFSET_Y        = 150;
        const int STAGE_BTN_COLUMN_COUNT    = 5;
        const int STAGE_BTN_LINE_COUNT      = 10;
        const int STAGE_PAGE_COUNT          = 5;
        const float STAGE_PAGE_MOVE_SPEED   = 0.3f;
        readonly float STAGE_BTN_POS_X      = (STAGE_BTN_COLUMN_COUNT - 1) / 2.0f * -STAGE_BTN_OFFSET_X;
        readonly float STAGE_BTN_POS_Y      = STAGE_BTN_LINE_COUNT / 2.0f * STAGE_BTN_OFFSET_Y;
        readonly int STAGE_MAX_PAGE_INDEX   = STAGE_PAGE_COUNT - 1;
        int mDisplayPage = 0;
        Coroutine mPageMoveCor; 

        //ステージ選択オブジェクトの子オブジェクト
        enum StageSelChild
        { Stages, RightArrow, LeftArrow, Count }
        readonly GameObject[] mStageSelChildObj = new GameObject[(int)StageSelChild.Count];
        readonly RectTransform[] mStageSelChildTra = new RectTransform[(int)StageSelChild.Count];

        //ステージボタンオブジェクトの子オブジェクト
        enum StageBtnChild
        { Number, Filter, Clear }

        /// <summary>
        /// ステージ選択画面表示
        /// </summary>
        void StageSelectDisplay()
        {
            //タイトル生成済の場合
            if (mStageSelScreenObj != null)
            {
                //アクティブ状態へ
                mStageSelScreenObj.SetActive(true);

                //初期表示ページ指定
                mDisplayPage = ClearStageNum / (STAGE_BTN_COLUMN_COUNT * STAGE_BTN_LINE_COUNT);
                StagePageChange();

                //以下生成処理をスキップ
                return;
            }

            //ステージ選択画面生成
            mStageSelScreenObj = Instantiate(mStageSelScreenPre);

            //タイトル画面配置,コンポーネント取得
            RectTransform screenTra = mStageSelScreenObj.GetComponent<RectTransform>();
            screenTra.SetParent(mBackGroundTra, false);
            screenTra.localScale = Vector2.one;
            for (int i = 0; i < (int)StageSelChild.Count; i++)
            {
                mStageSelChildObj[i] = screenTra.GetChild(i).gameObject;
                mStageSelChildTra[i] = mStageSelChildObj[i].GetComponent<RectTransform>();
            }

            //ステージボタン生成
            int stageNumber = 0;
            for (int i = 0; i < STAGE_PAGE_COUNT; i++)
            {
                float posX = STAGE_BTN_POS_X + PLAY_SCREEN_WIDTH * i;
                for (int a = 0; a < STAGE_BTN_LINE_COUNT; a++)
                {
                    //行毎に色と配置座標Y設定
                    int colorId = a;
                    if (colorId >= (int)StageBtnColor.Count) colorId -= (int)StageBtnColor.Count;
                    float posY = STAGE_BTN_POS_Y - STAGE_BTN_OFFSET_Y * a;
                    for (int b = 0; b < STAGE_BTN_COLUMN_COUNT; b++)
                    {
                        //ステージオブジェクト生成
                        GameObject obj = Instantiate(mStageBtnPre);
                        obj.GetComponent<Image>().sprite = mStageBtnSpr[colorId];
                        RectTransform stageBtntra = obj.GetComponent<RectTransform>();

                        //サイズ,座標指定
                        stageBtntra.SetParent(mStageSelChildTra[(int)StageSelChild.Stages], false);
                        stageBtntra.sizeDelta = new Vector2(STAGE_BTN_WIDTH, STAGE_BTN_HEIGHT);
                        stageBtntra.localPosition = new Vector2(posX + STAGE_BTN_OFFSET_X * b, posY);

                        //ステージ番号更新,解放ボタンの表示
                        stageNumber = i * STAGE_BTN_COLUMN_COUNT * STAGE_BTN_LINE_COUNT + a * STAGE_BTN_COLUMN_COUNT + b + 1;
                        stageBtntra.GetChild((int)StageBtnChild.Number).GetComponent<Text>().text = stageNumber.ToString();
                        if (stageNumber <= ClearStageNum + 1)
                        {
                            stageBtntra.GetChild((int)StageBtnChild.Filter).gameObject.SetActive(false);
                            int n = stageNumber;
                            obj.GetComponent<Button>().onClick.AddListener(() => IsPushStageBtn(n));

                            //Clear表示
                            if (stageNumber <= ClearStageNum)
                                stageBtntra.GetChild((int)StageBtnChild.Clear).gameObject.SetActive(true);
                        }
                    }
                }
            }

            //初期表示ページ指定
            mDisplayPage = ClearStageNum / (STAGE_BTN_COLUMN_COUNT * STAGE_BTN_LINE_COUNT);
            StagePageChange();
        }

        /// <summary>
        /// ステージボタン押した
        /// </summary>
        /// <param name="_stageNum"></param>
        void IsPushStageBtn(int _stageNum)
        {
            SceneNavigator.Instance.Change(PUZZLE_SCENE_NAME);
        }

        /// <summary>
        /// ページ変更
        /// </summary>
        /// <returns></returns>
        void StagePageChange()
        {
            mDisplayPage = Mathf.Clamp(mDisplayPage, 0, STAGE_MAX_PAGE_INDEX);

            //ページ移動
            Vector2 targetPos = new Vector2(mDisplayPage * -PLAY_SCREEN_WIDTH, 0.0f);
            if (mPageMoveCor != null) StopCoroutine(mPageMoveCor);
            mPageMoveCor = StartCoroutine(DecelerationMovement(mStageSelChildTra[(int)StageSelChild.Stages], STAGE_PAGE_MOVE_SPEED, targetPos));

            //矢印表示状態変更
            mStageSelChildObj[(int)StageSelChild.RightArrow].SetActive(mDisplayPage < STAGE_MAX_PAGE_INDEX);
            mStageSelChildObj[(int)StageSelChild.LeftArrow].SetActive(mDisplayPage > 0);
        }


        //==========================================================//
        //----------------------ボタン判定--------------------------//
        //==========================================================//

        /// <summary>
        /// スタート
        /// </summary>
        public void IsPushStart()
        {
            StopAllCoroutines();
            Destroy(mTitleScreenObj);
            sTitleState = TitleState.StageSelect;
            StageSelectDisplay();
        }

        /// <summary>
        /// もどる
        /// </summary>
        public void IsPushBack()
        {
            StopAllCoroutines();
            mStageSelScreenObj.SetActive(false);
            sTitleState = TitleState.None;
            TitleDisplay();
        }

        /// <summary>
        /// 右矢印
        /// </summary>
        public void IsPushRightArrow()
        {
            mDisplayPage++;
            StagePageChange();
        }

        /// <summary>
        /// 左矢印
        /// </summary>
        public void IsPushLeftArrow()
        {
            mDisplayPage--;
            StagePageChange();
        }
    }
}