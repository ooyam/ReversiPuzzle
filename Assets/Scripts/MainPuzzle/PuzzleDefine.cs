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
        Steel,                //�S
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
        All             //�S����
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
    public const float  PIECE_DEFAULT_SCALE     = 0.6f;                                     //��̃X�P�[��

    public static readonly Vector3 PIECE_DEFAULT_POS     = new Vector3(0.0f, 0.0f, Z_PIECE);        //��̊�{���W
    public static readonly Quaternion PIECE_GENERATE_QUA = Quaternion.Euler(0.0f, -90.0f, 0.0f);    //��̐������̊p�x

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
    public const float REVERSE_PIECE_CHANGE_SCALE  = 0.8f;   //�g�厞�̃X�P�[��

    //��j��
    public const float DESTROY_PIECE_SCALING_SPEED = 0.03f;  //�g�k���x
    public const float DESTROY_PIECE_CHANGE_SCALE  = 0.0f;   //�j�󎞂̃X�P�[��

    //��z�u
    public const float PUT_PIECE_SCALING_SPEED = 0.02f;  //�g�k���x
    public const float PUT_PIECE_CHANGE_SCALE  = 0.8f;   //�g�厞�̃X�P�[��
    public const float PUT_PIECE_MOVE_START_Z  = Z_GIMMICK - 0.1f;  //�ړ��J�nz���W(localPosition)
    public const float PUT_PIECE_MOVE_SPEED    = 0.2f;   //�ړ����x
    public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;   //�ҋ@��̃X���C�h���x

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
    public static int     GIMMICKS_COUNT;       //�M�~�b�N�̐ݒ萔
    public static int     GIMMICKS_GROUP_COUNT; //�M�~�b�N�̃O���[�v�̐ݒ萔
    public static int[][] GIMMICKS_INFO_ARR;    //�M�~�b�N�̎�ނƃ}�X�̊Ǘ��ԍ�

    //�M�~�b�N�f�[�^
    public static GimmicksData GIMMICKS_DATA;

    //�M�~�b�N�f�[�^�擾
    public static void GetGimmicksData()
    {
        GIMMICKS_DATA = Resources.Load("gimmicks_data") as GimmicksData;
    }

    //�I�u�W�F�N�g�̃^�O
    public const string GIMMICK_TAG = "Gimmick";    //�M�~�b�N
    public const string PIECE_TAG   = "Piece";      //��

    //�M�~�b�N���z��̃C���f�b�N�X�ԍ�
    public const int SQUARE   = 0;  //�z�u�}�X
    public const int GIMMICK  = 1;  //�M�~�b�N�̎��
    public const int COLOR    = 2;  //�w��F
    public const int GROUP    = 3;  //�Ǘ��O���[�v
    public const int WIDTH    = 4;  //����
    public const int HEIGHT   = 5;  //����
    public const int QUANTITY = 6;  //�w���
    public const int ORDER    = 7;  //�w�菇��

    public const int DEF_SIZE = 1;  //�T�C�Y�̏����l
    public const int NOT_NUM  = -1; //�e���ڔԍ��w���Ȃ�

    //�X�e�[�W�ݒ�
    public static void StageSetting()
    {
        USE_COLOR_TYPE_ARR = new int[] {
            (int)Colors.Blue,   //��
            //(int)Colors.Red,    //��
            //(int)Colors.Yellow, //��
            //(int)Colors.Green,  //��
            //(int)Colors.Violet, //��
            (int)Colors.Orange  //��
        };
        USE_COLOR_COUNT = USE_COLOR_TYPE_ARR.Length;
        STAGE_NUMBER    = 1;
        HIDE_SQUARE_ARR = new int[] {
            (int)Squares.A2,
            (int)Squares.B2,
            (int)Squares.C2,
            (int)Squares.D2,
            (int)Squares.E2,
            (int)Squares.F2,
            (int)Squares.G2,
            (int)Squares.H2
        };
        GIMMICKS_COUNT       = 4;
        GIMMICKS_INFO_ARR    = new int[GIMMICKS_COUNT][];
        GIMMICKS_INFO_ARR[0] = new int[] { (int)Squares.B1, (int)Gimmicks.Jewelry, (int)Colors.Blue, NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_INFO_ARR[1] = new int[] { (int)Squares.D1, (int)Gimmicks.Hamster, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_INFO_ARR[2] = new int[] { (int)Squares.H1, (int)Gimmicks.Tornado, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };
        GIMMICKS_INFO_ARR[3] = new int[] { (int)Squares.D3, (int)Gimmicks.Tornado, COLORLESS_ID,     NOT_NUM, DEF_SIZE, DEF_SIZE, NOT_NUM, NOT_NUM };

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
}