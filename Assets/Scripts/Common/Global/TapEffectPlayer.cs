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

    [Header("GlobalCanvas")]
    [SerializeField]
    Canvas mGlobalCanvas;
    RectTransform mGlobalCanvasTra;
    Camera mGlobalCamera;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    Touch mTouch;
#endif

    readonly WaitForSeconds DESTROY_TIME = new WaitForSeconds(2f);
    const float POS_Z = -5f;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        mGlobalCanvasTra = mGlobalCanvas.GetComponent<RectTransform>();
        mGlobalCamera = mGlobalCanvas.worldCamera;
    }

    /// <summary>
    /// 更新
    /// </summary>
    void Update()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        //画面に触れている
        if (Input.touchCount > 0)
        {
            //タップ情報取得
            mTouch = Input.GetTouch(0);

            //タップを離した
            if (mTouch.phase == TouchPhase.Ended) StartCoroutine(EffectGenerate());
        }
#else
        //マウスを離した
        if (Input.GetMouseButtonUp(0)) StartCoroutine(EffectGenerate());
#endif
    }

    /// <summary>
    /// エフェクト生成
    /// </summary>
    IEnumerator EffectGenerate()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mGlobalCanvasTra, mTouch.position, mGlobalCamera, out Vector2 localpoint);
#else
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mGlobalCanvasTra, Input.mousePosition, mGlobalCamera, out Vector2 localpoint);
#endif
        GameObject effect = Instantiate(mEffect);
        RectTransform effectTra = effect.GetComponent<RectTransform>();
        effectTra.SetParent(mEffectBox);
        effectTra.localPosition = new Vector3(localpoint.x, localpoint.y, POS_Z);

        //Effectオブジェクト破壊
        yield return DESTROY_TIME;
        Destroy(effect);
    }
}
