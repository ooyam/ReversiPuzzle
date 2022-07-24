using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleDefine : MonoBehaviour
{
    //��̐F
    public enum Colors
    {
        Blue,           //��
        Red,            //��
        Yellow,         //��
        Green,          //��
        Violet,         //��
        Orange          //��
    }
    public const int COLORLESS_ID = -1; //���F(�Ǘ��ԍ�)
    public static readonly int COLORS_COUNT = Enum.GetValues(typeof(Colors)).Length; //�F�̐�

    //�M�~�b�N
    public enum Gimmicks
    {
        Balloon,              //���D
        Balloon_Color,        //���D(�F)
        Jewelry,              //���
        Wall,                 //��
        Flower,               //�큨�Q������
        Frame,                //�g
        Frame_Color,          //�g(�F)
        Frame_Color_Change,   //�g(�F�ω�)
        Hamster,              //�n���X�^�[
        GreenWorm,            //��
        GreenWorm_Color,      //��(�F)
        Cage,                 //�B
        NumberTag,            //�ԍ��D
        Thief,                //�D�_
        Steel,                //�|
        FireworksBox,         //�ԉΔ�
        RocketBox,            //���P�b�g��
        Tornado               //����
    }

    //����A�C�e��
    public enum SupportItems
    {
        Duck,           //�A�q��
        Firework,       //�ԉ�
        RocketLine,     //���P�b�g(��)
        RocketColumn,   //���P�b�g(�c)
        Star            //��
    }
    public static readonly int SUPPORT_ITEMS_COUNT = Enum.GetValues(typeof(SupportItems)).Length;

    //8����
    public enum Directions
    {
        Up,         //��
        Down,       //��
        Left,       //��
        Right,      //�E
        UpLeft,     //����
        UpRight,    //�E��
        DownLeft,   //����
        DownRight   //�E��
    }
    public static readonly int DIRECTIONS_COUNT = Enum.GetValues(typeof(Directions)).Length;

    //4����
    public enum FourDirections
    {
        Up,         //��
        Down,       //��
        Left,       //��
        Right       //�E
    }
    public static readonly int FOUR_DIRECTIONS_COUNT = Enum.GetValues(typeof(FourDirections)).Length;

    //�}�X
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

    //Z���W
    public const float Z_ZERO    = 0.0f;    //0
    public const float Z_PIECE   = -1.0f;   //��
    public const float Z_GIMMICK = -2.0f;   //�M�~�b�N(��Ƃ��ĊǗ����Ȃ�)

    //��������p
    public const int TEN     = 10;
    public const int HUNDRED = 100;

    //�ėp�萔
    public const int    BOARD_COLUMN_COUNT      = 8;                                        //�{�[�h��
    public const int    BOARD_LINE_COUNT        = 8;                                        //�{�[�h�s��
    public const int    SQUARES_COUNT           = BOARD_LINE_COUNT * BOARD_COLUMN_COUNT;    //�{�[�h����
    public const int    INT_NULL                = -99;                                      //null�̑�p�萔(int�^��null�����������ꍇ�Ɏg�p)
    public const float  SQUARE_DISTANCE         = 1.46f;                                    //�}�X�̋���
    public const float  SQUARE_DISTANCE_HALF    = SQUARE_DISTANCE / 2.0f;                   //���}�X�̋���
    public const float  PIECE_DEFAULT_SCALE     = 0.65f;                                    //��̃X�P�[��

    public static readonly Vector3 PIECE_DEFAULT_POS        = new Vector3(0.0f, 0.0f, Z_PIECE);     //��̊�{���W
    public static readonly Quaternion PIECE_GENERATE_QUEST  = Quaternion.Euler(0.0f, -90.0f, 0.0f); //��̐������̊p�x
    public static readonly Quaternion DEFAULT_QUEST         = Quaternion.Euler(0.0f, 00.0f, 0.0f);  //��{�̊p�x

    public static readonly Color COLOR_PRIMARY    = new Color(1.0f, 1.0f, 1.0f, 1.0f);               //���F
    public static readonly Color COLOR_ALPHA_ZERO = new Color(1.0f, 1.0f, 1.0f, 0.0f);               //����
    public static readonly Color[] COLOR_FADE_OUT = new Color[] { COLOR_PRIMARY, COLOR_ALPHA_ZERO }; //�t�F�[�h�A�E�g
    public static readonly Color[] COLOR_FADE_IN  = new Color[] { COLOR_ALPHA_ZERO, COLOR_PRIMARY }; //�t�F�[�h�C��

    //�}�X�F
    public static readonly Color SQUARE_BLUE   = new Color(0.6f, 0.6f, 1.0f, 1.0f);   //��
    public static readonly Color SQUARE_RED    = new Color(1.0f, 0.6f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_YELLOW = new Color(1.0f, 1.0f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_GREEN  = new Color(0.6f, 1.0f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_VIOLET = new Color(1.0f, 0.6f, 1.0f, 1.0f);   //��
    public static readonly Color SQUARE_ORANGE = new Color(1.0f, 0.6f, 0.3f, 1.0f);   //��
    public static readonly Color SQUARE_BLACK  = new Color(0.6f, 0.6f, 0.6f, 1.0f);   //��
    public static readonly Color SQUARE_WHITE  = new Color(1.0f, 1.0f, 1.0f, 1.0f);   //��

    //�}�X�̐F�ω����x
    public const float SQUARE_CHANGE_SPEED = 0.3f;

    //��]
    public static readonly Vector3 REVERSE_PIECE_ROT_SPEED              = new Vector3(0.0f, 10.0f, 0.0f);  //��]���x
    public static readonly Vector3 REVERSE_PIECE_SWITCH_ROT             = new Vector3(0.0f, 90.0f, 0.0f);  //���]����؂�ւ��p�x
    public static readonly Vector3 REVERSE_PIECE_FRONT_ROT              = Vector3.zero;                    //���
    public static readonly WaitForSeconds PIECE_REVERSAL_INTERVAL       = new WaitForSeconds(0.05f);       //��̔��]�Ԋu
    public static readonly WaitForSeconds PIECE_GROUP_REVERSAL_INTERVAL = new WaitForSeconds(0.1f);        //��O���[�v�̔��]�Ԋu
    public const float REVERSE_PIECE_SCALING_SPEED = 0.02f;  //�g�k���x
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.9f;   //�g�厞�̃X�P�[��

    //��j��
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //�g�k���x
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //�j�󎞂̃X�P�[��

    //��z�u
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;             //�g�k���x
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;              //�g�厞�̃X�P�[��
    public const float PUT_PIECE_MOVE_START_Z  = Z_GIMMICK - 0.1f;  //�ړ��J�nz���W(localPosition)
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;              //�ړ����x
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;              //�ҋ@��̃X���C�h���x

    //���
    public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //�������x
    public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //��������


    //�t���O
    public static bool GAME_START                   = false;  //�Q�[���J�n�H
    public static bool GAME_OVER                    = false;  //�Q�[���I�[�o�[�H
    public static bool GAME_CLEAR                   = false;  //�Q�[���N���A�H
    public static bool NOW_PUTTING_PIECES           = false;  //��z�u���H
    public static bool NOW_REVERSING_PIECES         = false;  //��]���H
    public static bool NOW_DESTROYING_PIECES        = false;  //��j�󒆁H
    public static bool NOW_FALLING_PIECES           = false;  //������H
    public static bool NOW_GIMMICK_DESTROY_WAIT     = false;  //�M�~�b�N�j��ҋ@���H
    public static bool NOW_GIMMICK_STATE_CHANGE     = false;  //�M�~�b�N��ԕω����H
    public static bool NOW_SUPPORT_ITEM_USE         = false;  //����A�C�e���g�p��?
    public static bool NOW_SUPPORT_ITEM_READY       = false;  //����A�C�e��������?
    public static bool NOW_TURN_END_PROCESSING      = false;  //�^�[���I���������H

    //�t���O���Z�b�g
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

    //�X�e�[�W�ʒ萔
    public static int[]   USE_COLOR_TYPE_ARR;   //�g�p�F�̎��
    public static int     USE_COLOR_COUNT;      //�g�p�F�̐�
    public static int     STAGE_NUMBER;         //�X�e�[�W�ԍ�
    public static int[]   HIDE_SQUARE_ARR;      //��\���}�X�̊Ǘ��ԍ�
    public static int[][] GIMMICKS_INFO_ARR;    //�M�~�b�N�̎�ނƃ}�X�̊Ǘ��ԍ�
    public static int     GIMMICKS_COUNT;       //�����M�~�b�N�̐ݒ萔
    public static int     GIMMICKS_GROUP_COUNT; //�M�~�b�N�̃O���[�v�̐ݒ萔
    public static int[][] TARGETS_INFO_ARR;     //�ڕW���
    public static int     TARGETS_COUNT;        //�ڕW�̃I�u�W�F�N�g��

    //�ڕW���z��̃C���f�N�X�ԍ�
    enum TargetInfoIndex
    {
        Object, //�I�u�W�F�N�g
        Color,  //�F
        Count   //�M�~�b�N�̎��
    }
    public static readonly int TARGET_INFO_OBJ   = (int)TargetInfoIndex.Object; //�I�u�W�F�N�g
    public static readonly int TARGET_INFO_COLOR = (int)TargetInfoIndex.Color;  //�F
    public static readonly int TARGET_INFO_COUNT = (int)TargetInfoIndex.Count;  //�M�~�b�N�̎��

    //�M�~�b�N�f�[�^
    public static GimmicksData GIMMICKS_DATA;

    //�M�~�b�N�f�[�^�擾
    public static void GetGimmicksData()
    {
        GIMMICKS_DATA = Resources.Load("gimmicks_data") as GimmicksData;
    }

    //�I�u�W�F�N�g�̃^�O
    public const string GIMMICK_TAG      = "Gimmick";       //�M�~�b�N
    public const string PIECE_TAG        = "Piece";         //��
    public const string SUPPORT_ITEM_TAG = "SupportItem";   //����A�C�e��

    //�M�~�b�N���z��̃C���f�b�N�X�ԍ�
    enum GimmickInfoIndex
    {
        Square,     //�z�u�}�X
        Gimmick,    //�M�~�b�N�̎��
        Color,      //�w��F
        Group,      //�Ǘ��O���[�v
        Width,      //����
        Height,     //����
        Quantity,   //�w���
        Order       //�w�菇��
    }
    public static readonly int SQUARE   = (int)GimmickInfoIndex.Square;     //�z�u�}�X
    public static readonly int GIMMICK  = (int)GimmickInfoIndex.Gimmick;    //�M�~�b�N�̎��
    public static readonly int COLOR    = (int)GimmickInfoIndex.Color;      //�w��F
    public static readonly int GROUP    = (int)GimmickInfoIndex.Group;      //�Ǘ��O���[�v
    public static readonly int WIDTH    = (int)GimmickInfoIndex.Width;      //����
    public static readonly int HEIGHT   = (int)GimmickInfoIndex.Height;     //����
    public static readonly int QUANTITY = (int)GimmickInfoIndex.Quantity;   //�w���
    public static readonly int ORDER    = (int)GimmickInfoIndex.Order;      //�w�菇��

    public const int DEF_SIZE = 1;  //�T�C�Y�̏����l
    public const int NOT_NUM  = -1; //�e���ڔԍ��w���Ȃ�

    //�X�e�[�W�ݒ�
    public static void StageSetting()
    {
        USE_COLOR_TYPE_ARR = new int[] {
            (int)Colors.Blue,   //��
            (int)Colors.Red,    //��
            (int)Colors.Yellow, //��
            (int)Colors.Green,  //��
            (int)Colors.Violet, //��
            (int)Colors.Orange  //��
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

        //(��)
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

        //(��)
        int[][] dummy = new int[5][];
        dummy[0]  = new int[] { (int)Gimmicks.Balloon,       COLORLESS_ID,    1 };
        dummy[1]  = new int[] { (int)Gimmicks.Balloon_Color, (int)Colors.Red, 1 };
        dummy[2]  = new int[] { (int)Gimmicks.Jewelry,       COLORLESS_ID,    1 };
        dummy[3]  = new int[] { (int)Gimmicks.Wall,          COLORLESS_ID,    1 };
        dummy[4]  = new int[] { (int)Gimmicks.Hamster,       COLORLESS_ID,    1 };


        //�����M�~�b�N���ݒ�
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

        //�ڕW���ݒ�
        TARGETS_INFO_ARR = dummy;
        TARGETS_COUNT = TARGETS_INFO_ARR.Length;
    }
}