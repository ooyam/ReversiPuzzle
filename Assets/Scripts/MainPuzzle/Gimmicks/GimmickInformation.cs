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

    //�M�~�b�N���
    [System.NonSerialized] public int     startSquareId;  //�����}�X�ԍ�
    [System.NonSerialized] public int     groupId;        //�O���[�v�ԍ�
    [System.NonSerialized] public int     id;             //�M�~�b�N�ԍ�
    [System.NonSerialized] public int     colorId;        //�F�ԍ�
    [System.NonSerialized] public int     remainingTimes; //�c��_���[�W��
    [System.NonSerialized] public bool    freeFall;       //���R�����t���O
    [System.NonSerialized] public bool    destructible;   //�A���_���[�W�t���O(true�̎��j��\)
    [System.NonSerialized] public bool    nowTurnDamage;  //���̃^�[���Ƀ_���[�W���󂯂����̃t���O
    [System.NonSerialized] public Vector3 defaultPos;     //����W

    /// <summary>
    /// �R���|�[�l���g�̐ݒ�
    /// </summary>
    void ComponentSetting()
    {
        ani     = this.GetComponent<Animator>();
        tra     = this.transform;
        spriRen = this.GetComponent<SpriteRenderer>();

        int SpriRenCount = tra.childCount;
        spriRenChild = new SpriteRenderer[SpriRenCount];
        for (int i = 0; i < SpriRenCount; i++)
        { spriRenChild[i] = tra.GetChild(i).GetComponent<SpriteRenderer>(); }
    }

    /// <summary>
    /// �M�~�b�N���̐ݒ�
    /// </summary>
    /// <param name="_index">�X�e�[�W���̃M�~�b�N�Ǘ��ԍ�</param>
    public void InformationSetting(int _index)
    {
        ComponentSetting();
        var gimmickData = GIMMICKS_DATA.param[GIMMICKS_INFO_ARR[_index][GIMMICK]];
        startSquareId   = GIMMICKS_INFO_ARR[_index][SQUARE];
        groupId         = NOT_GROUP_ID;
        id              = gimmickData.id;
        colorId         = GIMMICKS_INFO_ARR[_index][COLOR];
        remainingTimes  = gimmickData.damage_times;
        freeFall        = gimmickData.free_fall;
        destructible    = !gimmickData.continuous;
        defaultPos      = new Vector3(gimmickData.position_x, gimmickData.position_y, PIECE_DEFAULT_POS.z);
    }

    /// <summary>
    /// �O���[�v�M�~�b�N���̐ݒ�
    /// </summary>
    /// <param name="_groupId">�O���[�v�ԍ�</param>
    public void GroupInformationSetting(int _groupId)
    {
        for (int i = 0; i < GIMMICKS_COUNT; i++)
        {
            if (GIMMICKS_INFO_ARR[i][GROUP] == _groupId)
            {
                InformationSetting(i);
                break;
            }
        }
        groupId = _groupId;
    }
}