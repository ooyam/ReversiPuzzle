using UnityEngine;

public class CommonDefine
{
    public const string TITLE_SCENE_NAME     = "TitleScene";        //タイトルシーン名
    public const string PUZZLE_SCENE_NAME    = "MainPuzzleScene";   //パズルシーン名
    
    public const int   INT_NULL             = -99;      //nullの代用定数(int型でnullを代入したい場合に使用)
    public const float PLAY_SCREEN_WIDTH    = 1080.0f;  //プレイ画面幅(1080)
    public const float PLAY_SCREEN_HEIGHT   = 1920.0f;  //プレイ画面高さ(1920)
    public const float ONE_FRAME_TIMES      = 0.02f;    //1フレームの処理時間
    public const float PAGE_MOVE_SPEED      = 0.3f;     //ページ移動速度
    public static readonly WaitForFixedUpdate FIXED_UPDATE = new WaitForFixedUpdate();  //FixedUpdateのインスタンス
}