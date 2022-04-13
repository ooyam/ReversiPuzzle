using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDefine : MonoBehaviour
{
    //îÌF
    public enum PieceColors
    {
        Blue = 0,  //ÂF
        Red,       //ÔF
        Yellow,    //©F
        Green,     //ÎF
        Violet,    //F
        Orange,    //òF
        Pink,      //F
        LightBlue  //F
    }

    //M~bN
    public enum Gimmicks
    {
        Balloon = 0,          //D
        Balloon_Color,        //D(F)
        DiscoBall,            //~[{[
        Stone,                //Î
        Flower,               //í¨åQ¨¨Ô
        ColorFrame,           //FÌg
        ColorFrame_Blinking,  //FÌg(_Å)
        ColorFrame_Thick,     //FÌg(¾)
        Hamster,              //nX^[
        GreenWorm,            //Â
        GreenWorm_Color,      //Â(F)
        Cage,                 //B
        NumberTag,            //ÔD
        Thief,                //D_
        Steel,                //S
        FireworksBox,         //ÔÎ 
        RocketBox,            //Pbg 
        Tornado               //³ª
    }

    //ìACe
    public enum SupportItems
    {
        Firework = 0,  //ÔÎ 
        Rocket         //Pbg 
    }

    //ûü
    public enum Directions
    {
        Up = 0,     //ã
        UpRight,    //Eã
        Right,      //E
        DownRight,  //Eº
        Down,       //º
        DownLeft,   //¶º
        Left,       //¶
        UpLeft      //¶ã
    }

    //Äpè
    public const int BOARD_COLUMN_COUNT = 8;       //{[hñ
    public const int BOARD_LINE_COUNT   = 8;       //{[hs
    public const int NULL_NUMBER        = -99;     //nullÌãpè(int^Ånullðãüµ½¢êÉgp)
    public const float SQUARE_DISTANCE  = 0.73f;   //1}XÌ£
    public const float PIECE_DEFAULT_SCALE = 0.6f; //îÌXP[
    public static readonly Vector3 PIECE_DEFAULT_POS     = new Vector3(0.0f, 0.0f, -0.1f);       //îÌî{ÀW
    public static readonly Quaternion PIECE_GENERATE_QUA = Quaternion.Euler(0.0f, -90.0f, 0.0f); //îÌ¶¬Ìpx

    //î½]
    public static readonly Vector3 REVERSE_PIECE_ROT_SPEED              = new Vector3(0.0f, 10.0f, 0.0f);  //î½]¬x
    public static readonly Vector3 REVERSE_PIECE_SWITCH_ROT             = new Vector3(0.0f, 90.0f, 0.0f);  //½]îØèÖ¦px
    public static readonly Vector3 REVERSE_PIECE_FRONT_ROT              = Vector3.zero;                    //î³Ê
    public static readonly WaitForSeconds PIECE_REVERSAL_INTERVAL       = new WaitForSeconds(0.05f);       //îÌ½]Ôu
    public static readonly WaitForSeconds PIECE_GROUP_REVERSAL_INTERVAL = new WaitForSeconds(0.1f);        //îO[vÌ½]Ôu
    public const float REVERSE_PIECE_SCALING_SPEED = 0.02f;  //gk¬x
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.8f;   //gåÌXP[

    //îjó
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //gk¬x
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //jóÌXP[

    //îzu
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;  //gk¬x
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;   //gåÌXP[
    public const float PUT_PIECE_MOVE_START_Z  = -0.3f;  //Ú®JnzÀW(localPosition)
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;   //Ú®¬x
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;   //Ò@îÌXCh¬x

    //îº
    public static readonly Vector3 FALL_PIECE_GENERATE_POS = new Vector3(PIECE_DEFAULT_POS.x, SQUARE_DISTANCE * BOARD_LINE_COUNT, PIECE_DEFAULT_POS.z);  //î½]¬x
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //º¬x
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //ºÁ¬


    //tO
    public static bool GAME_START            = false;  //Q[JnH
    public static bool GAME_OVER             = false;  //Q[I[o[H
    public static bool GAME_CLEAR            = false;  //Q[NAH
    public static bool NOW_PUTTING_PIECES    = false;  //îzuH
    public static bool NOW_REVERSING_PIECES  = false;  //î½]H
    public static bool NOW_DESTROYING_PIECES = false;  //îjóH
    public static bool NOW_FALLING_PIECES    = false;  //îºH

    //tOZbg
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

    //M~bNîñzñÌCfbNXÔ
    public const int SQUARE  = 0;
    public const int GIMMICK = 1;
    public const int COLOR   = 2;

    //M~bNIuWFNgÌ^O
    public const string GIMMICK_TAG = "Gimmick";

    //Xe[WÊè
    public static int     STAGE_NUMBER;        //Xe[WÔ
    public static int     USE_PIECE_COUNT;     //gpîÌíÞ
    public static int[]   HIDE_SQUARE_ARR;     //ñ\¦}XÌÇÔ
    public static int[][] GIMMICK_INFO_ARR;    //M~bNÌíÞÆ}XÌÇÔ

    //Xe[WÝè
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