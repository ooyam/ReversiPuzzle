using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;

public class GimmickInformation : MonoBehaviour
{
    //�R���|�[�l���g
    [System.NonSerialized] public Animator ani;
    [System.NonSerialized] public Transform tra;
    [System.NonSerialized] public SpriteRenderer   spriRen;
    [System.NonSerialized] public SpriteRenderer[] spriRenChild;
    [System.NonSerialized] public GameObject[]     objChild;

    //�M�~�b�N���
    [System.NonSerialized] public int     settingIndex;         //�X�e�[�W���̐ݒ�ԍ�
    [System.NonSerialized] public int     startSquareId;        //�����}�X�ԍ�
    [System.NonSerialized] public int     groupId;              //�O���[�v�ԍ�
    [System.NonSerialized] public int     id;                   //�M�~�b�N�ԍ�
    [System.NonSerialized] public int     colorId;              //�F�ԍ�
    [System.NonSerialized] public int     remainingTimes;       //�c��_���[�W��(�M�~�b�N�̌Œ萔)
    [System.NonSerialized] public int     remainingQuantity;    //�c��_���[�W��(�X�e�[�W���̎w�萔)
    [System.NonSerialized] public bool    freeFall;             //���R�����t���O
    [System.NonSerialized] public bool    destructible;         //�j��\�t���O(true�F�j��\)
    [System.NonSerialized] public bool    nowTurnDamage;        //���̃^�[���Ƀ_���[�W���󂯂����̃t���O
    [System.NonSerialized] public bool    inSquare;             //��Ƃ��Ĕz�u���邩�̃t���O(true�F��Ƃ��Ĕz�u)
    [System.NonSerialized] public Vector3 defaultPos;           //����W
    [System.NonSerialized] public Vector3 defaultScale;         //��X�P�[��
    [System.NonSerialized] public int[]   innerSquaresId;       //�����̃}�X�ԍ�

    /// <summary>
    /// �R���|�[�l���g�̐ݒ�
    /// </summary>
    void ComponentSetting()
    {
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
        spriRen = this.GetComponent<SpriteRenderer>();

        int childCount = tra.childCount;
        objChild     = new GameObject[childCount];
        spriRenChild = new SpriteRenderer[childCount];
        for (int i = 0; i < childCount; i++)
        {
            objChild[i]     = tra.GetChild(i).gameObject;
            spriRenChild[i] = objChild[i].GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�
    /// </summary>
    /// <param name="_index">�X�e�[�W���̃M�~�b�N�Ǘ��ԍ�</param>
    public void InformationSetting(int _index)
    {
        ComponentSetting();
        var gimmickData     = GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[_index][GIMMICK]];
        settingIndex        = _index;
        startSquareId       = GIMMICKS_INFO_ARR[_index][SQUARE];
        groupId             = GIMMICKS_INFO_ARR[_index][GROUP];
        id                  = gimmickData.id;
        colorId             = GIMMICKS_INFO_ARR[_index][COLOR];
        remainingTimes      = gimmickData.damage_times;
        remainingQuantity   = GIMMICKS_INFO_ARR[_index][QUANTITY];
        freeFall            = gimmickData.free_fall;
        destructible        = !gimmickData.continuous;
        inSquare            = gimmickData.in_square;
        defaultPos          = new Vector3(gimmickData.position_x, gimmickData.position_y, (inSquare) ? Z_PIECE : Z_GIMMICK);
        defaultScale        = new Vector3(gimmickData.scale_x, gimmickData.scale_y, 1.0f);

        switch (id)
        {
            //�M�~�b�N�����̋�𑀍�֎~�ɂ���M�~�b�N
            case (int)Gimmicks.Cage:    //�B
                innerSquaresId = new int[GIMMICKS_INFO_ARR[_index][WIDTH] * GIMMICKS_INFO_ARR[_index][HEIGHT]];
                int i = 0;
                for (int w = 0; w < GIMMICKS_INFO_ARR[_index][WIDTH]; w++)      //�������[�v
                {
                    for (int h = 0; h < GIMMICKS_INFO_ARR[_index][HEIGHT]; h++) //���������[�v
                    {
                        innerSquaresId[i] = startSquareId + w * BOARD_LINE_COUNT + h;
                        i++;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�(�}�X�ԍ�����)
    /// </summary>
    /// <param name="_squareIndex">�}�X�Ǘ��ԍ�</param>
    /// <param name="_gimmickId">  �M�~�b�N�ԍ�</param>
    /// <param name="_groupId">    �O���[�v�ԍ�</param>
    public void InformationSetting_SquareIndex(int _squareIndex, int _gimmickId, int _groupId)
    {
        for (int i = 0; i < GIMMICKS_COUNT; i++)
        {
            if (GIMMICKS_INFO_ARR[i][SQUARE] == _squareIndex && 
                (GIMMICKS_INFO_ARR[i][GIMMICK] == _gimmickId || GIMMICKS_INFO_ARR[i][GROUP] == _groupId))
            {
                InformationSetting(i);
                break;
            }
        }
    }
}