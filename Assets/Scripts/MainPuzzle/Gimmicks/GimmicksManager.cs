using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;
using static animation.AnimationManager;
using static ObjectMove_2D.ObjectMove_2D;

namespace PuzzleMain
{
    public class GimmicksManager : MonoBehaviour
    {
        SquaresManager squaresMgr;  //SquaresManager
        PiecesManager  piecesMgr;   //PiecesManager

        [Header("ギミックプレハブの取得")]
        public GimmickArr[] gimmickPrefabArr;
        [Serializable]
        public class GimmickArr
        { public GameObject[] prefab; }

        [Header("宝石sprite")]
        [SerializeField]
        Sprite[] jewelrySprArr;
    
        [Header("枠sprite(縦)")]
        [SerializeField]
        Sprite[] frameHeightSprArr;
    
        [Header("枠sprite(横)")]
        [SerializeField]
        Sprite[] frameWidthSprArr;
    
        [Header("枠sprite(角)")]
        [SerializeField]
        Sprite[] frameCornerSprArr;

        [Header("檻sprite(爆弾)")]
        [SerializeField]
        Sprite[] CageBobmSprArr;

        [Header("檻sprite(数字)")]
        [SerializeField]
        Sprite[] CageNumberSprArr;

        [Header("番号札sprite(数字)")]
        [SerializeField]
        Sprite[] NumberTagSprArr;

        //グループギミックのリスト
        List<GameObject>[] frameObjListArr;             //枠オブジェクトリスト(グループ別)
        List<GimmickInformation>[] frameInfoListArr;    //枠オブジェクト情報リスト(グループ別)
        List<int>[] frameSquareIdListArr;               //枠配置マスリスト(グループ別)
        int[] groupColorNumArr;                         //グループ毎の指定色番号

        //サイズ可変ギミックの配列
        GameObject[] cageObjArr;            //檻オブジェクトリスト
        GimmickInformation[] cageInfoArr;   //檻オブジェクト情報リスト
        int[] cageSquareIdArr;              //檻配置マスリスト

        //攻撃を行う駒リスト
        List<int> attackPiecesList;


        //==========================================================//
        //----------------------初期設定,取得-----------------------//
        //==========================================================//

        /// <summary>
        /// GimmicksManagerの初期化
        /// </summary>
        public void Initialize()
        {
            squaresMgr = sPuzzleMain.GetSquaresManager();
            piecesMgr  = sPuzzleMain.GetPiecesManager();

            //ギミック生成
            sGimmickObjArr  = new GameObject[GIMMICKS_DEPLOY_COUNT];
            sGimmickInfoArr = new GimmickInformation[GIMMICKS_DEPLOY_COUNT];

            //駒として配置しないギミックの生成
            PlaceGimmickNotInSquare();

            //駒として管理するギミック
            for (int i = 0; i < GIMMICKS_DEPLOY_COUNT; i++)
            {
                if (GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[i][GIMMICK]].in_square)
                {
                    GeneraeGimmick(i);
                }
            }

            //攻撃を行う駒リスト
            attackPiecesList = new List<int>();
        }


        //===============================================//
        //=================ギミック生成==================//
        //===============================================//

        /// <summary>
        /// ギミック作成(駒として管理する)
        /// </summary>
        /// /// <param name="gimmickIndex"> ギミック管理番号</param>
        void GeneraeGimmick(int gimmickIndex)
        {
            int colorId = (GIMMICKS_INFO_ARR[gimmickIndex][COLOR] < 0) ? 0 : GIMMICKS_INFO_ARR[gimmickIndex][COLOR];
            sGimmickObjArr[gimmickIndex] = Instantiate(gimmickPrefabArr[GIMMICKS_INFO_ARR[gimmickIndex][GIMMICK]].prefab[colorId]);

            //Component取得
            sGimmickInfoArr[gimmickIndex] = sGimmickObjArr[gimmickIndex].GetComponent<GimmickInformation>();
            sGimmickInfoArr[gimmickIndex].InformationSetting(gimmickIndex);

            //番号札の場合
            if (sGimmickInfoArr[gimmickIndex].id == (int)Gimmicks.NumberTag)
                NumberTagOrderSetting(ref sGimmickInfoArr[gimmickIndex]);
        }


        //===============================================//
        //=====================汎用======================//
        //===============================================//

        /// <summary>
        /// マスにあるギミックオブジェクト番号取得
        /// </summary>
        /// <param name="squareId">マス管理番号</param>
        /// <returns>ギミックの管理番号</returns>
        public int GetGimmickIndex_Square(int squareId)
        {
            GameObject pieceObj = sPieceObjArr[squareId];
            if (pieceObj == null) return INT_NULL;
            return GetGimmickIndex_Obj(pieceObj);
        }

        /// <summary>
        /// オブジェクトからギミック管理番号を取得
        /// </summary>
        /// <param name="obj">マス管理番号</param>
        /// <returns>ギミックの管理番号</returns>
        public int GetGimmickIndex_Obj(GameObject obj)
        {
            return Array.IndexOf(sGimmickObjArr, obj);
        }

        //===============================================//
        //=========ギミックダメージ・状態変化============//
        //===============================================//

        /// <summary>
        /// ギミックにダメージがあるかの確認
        /// </summary>
        /// <param name="putPieceColorId">  置いた駒のタグ</param>
        /// <param name="gimmickIndex">     ギミック管理番号(ステージ毎の)</param>
        /// <param name="assault">          強撃</param>
        public bool DamageCheck(ref int putPieceColorId, ref int gimmickIndex, bool assault = false)
        {
            //ダメージの有無フラグ
            bool damage = false;

            switch (sGimmickInfoArr[gimmickIndex].id)
            {
                //無条件
                case (int)Gimmicks.Balloon: //風船
                case (int)Gimmicks.Wall:    //壁
                case (int)Gimmicks.Flower:  //花
                case (int)Gimmicks.Hamster: //ハムスター
                case (int)Gimmicks.Thief:   //泥棒
                case (int)Gimmicks.Tornado: //竜巻
                    damage = true;
                    break;

                //色判定
                case (int)Gimmicks.Balloon_Color: //風船(色)
                case (int)Gimmicks.Jewelry:       //宝石
                    damage = putPieceColorId == sGimmickInfoArr[gimmickIndex].colorId;
                    break;

                //強撃
                case (int)Gimmicks.Steel: //鋼
                    damage = assault;
                    break;

                //順番
                case (int)Gimmicks.NumberTag:   //番号札
                    if (sNumberTagNextOrder == sGimmickInfoArr[gimmickIndex].order)
                    {
                        damage = true;
                        sNumberTagNextOrder++;
                    }
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
        /// /// <param name="stateAddName">ステート名追加文字列</param>
        public void DamageGimmick(ref int gimmickIndex, int squareIndex, string stateAddName = "")
        {
            string stateName = "";     //ステート名
            GimmickInformation gimmInfo = sGimmickInfoArr[gimmickIndex]; //ギミックの情報取得

            switch (gimmInfo.id)
            {
                //無条件破壊
                case (int)Gimmicks.Balloon:         //風船
                case (int)Gimmicks.Balloon_Color:   //風船(色)
                case (int)Gimmicks.Jewelry:         //宝石
                case (int)Gimmicks.NumberTag:       //番号札
                case (int)Gimmicks.Thief:           //泥棒
                case (int)Gimmicks.Tornado:         //竜巻
                    stateName = STATE_NAME_BURST;
                    break;

                //無条件破壊(ステート名追加)
                case (int)Gimmicks.Steel:           //鋼
                    stateName = STATE_NAME_BURST + stateAddName;
                    break;

                //複数回ダメージ
                case (int)Gimmicks.Wall:    //壁
                case (int)Gimmicks.Flower:  //花
                case (int)Gimmicks.Hamster: //ハムスター
                    stateName = STATE_NAME_DAMAGE + gimmInfo.remainingTimes.ToString();
                    if (!gimmInfo.destructible) gimmInfo.destructible = true;
                    break;
            }

            //ダメージアニメーション開始
            Coroutine coroutine = StartCoroutine(AnimationStart(gimmInfo.ani, stateName));
            sGimmickCorList.Add(coroutine);

            //ダメージ回数計算
            gimmInfo.remainingTimes--;

            //今のターンにダメージを受けたかのフラグON
            gimmInfo.nowTurnDamage = true;

            //ダメージ残回数が0で破壊
            if (gimmInfo.remainingTimes <= 0)
                sDestroyPiecesIndexList.Add(squareIndex);
        }

        /// <summary>
        /// ギミックの状態変化
        /// </summary>
        public IEnumerator ChangeGimmickState()
        {
            //コルーチンリスト
            List<Coroutine> coroutineList = new List<Coroutine>();
            Coroutine coroutine;

            //竜巻オブジェクト情報リスト
            tornadoInfoList = new List<GimmickInformation>();

            //通常ギミック
            if (sGimmickInfoArr != null)
            {
                foreach (GimmickInformation gimmInfo in sGimmickInfoArr)
                {
                    if (gimmInfo == null) continue;

                    switch (gimmInfo.id)
                    {
                        //宝石(sprite切替)
                        case (int)Gimmicks.Jewelry:

                            //子オブジェクトのsprit更新
                            int oldColorType = Array.IndexOf(USE_COLOR_TYPE_ARR, gimmInfo.colorId);
                            int newColorType = (oldColorType == USE_COLOR_COUNT - 1) ? 0 : oldColorType + 1;
                            gimmInfo.colorId = USE_COLOR_TYPE_ARR[newColorType];
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
                                //このターンにダメージを受けていない場合
                                if (!gimmInfo.nowTurnDamage)
                                {
                                    //初期状態に戻す
                                    gimmInfo.destructible = false;
                                    gimmInfo.remainingTimes++;
                                    coroutineList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN)));
                                }
                            }
                            break;

                        //泥棒
                        case (int)Gimmicks.Thief:

                            //攻撃開始
                            coroutineList.Add(StartCoroutine(StartThiefAttack(gimmInfo)));
                            break;

                        //竜巻
                        case (int)Gimmicks.Tornado:
                            tornadoInfoList.Add(gimmInfo);
                            break;
                    }
                }
            }

            //竜巻ギミック動作開始
            if (tornadoInfoList.Count > 0)
            {
                coroutine = StartCoroutine(StartTornadoAttack());
                coroutineList.Add(coroutine);
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
                                int oldColorType = Array.IndexOf(USE_COLOR_TYPE_ARR, gimmInfo.colorId);
                                int newColorType = (oldColorType == USE_COLOR_COUNT - 1) ? 0 : oldColorType + 1;
                                gimmInfo.colorId = USE_COLOR_TYPE_ARR[newColorType];
                                Sprite newSprite = frameHeightSprArr[gimmInfo.colorId];
                                if (gimmInfo.tra.localPosition.x == 0.0f) newSprite = frameWidthSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[0].sprite = newSprite;

                                //sprit変更
                                coroutine = StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite));
                                coroutineList.Add(coroutine);

                                //マスの色変更
                                if (!changedSquare.Contains(gimmInfo.startSquareId))
                                {
                                    coroutine = StartCoroutine(squaresMgr.SquareColorChange(GetSquareColor(gimmInfo.colorId), gimmInfo.startSquareId, true));
                                    coroutineList.Add(coroutine);
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
            foreach (Coroutine c in coroutineList)
            { yield return c; }
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

        /// <summary>
        /// ターン終了時のギミック破壊
        /// </summary>
        /// <returns></returns>
        public IEnumerator DestroyGimmicks_TurnEnd()
        {
            //ギミック破壊
            Coroutine[] coroutines = new Coroutine[]
            {
                StartCoroutine(DestroyFrame()),     //枠
                StartCoroutine(DestroyCage())       //檻
            };
            foreach (Coroutine c in coroutines)
            { yield return c; }
        }

        /// <summary>
        /// ターン情報をリセット
        /// </summary>
        public void ResetTurnInfo()
        {
            //ギミックのフラグリセット
            foreach (GimmickInformation gimmickInfo in sGimmickInfoArr)
            {
                if (gimmickInfo == null) continue;
                gimmickInfo.nowTurnDamage = false;
            }

            //ギミック待機リストの初期化
            sGimmickCorList = new List<Coroutine>();

            //攻撃を行う駒リストの初期化
            attackPiecesList = new List<int>();
        }

        //===============================================//
        //==================ギミック配置=================//
        //===============================================//

        /// <summary>
        /// マスとして管理しないギミックの配置
        /// </summary>
        public void PlaceGimmickNotInSquare()
        {
            //グループ番号に応じた色番号配列
            groupColorNumArr     = new int[GIMMICKS_GROUP_COUNT];
            frameSquareIdListArr = new List<int>[GIMMICKS_GROUP_COUNT];

            //檻情報リスト
            List<int[]> cageInfoArrList = new List<int[]>();

            //オブジェクト管理リスト作成
            foreach (int[] gimInfo in GIMMICKS_INFO_ARR)
            {
                switch (gimInfo[GIMMICK])
                {
                    //枠
                    case (int)Gimmicks.Frame:               //枠
                    case (int)Gimmicks.Frame_Color:         //枠(色)
                    case (int)Gimmicks.Frame_Color_Change:  //枠(色変更)
                        //グループごとにリスト作成
                        if (frameSquareIdListArr[gimInfo[GROUP]] == null)
                            frameSquareIdListArr[gimInfo[GROUP]] = new List<int>();
                        frameSquareIdListArr[gimInfo[GROUP]].Add(gimInfo[SQUARE]);
                        break;

                    //サイズ可変ギミック
                    case (int)Gimmicks.Cage:    //檻
                        cageInfoArrList.Add(gimInfo);
                        break;
                }

                //グループの指定色番号
                if (gimInfo[GROUP] != NOT_NUM) groupColorNumArr[gimInfo[GROUP]] = gimInfo[COLOR];
            }

            //枠配置
            if (frameSquareIdListArr != null) PlacementLocation_Frame();

            //檻配置
            if (cageInfoArrList != null) GenerateCage(ref cageInfoArrList);
        }



        //===============================================//
        //===========枠（Frame）の固有関数===============//
        //===============================================//

        //プレハブの子オブジェクト番号
        const int FRAME_CORNER_1 = 1; //角1
        const int FRAME_CORNER_2 = 2; //角2

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
            foreach (List<int> squareList in frameSquareIdListArr)
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
                    StartCoroutine(squaresMgr.SquareColorChange(color, squareIndex, false));
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
            //フレーム生成,配置
            GameObject frameObj = Instantiate(gimmickPrefabArr[(int)Gimmicks.Frame].prefab[(int)direction]);
            frameObjListArr[groupId].Add(frameObj);
            piecesMgr.PlaceGimmick(frameObj, squareIndex);

            //フレームギミックの情報取得
            GimmickInformation gimInfo = frameObj.GetComponent<GimmickInformation>();
            frameInfoListArr[groupId].Add(gimInfo);
            gimInfo.InformationSetting_SquareIndex(squareIndex, INT_NULL, groupId);
            sGimmickInfoArr[gimInfo.settingIndex] = gimInfo;

            //sprite設定
            if (colorId == COLORLESS_ID) colorId = COLORS_COUNT;
            gimInfo.spriRen.sprite = spriArr[colorId];
            gimInfo.spriRenChild[FRAME_CORNER_1].sprite = frameCornerSprArr[colorId]; //角1
            gimInfo.spriRenChild[FRAME_CORNER_2].sprite = frameCornerSprArr[colorId]; //角2
        }

        /// <summary>
        /// 枠破壊確認・実行
        /// </summary>
        IEnumerator DestroyFrame()
        {
            if (frameSquareIdListArr == null) yield break;

            bool frameListNull = true; //ギミックの存在の有無
            int groupId = 0;           //グループ番号

            foreach (List<int> squareList in frameSquareIdListArr)
            {
                if (squareList == null)
                {
                    groupId++;
                    continue;
                }

                frameListNull = false;

                //指定色の取得
                int colorId = frameInfoListArr[groupId][0].colorId;
                bool burst = true; //ギミック破壊フラグ

                foreach (int i in squareList)
                {
                    //駒でない場合は処理終了
                    if (sPieceInfoArr[i] == null)
                    {
                        burst = false;
                        break;
                    }

                    //最初の駒のタグを指定色に設定
                    if (colorId == COLORLESS_ID && squareList[0] == i)
                    {
                        colorId = sPieceInfoArr[i].colorId;
                    }

                    //指定色でない場合
                    if (colorId != sPieceInfoArr[i].colorId)
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
                    { StartCoroutine(squaresMgr.SquareColorChange(SQUARE_WHITE, i, true)); }
                    frameSquareIdListArr[groupId] = null;
                }

                //次のグループへ
                groupId++;
            }

            //処理が一度も入らなかった(リストが空)場合
            if (frameListNull)
            {
                frameInfoListArr     = null;
                frameObjListArr      = null;
                frameSquareIdListArr = null;
            }
        }



        //===============================================//
        //============檻（Cage）の固有関数===============//
        //===============================================//

        //プレハブの子オブジェクト番号
        const int CAGE_BOBM          = 0; //爆弾
        const int CAGE_NUMBER_RIGHT  = 1; //数字右配置
        const int CAGE_NUMBER_LEFT   = 2; //数字左配置
        const int CAGE_NUMBER_CENTER = 3; //数字中央配置(1桁用)

        /// <summary>
        /// 檻生成
        /// </summary>
        /// <param name="cageInfoArrList">檻の情報配列</param>
        void GenerateCage(ref List<int[]> cageInfoArrList)
        {
            int cageCount    = cageInfoArrList.Count;
            cageObjArr       = new GameObject[cageCount];               //檻オブジェクトリスト
            cageInfoArr      = new GimmickInformation[cageCount];       //檻オブジェクト情報リスト
            cageSquareIdArr = new int[cageCount];                       //檻配置マスリスト
            for (int i = 0; i < cageCount; i++)
            {
                int[] cageInfo = cageInfoArrList[i];

                //檻生成,配置
                cageObjArr[i]  = Instantiate(gimmickPrefabArr[cageInfo[GIMMICK]].prefab[0]);
                cageInfoArr[i] = cageObjArr[i].GetComponent<GimmickInformation>();
                cageSquareIdArr[i] = cageInfo[SQUARE];
                piecesMgr.PlaceGimmick(cageObjArr[i], cageInfo[SQUARE]);
                cageInfoArr[i].InformationSetting_SquareIndex(cageInfo[SQUARE], cageInfo[GIMMICK], NOT_NUM);
                sGimmickInfoArr[cageInfoArr[i].settingIndex] = cageInfoArr[i];
                cageInfoArr[i].spriRenChild[CAGE_BOBM].sprite = CageBobmSprArr[cageInfo[COLOR]];

                //座標指定
                Vector3 _pos = new Vector3(
                    cageInfoArr[i].defaultPos.x * (cageInfo[WIDTH] - 1),
                    cageInfoArr[i].defaultPos.y * (cageInfo[HEIGHT] - 1),
                    cageInfoArr[i].defaultPos.z);
                cageInfoArr[i].tra.localPosition = _pos;

                //スケール指定
                Vector3 _scale = new Vector3(
                    cageInfoArr[i].defaultScale.x * cageInfo[WIDTH],
                    cageInfoArr[i].defaultScale.y * cageInfo[HEIGHT],
                    cageInfoArr[i].defaultScale.z);
                cageInfoArr[i].spriRen.size = _scale;

                //sprite更新(数字)
                BobmCountSpriteUpdate(cageInfoArr[i]);
            }
        }

        /// <summary>
        /// 爆弾の番号更新
        /// </summary>
        /// <param name="_cageInfo"></param>
        void BobmCountSpriteUpdate(GimmickInformation _cageInfo)
        {
            if (_cageInfo.remainingQuantity < TEN)
            {
                //1桁(中央表示)
                _cageInfo.objChild[CAGE_NUMBER_RIGHT].SetActive(false);     //右
                _cageInfo.objChild[CAGE_NUMBER_LEFT].SetActive(false);      //左
                _cageInfo.objChild[CAGE_NUMBER_CENTER].SetActive(true);     //中央
                _cageInfo.spriRenChild[CAGE_NUMBER_CENTER].sprite = CageNumberSprArr[_cageInfo.remainingQuantity];
            }
            else
            {
                //2桁
                _cageInfo.objChild[CAGE_NUMBER_RIGHT].SetActive(true);      //右
                _cageInfo.objChild[CAGE_NUMBER_LEFT].SetActive(true);       //左
                _cageInfo.objChild[CAGE_NUMBER_CENTER].SetActive(false);    //中央
                _cageInfo.spriRenChild[CAGE_NUMBER_RIGHT].sprite = CageNumberSprArr[_cageInfo.remainingQuantity % TEN];   //1の位
                _cageInfo.spriRenChild[CAGE_NUMBER_LEFT].sprite  = CageNumberSprArr[_cageInfo.remainingQuantity / TEN];   //10の位
            }
        }

        /// <summary>
        /// 檻ダメージ
        /// </summary>
        /// <param name="_colorId">色番号</param>
        public IEnumerator DamageCage(int _colorId)
        {
            foreach (GimmickInformation cageInfo in cageInfoArr)
            {
                if (cageInfo == null) continue;
                if (cageInfo.remainingQuantity == 0) continue;

                if (cageInfo.colorId == _colorId && cageInfo.remainingQuantity > 0)
                {
                    //同色の爆弾カウントを減らす
                    cageInfo.remainingQuantity--;

                    //sprite更新(数字)
                    BobmCountSpriteUpdate(cageInfo);
                    yield return StartCoroutine(AnimationStart(cageInfo.ani, STATE_NAME_DAMAGE));
                    break;
                }
            }
        }

        /// <summary>
        /// 檻破壊
        /// </summary>
        IEnumerator DestroyCage()
        {
            List<int> desIndexList = new List<int>();
            Coroutine coroutine = null;
            int index = -1;

            foreach (GimmickInformation cageInfo in cageInfoArr)
            {
                index++;
                if (cageInfo == null) continue;

                //ギミック破壊
                if (cageInfo.remainingQuantity == 0)
                {
                    coroutine = StartCoroutine(AnimationStart(cageInfo.ani, STATE_NAME_BURST));
                    desIndexList.Add(index);
                }
            }
            yield return coroutine;

            if (desIndexList.Count == 0) yield break;

            //ギミック破壊
            foreach (int desIndex in desIndexList)
            {
                //駒の操作フラグ切替
                foreach (int squareId in cageInfoArr[desIndex].innerSquaresId)
                { piecesMgr.PieceOperationFlagChange(squareId, true); }

                //オブジェクト破壊
                Destroy(cageObjArr[desIndex]);

                //管理配列リセット
                cageObjArr[desIndex] = null;           //檻オブジェクトリスト
                cageInfoArr[desIndex] = null;          //檻オブジェクト情報リスト
                cageSquareIdArr[desIndex] = INT_NULL;  //檻配置マスリスト
            }
        }


        //===============================================//
        //=========番号札（NumberTag）の固有関数=========//
        //===============================================//

        //プレハブの子オブジェクト番号
        const int NUMBERTAG_FRONT = 0; //前面(番号記載面)

        /// <summary>
        /// 順番設定(sprite設定)
        /// </summary>
        /// <param name="gimInfo">ギミック情報</param>
        public void NumberTagOrderSetting(ref GimmickInformation gimInfo)
        {
            gimInfo.spriRenChild[NUMBERTAG_FRONT].sprite = NumberTagSprArr[gimInfo.order];
        }


        //===============================================//
        //=============泥棒（Thief）の固有関数===========//
        //===============================================//

        //攻撃時の移動速度定数
        const float THIEF_ATTACK_MOVE_SPEED = 0.2f;

        /// <summary>
        /// 泥棒攻撃開始
        /// </summary>
        /// <param name="gimInfo">ギミック情報</param>
        IEnumerator StartThiefAttack(GimmickInformation gimInfo)
        {
            //反転可能駒のリスト取得
            List<int> invertableSquareIdList = new List<int>();
            foreach (PieceInformation pieceInfo in sPieceInfoArr)
            {
                if (pieceInfo == null) continue;                                //空マス
                if (!pieceInfo.invertable) continue;                            //反転不可
                if (attackPiecesList.Contains(pieceInfo.squareId)) continue;    //すでに攻撃リストにある

                //反転可能リスとに追加
                invertableSquareIdList.Add(pieceInfo.squareId);
            }

            //攻撃可能な駒がなければ終了
            int squareIdLCount = invertableSquareIdList.Count;
            if (squareIdLCount == 0) yield break;

            //攻撃する駒をランダムに選出
            int attackSquareId = invertableSquareIdList[UnityEngine.Random.Range(0, squareIdLCount)];
            attackPiecesList.Add(attackSquareId);

            //移動する座標を算出
            int thiefLine   = squaresMgr.GetLineNumber(gimInfo.nowSquareId);
            int thiefColumn = squaresMgr.GetColumnNumber(gimInfo.nowSquareId);
            int pieseLine   = squaresMgr.GetLineNumber(attackSquareId);
            int pieseColumn = squaresMgr.GetColumnNumber(attackSquareId);
            int moveLine = pieseLine - thiefLine;
            int moveColumn = pieseColumn - thiefColumn;
            Vector3 movePos = new Vector3(moveColumn * SQUARE_DISTANCE, -moveLine * SQUARE_DISTANCE, Z_GIMMICK);

            //移動開始アニメーション
            int attackNumber = 1;
            yield return StartCoroutine(AnimationStart(gimInfo.ani, STATE_NAME_ATTACK + attackNumber.ToString()));

            //攻撃するマスを向く(デフォルトは左向き)
            Vector2 dt = gimInfo.defaultPos - movePos;
            float rad = Mathf.Atan2(dt.y, dt.x);
            float degreeZ = rad * Mathf.Rad2Deg;
            float degreeX = 0.0f;
            if (Mathf.Abs(degreeZ) >= 90)
            {
                degreeX = 180.0f;
                degreeZ = -degreeZ;
            }
            gimInfo.tra.rotation = Quaternion.Euler(degreeX, 0.0f, degreeZ);

            //移動
            yield return StartCoroutine(ConstantSpeedMovement(gimInfo.tra, THIEF_ATTACK_MOVE_SPEED, movePos));
            gimInfo.tra.rotation = DEFAULT_QUEST;

            //攻撃前左右確認アニメーション
            attackNumber++;
            yield return StartCoroutine(AnimationStart(gimInfo.ani, STATE_NAME_ATTACK + attackNumber.ToString()));

            //駒の反転開始
            int generateColor = piecesMgr.GetRandomPieceColor(piecesMgr.GetSquarePieceColorId(attackSquareId));
            StartCoroutine(piecesMgr.ReversingPieces(attackSquareId, generateColor));

            //攻撃アニメーション
            attackNumber++;
            yield return StartCoroutine(AnimationStart(gimInfo.ani, STATE_NAME_ATTACK + attackNumber.ToString()));

            //帰るマスを向く
            dt = movePos - gimInfo.defaultPos ;
            rad = Mathf.Atan2(dt.y, dt.x);
            degreeZ = rad * Mathf.Rad2Deg;
            degreeX = 0.0f;
            if (Mathf.Abs(degreeZ) >= 90)
            {
                degreeX = 180.0f;
                degreeZ = -degreeZ;
            }
            gimInfo.tra.rotation = Quaternion.Euler(degreeX, 0.0f, degreeZ);

            //移動
            yield return StartCoroutine(ConstantSpeedMovement(gimInfo.tra, THIEF_ATTACK_MOVE_SPEED, gimInfo.defaultPos));
            gimInfo.tra.rotation = DEFAULT_QUEST;

            //待機アニメーション
            LoopAnimationStart(gimInfo.ani);
        }


        //===============================================//
        //============竜巻（Tornado）の固有関数==========//
        //===============================================//

        //竜巻が攻撃するマス番号配列のインデックス番号
        enum TornadoAttackInfoIndex
        {
            AttackFirst = 0,    //最初に攻撃するマス番号
            AttackSecond,       //2つめに攻撃するマス番号
            AttackThird,        //3つめに攻撃するマス番号
            AttackDirection,    //攻撃方向

            ArrayCount          //配列サイズ
        }
        const int ATK_FIRST  = (int)TornadoAttackInfoIndex.AttackFirst;
        const int ATK_SECOND = (int)TornadoAttackInfoIndex.AttackSecond;
        const int ATK_THIRD  = (int)TornadoAttackInfoIndex.AttackThird;
        const int ATK_COUNT  = ATK_THIRD + 1; //攻撃箇所の数(3)
        const int ATK_DIR    = (int)TornadoAttackInfoIndex.AttackDirection;

        //竜巻が攻撃する情報配列
        List<GimmickInformation> tornadoInfoList;
        int[][] tornadoAttackInfoArr;

        /// <summary>
        /// 竜巻が攻撃する情報の設定
        /// </summary>
        void SetTornadoAttackInfo()
        {
            //攻撃マス番号の設定
            int tornadoCount = tornadoInfoList.Count;
            tornadoAttackInfoArr = new int[tornadoCount][];
            for (int i = 0; i < tornadoCount; i++)
            {
                tornadoAttackInfoArr[i] = new int[(int)TornadoAttackInfoIndex.ArrayCount];

                //周囲8マスが攻撃可能か確認
                int[] piecesIndexArr      = new int[DIRECTIONS_COUNT];      //周辺駒の管理番号
                bool[] atkPossibleSquares = new bool[DIRECTIONS_COUNT];     //攻撃可能
                bool[] squareNull         = new bool[DIRECTIONS_COUNT];     //空マス判定
                int nowSquareId           = tornadoInfoList[i].nowSquareId; //現在のマスID
                foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                {
                    squareNull[(int)dir] = false;
                    atkPossibleSquares[(int)dir] = false;

                    //竜巻が盤の端にある場合は処理スキップ
                    if (!squaresMgr.IsSquareSpecifiedDirection(dir, nowSquareId)) continue;

                    //各方向のマス管理番号取得
                    int piecesIndex = squaresMgr.GetDesignatedDirectionIndex((int)dir, nowSquareId);

                    //すでに攻撃予定駒の場合は処理スキップ
                    if (attackPiecesList.Contains(piecesIndex)) continue;

                    //if (0 <= piecesIndex && piecesIndex < SQUARES_COUNT)   //必要のない条件であると思われる
                    {
                        if (sPieceObjArr[piecesIndex] == null)
                        {
                            //空マス
                            squareNull[(int)dir] = true;
                            atkPossibleSquares[(int)dir] = true;
                        }
                        else if (sPieceInfoArr[piecesIndex] != null && sPieceInfoArr[piecesIndex].invertable)
                        {
                            //反転可能駒
                            atkPossibleSquares[(int)dir] = true;
                        }
                    }

                    //配列に格納
                    piecesIndexArr[(int)dir] = piecesIndex;
                }

                int[]  atkDirNumArr          = new int[ATK_COUNT];               //攻撃箇所の番号
                bool[] atkPossibleDirections = new bool[FOUR_DIRECTIONS_COUNT];  //攻撃可能方向の取得
                int[]  nullSquareCount       = new int[FOUR_DIRECTIONS_COUNT];   //空マスの数
                foreach (FourDirections fourDir in Enum.GetValues(typeof(FourDirections)))
                {
                    switch (fourDir)
                    {
                        //上方向
                        case FourDirections.Up:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Up;         //上
                            atkDirNumArr[ATK_SECOND] = (int)Directions.UpRight;    //右上
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Right;      //右
                            break;
                        //下方向
                        case FourDirections.Down:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Down;       //下
                            atkDirNumArr[ATK_SECOND] = (int)Directions.DownLeft;   //左下
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Left;       //左
                            break;
                        //左方向
                        case FourDirections.Left:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Left;       //左
                            atkDirNumArr[ATK_SECOND] = (int)Directions.UpLeft;     //左上
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Up;         //上
                            break;
                        //右方向
                        case FourDirections.Right:
                            atkDirNumArr[ATK_FIRST]  = (int)Directions.Right;      //右
                            atkDirNumArr[ATK_SECOND] = (int)Directions.DownRight;  //右下
                            atkDirNumArr[ATK_THIRD]  = (int)Directions.Down;       //下
                            break;
                    }

                    int fourDirNum = (int)fourDir;
                    nullSquareCount[fourDirNum] = 0;
                    bool atkPoss = true;　//攻撃可能方向判定フラグ

                    foreach (int atkDirNum in atkDirNumArr)
                    {
                        //空マスのカウント
                        if (squareNull[atkDirNum]) nullSquareCount[fourDirNum]++;

                        //攻撃可能方向判定(全方向攻撃可能でtrue)
                        if (!atkPossibleSquares[atkDirNum]) atkPoss = false;
                    }

                    //攻撃可能方向の設定
                    atkPossibleDirections[fourDirNum] = atkPoss;
                }

                //空マス最小値取得
                int nullMinCount = ATK_COUNT;
                for (int a = 0; a < FOUR_DIRECTIONS_COUNT; a++)
                {
                    if (!atkPossibleDirections[a]) continue;
                    if (nullSquareCount[a] >= nullMinCount) continue;
                    nullMinCount = nullSquareCount[a];
                }

                //使用するパターン番号取得
                List<int> usePatternList = new List<int>();
                for (int a = 0; a < FOUR_DIRECTIONS_COUNT; a++)
                {
                    if (!atkPossibleDirections[a]) continue;
                    if (nullMinCount != nullSquareCount[a]) continue;
                    usePatternList.Add(a);
                }
                int usePossibleCount = usePatternList.Count;
                if (usePossibleCount == 0)
                {
                    tornadoAttackInfoArr[i] = null;
                    continue;
                }
                int usePatternIndex = usePatternList[UnityEngine.Random.Range(0, usePossibleCount)];

                //攻撃方向の設定
                tornadoAttackInfoArr[i][ATK_DIR] = usePatternIndex;
                switch (usePatternIndex)
                {
                    //上方向
                    case (int)FourDirections.Up:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Up];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.UpRight];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Right];
                        break;
                    //下方向
                    case (int)FourDirections.Down:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Down];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.DownLeft];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Left];
                        break;
                    //左方向
                    case (int)FourDirections.Left:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Left];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.UpLeft];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Up];
                        break;
                    //右方向
                    case (int)FourDirections.Right:
                        tornadoAttackInfoArr[i][ATK_FIRST]  = piecesIndexArr[(int)Directions.Right];
                        tornadoAttackInfoArr[i][ATK_SECOND] = piecesIndexArr[(int)Directions.DownRight];
                        tornadoAttackInfoArr[i][ATK_THIRD]  = piecesIndexArr[(int)Directions.Down];
                        break;
                }

                //攻撃駒リストに追加
                attackPiecesList.Add(tornadoAttackInfoArr[i][ATK_FIRST]);
                attackPiecesList.Add(tornadoAttackInfoArr[i][ATK_SECOND]);
                attackPiecesList.Add(tornadoAttackInfoArr[i][ATK_THIRD]);
            }
        }

        /// <summary>
        /// 竜巻の攻撃開始
        /// </summary>
        IEnumerator StartTornadoAttack()
        {
            //攻撃設定
            SetTornadoAttackInfo();

            //コルーチンリスト
            List<Coroutine> coroutineList = new List<Coroutine>();
            Coroutine coroutine;

            //攻撃開始
            int tornadoCount = tornadoInfoList.Count;
            for (int i = 0; i < tornadoCount; i++)
            {
                if (tornadoAttackInfoArr[i] == null) continue;
                string directionName = Enum.GetNames(typeof(FourDirections))[tornadoAttackInfoArr[i][ATK_DIR]];
                coroutine = StartCoroutine(AnimationStart(tornadoInfoList[i].ani, STATE_NAME_ATTACK + directionName));
                coroutineList.Add(coroutine);
            }

            //終了待機
            if (coroutineList.Count == 0) yield break;
            foreach (Coroutine c in coroutineList)
            { yield return c; }

            //メモリ開放
            tornadoInfoList = null;
            tornadoAttackInfoArr = null;
        }

        /// <summary>
        /// 竜巻の攻撃
        /// </summary>
        /// <param name="atkNum">攻撃番号</param>
        public void TornadoAttackPieceChange(GimmickInformation gimInfo, int atkNum)
        {
            int gimIndex = tornadoInfoList.IndexOf(gimInfo);
            int nowPieceColor = piecesMgr.GetSquarePieceColorId(tornadoAttackInfoArr[gimIndex][atkNum]);

            StartCoroutine(piecesMgr.ReversingPieces(tornadoAttackInfoArr[gimIndex][atkNum], piecesMgr.GetRandomPieceColor(nowPieceColor)));
        }
    }
}