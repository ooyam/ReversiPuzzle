using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommonDefine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        //音源ファイル格納ディレクトリ
        const string CLIP_DIR = "SoundClip/";
        const string SE_DIR = CLIP_DIR + "SE/";
        const string BGM_DIR = CLIP_DIR + "BGM/";

        //クリップファイル名
        public const string BGM_TITLE = "bgm_title";
        public const string SE_BTN_YES = "button_yes";

        //音量
        const float BGM_VOLUME = 1.0f;
        const float SE_VOLUME = 1.0f;
        const float BGM_FADE_SPEED = 0.02f;

        //SE同時再生最大数
        const int SE_PLAY_MAX = 70;

        //AudioSource
        static AudioSource[] mSE_AudioArr;
        static AudioSource mBGM_Audio;

        static SoundManager instance = null;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);

                //BGM用AudioSourceの取得
                mBGM_Audio = GetComponent<AudioSource>();

                //SE用AudioSourceの追加
                mSE_AudioArr = new AudioSource[SE_PLAY_MAX];
                for (int i = 0; i < SE_PLAY_MAX; i++)
                {
                    mSE_AudioArr[i] = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        //=============================================
        //BGM
        //=============================================

        /// <summary>
        /// BGM開始(フェード有)
        /// </summary>
        /// <param name="_BGMName">クリップ名</param>
        public static void BGM_FadeStart(string _BGMName)
        {
            mBGM_Audio.clip = (AudioClip)Resources.Load(BGM_DIR + _BGMName);
            mBGM_Audio.Play();
            if (Preferences.Bgm)
            {
                mBGM_Audio.volume = 0.0f;
                instance.StartCoroutine(BGM_SetVolumeFade(BGM_VOLUME));
            }
        }

        /// <summary>
        /// BGM再開(フェード有)
        /// </summary>
        public static void BGM_FadeRestart()
        {
            if (Preferences.Bgm)
            {
                mBGM_Audio.volume = 0.0f;
                instance.StartCoroutine(BGM_SetVolumeFade(BGM_VOLUME));
            }
        }

        /// <summary>
        /// BGM終了(フェード有)
        /// </summary>
        /// <param name="_BGMName">クリップ名</param>
        public static IEnumerator BGM_FadeStop()
        {
            yield return instance.StartCoroutine(BGM_SetVolumeFade(0.0f));
        }

        /// <summary>
        /// BGM終了(フェード無)
        /// </summary>
        /// <param name="_BGMName">クリップ名</param>
        public static void BGM_Stop()
        {
            mBGM_Audio.Stop();
        }

        /// <summary>
        /// BGM音量フェード
        /// </summary>
        /// <param name="_volume"></param>
        static IEnumerator BGM_SetVolumeFade(float _volume)
        {
            if (mBGM_Audio.volume < _volume)
            {
                //フェードイン
                while (Preferences.Bgm)
                {
                    BGM_SetVolume(mBGM_Audio.volume + BGM_FADE_SPEED);
                    yield return FIXED_UPDATE;
                    if (mBGM_Audio.volume >= _volume) break;
                }
            }
            else
            {
                //フェードアウト
                while (Preferences.Bgm)
                {
                    BGM_SetVolume(mBGM_Audio.volume - BGM_FADE_SPEED);
                    yield return FIXED_UPDATE;
                    if (mBGM_Audio.volume <= _volume) break;
                }
            }

            if (Preferences.Bgm) BGM_SetVolume(_volume);
            else BGM_SetVolume(0.0f);
        }

        /// <summary>
        /// BGM音量フェード
        /// </summary>
        /// <param name="_volume"></param>
        static void BGM_SetVolume(float _volume)
        {
            mBGM_Audio.volume = _volume;
        }

        //=============================================
        //SE
        //=============================================

        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_SEName">クリップ名</param>
        public static void SE_Onshot(string _SEName)
        {
            for (int i = 0; i < SE_PLAY_MAX; i++)
            {
                if (mSE_AudioArr[i].isPlaying) continue;
                mSE_AudioArr[i].volume = SE_VOLUME;
                mSE_AudioArr[i].PlayOneShot((AudioClip)Resources.Load(SE_DIR + _SEName));
            }
        }

        /// <summary>
        /// SEを止める
        /// </summary>
        public static void SE_Stop()
        {
            for (int i = 0; i < SE_PLAY_MAX; i++)
            {
                mSE_AudioArr[i].Stop();
            }
        }
    }
}
