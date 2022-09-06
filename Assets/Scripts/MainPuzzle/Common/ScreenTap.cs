using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzleDefine;
using static PuzzleMain.PuzzleMain;

public class ScreenTap : MonoBehaviour
{
    const float RAY_DISTANCE = 10.0f;
    Camera mMainCamera;  //メインカメラ

    //スマホだとなぜかInput.GetMouseButtonUpが効きにくい為代用
    bool mIsPush = false;
    bool mIsRelease = false;

    public void Initialize()
    {
        mMainCamera = Camera.main;
    }

    void Update()
    {
        if (mIsPush)
        {
            if (!Input.GetMouseButton(0))
            {
                mIsPush = false;
                mIsRelease = true;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            mIsPush = true;
        }

        //指を離した
        if (mIsRelease)
        {
            //操作可能
            if (IsOperable())
            {
                FlyRay();
            }
            mIsRelease = false;
        }

    }

    /// <summary>
    /// レイを飛ばす
    /// </summary>
    void FlyRay()
    {
        Vector3 mousePos = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = new Ray(mousePos, Vector3.forward);
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
