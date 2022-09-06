using UnityEngine;

public class CommonDefine
{
    public const string TITLE_SCENE_NAME     = "TitleScene";        //�^�C�g���V�[����
    public const string PUZZLE_SCENE_NAME    = "MainPuzzleScene";   //�p�Y���V�[����
    
    public const int   INT_NULL             = -99;      //null�̑�p�萔(int�^��null�����������ꍇ�Ɏg�p)
    public const float PLAY_SCREEN_WIDTH    = 1080.0f;  //�v���C��ʕ�(1080)
    public const float PLAY_SCREEN_HEIGHT   = 1920.0f;  //�v���C��ʍ���(1920)
    public const float ONE_FRAME_TIMES      = 0.02f;    //1�t���[���̏�������
    public const float PAGE_MOVE_SPEED      = 0.3f;     //�y�[�W�ړ����x
    public static readonly WaitForFixedUpdate FIXED_UPDATE = new WaitForFixedUpdate();  //FixedUpdate�̃C���X�^���X
}