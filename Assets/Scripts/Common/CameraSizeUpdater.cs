using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class CameraSizeUpdater : MonoBehaviour
{
    private Camera _camera;

    //縦、横、もしくは両方のどれを基準にするか
    private enum BaseType
    {
        Both, Width, Height
    }
    [SerializeField]
    private BaseType _baseType = BaseType.Both;

    //基準の画面サイズ
    [SerializeField]
    private float _baseWidth = 1080, _baseHeight = 1920;

    //画像のPixel Per Unit
    [SerializeField]
    private float _pixelPerUnit = 100f;

    //現在のアス比
    private float _currentAspect;

    private void Awake()
    {
        UpdateOrthographicSize();
    }

    //インスペクターの値が変更された時実行、OrthographicSizeを強制的に更新する
    private void OnValidate()
    {
        _currentAspect = 0;
        UpdateOrthographicSize();
    }

    private void Update()
    {
        if (Application.isPlaying) return;
        UpdateOrthographicSize();
    }

    //カメラのOrthographicSizeをアス比に応じて更新
    private void UpdateOrthographicSize()
    {
        //現在のアスペクト比を取得し、変化がなければ更新しない
        float currentAspect = (float)Screen.height / (float)Screen.width;
        if (Mathf.Approximately(_currentAspect, currentAspect)) return;
        _currentAspect = currentAspect;

        //カメラを取得していなければ取得
        if (_camera == null) _camera = gameObject.GetComponent<Camera>();

        //基準のアスペクト比と、基準のアスペクト比の時のSize
        float baseAspect = _baseHeight / _baseWidth;
        float baseOrthographicSize = _baseHeight / _pixelPerUnit / 2f;

        //カメラのorthographicSizeを設定しなおす
        if (_baseType == BaseType.Height || (baseAspect > _currentAspect && _baseType != BaseType.Width))
            _camera.orthographicSize = baseOrthographicSize;
        else _camera.orthographicSize = baseOrthographicSize * (_currentAspect / baseAspect);
    }

}