using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleDefine : MonoBehaviour
{
    //îÌF
    public enum Colors
    {
        Blue,           //Â
        Red,            //Ô
        Yellow,         //©
        Green,          //Î
        Violet,         //
        Orange          //ò
    }
    public const int COLORLESS_ID = -1; //łF(ÇÔ)
    public static readonly int COLORS_COUNT = Enum.GetValues(typeof(Colors)).Length; //FÌ

    //M~bN
    public enum Gimmicks
    {
        Balloon,              //D
        Balloon_Color,        //D(F)
        Jewelry,              //óÎ
        Wall,                 //Ç
        Flower,               //íšćQššÔ
        Frame,                //g
        Frame_Color,          //g(F)
        Frame_Color_Change,   //g(FÏ»)
        Hamster,              //nX^[
        GreenWorm,            //Â
        GreenWorm_Color,      //Â(F)
        Cage,                 //B
        NumberTag,            //ÔD
        Thief,                //D_
        Steel,                //|
        FireworksBox,         //ÔÎ 
        RocketBox,            //Pbg 
        Tornado               //łȘ
    }

    //ìACe
    public enum SupportItems
    {
        Duck,           //Aq
        Firework,       //ÔÎ
        RocketLine,     //Pbg(Ą)
        RocketColumn,   //Pbg(c)
        Star            //Ż
    }
    public static readonly int SUPPORT_ITEMS_COUNT = Enum.GetValues(typeof(SupportItems)).Length;

    //8ûü
    public enum Directions
    {
        Up,         //ă
        Down,       //ș
        Left,       //¶
        Right,      //E
        UpLeft,     //¶ă
        UpRight,    //Eă
        DownLeft,   //¶ș
        DownRight   //Eș
    }
    public static readonly int DIRECTIONS_COUNT = Enum.GetValues(typeof(Directions)).Length;

    //4ûü
    public enum FourDirections
    {
        Up,         //ă
        Down,       //ș
        Left,       //¶
        Right       //E
    }
    public static readonly int FOUR_DIRECTIONS_COUNT = Enum.GetValues(typeof(FourDirections)).Length;

    //}X
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

    //ZÀW
    public const float Z_ZERO    = 0.0f;    //0
    public const float Z_PIECE   = -1.0f;   //î
    public const float Z_GIMMICK = -2.0f;   //M~bN(îÆ”ÄÇ”Èą)

    //»èp
    public const int TEN     = 10;
    public const int HUNDRED = 100;

    //Äpè
    public const int    BOARD_COLUMN_COUNT      = 8;                                        //{[hń
    public const int    BOARD_LINE_COUNT        = 8;                                        //{[hs
    public const int    SQUARES_COUNT           = BOARD_LINE_COUNT * BOARD_COLUMN_COUNT;    //{[h
    public const int    INT_NULL                = -99;                                      //nullÌăpè(int^Ćnullđăü”œąêÉgp)
    public const float  SQUARE_DISTANCE         = 1.46f;                                    //}XÌŁ
    public const float  SQUARE_DISTANCE_HALF    = SQUARE_DISTANCE / 2.0f;                   //Œ}XÌŁ
    public const float  PIECE_DEFAULT_SCALE     = 0.65f;                                    //îÌXP[

    public static readonly Vector3 PIECE_DEFAULT_POS        = new Vector3(0.0f, 0.0f, Z_PIECE);     //îÌî{ÀW
    public static readonly Quaternion PIECE_GENERATE_QUEST  = Quaternion.Euler(0.0f, -90.0f, 0.0f); //îÌ¶ŹÌpx
    public static readonly Quaternion DEFAULT_QUEST         = Quaternion.Euler(0.0f, 00.0f, 0.0f);  //î{Ìpx

    public static readonly Color COLOR_PRIMARY    = new Color(1.0f, 1.0f, 1.0f, 1.0f);               //ŽF
    public static readonly Color COLOR_ALPHA_ZERO = new Color(1.0f, 1.0f, 1.0f, 0.0f);               //§Ÿ
    public static readonly Color[] COLOR_FADE_OUT = new Color[] { COLOR_PRIMARY, COLOR_ALPHA_ZERO }; //tF[hAEg
    public static readonly Color[] COLOR_FADE_IN  = new Color[] { COLOR_ALPHA_ZERO, COLOR_PRIMARY }; //tF[hC

    //}XF
    public static readonly Color SQUARE_BLUE   = new Color(0.6f, 0.6f, 1.0f, 1.0f);   //Â
    public static readonly Color SQUARE_RED    = new Color(1.0f, 0.6f, 0.6f, 1.0f);   //Ô
    public static readonly Color SQUARE_YELLOW = new Color(1.0f, 1.0f, 0.6f, 1.0f);   //©
    public static readonly Color SQUARE_GREEN  = new Color(0.6f, 1.0f, 0.6f, 1.0f);   //Î
    public static readonly Color SQUARE_VIOLET = new Color(1.0f, 0.6f, 1.0f, 1.0f);   //
    public static readonly Color SQUARE_ORANGE = new Color(1.0f, 0.6f, 0.3f, 1.0f);   //ò
    public static readonly Color SQUARE_BLACK  = new Color(0.6f, 0.6f, 0.6f, 1.0f);   //
    public static readonly Color SQUARE_WHITE  = new Color(1.0f, 1.0f, 1.0f, 1.0f);   //

    //}XÌFÏ»Źx
    public const float SQUARE_CHANGE_SPEED = 0.3f;

    //îœ]
    public static readonly Vector3 REVERSE_PIECE_ROT_SPEED              = new Vector3(0.0f, 10.0f, 0.0f);  //îœ]Źx
    public static readonly Vector3 REVERSE_PIECE_SWITCH_ROT             = new Vector3(0.0f, 90.0f, 0.0f);  //œ]îŰèÖŠpx
    public static readonly Vector3 REVERSE_PIECE_FRONT_ROT              = Vector3.zero;                    //îłÊ
    public static readonly WaitForSeconds PIECE_REVERSAL_INTERVAL       = new WaitForSeconds(0.05f);       //îÌœ]Ôu
    public static readonly WaitForSeconds PIECE_GROUP_REVERSAL_INTERVAL = new WaitForSeconds(0.1f);        //îO[vÌœ]Ôu
    public const float REVERSE_PIECE_SCALING_SPEED = 0.02f;  //gkŹx
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.9f;   //gćÌXP[

    //îjó
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //gkŹx
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //jóÌXP[

    //îzu
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;             //gkŹx
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;              //gćÌXP[
    public const float PUT_PIECE_MOVE_START_Z  = Z_GIMMICK - 0.1f;  //ÚźJnzÀW(localPosition)
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;              //ÚźŹx
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;              //Ò@îÌXChŹx

    //îș
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //șŹx
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //șÁŹ


    //tO
    public static bool GAME_START                   = false;  //Q[JnH
    public static bool GAME_OVER                    = false;  //Q[I[o[H
    public static bool GAME_CLEAR                   = false;  //Q[NAH
    public static bool NOW_PUTTING_PIECES           = false;  //îzuH
    public static bool NOW_REVERSING_PIECES         = false;  //îœ]H
    public static bool NOW_DESTROYING_PIECES        = false;  //îjóH
    public static bool NOW_FALLING_PIECES           = false;  //îșH
    public static bool NOW_GIMMICK_DESTROY_WAIT     = false;  //M~bNjóÒ@H
    public static bool NOW_GIMMICK_STATE_CHANGE     = false;  //M~bNóÔÏ»H
    public static bool NOW_SUPPORT_ITEM_USE         = false;  //ìACegp?
    public static bool NOW_SUPPORT_ITEM_READY       = false;  //ìACeő?
    public static bool NOW_TURN_END_PROCESSING      = false;  //^[IčH

    //tOZbg
    public static void FlagReset()
    {
        GAME_START                  = false;
        GAME_OVER                   = false;
        GAME_CLEAR                  = false;
        NOW_PUTTING_PIECES          = false;
        NOW_REVERSING_PIECES        = false;
        NOW_DESTROYING_PIECES       = false;
        NOW_FALLING_PIECES          = false;
        NOW_GIMMICK_DESTROY_WAIT    = false;
        NOW_GIMMICK_STATE_CHANGE    = false;
        NOW_SUPPORT_ITEM_USE        = false;
        NOW_SUPPORT_ITEM_READY      = false;
        NOW_TURN_END_PROCESSING     = false;
    }

    //Xe[WÊè
    public static int[]   USE_COLOR_TYPE_ARR;   //gpFÌíȚ
    public static int     USE_COLOR_COUNT;      //gpFÌ
    public static int     STAGE_NUMBER;         //Xe[WÔ
    public static int[]   HIDE_SQUARE_ARR;      //ń\Š}XÌÇÔ
    public static int[][] GIMMICKS_INFO_ARR;    //M~bNÌíȚÆ}XÌÇÔ
    public static int     GIMMICKS_COUNT;       //úM~bNÌĘè
    public static int     GIMMICKS_GROUP_COUNT; //M~bNÌO[vÌĘè
    public static int[][] TARGETS_INFO_ARR;     //ÚWîń
    public static int     TARGETS_COUNT;        //ÚWÌIuWFNg

    //ÚWîńzńÌCfNXÔ
    enum TargetInfoIndex
    {
        Object, //IuWFNg
        Color,  //F
        Count   //M~bNÌíȚ
    }
    public static readonly int TARGET_INFO_OBJ   = (int)TargetInfoIndex.Object; //IuWFNg
    public static readonly int TARGET_INFO_COLOR = (int)TargetInfoIndex.Color;  //F
    public static readonly int TARGET_INFO_COUNT = (int)TargetInfoIndex.Count;  //M~bNÌíȚ

    //M~bNf[^
    public static GimmicksData GIMMICKS_DATA;

    //M~bNf[^æŸ
    public static void GetGimmicksData()
    {
        GIMMICKS_DATA = Resources.Load("gimmicks_data") as GimmicksData;
    }

    //IuWFNgÌ^O
    public const string GIMMICK_TAG      = "Gimmick";       //M~bN
    public const string PIECE_TAG        = "Piece";         //î
    public const string SUPPORT_ITEM_TAG = "SupportItem";   //ìACe

    //M~bNîńzńÌCfbNXÔ
    enum GimmickInfoIndex
    {
        Square,     //zu}X
        Gimmick,    //M~bNÌíȚ
        Color,      //wèF
        Group,      //ÇO[v
        Width,      //Ą
        Height,     //ł
        Quantity,   //wèÊ
        Order       //wèÔ
    }
    public static readonly int SQUARE   = (int)GimmickInfoIndex.Square;     //zu}X
    public static readonly int GIMMICK  = (int)GimmickInfoIndex.Gimmick;    //M~bNÌíȚ
    public static readonly int COLOR    = (int)GimmickInfoIndex.Color;      //wèF
    public static readonly int GROUP    = (int)GimmickInfoIndex.Group;      //ÇO[v
    public static readonly int WIDTH    = (int)GimmickInfoIndex.Width;      //Ą
    public static readonly int HEIGHT   = (int)GimmickInfoIndex.Height;     //ł
    public static readonly int QUANTITY = (int)GimmickInfoIndex.Quantity;   //wèÊ
    public static readonly int ORDER    = (int)GimmickInfoIndex.Order;      //wèÔ

    public const int DEF_SIZE = 1;  //TCYÌúl
    public const int NOT_NUM  = -1; //eÚÔwŠÈ”

    //Xe[WĘè
    public static void StageSetting()
    {
        USE_COLOR_TYPE_ARR = new int[] {
            (int)Colors.Blue,   //Â
            (int)Colors.Red,    //Ô
            (int)Colors.Yellow, //©
            (int)Colors.Green,  //Î
            (int)Colors.Violet, //
            (int)Colors.Orange  //ò
        };
        USE_COLOR_COUNT = USE_COLOR_TYPE_ARR.Length;
        STAGE_NUMBER    = 1;
        HIDE_SQUARE_ARR = new int[] {
            /*
            (int)Squares.A2,
            (int)Squares.B2,
            (int)Squares.C2,
            (int)Squares.D2,
            (int)Squares.E2,
            (int)Squares.F2,
            (int)Squares.G2,
            (int)Squares.H2
            */
        };

        //(Œ)
        int[][] GIMMICKS_dummy     = new int[5][];
        GIMMICKS_dummy[0]  = new int[] { (int)Squares.B1, (int)Gimmicks.Balloon,            COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[1]  = new int[] { (int)Squares.D1, (int)Gimmicks.Balloon_Color,      (int)Colors.Red,  NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[2]  = new int[] { (int)Squares.F1, (int)Gimmicks.Jewelry,            (int)Colors.Red,  NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[3]  = new int[] { (int)Squares.C2, (int)Gimmicks.Wall,               COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[4]  = new int[] { (int)Squares.E2, (int)Gimmicks.Hamster,            COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        /*
        GIMMICKS_dummy[5]  = new int[] { (int)Squares.G2, (int)Gimmicks.NumberTag,          COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 0 };
        GIMMICKS_dummy[6]  = new int[] { (int)Squares.B3, (int)Gimmicks.NumberTag,          COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 1 };
        GIMMICKS_dummy[7]  = new int[] { (int)Squares.D3, (int)Gimmicks.NumberTag,          COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, 2 };
        GIMMICKS_dummy[8]  = new int[] { (int)Squares.F3, (int)Gimmicks.Tornado,            COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[9]  = new int[] { (int)Squares.G7, (int)Gimmicks.Cage,               (int)Colors.Blue, NOT_NUM, 2,        2,        10,      NOT_NUM };
        GIMMICKS_dummy[10] = new int[] { (int)Squares.G3, (int)Gimmicks.Frame,              COLORLESS_ID,     0,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[11] = new int[] { (int)Squares.G4, (int)Gimmicks.Frame,              COLORLESS_ID,     0,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[12] = new int[] { (int)Squares.A1, (int)Gimmicks.Frame_Color,        (int)Colors.Blue, 1,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[13] = new int[] { (int)Squares.B1, (int)Gimmicks.Frame_Color,        (int)Colors.Blue, 1,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[14] = new int[] { (int)Squares.E5, (int)Gimmicks.Frame_Color_Change, (int)Colors.Red,  2,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[15] = new int[] { (int)Squares.F5, (int)Gimmicks.Frame_Color_Change, (int)Colors.Red,  2,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[16] = new int[] { (int)Squares.F6, (int)Gimmicks.Frame_Color_Change, (int)Colors.Red,  2,       DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[17] = new int[] { (int)Squares.B5, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[18] = new int[] { (int)Squares.C5, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[19] = new int[] { (int)Squares.D5, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[20] = new int[] { (int)Squares.B6, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[21] = new int[] { (int)Squares.D6, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[22] = new int[] { (int)Squares.B7, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[23] = new int[] { (int)Squares.C7, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[24] = new int[] { (int)Squares.D7, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[25] = new int[] { (int)Squares.C6, (int)Gimmicks.Steel,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[26] = new int[] { (int)Squares.B4, (int)Gimmicks.Thief,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_dummy[27] = new int[] { (int)Squares.C4, (int)Gimmicks.Thief,              COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        */

        //(Œ)
        int[][] dummy = new int[5][];
        dummy[0]  = new int[] { (int)Gimmicks.Balloon,       COLORLESS_ID,    1 };
        dummy[1]  = new int[] { (int)Gimmicks.Balloon_Color, (int)Colors.Red, 1 };
        dummy[2]  = new int[] { (int)Gimmicks.Jewelry,       COLORLESS_ID,    1 };
        dummy[3]  = new int[] { (int)Gimmicks.Wall,          COLORLESS_ID,    1 };
        dummy[4]  = new int[] { (int)Gimmicks.Hamster,       COLORLESS_ID,    1 };


        //úM~bNîńĘè
        GIMMICKS_INFO_ARR = GIMMICKS_dummy;
        GIMMICKS_COUNT = GIMMICKS_INFO_ARR.Length;
        List<int> usedGroupNum = new List<int>();
        foreach (int[] gimmickInfo in GIMMICKS_INFO_ARR)
        {
            if (gimmickInfo[GROUP] > NOT_NUM && !usedGroupNum.Contains(gimmickInfo[GROUP]))
            {
                usedGroupNum.Add(GIMMICKS_GROUP_COUNT);
                GIMMICKS_GROUP_COUNT++;
            }
        }

        //ÚWîńĘè
        TARGETS_INFO_ARR = dummy;
        TARGETS_COUNT = TARGETS_INFO_ARR.Length;
    }
}