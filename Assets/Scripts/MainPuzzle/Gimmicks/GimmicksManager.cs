using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PuzzleDefine;
using static PuzzleMain.PieceManager;
using static ObjectMove_2D.ObjectMove_2D;

public class GimmicksManager : MonoBehaviour
{
    [Header("��΂�sprite")]
    [SerializeField]
    Sprite[] jewelrySpr;

    /// <summary>
    /// �M�~�b�N�Ƀ_���[�W�����邩�̊m�F
    /// </summary>
    /// /// <param name="putPieceTag"> �u������̃^�O</param>
    /// /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    public bool DamageCheck(ref string putPieceTag, ref int gimmickIndex)
    {
        //�_���[�W�̗L���t���O
        bool damage = false;

        switch (gimmickInfoArr[gimmickIndex].id)
        {
            case (int)Gimmicks.Balloon:  //���D
            case (int)Gimmicks.Wall:     //��
            case (int)Gimmicks.Flower:   //��
                damage = true;
                break;

            case (int)Gimmicks.Balloon_Color: //���D(�F)
            case (int)Gimmicks.Jewelry:       //���
                //�F����
                int colorIndex = gimmickInfoArr[gimmickIndex].colorNum;
                Colors gimmickColor = (Colors)Enum.ToObject(typeof(Colors), colorIndex);
                if (gimmickColor.ToString() == putPieceTag) damage = true;
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
    public void DamageGimmick(ref string putPieceTag, ref int gimmickIndex, int squareIndex)
    {
        int damageTimesfixNum = 0; //�X�e�[�g���Z�o�p
        string stateName = "";     //�X�e�[�g��
        GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex]; //�M�~�b�N�̏��擾

        switch (gimmInfo.id)
        {
            //���D
            case (int)Gimmicks.Balloon:
                stateName = COLORLESS_ANI_STATE_NAME;
                break;

            //�F�w��M�~�b�N
            case (int)Gimmicks.Balloon_Color: //���D(�F)
            case (int)Gimmicks.Jewelry:       //���
                stateName = "Burst" + putPieceTag;
                break;

            //��
            case (int)Gimmicks.Wall:
                damageTimesfixNum = GIMMICK_DAMAGE_TIMES[(int)Gimmicks.Wall] + 1;
                stateName = "WallDamage" + (-gimmInfo.remainingTimes + damageTimesfixNum).ToString();
                break;

            //��
            case (int)Gimmicks.Flower:
                damageTimesfixNum = GIMMICK_DAMAGE_TIMES[(int)Gimmicks.Flower] + 1;
                stateName = "FlowerDamage" + (-gimmInfo.remainingTimes + damageTimesfixNum).ToString();
                break;
        }

        //�_���[�W�A�j���[�V�����J�n
        gimmickCorList.Add(StartCoroutine(AnimationStart(gimmInfo.ani, stateName)));

        //�_���[�W�񐔌v�Z
        gimmInfo.remainingTimes--;

        //�_���[�W�c�񐔂�0�Ŕj��
        if (gimmInfo.remainingTimes <= 0)
            destroyPiecesIndexList.Add(squareIndex);
    }

    /// <summary>
    /// �M�~�b�N�̏�ԕω�
    /// </summary>
    /// <param name="gimmickIndex">�M�~�b�N�Ǘ��ԍ�(�X�e�[�W����)</param>
    /// <returns></returns>
    public IEnumerator ChangeGimmickState(int gimmickIndex)
    {
        //�M�~�b�N�̏��擾
        GimmickInformation gimmInfo = gimmickInfoArr[gimmickIndex];

        switch (gimmInfo.id)
        {
            //���(sprite�ؑ�)
            case (int)Gimmicks.Jewelry:

                //�q�I�u�W�F�N�g��sprit�X�V
                SpriteRenderer spriRenChild = gimmInfo.tra.GetChild(0).GetComponent<SpriteRenderer>();
                gimmInfo.colorNum++;
                if (gimmInfo.colorNum >= USE_PIECE_COUNT) gimmInfo.colorNum = 0;
                Sprite newSprite = jewelrySpr[gimmInfo.colorNum];

                //�؂�ւ����sprite���q�I�u�W�F�N�g�ɐݒ�
                gimmInfo.ani.enabled = false;
                spriRenChild.sprite = newSprite;

                //�F�ύX
                yield return StartCoroutine(SpriteRendererPaletteChange(gimmInfo.spriRen, JEWELRY_CHANGE_SPEED, COLOR_FADE_OUT, COLOR_FADE_COMPARE_INDEX));
                gimmInfo.spriRen.sprite = newSprite;
                gimmInfo.spriRen.color = COLOR_PRIMARY;
                gimmInfo.ani.enabled = true;
                Colors colorEnum = (Colors)Enum.ToObject(typeof(Colors), gimmInfo.colorNum);
                LoopAnimationStart(gimmInfo.ani, "Wait" + colorEnum.ToString());
                break;
        }
    }

    /// <summary>
    /// �A�j���[�V�����Đ�
    /// </summary>
    /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
    /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
    IEnumerator AnimationStart(Animator ani, string stateName)
    {
        //�A�j���[�V�����J�n
        ani.Play(stateName, 0, 0.0f);
        ani.speed = 1.0f;

        //�A�j���[�V�����I���ҋ@
        yield return null;
        yield return new WaitUntil(() => ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        ani.speed = 0.0f;
    }

    /// <summary>
    /// ���[�v�A�j���[�V�����Đ�
    /// </summary>
    /// <param name="ani">      �j�󂷂�I�u�W�F�N�g��Animator</param>
    /// <param name="stateName">�Đ��A�j���[�V�����X�e�[�g��</param>
    void LoopAnimationStart(Animator ani, string stateName = "Empty")
    {
        //�A�j���[�V�����J�n
        ani.Play(stateName, 0, 0.0f);
        ani.speed = 1.0f;
    }
}
