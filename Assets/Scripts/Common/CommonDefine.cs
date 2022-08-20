using UnityEngine;

public class CommonDefine : MonoBehaviour
{
    public const string TITLE_SCENE_NAME     = "TitleScene";        //タイトルシーン名
    public const string PUZZLE_SCENE_NAME    = "MainPuzzleScene";   //パズルシーン名

    public const float PLAY_SCREEN_WIDTH    = 1080.0f;  //プレイ画面幅(1080)
    public const float PLAY_SCREEN_HEIGHT   = 1920.0f;  //プレイ画面高さ(1920)
    public const float ONE_FRAME_TIMES      = 0.02f;    //1フレームの処理時間
    public static readonly WaitForFixedUpdate FIXED_UPDATE = new WaitForFixedUpdate();  //FixedUpdateのインスタンス
}