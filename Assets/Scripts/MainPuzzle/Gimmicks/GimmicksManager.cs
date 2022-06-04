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
        PiecesManager piecesMan;

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

        //アニメーションステート名
        const string STATE_NAME_EMPTY        = "Empty";         //初期状態
        const string STATE_NAME_WAIT         = "Wait";          //待機
        const string STATE_NAME_DAMAGE       = "Damage";        //複数回ダメージ
        const string STATE_NAME_BURST        = "Burst";         //破壊
        const string STATE_NAME_COLOR_CHANGE = "ColorChange";   //色の更新
        const string STATE_NAME_RETURN       = "Return";        //状態を戻す
        const string STATE_NAME_ATTACK       = "Attack";        //攻撃

        //グループギミックのリスト
        List<GameObject>[] frameObjListArr;             //枠オブジェクトリスト(グループ別)
        List<GimmickInformation>[] frameInfoListArr;    //枠オブジェクト情報リスト(グループ別)
        List<int>[] frameSquareIdListArr;               //枠配置マスリスト(グループ別)
        int[] groupColorNumArr;                         //グループ毎の指定色番号

        //サイズ可変ギミックの配列
        GameObject[] cageObjArr;            //檻オブジェクトリスト
        GimmickInformation[] cageInfoArr;   //檻オブジェクト情報リスト
        int[] cageSquareIdArr;              //檻配置マスリスト

        //番号札ギミック用変数
        public static int nextOrder = 0;  //次に破壊する番号

        void Awake()
        {
            //ギミックの設定読み込み
            GimmickSetting();
            StageSetting();
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
            yield return new WaitWhile(() => ani.GetCurrentAnimatorStateInfo(0).IsName(stateName));
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
                //無条件
                case (int)Gimmicks.Balloon:     //風船
                case (int)Gimmicks.Wall:        //壁
                case (int)Gimmicks.Flower:      //花
                case (int)Gimmicks.Hamster:     //ハムスター
                case (int)Gimmicks.Tornado:     //竜巻
                    damage = true;
                    break;

                //色判定
                case (int)Gimmicks.Balloon_Color: //風船(色)
                case (int)Gimmicks.Jewelry:       //宝石
                    if (putPieceColorId == gimmickInfoArr[gimmickIndex].colorId)
                        damage = true;
                    break;

                //順番
                case (int)Gimmicks.NumberTag:
                    if (nextOrder == gimmickInfoArr[gimmickIndex].order)
                    {
                        damage = true;
                        nextOrder++;
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
        public void DamageGimmick(ref int gimmickIndex, int squareIndex)
        {
            string stateName = "";     //ステート名
            GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex]; //ギミックの情報取得

            switch (gimmInfo.id)
            {
                //無条件破壊
                case (int)Gimmicks.Balloon:         //風船
                case (int)Gimmicks.Balloon_Color:   //風船(色)
                case (int)Gimmicks.Jewelry:         //宝石
                case (int)Gimmicks.NumberTag:       //番号札
                case (int)Gimmicks.Tornado:         //竜巻
                    stateName = STATE_NAME_BURST;
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
            //コルーチンリスト
            List<Coroutine> coroutineList = new List<Coroutine>();
            Coroutine coroutine = null;

            //竜巻オブジェクト情報リスト
            tornadoInfoList = new List<GimmickInformation>();

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
                            int oldColorType = Array.IndexOf(USE_COLOR_TYPE_ARR, gimmInfo.colorId);
                            int newColorType = (oldColorType == USE_COLOR_COUNT - 1) ? 0 : oldColorType + 1;
                            gimmInfo.colorId = USE_COLOR_TYPE_ARR[newColorType];
                            Sprite newSprite = jewelrySprArr[gimmInfo.colorId];
                            gimmInfo.spriRenChild[0].sprite = newSprite;

                            //sprit変更
                            coroutine = StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite));
                            coroutineList.Add(coroutine);
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
                                    coroutine = StartCoroutine(AnimationStart(gimmInfo.ani, STATE_NAME_RETURN));
                                    coroutineList.Add(coroutine);
                                }
                            }
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
                                Sprite newSprite = (gimmInfo.tra.localPosition.x == 0.0f) ? frameWidthSprArr[gimmInfo.colorId] : frameHeightSprArr[gimmInfo.colorId];
                                gimmInfo.spriRenChild[0].sprite = newSprite;

                                //sprit変更
                                coroutine = StartCoroutine(SpriteChange(gimmInfo.ani, gimmInfo.spriRen, newSprite));
                                coroutineList.Add(coroutine);

                                //マスの色変更
                                if (!changedSquare.Contains(gimmInfo.startSquareId))
                                {
                                    coroutine = StartCoroutine(piecesMan.SquareColorChange(GetSquareColor(gimmInfo.colorId), gimmInfo.startSquareId, true));
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
                    StartCoroutine(piecesMan.SquareColorChange(color, squareIndex, false));
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
            GameObject frameObj = Instantiate(piecesMan.gimmickPrefabArr[(int)Gimmicks.Frame].prefab[(int)direction]);
            frameObjListArr[groupId].Add(frameObj);
            piecesMan.PlaceGimmick(frameObj, squareIndex);

            //フレームギミックの情報取得
            GimmickInformation gimInfo = frameObj.GetComponent<GimmickInformation>();
            frameInfoListArr[groupId].Add(gimInfo);
            gimInfo.InformationSetting_SquareIndex(squareIndex, INT_NULL, groupId);
            gimmickInfoArr[gimInfo.settingIndex] = gimInfo;

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
                    { StartCoroutine(piecesMan.SquareColorChange(SQUARE_WHITE, i, true)); }
                    frameSquareIdListArr[groupId] = null;
                }

                //次のグループへ
                groupId++;
            }

            //処理が一度も入らなかった(リストが空)場合
            if (frameListNull)
            {
                frameInfoListArr      = null;
                frameObjListArr       = null;
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
                cageObjArr[i]  = Instantiate(piecesMan.gimmickPrefabArr[cageInfo[GIMMICK]].prefab[0]);
                cageInfoArr[i] = cageObjArr[i].GetComponent<GimmickInformation>();
                cageSquareIdArr[i] = cageInfo[SQUARE];
                piecesMan.PlaceGimmick(cageObjArr[i], cageInfo[SQUARE]);
                cageInfoArr[i].InformationSetting_SquareIndex(cageInfo[SQUARE], cageInfo[GIMMICK], NOT_NUM);
                gimmickInfoArr[cageInfoArr[i].settingIndex] = cageInfoArr[i];
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
                { piecesMan.PieceOperationFlagChange(squareId, true); }

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
        //============竜巻（Tornado）の固有関数==========//
        //===============================================//

        //竜巻が攻撃するマス番号配列のインデックス番号
        enum TornadoAttackInfoIndex
        {
            AttackFirst = 0,    //最初に攻撃するマス番号
            AttackSecond,       //最初に攻撃するマス番号
            AttackThird,        //最初に攻撃するマス番号
            AttackDirection,    //攻撃方向

            ArrayCount          //配列サイズ
        }

        //竜巻が攻撃する情報配列
        static List<GimmickInformation> tornadoInfoList;
        static int[][] tornadoAttackInfoArr;

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
                int directionCounts          = Enum.GetValues(typeof(Directions)).Length;
                int[] piecesIndexArr         = new int[directionCounts];    //周辺駒の管理番号
                bool[] attackPossibleSquares = new bool[directionCounts];   //攻撃可能
                int[] nullSquareCount        = new int[directionCounts];    //空マスの数
                foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                {
                    nullSquareCount[(int)dir] = 0;
                    attackPossibleSquares[(int)dir] = false;

                    //竜巻盤の端にある場合
                    switch (dir)
                    {
                        //上
                        case Directions.Up:
                        case Directions.UpLeft:
                        case Directions.UpRight:
                            if (tornadoInfoList[i].nowSquareId % BOARD_LINE_COUNT == 0) continue;
                            break;
                        //下
                        case Directions.Down:
                        case Directions.DownLeft:
                        case Directions.DownRight:
                            if (tornadoInfoList[i].nowSquareId % (BOARD_LINE_COUNT + 1) == 0) continue;
                            break;
                    }
                    switch (dir)
                    {
                        //左
                        case Directions.Left:
                        case Directions.UpLeft:
                        case Directions.DownLeft:
                            if (tornadoInfoList[i].nowSquareId < BOARD_LINE_COUNT) continue;
                            break;
                        //右
                        case Directions.Right:
                        case Directions.UpRight:
                        case Directions.DownRight:
                            if (tornadoInfoList[i].nowSquareId >= SQUARES_COUNT - BOARD_LINE_COUNT) continue;
                            break;
                    }

                    //各方向のマス管理番号取得
                    piecesIndexArr[(int)dir] = piecesMan.GetDesignatedDirectionIndex((int)dir, tornadoInfoList[i].nowSquareId);
                    if (0 <= piecesIndexArr[(int)dir] && piecesIndexArr[(int)dir] < SQUARES_COUNT)
                    {
                        if (pieceObjArr[piecesIndexArr[(int)dir]] == null)
                        {
                            //空マス
                            nullSquareCount[(int)dir]++;
                            attackPossibleSquares[(int)dir] = true;
                        }
                        else if (pieceInfoArr[piecesIndexArr[(int)dir]] != null && pieceInfoArr[piecesIndexArr[(int)dir]].invertable)
                        {
                            //反転可能駒
                            attackPossibleSquares[(int)dir] = true;
                        }
                    }
                }
                
                //攻撃可能方向の取得
                int attackPatternCount = Enum.GetValues(typeof(FourDirections)).Length;
                bool[] attackPossibleDirections = new bool[attackPatternCount];

                //上方向
                if (attackPossibleSquares[(int)Directions.Up] &&        //上
                    attackPossibleSquares[(int)Directions.UpRight] &&   //右上
                    attackPossibleSquares[(int)Directions.Right])       //右
                {
                    attackPossibleDirections[(int)FourDirections.Up] = true;
                }
                //下方向
                if (attackPossibleSquares[(int)Directions.Down] &&      //下
                    attackPossibleSquares[(int)Directions.DownLeft] &&  //左下
                    attackPossibleSquares[(int)Directions.Left])        //左
                {
                    attackPossibleDirections[(int)FourDirections.Down] = true;
                }
                //左方向
                if (attackPossibleSquares[(int)Directions.Left] &&      //左
                    attackPossibleSquares[(int)Directions.UpLeft] &&    //左上
                    attackPossibleSquares[(int)Directions.Up])          //上
                {
                    attackPossibleDirections[(int)FourDirections.Left] = true;
                }
                //右方向
                if (attackPossibleSquares[(int)Directions.Right] &&     //右
                    attackPossibleSquares[(int)Directions.DownRight] && //右下
                    attackPossibleSquares[(int)Directions.Down])        //下
                {
                    attackPossibleDirections[(int)FourDirections.Right] = true;
                }

                //空マス最小値取得
                int nullMinCount = (int)TornadoAttackInfoIndex.ArrayCount - 1;  //方向設定の分を-1
                foreach (int nullCount in nullSquareCount)
                { if (nullCount < nullMinCount) nullMinCount = nullCount; }

                //使用するパターン番号取得
                List<int> usePatternList = new List<int>();
                for (int a = 0; a < attackPatternCount; a++)
                {
                    if (attackPossibleDirections[a])
                    {
                        if (nullMinCount == nullSquareCount[a]) usePatternList.Add(a);
                    }
                }
                int usePossibleCount = usePatternList.Count;
                if (usePossibleCount == 0)
                {
                    tornadoAttackInfoArr[i] = null;
                    continue;
                }
                int usePatternIndex = usePatternList[UnityEngine.Random.Range(0, usePossibleCount)];

                //攻撃方向の設定
                tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackDirection] = usePatternIndex;
                switch (usePatternIndex)
                {
                    //上方向
                    case (int)FourDirections.Up:
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackFirst]  = piecesIndexArr[(int)Directions.Up];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackSecond] = piecesIndexArr[(int)Directions.UpRight];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackThird]  = piecesIndexArr[(int)Directions.Right];
                        break;
                    //下方向
                    case (int)FourDirections.Down:
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackFirst]  = piecesIndexArr[(int)Directions.Down];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackSecond] = piecesIndexArr[(int)Directions.DownLeft];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackThird]  = piecesIndexArr[(int)Directions.Left];
                        break;
                    //左方向
                    case (int)FourDirections.Left:
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackFirst]  = piecesIndexArr[(int)Directions.Left];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackSecond] = piecesIndexArr[(int)Directions.UpLeft];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackThird]  = piecesIndexArr[(int)Directions.Up];
                        break;
                    //右方向
                    case (int)FourDirections.Right:
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackFirst]  = piecesIndexArr[(int)Directions.Right];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackSecond] = piecesIndexArr[(int)Directions.DownRight];
                        tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackThird]  = piecesIndexArr[(int)Directions.Down];
                        break;
                }
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
            Coroutine coroutine = null;

            //攻撃開始
            int tornadoCount = tornadoInfoList.Count;
            for (int i = 0; i < tornadoCount; i++)
            {
                if (tornadoAttackInfoArr[i] == null) continue;
                string directionName = Enum.GetNames(typeof(FourDirections))[tornadoAttackInfoArr[i][(int)TornadoAttackInfoIndex.AttackDirection]];
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
        /// <param name="attackNum">攻撃番号</param>
        public void TornadoAttackPieceChange(GimmickInformation gimInfo, int attackNum)
        {
            int gimIndex = tornadoInfoList.IndexOf(gimInfo);
            int nowPieceColor = piecesMan.GetSquarePieceColorId(tornadoAttackInfoArr[gimIndex][attackNum]);

            //今の駒と同色だった場合は10回まで再試行する
            int generateColorId = 0;
            for (int i = 0; i < 10; i++)
            {
                generateColorId = piecesMan.GetRandomPieceColor();
                if (nowPieceColor != generateColorId) break;
            }
            StartCoroutine(piecesMan.ReversingPieces(tornadoAttackInfoArr[gimIndex][attackNum], generateColorId));
        }
    }
}