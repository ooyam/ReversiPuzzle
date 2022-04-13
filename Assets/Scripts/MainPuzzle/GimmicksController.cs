using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PuzzleDefine;
using static PieceManager;

public class GimmicksController : MonoBehaviour
{
    public static GimmicksController sGimmicksController;
    void Awake()
    {
        if (sGimmicksController == null)
        {
            sGimmicksController = this;
        }
    }

    /// <summary>
    /// �M�~�b�N�Ƀ_���[�W�����邩�̊m�F
    /// </summary>
    /// /// <param name="putPieceTag"> �u������̃^�O</param>
    /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    /// /// <param name="squareIndex"> �M�~�b�N�z�u�ԍ�</param>
    /// /// <param name="gimmickObj">  �M�~�b�N�I�u�W�F�N�g</param>
    public bool DamageCheck(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj)
    {
        //�_���[�W�̗L���t���O
        bool damage = false;

        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            //----------------------
            //���D
            //----------------------
            case (int)Gimmicks.Balloon:
                damage = true;
                break;

            //----------------------
            //���D(�F)
            //----------------------
            case (int)Gimmicks.Balloon_Color:
                //�F����
                int balloonColorNum = GIMMICK_INFO_ARR[gimmickIndex][COLOR];
                PieceColors pieceColor = (PieceColors)Enum.ToObject(typeof(PieceColors), balloonColorNum);
                if (pieceColor.ToString() == putPieceTag) damage = true;
                break;
        }

        //���ʂ�Ԃ�
        return damage;
    }

    /// <summary>
    /// �M�~�b�N�Ƀ_���[�W
    /// </summary>
    /// /// <param name="putPieceTag"> �u������̃^�O</param>
    /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    /// /// <param name="squareIndex"> �M�~�b�N�z�u�ԍ�</param>
    /// /// <param name="gimmickObj">  �M�~�b�N�I�u�W�F�N�g</param>
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex, ref GameObject gimmickObj)
    {
        switch (GIMMICK_INFO_ARR[gimmickIndex][GIMMICK])
        {
            //----------------------
            //���D
            //----------------------
            case (int)Gimmicks.Balloon:
                destroyPiecesIndexList.Add(squareIndex); //�폜�Ώۂɔ��]��̊Ǘ��ԍ��ǉ�
                gimmickCorList.Add(StartCoroutine(BalloonBreak(gimmickObj)));
                break;

            //----------------------
            //���D(�F)
            //----------------------
            case (int)Gimmicks.Balloon_Color:
                destroyPiecesIndexList.Add(squareIndex);
                gimmickCorList.Add(StartCoroutine(BalloonColorBreak(gimmickObj, putPieceTag)));
                break;
        }
    }


    /// <summary>
    /// ���D�j��
    /// </summary>
    /// <param name="obj">�j�󂷂�I�u�W�F�N�g</param>
    IEnumerator BalloonBreak(GameObject obj)
    {
        Animator ani = obj.GetComponent<Animator>();
        string stateName = "Black";

        //�A�j���[�V�����J�n
        ani.Play(stateName, 0, 0.0f);

        //�A�j���[�V�����I���ҋ@
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }

    /// <summary>
    /// ���D(�F)�j��
    /// </summary>
    /// <param name="obj">      �j�󂷂�I�u�W�F�N�g</param>
    /// <param name="colorName">�F�̖���(�X�e�[�W��)</param>
    /// <returns></returns>
    IEnumerator BalloonColorBreak(GameObject obj, string colorName)
    {
        Animator ani = obj.GetComponent<Animator>();

        //�A�j���[�V�����J�n
        ani.Play(colorName, 0, 0.0f);

        //�A�j���[�V�����I���ҋ@
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }
}
