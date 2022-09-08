using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectPlayer : MonoBehaviour
{
    [Header("タップエフェクト")]
    [SerializeField]
    GameObject mEffect;

    [Header("エフェクト格納箱")]
    [SerializeField]
    RectTransform mEffectBox;

    Canvas mCanvas;
    RectTransform mCanvasTra;
    Camera mCamera;

    readonly WaitForSeconds DESTROY_TIME = new WaitForSeconds(2f);
    const float POS_Z = -5f;

    //スマホだとなぜかInput.GetMouseButtonUpが効きにくい為代用
    bool mIsPush = false;
    bool mIsRelease = false;

    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        mCanvasTra = mCanvas.GetComponent<RectTransform>();
        mCamera = mCanvas.worldCamera;
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
            mIsRelease = false;
            StartCoroutine(EffectGenerate());
        }
    }

    /// <summary>
    /// エフェクト破壊
    /// </summary>
    IEnumerator EffectGenerate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mCanvasTra, Input.mousePosition, mCamera, out Vector2 localpoint);
        GameObject effect = Instantiate(mEffect);
        RectTransform effectTra = effect.GetComponent<RectTransform>();
        effectTra.SetParent(mEffectBox);
        effectTra.localPosition = new Vector3(localpoint.x, localpoint.y, POS_Z);

        //Effectオブジェクト破壊
        yield return DESTROY_TIME;
        Destroy(effect);
    }
}
