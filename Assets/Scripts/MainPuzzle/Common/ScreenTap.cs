using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

public class ScreenTap : MonoBehaviour
{
    const float RAY_DISTANCE = 10.0f;
    Camera mMainCamera;  //メインカメラ

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    Touch mTouch;
#endif

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        mMainCamera = Camera.main;
    }

    /// <summary>
    /// 更新
    /// </summary>
    void Update()
    {
        //操作可能
        if (IsOperable())
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            //画面に触れている
            if (Input.touchCount > 0)
            {
                //タップ情報取得
                mTouch = Input.GetTouch(0);

                //タップを離した
                if (mTouch.phase == TouchPhase.Ended) FlyRay();
            }
#else
            //マウスを離した
            if (Input.GetMouseButtonUp(0)) FlyRay();
#endif
        }
    }

    /// <summary>
    /// レイを飛ばす
    /// </summary>
    void FlyRay()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        Vector3 touchPos = mMainCamera.ScreenToWorldPoint(mTouch.position);
#else
        Vector3 touchPos = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
#endif
        Ray ray = new Ray(touchPos, Vector3.forward);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, RAY_DISTANCE);
        var hitCollider = hit.collider;
        if (hitCollider)
        {
            //Rayが当たったオブジェクト
            switch (hitCollider.tag)
            {
                //駒,ギミック
                case PIECE_TAG:
                case GIMMICK_TAG:
                    PiecesMgr.TapObject(hitCollider.gameObject);
                    break;

                //援護アイテム
                case SUPPORT_ITEM_TAG:
                    SupportItemsMgr.TapItem(hitCollider.gameObject);
                    break;
            }
        }
    }
}
