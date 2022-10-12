using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Option;

public class PuzzleDefine : MonoBehaviour
{
    //----------------パズル汎用定数----------------//

    //駒の色
    public enum Colors
    {
        Blue,           //青
        Red,            //赤
        Yellow,         //黄
        Green,          //緑
        Violet,         //紫
        Orange          //橙
    }
    public const int COLORLESS_ID = -1; //無色(管理番号)
    public static readonly int COLORS_COUNT = Enum.GetValues(typeof(Colors)).Length; //色の数

    //ギミック
    public enum Gimmicks
    {
        Balloon,              //風船
        Balloon_Color,        //風船(色)
        Jewelry,              //宝石
        Wall,                 //壁
        Flower,               //種→蕾→お花
        Frame,                //枠
        Frame_Color,          //枠(色)
        Frame_Color_Change,   //枠(色変化)
        Hamster,              //ハムスター
        GreenWorm,            //青虫
        GreenWorm_Color,      //青虫(色)
        Cage,                 //檻
        NumberTag,            //番号札
        Thief,                //泥棒
        Steel,                //鋼
        FireworksBox,         //花火箱
        RocketBox,            //ロケット箱
        Tornado               //竜巻
    }
    public static readonly int GIMMICKS_COUNT = Enum.GetValues(typeof(Gimmicks)).Length;

    //援護アイテム
    public enum SupportItems
    {
        Duck,           //アヒル
        Firework,       //花火
        RocketLine,     //ロケット(横)
        RocketColumn,   //ロケット(縦)
        Star            //星
    }
    public static readonly int SUPPORT_ITEMS_COUNT = Enum.GetValues(typeof(SupportItems)).Length;

    //8方向
    public enum Directions
    {
        Up,         //上
        Down,       //下
        Left,       //左
        Right,      //右
        UpLeft,     //左上
        UpRight,    //右上
        DownLeft,   //左下
        DownRight   //右下
    }
    public static readonly int DIRECTIONS_COUNT = Enum.GetValues(typeof(Directions)).Length;

    //4方向
    public enum FourDirections
    {
        Up,         //上
        Down,       //下
        Left,       //左
        Right       //右
    }
    public static readonly int FOUR_DIRECTIONS_COUNT = Enum.GetValues(typeof(FourDirections)).Length;

    //マス
    public enum Squares
    {
        A1, A2, A3, A4, A5, A6, A7, A8,
        B1, B2, B3, B4, B5, B6, B7, B8,
        C1, C2, C3, C4, C5, C6, C7, C8,
        D1, D2, D3, D4, D5, D6, D7, D8,
        E1, E2, E3, E4, E5, E6, E7, E8,
        F1, F2, F3, F4, F5, F6, F7, F8,
        G1, G2, G3, G4, G5, G6, G7, G8,
        H1, H2, H3, H4, H5, H6, H7, H8
    }

    //Z座標
    public const float Z_ZERO           = 0f;   //0
    public const float Z_PIECE          = -1f;  //駒
    public const float Z_GIMMICK        = -2f;  //ギミック(駒として管理しない)
    public const float Z_FRONT_GIMMICK  = -3f;  //より手前のギミック
    public const float Z_MOST_FRONT     = -4f;  //最も手前

    //オブジェクトのタグ
    public const string GIMMICK_TAG      = "Gimmick";       //ギミック
    public const string PIECE_TAG        = "Piece";         //駒
    public const string SUPPORT_ITEM_TAG = "SupportItem";   //援護アイテム

    //桁数判定用
    public const int TEN     = 10;
    public const int HUNDRED = 100;

    //汎用
    public const int    BOARD_COLUMN_COUNT      = 8;                                        //ボード列数
    public const int    BOARD_LINE_COUNT        = 8;                                        //ボード行数
    public const int    SQUARES_COUNT           = BOARD_LINE_COUNT * BOARD_COLUMN_COUNT;    //ボード総数
    public const float  SQUARE_DISTANCE         = 1.46f;                                    //マスの距離
    public const float  SQUARE_DISTANCE_HALF    = SQUARE_DISTANCE / 2.0f;                   //半マスの距離
    public const float  PIECE_DEFAULT_SCALE     = 0.65f;                                    //駒のスケール

    public static readonly Vector3 PIECE_DEFAULT_POS        = new Vector3(0.0f, 0.0f, Z_PIECE);     //駒の基本座標
    public static readonly Quaternion PIECE_GENERATE_QUEST  = Quaternion.Euler(0.0f, -90.0f, 0.0f); //駒の生成時の角度
    public static readonly Quaternion DEFAULT_QUEST         = Quaternion.Euler(0.0f, 00.0f, 0.0f);  //基本の角度

    public static readonly Color COLOR_PRIMARY    = new Color(1.0f, 1.0f, 1.0f, 1.0f);               //原色
    public static readonly Color COLOR_ALPHA_ZERO = new Color(1.0f, 1.0f, 1.0f, 0.0f);               //透明
    public static readonly Color[] COLOR_FADE_OUT = new Color[] { COLOR_PRIMARY, COLOR_ALPHA_ZERO }; //フェードアウト
    public static readonly Color[] COLOR_FADE_IN  = new Color[] { COLOR_ALPHA_ZERO, COLOR_PRIMARY }; //フェードイン


    //----------------演出定数----------------//

    //マス色
    public static readonly Color SQUARE_BLUE   = new Color(0.6f, 0.6f, 1.0f, 1.0f);   //青
    public static readonly Color SQUARE_RED    = new Color(1.0f, 0.6f, 0.6f, 1.0f);   //赤
    public static readonly Color SQUARE_YELLOW = new Color(1.0f, 1.0f, 0.6f, 1.0f);   //黄
    public static readonly Color SQUARE_GREEN  = new Color(0.6f, 1.0f, 0.6f, 1.0f);   //緑
    public static readonly Color SQUARE_VIOLET = new Color(1.0f, 0.6f, 1.0f, 1.0f);   //紫
    public static readonly Color SQUARE_ORANGE = new Color(1.0f, 0.6f, 0.3f, 1.0f);   //橙
    public static readonly Color SQUARE_BLACK  = new Color(0.6f, 0.6f, 0.6f, 1.0f);   //黒
    public static readonly Color SQUARE_WHITE  = new Color(1.0f, 1.0f, 1.0f, 1.0f);   //白

    //マスの色変化速度
    public const float SQUARE_CHANGE_SPEED = 0.3f;

    //駒反転
    public static readonly Vector3 REVERSE_PIECE_ROT_SPEED              = new Vector3(0.0f, 10.0f, 0.0f);  //駒反転速度
    public static readonly Vector3 REVERSE_PIECE_SWITCH_ROT             = new Vector3(0.0f, 90.0f, 0.0f);  //反転中駒切り替え角度
    public static readonly Vector3 REVERSE_PIECE_FRONT_ROT              = Vector3.zero;                    //駒正面
    public static readonly WaitForSeconds PIECE_REVERSAL_INTERVAL       = new WaitForSeconds(0.05f);       //駒の反転間隔
    public static readonly WaitForSeconds PIECE_GROUP_REVERSAL_INTERVAL = new WaitForSeconds(0.1f);        //駒グループの反転間隔
    public const float REVERSE_PIECE_SCALING_SPEED = 0.02f;  //拡縮速度
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.9f;   //拡大時のスケール

    //駒破壊
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //拡縮速度
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //破壊時のスケール

    //駒配置
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;         //拡縮速度
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;          //拡大時のスケール
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;          //移動速度
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;          //待機駒のスライド速度
    public static readonly Vector3 PUT_PIECE_MOVE_TARGET_POS = new Vector3(0.0f, 0.0f, Z_MOST_FRONT);   //移動目標座標

    //駒落下
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //落下速度
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //落下加速



    //----------------ステージ定数----------------//

    //目標情報配列のインデクス番号
    enum TargetInfoIndex
    {
        Object, //オブジェクト
        Color,  //色
        Count,  //数量

        Length  //情報配列サイズ
    }
    public static readonly int TARGET_INFO_OBJ      = (int)TargetInfoIndex.Object;  //オブジェクト
    public static readonly int TARGET_INFO_COLOR    = (int)TargetInfoIndex.Color;   //色
    public static readonly int TARGET_INFO_COUNT    = (int)TargetInfoIndex.Count;   //数量
    public static readonly int TARGET_INFO_LENGTH   = (int)TargetInfoIndex.Length;  //配列の長さ

    //落下ギミック情報配列のインデクス番号
    enum FallGimmickInfoIndex
    {
        Type,    //ギミックの種類
        Color,      //色
        Count,      //数量

        Length      //情報配列サイズ
    }
    public static readonly int FALL_GMCK_TYPE    = (int)FallGimmickInfoIndex.Type;       //ギミックの種類
    public static readonly int FALL_GMCK_COLOR   = (int)FallGimmickInfoIndex.Color;      //色
    public static readonly int FALL_GMCK_COUNT   = (int)FallGimmickInfoIndex.Count;      //数量
    public static readonly int FALL_GMCK_LENGTH  = (int)FallGimmickInfoIndex.Length;     //配列の長さ

    //配置ギミック情報配列のインデックス番号
    enum SetGimmickInfoIndex
    {
        Square,     //配置マス
        Type,       //ギミックの種類
        Color,      //指定色
        Group,      //管理グループ
        Width,      //横幅
        Height,     //高さ
        Quantity,   //指定量
        Order,      //指定順番

        Length      //情報配列サイズ
    }
    public static readonly int SET_GMCK_SQUARE   = (int)SetGimmickInfoIndex.Square;      //配置マス
    public static readonly int SET_GMCK_TYPE     = (int)SetGimmickInfoIndex.Type;        //ギミックの種類
    public static readonly int SET_GMCK_COLOR    = (int)SetGimmickInfoIndex.Color;       //指定色
    public static readonly int SET_GMCK_GROUP    = (int)SetGimmickInfoIndex.Group;       //管理グループ
    public static readonly int SET_GMCK_WIDTH    = (int)SetGimmickInfoIndex.Width;       //横幅
    public static readonly int SET_GMCK_HEIGHT   = (int)SetGimmickInfoIndex.Height;      //高さ
    public static readonly int SET_GMCK_QUANTITY = (int)SetGimmickInfoIndex.Quantity;    //指定量
    public static readonly int SET_GMCK_ORDER    = (int)SetGimmickInfoIndex.Order;       //指定順番
    public static readonly int SET_GMCK_LENGTH   = (int)SetGimmickInfoIndex.Length;      //配列の長さ
    public const int NOT_NUM = -1;  //各項目番号指示なし定数

    private const int TURN_MAX = 99;    //ターン最大数


    //----------------リソースデータ----------------//

    public  static GimmicksData GIMMICKS_DATA { get; private set; }  //ギミックデータ


    //----------------ステージ別スタティック変数----------------//

    //パズルシーンフラグ
    public enum PuzzleFlag
    {
        GamePreparation,        //ゲーム準備中？
        GameOver,               //ゲームオーバー？
        GameClear,              //ゲームクリア？
        TurnRecovered,          //ターン回復済？(一度ゲームーバーした)
        Uncontinuable,          //ゲーム継続不可能
        NowPuttingPieces,       //駒配置中？
        NowReversingPieces,     //駒反転中？
        NowDestroyingPieces,    //駒破壊中？
        NowFallingPieces,       //駒落下中？
        NowGimmickDestroyWait,  //ギミック破壊待機中？
        NowGimmickStateChange,  //ギミック状態変化中？
        NowSupportItemUse,      //援護アイテム使用中?
        NowSupportItemReady,    //援護アイテム準備中?
        NowTurnEndProcessing,   //ターン終了処理中？
        NowOptionView,          //オプション表示中？
        NowForcedTutorial,      //強制チュートリアル中？

        Length   //フラグ配列サイズ
    }
    static readonly bool[] PuzzleFlags = new bool[(int)PuzzleFlag.Length];

    //ステージ別設定項目
    public static int     STAGE_NUMBER          { get; private set; }   //ステージ番号
    public static int[]   USE_COLOR_TYPE_ARR    { get; private set; }   //使用色の種類
    public static int     USE_COLOR_COUNT       { get; private set; }   //使用色の数
    public static int[]   HIDE_SQUARE_ARR       { get; private set; }   //非表示マスの管理番号
    public static int[][] GIMMICKS_INFO_ARR     { get; private set; }   //配置ギミックの種類とマスの管理番号
    public static int     GIMMICKS_DEPLOY_COUNT { get; private set; }   //初期ギミックの配置数
    public static int     GIMMICKS_GROUP_COUNT  { get; private set; }   //ギミックのグループの設定数
    public static int[][] FALL_GMCK_INFO_ARR    { get; private set; }   //落下ギミックの種類と数量
    public static int     FALL_GMCK_OBJ_COUNT   { get; private set; }   //落下ギミックの総数
    public static int[][] TARGETS_INFO_ARR      { get; private set; }   //目標情報
    public static int     TARGETS_COUNT         { get; private set; }   //目標のオブジェクト数
    public static int     TURN_COUNT            { get; private set; }   //ターン数


    //=================================================//
    //----------------------関数-----------------------//
    //=================================================//

    /// <summary>
    /// フラグ取得
    /// </summary>
    /// <param name="_flag">フラグタイプ</param>
    public static bool GetFlag(PuzzleFlag _flag) => PuzzleFlags[(int)_flag];

    /// <summary>
    /// フラグON
    /// </summary>
    /// <param name="_flag">フラグタイプ</param>
    public static void FlagOn(PuzzleFlag _flag) => PuzzleFlags[(int)_flag] = true;

    /// <summary>
    /// フラグOFF
    /// </summary>
    /// <param name="_flag">フラグタイプ</param>
    public static void FlagOff(PuzzleFlag _flag) => PuzzleFlags[(int)_flag] = false;

    /// <summary>
    /// フラグリセット
    /// </summary>
    public static void FlagReset()
    {
        for (int i = 0; i < (int)PuzzleFlag.Length; i++)
        { PuzzleFlags[i] = false; }
    }

    /// <summary>
    /// 操作可能確認
    /// </summary>
    /// <returns></returns>
    public static bool IsOperable()
    {
        for (int i = 0; i < (int)PuzzleFlag.Length; i++)
        {
            switch (i)
            {
                //操作に関係のないフラグは無視
                case (int)PuzzleFlag.TurnRecovered:         //ターン回復済？(一度ゲームーバーした)
                case (int)PuzzleFlag.NowSupportItemReady:   //援護アイテム準備中？
                    continue;
            }
            if (PuzzleFlags[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// リソースデータ取得
    /// </summary>
    public static void LoadResourcesData()
    {
        GIMMICKS_DATA = Resources.Load("GimmicksData") as GimmicksData;
    }

    /// <summary>
    /// ステージ設定
    /// </summary>
    public static void StageSetting()
    {
        //ステージ設定取得
        StagesDataData stageData = GameManager.SelectStageData;

        //ステージ番号設定
        STAGE_NUMBER = stageData.Id;

        //使用色
        USE_COLOR_TYPE_ARR = stageData.Usecolor;
        USE_COLOR_COUNT = USE_COLOR_TYPE_ARR.Length;

        //非表示マス
        HIDE_SQUARE_ARR = stageData.Hidesqr;

        //ターン数
        TurnSet(stageData.Turn);

        //強制表示するチュートリアルタイプ
        OptionManager.ForcedTutorialType = stageData.TUTORIALTYPE;

        //配置ギミック, 落下ギミック,目標の取得用リスト・定数設定
        const string setGimInfoName     = "Setgimmicks_";
        const string fallGimInfoName    = "Fallgimmicks_";
        const string targetInfoName     = "Tagets_";
        List<int[]> setGimList          = new List<int[]>();
        List<int[]> fallGimList         = new List<int[]>();
        List<int[]> piecesTargetsList   = new List<int[]>();

        //StagesDataDataクラスのメンバプロパティ(変数)を取得
        System.Reflection.PropertyInfo[] properties = typeof(StagesDataData).GetProperties();
        foreach (System.Reflection.PropertyInfo pro in properties)
        {
            //配置ギミック情報取得
            if (pro.Name.Contains(setGimInfoName))
            {
                //変数名に{setGimInfoName}が含まれているものを取得
                int[] gimInfoArr = pro.GetValue(stageData) as int[];
                if (gimInfoArr[0] < 0) continue;

                //固定長配列に修正
                int[] infoArr = new int[SET_GMCK_LENGTH];
                for (int i = 0; i < SET_GMCK_LENGTH; i++)
                {
                    if (gimInfoArr.Length > i) infoArr[i] = gimInfoArr[i];
                    else infoArr[i] = NOT_NUM;
                }
                setGimList.Add(infoArr);
            }
            //落下ギミック情報取得
            else if (pro.Name.Contains(fallGimInfoName))
            {
                int[] gimInfoArr = pro.GetValue(stageData) as int[];
                if (gimInfoArr[0] < 0) continue;
                fallGimList.Add(gimInfoArr);
            }
            //目標情報取得(駒のみ)
            else if (pro.Name.Contains(targetInfoName))
            {
                int[] targetInfoArr = pro.GetValue(stageData) as int[];
                if (targetInfoArr[0] < 0) continue;

                //固定長配列に修正
                int[] infoArr = new int[TARGET_INFO_LENGTH];
                for (int i = 0; i < TARGET_INFO_LENGTH; i++)
                {
                    if (i == TARGET_INFO_OBJ) infoArr[i] = -1;  //駒のオブジェクト番号は0未満を指定
                    else infoArr[i] = targetInfoArr[i - 1];
                }
                piecesTargetsList.Add(infoArr);
            }
        }

        //配置ギミック情報設定
        GIMMICKS_INFO_ARR = setGimList.ToArray();
        GIMMICKS_DEPLOY_COUNT = GIMMICKS_INFO_ARR.Length;
        List<int[]> targetsList = new List<int[]>();
        List<int> usedGroupNum = new List<int>();
        GIMMICKS_GROUP_COUNT = 0;
        foreach (int[] gimmickInfo in GIMMICKS_INFO_ARR)
        {
            //ギミックのグループ数取得
            if (gimmickInfo[SET_GMCK_GROUP] > NOT_NUM && !usedGroupNum.Contains(gimmickInfo[SET_GMCK_GROUP]))
            {
                usedGroupNum.Add(GIMMICKS_GROUP_COUNT);
                GIMMICKS_GROUP_COUNT++;

                //目標追加（色枠系はここで追加）
                TargetAdd(gimmickInfo[SET_GMCK_TYPE], gimmickInfo[SET_GMCK_COLOR], 1);
            }

            //色枠系はスキップ（グループ単位で1つ扱いの為例外）
            switch (gimmickInfo[SET_GMCK_TYPE])
            {
                //色枠系はスキップ（グループ単位で1つ扱いの為例外）
                case (int)Gimmicks.Frame:
                case (int)Gimmicks.Frame_Color:
                case (int)Gimmicks.Frame_Color_Change:
                    break;

                //目標追加
                default:
                    TargetAdd(gimmickInfo[SET_GMCK_TYPE], gimmickInfo[SET_GMCK_COLOR], 1);
                    break;
            }
        }

        //落下ギミック情報設定
        FALL_GMCK_INFO_ARR = fallGimList.ToArray();
        foreach (int[] fallGmckInfo in FALL_GMCK_INFO_ARR)
        {
            FALL_GMCK_OBJ_COUNT += fallGmckInfo[FALL_GMCK_COUNT];
        }

        //落下ギミックの目標追加
        foreach (int[] gimmickInfo in FALL_GMCK_INFO_ARR)
        {
            TargetAdd(gimmickInfo[FALL_GMCK_TYPE], gimmickInfo[FALL_GMCK_COLOR], gimmickInfo[FALL_GMCK_COUNT]);
        }

        //目標駒情報設定
        targetsList.AddRange(piecesTargetsList);    //駒とギミックの目標リストを結合
        TARGETS_INFO_ARR = targetsList.ToArray();
        TARGETS_COUNT = TARGETS_INFO_ARR.Length;

        //目標の追加
        void TargetAdd(int _gimTypeId, int _colorId, int _addCount)
        {
            //色番号修正
            switch (_gimTypeId)
            {
                //色指定があるが目標を色別に設定しないもの
                case (int)Gimmicks.Jewelry:             //宝石
                case (int)Gimmicks.Frame_Color_Change:  //色枠(色変化)
                case (int)Gimmicks.Cage:                //檻
                    _colorId = COLORLESS_ID;
                    break;
            }

            //目標設定(配置ギミック)
            for (int i = 0; i < targetsList.Count; i++)
            {
                //オブジェクト番号と色番号が同じ場合
                if (targetsList[i][TARGET_INFO_OBJ] == _gimTypeId &&
                    targetsList[i][TARGET_INFO_COLOR] == _colorId)
                {
                    //追加して以降の処理をスキップ
                    targetsList[i][TARGET_INFO_COUNT] += _addCount;
                    return;
                }
            }

            //対象ギミックの配列が未作成だった場合、新規作成
            int[] newTragets = new int[TARGET_INFO_LENGTH];
            newTragets[TARGET_INFO_OBJ] = _gimTypeId;
            newTragets[TARGET_INFO_COLOR] = _colorId;
            newTragets[TARGET_INFO_COUNT] = _addCount;
            targetsList.Add(newTragets);
        }
    }

    /// <summary>
    /// ターン設定
    /// </summary>
    /// <param name="_turn"></param>
    public static void TurnSet(int _turn)
    {
        TURN_COUNT = Mathf.Clamp(_turn, 0, TURN_MAX);
    }
}