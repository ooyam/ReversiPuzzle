using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CommonDefine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        //音源ファイル格納ディレクトリ
        const string CLIP_DIR = "SoundClip/";
        const string SE_DIR   = CLIP_DIR + "SE/";
        const string BGM_DIR  = CLIP_DIR + "BGM/";

        //データオブジェクト名
        const string SE_DATA_OBJ  = "SE_Data";
        const string BGM_DATA_OBJ = "BGM_Data";

        //BGMの種類
        public enum BGM_Type
        {
            Title,  //タイトル画面
            Stage1, //ステージ1
            Stage2, //ステージ2
            Stage3, //ステージ3
            Stage4, //ステージ4

            Count   //総数
        }

        //SEの種類
        public enum SE_Type
        {
            BtnYes,                 //ボタンタップ(yes)
            BtnNo,                  //ボタンタップ(no)
            StageSelect,            //ステージ選択
            GameClear,              //ゲームクリア
            GameOver,               //ゲームオーバー
            PieceMove,              //駒移動
            PiecePut,               //駒を置く(反転)
            PieceSelect,            //待機駒選択
            SupportItemAppearance,  //援護アイテム出現
            SupportItemSelect,      //援護アイテム選択
            Duck,                   //アヒル足音
            Firework,               //花火爆発
            Rocket,                 //ロケット移動
            Star,                   //星くるくる
            StarWave,               //星衝撃波
            BalloonBurst,           //風船破壊
            JewelryBurst,           //宝石破壊
            WallDamage,             //壁ダメージ
            WallBurst,              //壁破壊
            FlowerDamage,           //花ダメージ
            FlowerBurst,            //花破壊
            FrameBurst,             //枠破壊
            HamsterDamage,          //ハムスターダメージ
            HamsterBurst,           //ハムスター破壊
            CageIgnite,             //檻着火
            CageBurst,              //檻破壊
            NumberTagBurst,         //番号札破壊
            ThiefRun,               //泥棒走る
            ThiefBurst,             //泥棒破壊
            SteelBurst,             //鋼破壊
            TornadoAttack,          //竜巻いたずら
            TornadoBurst,           //竜巻破壊

            Count   //総数
        }

        //音量フェード時間
        const float BGM_FADE_IN_TIME = 1.0f;
        const float BGM_FADE_OUT_TIME = 0.5f;

        //SE定数
        const int SE_PLAY_MAX = 30;                     //SE同時再生最大数
        const int SE_MAX_DUPLICATE = 3;                 //同じSEの最大重複再生数
        const float TIME_CONSIDERED_DUPLICATE = 0.2f;   //重複とみなす時間

        //AudioSource
        static AudioSource mBGM_Audio;
        static AudioSource[] mSE_AudioArr;

        //データ
        static Dictionary<BGM_Type, BGM_DataData> mBGM_InfoDic;
        static Dictionary<SE_Type, SE_DataData> mSE_InfoDic;
        static Dictionary<BGM_Type, AudioClip> mBGM_ClipDic;
        static Dictionary<SE_Type, AudioClip> mSE_ClipDic;

        //再生中のBGM
        static BGM_Type mNowPlayBgm;

        //BGMフェード中のコルーチン
        static Coroutine mBGM_FadeCor;

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

                //データの取得
                BGM_Data bgmData = Resources.Load(BGM_DATA_OBJ) as BGM_Data;
                SE_Data seData = Resources.Load(SE_DATA_OBJ) as SE_Data;
                BGM_DataData[] bgmDataArr = bgmData.dataArray;
                SE_DataData[]  seDataArr = seData.dataArray;

                //BGMクリップ,情報取得
                mBGM_InfoDic = new Dictionary<BGM_Type, BGM_DataData>();
                mBGM_ClipDic = new Dictionary<BGM_Type, AudioClip>();
                for (int i = 0; i < bgmDataArr.Length; i++)
                {
                    mBGM_InfoDic.Add(bgmDataArr[i].BGM_TYPE, bgmDataArr[i]);
                    mBGM_ClipDic.Add(bgmDataArr[i].BGM_TYPE, (AudioClip)Resources.Load(BGM_DIR + bgmDataArr[i].Clipname));
                }

                //SEクリップ,情報取得
                mSE_InfoDic = new Dictionary<SE_Type, SE_DataData>();
                mSE_ClipDic = new Dictionary<SE_Type, AudioClip>();
                for (int i = 0; i < seDataArr.Length; i++)
                {
                    mSE_InfoDic.Add(seDataArr[i].SE_TYPE, seDataArr[i]);
                    mSE_ClipDic.Add(seDataArr[i].SE_TYPE, (AudioClip)Resources.Load(SE_DIR + seDataArr[i].Clipname));
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
        /// <param name="_bgmType">BGMの種類</param>
        public static void BGM_FadeStart(BGM_Type _bgmType)
        {
            //データ取得
            mNowPlayBgm = _bgmType;

            //他BGMがフェード中の場合はコルーチン停止
            BGM_FadeBreak();

            mBGM_Audio.clip = mBGM_ClipDic[_bgmType];
            mBGM_Audio.Play();
            if (Preferences.Bgm)
            {
                mBGM_Audio.volume = 0.0f;
                mBGM_FadeCor = instance.StartCoroutine(BGM_SetVolumeFade(mBGM_InfoDic[_bgmType].Volume));
            }
        }

        /// <summary>
        /// BGM再開(フェード有)
        /// </summary>
        public static void BGM_FadeRestart()
        {
            if (Preferences.Bgm)
            {
                //他BGMがフェード中の場合はコルーチン停止
                BGM_FadeBreak();
                mBGM_Audio.volume = 0.0f;
                mBGM_FadeCor = instance.StartCoroutine(BGM_SetVolumeFade(mBGM_InfoDic[mNowPlayBgm].Volume));
            }
        }

        /// <summary>
        /// BGM終了(フェード有)
        /// </summary>
        /// <param name="_BGMName">クリップ名</param>
        public static IEnumerator BGM_FadeStop()
        {
            mBGM_FadeCor = instance.StartCoroutine(BGM_SetVolumeFade(0.0f));
            yield return mBGM_FadeCor;
        }

        /// <summary>
        /// BGM終了(フェード無)
        /// </summary>
        /// <param name="_BGMName">クリップ名</param>
        public static void BGM_Stop()
        {
            BGM_SetVolume(0.0f);
        }

        /// <summary>
        /// BGM音量フェード
        /// </summary>
        /// <param name="_volume"></param>
        static IEnumerator BGM_SetVolumeFade(float _volume)
        {
            yield return null;
            float offset = _volume - mBGM_Audio.volume;
            if (offset == 0)
            {
                //フェード必要なし
                yield break;
            }
            else if (offset > 0)
            {
                //フェードイン
                float value = offset / (BGM_FADE_IN_TIME / ONE_FRAME_TIMES);
                float nowVolume = mBGM_Audio.volume;
                while (Preferences.Bgm)
                {
                    nowVolume += value;
                    BGM_SetVolume(nowVolume);
                    yield return FIXED_UPDATE;
                    if (nowVolume >= _volume) break;
                }
            }
            else
            {
                //フェードアウト
                float value = offset / (BGM_FADE_OUT_TIME / ONE_FRAME_TIMES);
                float nowVolume = mBGM_Audio.volume;
                while (Preferences.Bgm)
                {
                    nowVolume += value;
                    BGM_SetVolume(nowVolume);
                    yield return FIXED_UPDATE;
                    if (nowVolume <= _volume) break;
                }
            }

            if (Preferences.Bgm) BGM_SetVolume(_volume);
            else BGM_SetVolume(0.0f);
        }

        /// <summary>
        /// BGM音量設定
        /// </summary>
        /// <param name="_volume"></param>
        static void BGM_SetVolume(float _volume)
        {
            mBGM_Audio.volume = _volume;
        }

        /// <summary>
        /// BGMのフェードコルーチン停止
        /// </summary>
        static void BGM_FadeBreak()
        {
            if (mBGM_FadeCor != null) instance.StopCoroutine(mBGM_FadeCor);
        }


        //=============================================
        //SE
        //=============================================

        /// <summary>
        /// 使用可能なAudioSourceの管理番号取得
        /// </summary>
        /// <returns>使用可能なAudioSource</returns>
        static AudioSource SE_GetAudioSource(AudioClip clip)
        {
            AudioSource returnAudioSrc = null;
            int duplicateSeCnt = 0;
            for (int i = 0; i < SE_PLAY_MAX; i++)
            {
                //再生中のAudioSource
                if (mSE_AudioArr[i].isPlaying)
                {
                    //同じSEが同じタイミングで流れている場合
                    if (mSE_AudioArr[i].clip == clip && mSE_AudioArr[i].time < TIME_CONSIDERED_DUPLICATE)
                    {
                        //一定数を超えた場合はnullを返す
                        if (++duplicateSeCnt >= SE_MAX_DUPLICATE) return null;
                    }
                }
                //未再生
                else
                {
                    //AudioSourceの取得
                    returnAudioSrc ??= mSE_AudioArr[i];
                }
            }
            return returnAudioSrc;
        }

        /// <summary>
        /// SE再生
        /// </summary>
        /// <param name="_seType">SEの種類</param>
        /// <returns>クリップを設定したAudioSource</returns>
        public static AudioSource SE_OneShot(SE_Type _seType)
        {
            if (!Preferences.Se) return null;

            //使用可能なAudioSourceがない場合は再生しない
            AudioSource audio = SE_GetAudioSource(mSE_ClipDic[_seType]);
            if (audio == null) return null;

            //音量,クリップ設定,再生
            audio.volume = mSE_InfoDic[_seType].Volume;
            audio.clip = mSE_ClipDic[_seType];
            audio.Play();
            return audio;
        }

        /// <summary>
        /// SE連続再生
        /// </summary>
        /// <param name="_seType">SEの種類</param>
        /// <returns>クリップを設定したAudioSource</returns>
        public static AudioSource SE_ContinuousPlay(SE_Type _seType)
        {
            if (!Preferences.Se) return null;

            //使用可能なAudioSourceがない場合は再生しない
            AudioSource audio = SE_GetAudioSource(mSE_ClipDic[_seType]);
            if (audio == null) return null;

            //再生開始
            instance.StartCoroutine(SE_ContinuousPlayStart(audio, mSE_InfoDic[_seType]));
            return audio;
        }

        /// <summary>
        /// SE連続再生開始
        /// </summary>
        /// <param name="_audio">AudioSource</param>
        /// <param name="_data">データ</param>
        /// <returns></returns>
        static IEnumerator SE_ContinuousPlayStart(AudioSource _audio, SE_DataData _data)
        {
            //音量,クリップ設定
            _audio.volume = _data.Volume;
            AudioClip seClip = mSE_ClipDic[_data.SE_TYPE];

            //再生間隔設定
            WaitForSeconds playSpan = new WaitForSeconds(_data.Playspan);

            //再生
            for (int i = 0; i < _data.Playtimes; i++)
            {
                if (!Preferences.Se) break;
                _audio.clip = seClip;
                _audio.Play();
                yield return playSpan;
            }
        }

        /// <summary>
        /// SEを止める
        /// </summary>
        /// <param name="_audio">止めるAudioSource</param>
        public static void SE_Stop(AudioSource _audio)
        {
            if (_audio == null) return;
            _audio.Stop();
        }

        /// <summary>
        /// SEをすべて止める
        /// </summary>
        public static void SE_StopAll()
        {
            for (int i = 0; i < SE_PLAY_MAX; i++)
            {
                if (!mSE_AudioArr[i].isPlaying) continue;
                SE_Stop(mSE_AudioArr[i]);
            }
        }
    }
}
