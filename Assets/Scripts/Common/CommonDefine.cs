using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDefine : MonoBehaviour
{
    //�萔
    public static float CANVAS_WIDTH;           //Canvas��
    public static float CANVAS_HEIGHT;          //Canvas����
    public static float PLAY_SCREEN_WIDTH;      //�v���C��ʕ�(1080)
    public static float PLAY_SCREEN_HEIGHT;     //�v���C��ʍ���(1920)
    public const float ONE_FRAME_TIMES = 0.02f; //1�t���[���̏�������
    public static readonly WaitForFixedUpdate FIXED_UPDATE = new WaitForFixedUpdate();  //FixedUpdate�̃C���X�^���X
}