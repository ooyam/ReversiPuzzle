using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleDefine : MonoBehaviour
{
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
        Steel,                //鉄
        FireworksBox,         //花火箱
        RocketBox,            //ロケット箱
        Tornado               //竜巻
    }

    //援護アイテム
    public enum SupportItems
    {
        Firework,      //花火箱
        Rocket         //ロケット箱
    }

    //方向
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
        H1, H2, H3, H4, H5, H6, H7, H8,
    }

    //Z座標
    public const float Z_ZERO    = 0.0f;    //0
    public const float Z_PIECE   = -1.0f;   //駒
    public const float Z_GIMMICK = -2.0f;   //ギミック(駒として管理しない)

    //桁数判定用
    public const int TEN     = 10;
    public const int HUNDRED = 100;

    //汎用定数
    public const int BOARD_COLUMN_COUNT = 8;            //ボード列数
    public const int BOARD_LINE_COUNT = 8;              //ボード行数
    public const int INT_NULL = -99;                    //nullの代用定数(int型でnullを代入したい場合に使用)
    public const float SQUARE_DISTANCE_HALF = 0.73f;    //半マスの距離
    public const float PIECE_DEFAULT_SCALE = 0.6f;      //駒のスケール
    public static readonly Vector3 PIECE_DEFAULT_POS = new Vector3(0.0f, 0.0f, Z_PIECE);          //駒の基本座標
    public static readonly Quaternion PIECE_GENERATE_QUA = Quaternion.Euler(0.0f, -90.0f, 0.0f);  //駒の生成時の角度

    public static readonly Color COLOR_PRIMARY    = new Color(1.0f, 1.0f, 1.0f, 1.0f);               //原色
    public static readonly Color COLOR_ALPHA_ZERO = new Color(1.0f, 1.0f, 1.0f, 0.0f);               //透明
    public static readonly Color[] COLOR_FADE_OUT = new Color[] { COLOR_PRIMARY, COLOR_ALPHA_ZERO }; //フェードアウト
    public static readonly Color[] COLOR_FADE_IN  = new Color[] { COLOR_ALPHA_ZERO, COLOR_PRIMARY }; //フェードイン

    //マス色
    public static readonly Color SQUARE_BLUE   = new Color(0.6f, 0.6f, 1.0f, 1.0f);   //青
    public static readonly Color SQUARE_RED    = new Color(1.0f, 0.6f, 0.6f, 1.0f);   //赤
    public static readonly Color SQUARE_YELLOW = new Color(1.0f, 1.0f, 0.6f, 1.0f);   //黄
    public static readonly Color SQUARE_GREEN  = new Color(0.6f, 1.0f, 0.6f, 1.0f);   //緑
    public static readonly Color SQUARE_VIOLET = new Color(1.0f, 0.6f, 1.0f, 1.0f);   //紫
    public static readonly Color SQUARE_ORANGE = new Color(1.0f, 0.6f, 0.3f, 1.0f);   //橙
    public static readonly Color SQUARE_BLACK  = new Color(0.6f, 0.6f, 0.6f, 1.0f);   //黒
    public static readonly Color SQUARE_WHITE  = new Color(1.0f, 1.0f, 1.0f, 1.0f);   //白

    //駒反転
    public static readonly Vector3 REVERSE_PIECE_ROT_SPEED              = new Vector3(0.0f, 10.0f, 0.0f);  //駒反転速度
    public static readonly Vector3 REVERSE_PIECE_SWITCH_ROT             = new Vector3(0.0f, 90.0f, 0.0f);  //反転中駒切り替え角度
    public static readonly Vector3 REVERSE_PIECE_FRONT_ROT              = Vector3.zero;                    //駒正面
    public static readonly WaitForSeconds PIECE_REVERSAL_INTERVAL       = new WaitForSeconds(0.05f);       //駒の反転間隔
    public static readonly WaitForSeconds PIECE_GROUP_REVERSAL_INTERVAL = new WaitForSeconds(0.1f);        //駒グループの反転間隔
    public const float REVERSE_PIECE_SCALING_SPEED = 0.02f;  //拡縮速度
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.8f;   //拡大時のスケール

    //駒破壊
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //拡縮速度
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //破壊時のスケール

    //駒配置
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;  //拡縮速度
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;   //拡大時のスケール
    public const float PUT_PIECE_MOVE_START_Z  = -0.3f;  //移動開始z座標(localPosition)
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;   //移動速度
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;   //待機駒のスライド速度

    //駒落下
    public static readonly Vector3 FALL_PIECE_GENERATE_POS = new Vector3(PIECE_DEFAULT_POS.x, SQUARE_DISTANCE_HALF * BOARD_LINE_COUNT, PIECE_DEFAULT_POS.z);  //駒反転速度
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //落下速度
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //落下加速


    //フラグ
    public static bool GAME_START               = false;  //ゲーム開始？
    public static bool GAME_OVER                = false;  //ゲームオーバー？
    public static bool GAME_CLEAR               = false;  //ゲームクリア？
    public static bool NOW_PUTTING_PIECES       = false;  //駒配置中？
    public static bool NOW_REVERSING_PIECES     = false;  //駒反転中？
    public static bool NOW_DESTROYING_PIECES    = false;  //駒破壊中？
    public static bool NOW_FALLING_PIECES       = false;  //駒落下中？
    public static bool NOW_GIMMICK_DAMAGE_WAIT  = false;  //ギミックダメージ待機中？
    public static bool NOW_GIMMICK_STATE_CHANGE = false;  //ギミック状態変化中？
    public static bool NOW_TURN_END_PROCESSING  = false;  //ターン終了処理中？

    //フラグリセット
    public static void FlagReset()
    {
        GAME_START               = false;
        GAME_OVER                = false;
        GAME_CLEAR               = false;
        NOW_PUTTING_PIECES       = false;
        NOW_REVERSING_PIECES     = false;
        NOW_DESTROYING_PIECES    = false;
        NOW_FALLING_PIECES       = false;
        NOW_GIMMICK_DAMAGE_WAIT  = false;
        NOW_GIMMICK_STATE_CHANGE = false;
        NOW_TURN_END_PROCESSING  = false;
    }

    //ステージ別定数
    public static int     STAGE_NUMBER;         //ステージ番号
    public static int     USE_PIECE_COUNT;      //使用駒の種類数
    public static int[]   HIDE_SQUARE_ARR;      //非表示マスの管理番号
    public static int     GIMMICKS_COUNT;       //ギミックの設定数
    public static int     GIMMICKS_GROUP_COUNT; //ギミックのグループの設定数
    public static int[][] GIMMICKS_INFO_ARR;    //ギミックの種類とマスの管理番号

    //ギミックデータ
    public static GimmicksData GIMMICKS_DATA;

    //ギミックデータ取得
    public static void GimmickSetting()
    {
        GIMMICKS_DATA = Resources.Load("gimmicks_data") as GimmicksData;
    }

    //オブジェクトのタグ
    public const string GIMMICK_TAG = "Gimmick";    //ギミック
    public const string PIECE_TAG   = "Piece";      //駒

    //ギミック情報配列のインデックス番号
    public const int SQUARE   = 0;  //配置マス
    public const int GIMMICK  = 1;  //ギミックの種類
    public const int COLOR    = 2;  //指定色
    public const int GROUP    = 3;  //管理グループ
    public const int WIDTH    = 4;  //横幅
    public const int HEIGHT   = 5;  //高さ
    public const int QUANTITY = 6;  //指定量
    public const int ORDER    = 7;  //指定順番

    public const int DEF_SIZE = 1;  //サイズの初期値
    public const int NOT_NUM  = -1; //各項目番号指示なし

    //ステージ設定
    public static void StageSetting()
    {
        STAGE_NUMBER         = 1;
        USE_PIECE_COUNT      = 6;
        HIDE_SQUARE_ARR      = new int[0];
        GIMMICKS_COUNT       = 8;
        GIMMICKS_INFO_ARR    = new int[GIMMICKS_COUNT][];
        GIMMICKS_INFO_ARR[0] = new int[] { (int)Squares.B1, (int)Gimmicks.Jewelry,   (int)Colors.Blue, NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_INFO_ARR[1] = new int[] { (int)Squares.D1, (int)Gimmicks.Hamster,   COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_INFO_ARR[2] = new int[] { (int)Squares.B6, (int)Gimmicks.NumberTag, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 0 };
        GIMMICKS_INFO_ARR[3] = new int[] { (int)Squares.C6, (int)Gimmicks.NumberTag, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 1 };
        GIMMICKS_INFO_ARR[4] = new int[] { (int)Squares.D6, (int)Gimmicks.NumberTag, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 2 };
        GIMMICKS_INFO_ARR[5] = new int[] { (int)Squares.E6, (int)Gimmicks.NumberTag, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 3 };
        GIMMICKS_INFO_ARR[6] = new int[] { (int)Squares.F6, (int)Gimmicks.NumberTag, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 4 };
        GIMMICKS_INFO_ARR[7] = new int[] { (int)Squares.G6, (int)Gimmicks.NumberTag, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 5 };

        List<int> usedGroupNum = new List<int>();
        foreach (int[] gimmickInfo in GIMMICKS_INFO_ARR)
        {
            if (gimmickInfo[GROUP] > NOT_NUM && !usedGroupNum.Contains(gimmickInfo[GROUP]))
            {
                usedGroupNum.Add(GIMMICKS_GROUP_COUNT);
                GIMMICKS_GROUP_COUNT++;
            }
        }
    }


    //=============ギミック動作=============//

    //宝石フェードアウト速度
    public const float JEWELRY_CHANGE_SPEED = 0.1f;

    //マスの色変化速度
    public const float SQUARE_CHANGE_SPEED = 0.3f;
}