using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleDefine
{
    //��̐F
    public enum PieceColors
    {
        Blue = 0,  //�F
        Red,       //�ԐF
        Yellow,    //���F
        Green,     //�ΐF
        Violet,    //���F
        Orange,    //��F
        Pink,      //���F
        LightBlue  //���F
    }

    //�M�~�b�N
    public enum Gimmicks
    {
        Balloon = 0,          //���D
        Balloon_Color,        //���D(�F)
        DiscoBall,            //�~���[�{�[��
        Stone,                //��
        Flower,               //�큨�Q������
        ColorFrame,           //�F�̘g
        ColorFrame_Blinking,  //�F�̘g(�_��)
        ColorFrame_Thick,     //�F�̘g(��)
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
        Firework = 0,  //�ԉΔ�
        Rocket         //���P�b�g��
    }

    //����
    public enum Directions
    {
        Up = 0,     //��
        UpRight,    //�E��
        Right,      //�E
        DownRight,  //�E��
        Down,       //��
        DownLeft,   //����
        Left,       //��
        UpLeft      //����
    }

    public class PuzzleDefine : MonoBehaviour
    {
        //�ėp�萔
        public const int BOARD_COLUMN_COUNT = 8;       //�{�[�h��
        public const int BOARD_LINE_COUNT   = 8;       //�{�[�h�s��
        public const int NULL_NUMBER        = -99;     //null�̑�p�萔(int�^��null�����������ꍇ�Ɏg�p)
        public const float SQUARE_DISTANCE  = 0.73f;   //1�}�X�̋���
        public const float PIECE_DEFAULT_SCALE = 0.6f; //��̃X�P�[��
        public static readonly Vector3 PIECE_DEFAULT_POS     = new Vector3(0.0f, 0.0f, -0.1f);       //��̊�{���W
        public static readonly Quaternion PIECE_GENERATE_QUA = Quaternion.Euler(0.0f, -90.0f, 0.0f); //��̐������̊p�x

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
        public const float PUT_PIECE_MOVE_START_Z  = -0.3f;  //�ړ��J�nz���W(localPosition)
        public const float PUT_PIECE_MOVE_SPEED    = 0.2f;   //�ړ����x
        public const float NEXT_PIECE_SLIDE_SPEED  = 0.3f;   //�ҋ@��̃X���C�h���x

        //���
        public static readonly Vector3 FALL_PIECE_GENERATE_POS = new Vector3(PIECE_DEFAULT_POS.x, SQUARE_DISTANCE * BOARD_LINE_COUNT, PIECE_DEFAULT_POS.z);  //��]���x
        public const float FALL_PIECE_MOVE_SPEED  = 0.07f;  //�������x
        public const float FALL_PIECE_ACCELE_RATE = 0.02f;  //��������


        //�t���O
        public static bool GAME_START            = false;  //�Q�[���J�n�H
        public static bool GAME_OVER             = false;  //�Q�[���I�[�o�[�H
        public static bool GAME_CLEAR            = false;  //�Q�[���N���A�H
        public static bool NOW_PUTTING_PIECES    = false;  //��z�u���H
        public static bool NOW_REVERSING_PIECES  = false;  //��]���H
        public static bool NOW_DESTROYING_PIECES = false;  //��j�󒆁H
        public static bool NOW_FALLING_PIECES    = false;  //������H

        //�t���O���Z�b�g
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

        //�X�e�[�W�ʒ萔
        public static int   STAGE_NUMBER    = 1;          //�X�e�[�W�ԍ�
        public static int   USE_PIECE_COUNT = 8;          //�g�p��̎�ސ�
        public static int[] HIDE_SQUARE = new int[] { };  //��\���}�X�̊Ǘ��ԍ�
    }
}