using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PiecesManager;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class GimmicksManager : MonoBehaviour
    {
        [Header("PiecesManagerの取得")]
        [SerializeField]
        PiecesManager PiecesMan;

        [Header("宝石のsprite")]
        [SerializeField]
        Sprite[] jewelrySprArr;
    
        [Header("枠のPrefab")]
        [SerializeField]
        GameObject[] framePrefab; //0:横,1:縦
    
        [Header("枠のsprite(縦)")]
        [SerializeField]
        Sprite[] frameHeightSprArr;
    
        [Header("枠のsprite(横)")]
        [SerializeField]
        Sprite[] frameWidthSprArr;
    
        [Header("枠のsprite(角)")]
        [SerializeField]
        Sprite[] frameCornerSprArr;

        //アニメーションステート名
        const string STATE_NAME_EMPTY        = "Empty";       //初期状態
        const string STATE_NAME_WAIT         = "Wait";        //待機
        const string STATE_NAME_BURST        = "Burst";       //破壊
        const string STATE_NAME_DAMAGE       = "Damage";      //複数回ダメージ
        const string STATE_NAME_COLOR_CHANGE = "ColorChange"; //色の更新
        const string STATE_NAME_RETURN_STATE = "ReturnState"; //状態を戻す

        //範囲指定ギミックのリスト
        List<GameObject>[] frameObjListArr;             //枠オブジェクトリスト(グループ別)
        List<GimmickInformation>[] frameInfoListArr;    //枠オブジェクト情報リスト(グループ別)
        List<int>[] frameSquareNumListArr;              //枠配置マスリスト(グループ別)
        int[] groupColorNumArr;                         //グループ毎の指定色番号


        void Awake()
        {
            //ギミックの設定読み込み
            GimmickSetting();
            StageSetting();
        }

        IEnumerator Start()
        {
            //駒として管理しないギミックの配置
            yield return null;
            PlaceGimmickNotInSquare();
        }


        //===============================================//
        //===========アニメーション動作関数==============//
        //===============================================//

        /// <summary>
        /// アニメーション再生
        /// </summary>
        /// <param name="ani">      破壊するオブジェクトのAnimator</param>
        /// <param name="stateName">再生アニメーションステート名</param>
        IEnumerator AnimationStart(Animator ani, string stateName)
        {
            //アニメーション開始
            ani.Play(stateName, 0, 0.0f);

            //アニメーション終了待機
            yield return null;
            yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f);
        }

        /// <summary>
        /// ループアニメーション再生
        /// </summary>
        /// <param name="ani">      破壊するオブジェクトのAnimator</param>
        /// <param name="stateName">再生アニメーションステート名</param>
        void LoopAnimationStart(Animator ani, string stateName = STATE_NAME_EMPTY)
        {
            //アニメーション開始
            ani.Play(stateName, 0, 0.0f);
        }


        //===============================================//
        //=========ギミックダメージ・状態変化============//
        //===============================================//

        /// <summary>
        /// ギミックにダメージがあるかの確認
        /// </summary>
        /// /// <param name="putPieceColorId">  置いた駒のタグ</param>
        /// /// <param name="gimmickIndex">     ギミック管理番号(ステージ毎の)</param>
        public bool DamageCheck(ref int putPieceColorId, ref int gimmickIndex)
        {
            //ダメージの有無フラグ
            bool damage = false;

            switch (gimmickInfoArr[gimmickIndex].id)
            {
                case (int)Gimmicks.Balloon:  //風船
                case (int)Gimmicks.Wall:     //壁
                case (int)Gimmicks.Flower:   //花
                case (int)Gimmicks.Hamster:  //ハムスター
                    damage = true;
                    break;

                case (int)Gimmicks.Balloon_Color: //風船(色)
                case (int)Gimmicks.Jewelry:       //宝石
                    //色判定
                    if (putPieceColorId == gimmickInfoArr[gimmickIndex].colorId)
                        damage = true;
                    break;
            }

            //結果を返す
            return damage;
        }

        /// <summary>
        /// ギミックにダメージ
        /// </summary>
        /// /// <param name="gimmickIndex">ギミック管理番号(ステージ毎の)</param>
        /// /// <param name="squareIndex"> ギミック配置番号</param>
        public void DamageGimmick(ref int gimmickIndex, int squareIndex)
        {
            int damageTimesfixNum = 0; //ステート名算出用
            string stateName = "";     //ステート名
            GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex]; //ギミックの情報取得

            switch (gimmInfo.id)
            {
                //無条件
                case (int)Gimmicks.Balloon:         //風船
                case (int)Gimmicks.Balloon_Color:   //風船(色)
                case (int)Gimmicks.Jewelry:         //宝石
                    stateName = STATE_NAME_BURST;
                    break;

                //複数回ダメージ
                case (int)Gimmicks.Wall:    //壁
                case (int)Gimmicks.Flower:  //花
                case (int)Gimmicks.Hamster: //ハムスター
                    damageTimesfixNum = gimmInfo.remainingTimes + 1;
                    stateName = STATE_NAME_DAMAGE + (-gimmInfo.remainingTimes + damageTimesfixNum).ToString();
                    if (!gimmInfo.destructible) gimmInfo.destructible = true;
                    break;
            }

            //ダメージアニメーション開始
            gimmickCorList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, stateName)));

            //ダメージ回数計算
            gimmInfo.remainingTimes--;

            //今のターンにダメージを受けたかのフラグON
            gimmInfo.nowTurnDamage = true;

            //ダメージ残回数が0で破壊
            if (gimmInfo.remainingTimes <= 0)
                destroyPiecesIndexList.Add(squareIndex);
        }

        /// <summary>
        /// ギミックの状態変化
        /// </summary>
        public IEnumerator ChangeGimmickState()
        {
            List<Coroutine> coroutineList = new List<Coroutine>();

            //通常ギミック
            if (gimmickInfoArr != null)
            {
                foreach (GimmickInformation gimmInfo in gimmickInfoArr)
                {
                    if (gimmInfo == null) continue;

                    switch (gimmInfo.id)
                    {
                        //宝石(sprite切替)
                        case (int)Gimmicks.Jewelry:

                            //子オブジェクトのsprit更新
                            gimmInfo.colorId++;
                            if (gimmInfo.colorId >= USE_PIECE_COUNT) gimmInfo.colorId = 0;
                            Sprite newSprite = jewelrySprArr[gimmInfo.colorId];
                            gimmInfo.spriRenChild[0].sprite = newSprite;

                            //sprit変更
                            coroutineList.Add(StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite)));
                            break;

                        //ハムスター(連続フラグ確認)
                        case (int)Gimmicks.Hamster:

                            //ダメージ1状態
                            if (gimmInfo.destructible)
                            {
                                //このターンにダメージを受けた
                                if (gimmInfo.nowTurnDamage)
                                {
                                    //ダメージ1待機状態
                                    LoopAnimationStart(gimmInfo.ani, STATE_NAME_WAIT);
                                }
                                else
                                {
                                    //初期状態に戻す
                                    gimmInfo.destructible = false;
                                    gimmInfo.remainingTimes++;
                                    coroutineList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN_STATE)));
                                    LoopAnimationStart(gimmInfo.ani);
                                }
                            }
                            break;
                    }
                }
            }

            //枠ギミック
            if (frameInfoListArr != null)
            {
                foreach (List<GimmickInformation> frameInfoList in frameInfoListArr)
                {
                    if (frameInfoList == null) continue;
                    List<int> changedSquare = new List<int>();
                    foreach (GimmickInformation gimmInfo in frameInfoList)
                    {
                        switch (gimmInfo.id)
                        {
                            //枠(色変化)
                            case (int)Gimmicks.Frame_Color_Change:

                                //子オブジェクトのsprit更新
                                gimmInfo.colorId++;
                                if (gimmInfo.colorId >= USE_PIECE_COUNT) gimmInfo.colorId = 0;
                                Sprite newSprite = (gimmInfo.tra.localPosition.x == 0.0f) ? frameWidthSprArr[gimmInfo.colorId] : frameHeightSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[0].sprite = newSprite;

                                //sprit変更
                                coroutineList.Add(StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite)));

                                //マスの色変更
                                if (!changedSquare.Contains(gimmInfo.startSquareId))
                                {
                                    coroutineList.Add(StartCoroutine(PiecesMan.SquareColorChange(GetSquareColor(gimmInfo.colorId), gimmInfo.startSquareId, true)));
                                    changedSquare.Add(gimmInfo.startSquareId);
                                }

                                //角のSprite設定
                                gimmInfo.spriRenChild[1].sprite = frameCornerSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[2].sprite = frameCornerSprArr[gimmInfo.colorId];
                                break;
                        }
                    }
                }
            }

            //ギミック変更待機
            foreach (Coroutine coroutine in coroutineList)
            { yield return coroutine; }
        }

        /// <summary>
        /// スプライト差し替え
        /// </summary>
        /// <param name="ani"></param>
        /// <param name="newSprite"></param>
        IEnumerator SpriteChange(Animator ani, SpriteRenderer spriRen, Sprite newSprite)
        {
            yield return StartCoroutine(AnimationStart(ani, STATE_NAME_COLOR_CHANGE));
            spriRen.sprite = newSprite;
        }


        //===============================================//
        //==================ギミック配置=================//
        //===============================================//

        /// <summary>
        /// マスとして管理しないギミックの配置
        /// </summary>
        void PlaceGimmickNotInSquare()
        {
            //グループ番号に応じた色番号配列
            groupColorNumArr      = new int[GIMMICKS_GROUP_COUNT];
            frameSquareNumListArr = new List<int>[GIMMICKS_GROUP_COUNT];

            //グループごとにリスト作成
            foreach (int[] gimInfo in GIMMICKS_INFO_ARR)
            {
                switch (gimInfo[GIMMICK])
                {
                    //グループギミック
                    case (int)Gimmicks.Frame:               //枠
                    case (int)Gimmicks.Frame_Color:         //枠(色)
                    case (int)Gimmicks.Frame_Color_Change:  //枠(色変更)
                        if (frameSquareNumListArr[gimInfo[GROUP]] == null)
                            frameSquareNumListArr[gimInfo[GROUP]] = new List<int>();
                        frameSquareNumListArr[gimInfo[GROUP]].Add(gimInfo[SQUARE]);
                        break;
                }

                //グループの指定色番号
                if (gimInfo[GROUP] != NOT_GROUP_ID) groupColorNumArr[gimInfo[GROUP]] = gimInfo[COLOR];
            }

            //枠配置
            if (frameSquareNumListArr != null) PlacementLocation_Frame();
        }



        //===============================================//
        //===========枠（Frame）の固有関数===============//
        //===============================================//

        /// <summary>
        /// マス指定色取得
        /// </summary>
        /// <param name="colorId">色番号</param>
        /// <returns>マス色</returns>
        Color GetSquareColor(int colorId)
        {
            Color color = SQUARE_WHITE;
            switch (colorId)
            {
                case (int)Colors.Blue:   color = SQUARE_BLUE;   break;
                case (int)Colors.Red:    color = SQUARE_RED;    break;
                case (int)Colors.Yellow: color = SQUARE_YELLOW; break;
                case (int)Colors.Green:  color = SQUARE_GREEN;  break;
                case (int)Colors.Violet: color = SQUARE_VIOLET; break;
                case (int)Colors.Orange: color = SQUARE_ORANGE; break;
                case COLORLESS_ID:       color = SQUARE_BLACK;  break;
            }
            return color;
        }

        /// <summary>
        /// 配置場所判定(枠)
        /// Frame
        /// FrameColor
        /// FrameColorChange
        /// </summary>
        void PlacementLocation_Frame()
        {
            //オブジェクト,情報リストの生成
            frameObjListArr  = new List<GameObject>[GIMMICKS_GROUP_COUNT];
            frameInfoListArr = new List<GimmickInformation>[GIMMICKS_GROUP_COUNT];

            int groupId = 0;
            foreach (List<int> squareList in frameSquareNumListArr)
            {
                frameObjListArr[groupId]  = new List<GameObject>();
                frameInfoListArr[groupId] = new List<GimmickInformation>();
                foreach (int squareIndex in squareList)
                {
                    //枠生成
                    if (!squareList.Contains(squareIndex - 1))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameWidthSprArr, Directions.Up, groupId);        //上        
                    if (!squareList.Contains(squareIndex + 1))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameWidthSprArr, Directions.Down, groupId);      //下        
                    if (!squareList.Contains(squareIndex - BOARD_LINE_COUNT))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameHeightSprArr, Directions.Left, groupId);     //左        
                    if (!squareList.Contains(squareIndex + BOARD_LINE_COUNT))
                        GenerateFrame(groupColorNumArr[groupId], squareIndex, frameHeightSprArr, Directions.Right, groupId);    //右

                    //マスの色指定
                    Color color = GetSquareColor(groupColorNumArr[groupId]);
                    StartCoroutine(PiecesMan.SquareColorChange(color, squareIndex, false));
                }
                groupId++;
            }
        }

        /// <summary>
        /// 枠生成
        /// </summary>
        /// <param name="colorId">    色番号</param>
        /// <param name="squareIndex">マス管理番号</param>
        /// <param name="spriArr">    sprite配列</param>
        /// <param name="direction">  配置方向</param>
        /// <param name="groupId">    グループ番号</param>
        void GenerateFrame(int colorId, int squareIndex, Sprite[] spriArr, Directions direction, int groupId)
        {
            //フレーム生成・配置
            GameObject frameObj = Instantiate(framePrefab[(int)direction]);
            frameObjListArr[groupId].Add(frameObj);
            PiecesMan.PlaceGimmick(frameObj, squareIndex);

            //フレームギミックの情報取得
            GimmickInformation gimInfo = frameObj.GetComponent<GimmickInformation>();
            frameInfoListArr[groupId].Add(gimInfo);
            gimInfo.InformationSetting_SquareIndex(squareIndex);

            //sprite設定
            if (colorId == COLORLESS_ID) colorId = COLORS_COUNT;
            gimInfo.spriRen.sprite = spriArr[colorId];
            gimInfo.spriRenChild[1].sprite = frameCornerSprArr[colorId]; //角1
            gimInfo.spriRenChild[2].sprite = frameCornerSprArr[colorId]; //角2
        }

        /// <summary>
        /// 枠破壊確認・実行
        /// </summary>
        public IEnumerator DamageFrame()
        {
            if (frameSquareNumListArr == null) yield break;

            bool frameListNull = true; //ギミックの存在の有無
            int groupId = 0;           //グループ番号

            foreach (List<int> squareList in frameSquareNumListArr)
            {
                if (squareList == null)
                {
                    groupId++;
                    continue;
                }

                frameListNull = false;

                //指定色の取得
                string specifiedColor = "";
                int colorId = frameInfoListArr[groupId][0].colorId;
                if (colorId != COLORLESS_ID)
                {
                    Colors color = (Colors)Enum.ToObject(typeof(Colors), colorId);
                    specifiedColor = color.ToString();
                }

                bool first = true; //初回ループフラグ
                bool burst = true; //ギミック破壊フラグ

                foreach (int i in squareList)
                {
                    //指定色無しで初回ループの場合
                    if (first && colorId == COLORLESS_ID)
                    {
                        //最初の駒のタグを指定色に設定
                        specifiedColor = squareTraArr[i].GetChild(0).tag;
                        if (specifiedColor == GIMMICK_TAG)
                        {
                            //ギミックの場合は処理終了
                            burst = false;
                            break;
                        }
                        first = false;
                    }

                    //タグ名が指定色でない場合
                    if (specifiedColor != squareTraArr[i].GetChild(0).tag)
                    {
                        burst = false;
                        break;
                    }
                }

                //ギミック破壊開始
                if (burst)
                {
                    //ギミック破壊演出
                    Coroutine coroutine = null;
                    foreach (GimmickInformation gimmickInfo in frameInfoListArr[groupId])
                    { coroutine = StartCoroutine(AnimationStart(gimmickInfo.ani, STATE_NAME_BURST)); }
                    yield return coroutine;

                    //ギミック破壊
                    foreach (GameObject obj in frameObjListArr[groupId])
                    { Destroy(obj);}
                    frameObjListArr[groupId] = null;

                    //ギミック情報を削除
                    frameInfoListArr[groupId] = null;

                    //マスの色を戻す
                    foreach (int i in squareList)
                    { StartCoroutine(PiecesMan.SquareColorChange(SQUARE_WHITE, i, true)); }
                    frameSquareNumListArr[groupId] = null;
                }

                //次のグループへ
                groupId++;
            }

            //処理が一度も入らなかった(リストが空)場合
            if (frameListNull)
            {
                frameInfoListArr      = null;
                frameObjListArr       = null;
                frameSquareNumListArr = null;
            }
        }
    }
}