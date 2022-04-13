using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDefine : MonoBehaviour
{
    //駒の色
    public enum PieceColors
    {
        Blue = 0,  //青色
        Red,       //赤色
        Yellow,    //黄色
        Green,     //緑色
        Violet,    //紫色
        Orange,    //橙色
        Pink,      //桃色
        LightBlue  //水色
    }

    //ギミック
    public enum Gimmicks
    {
        Balloon = 0,          //風船
        Balloon_Color,        //風船(色)
        DiscoBall,            //ミラーボール
        Stone,                //石
        Flower,               //種→蕾→お花
        ColorFrame,           //色の枠
        ColorFrame_Blinking,  //色の枠(点滅)
        ColorFrame_Thick,     //色の枠(太)
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
        Firework = 0,  //花火箱
        Rocket         //ロケット箱
    }

    //方向
    public enum Directions
    {
        Up = 0,     //上
        UpRight,    //右上
        Right,      //右
        DownRight,  //右下
        Down,       //下
        DownLeft,   //左下
        Left,       //左
        UpLeft      //左上
    }

    //汎用定数
    public const int BOARD_COLUMN_COUNT = 8;       //ボード列数
    public const int BOARD_LINE_COUNT   = 8;       //ボード行数
    public const int NULL_NUMBER        = -99;     //nullの代用定数(int型でnullを代入したい場合に使用)
    public const float SQUARE_DISTANCE  = 0.73f;   //1マスの距離
    public const float PIECE_DEFAULT_SCALE = 0.6f; //駒のスケール
    public static readonly Vector3 PIECE_DEFAULT_POS     = new Vector3(0.0f, 0.0f, -0.1f);       //駒の基本座標
    public static readonly Quaternion PIECE_GENERATE_QUA = Quaternion.Euler(0.0f, -90.0f, 0.0f); //駒の生成時の角度

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
    public static readonly Vector3 FALL_PIECE_GENERATE_POS = new Vector3(PIECE_DEFAULT_POS.x, SQUARE_DISTANCE * BOARD_LINE_COUNT, PIECE_DEFAULT_POS.z);  //駒反転速度
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //落下速度
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //落下加速


    //フラグ
    public static bool GAME_START            = false;  //ゲーム開始？
    public static bool GAME_OVER             = false;  //ゲームオーバー？
    public static bool GAME_CLEAR            = false;  //ゲームクリア？
    public static bool NOW_PUTTING_PIECES    = false;  //駒配置中？
    public static bool NOW_REVERSING_PIECES  = false;  //駒反転中？
    public static bool NOW_DESTROYING_PIECES = false;  //駒破壊中？
    public static bool NOW_FALLING_PIECES    = false;  //駒落下中？

    //フラグリセット
    public static void FlagReset()
    {
        GAME_START            = false;
        GAME_OVER             = false;
        GAME_CLEAR            = false;
        NOW_PUTTING_PIECES    = false;
        NOW_REVERSING_PIECES  = false;
        NOW_DESTROYING_PIECES = false;
        NOW_FALLING_PIECES    = false;
    }

    //ギミック情報配列のインデックス番号
    public const int SQUARE  = 0;
    public const int GIMMICK = 1;
    public const int COLOR   = 2;

    //ギミックオブジェクトのタグ
    public const string GIMMICK_TAG = "Gimmick";

    //ステージ別定数
    public static int     STAGE_NUMBER;        //ステージ番号
    public static int     USE_PIECE_COUNT;     //使用駒の種類数
    public static int[]   HIDE_SQUARE_ARR;     //非表示マスの管理番号
    public static int[][] GIMMICK_INFO_ARR;    //ギミックの種類とマスの管理番号

    //ステージ設定
    public static void StageSetting()
    {
        STAGE_NUMBER = 1;
        USE_PIECE_COUNT = 8;
        HIDE_SQUARE_ARR = new int[0];
        GIMMICK_INFO_ARR = new int[12][];
        GIMMICK_INFO_ARR[0]  = new int[] { 8,  (int)Gimmicks.Balloon, 0 };
        GIMMICK_INFO_ARR[1]  = new int[] { 9,  (int)Gimmicks.Balloon, 0 };
        GIMMICK_INFO_ARR[2]  = new int[] { 10, (int)Gimmicks.Balloon, 0 };
        GIMMICK_INFO_ARR[3]  = new int[] { 11, (int)Gimmicks.Balloon, 0 };
        GIMMICK_INFO_ARR[4]  = new int[] { 27, (int)Gimmicks.Balloon_Color, 0 };
        GIMMICK_INFO_ARR[5]  = new int[] { 28, (int)Gimmicks.Balloon_Color, 1 };
        GIMMICK_INFO_ARR[6]  = new int[] { 29, (int)Gimmicks.Balloon_Color, 2 };
        GIMMICK_INFO_ARR[7]  = new int[] { 30, (int)Gimmicks.Balloon_Color, 3 };
        GIMMICK_INFO_ARR[8]  = new int[] { 50, (int)Gimmicks.Balloon_Color, 4 };
        GIMMICK_INFO_ARR[9]  = new int[] { 51, (int)Gimmicks.Balloon_Color, 5 };
        GIMMICK_INFO_ARR[10] = new int[] { 52, (int)Gimmicks.Balloon_Color, 6 };
        GIMMICK_INFO_ARR[11] = new int[] { 53, (int)Gimmicks.Balloon_Color, 7 };
    }
}