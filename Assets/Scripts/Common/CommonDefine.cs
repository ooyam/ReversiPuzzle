using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDefine : MonoBehaviour
{
    //定数
    public static float CANVAS_WIDTH;           //Canvas幅
    public static float CANVAS_HEIGHT;          //Canvas高さ
    public static float PLAY_SCREEN_WIDTH;      //プレイ画面幅(1080)
    public static float PLAY_SCREEN_HEIGHT;     //プレイ画面高さ(1920)
    public const float ONE_FRAME_TIMES = 0.02f; //1フレームの処理時間
    public static readonly WaitForFixedUpdate FIXED_UPDATE = new WaitForFixedUpdate();  //FixedUpdateのインスタンス
}