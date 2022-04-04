using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundFunction
{
    public class SoundManager : MonoBehaviour
    {
        //---------------------------------
        //共通
        //---------------------------------
        [Header("共通")]
        [Header("BGM")]
        [SerializeField]
        AudioClip[] bgm;
        [Header("Yesタップ")]
        [SerializeField]
        AudioClip yesTap;
        [Header("noタップ")]
        [SerializeField]
        AudioClip noTap;
        [Header("ゲームオーバー")]
        [SerializeField]
        AudioClip[] gameOver;
        [Header("ゲームクリア")]
        [SerializeField]
        AudioClip gameClear;

        //---------------------------------
        //パズルモード
        //---------------------------------
        [Space(20)]
        [Header("パズルモード")]
        [Header("ハムスター移動")]
        [SerializeField]
        AudioClip panelChange;
        [Header("収穫")]
        [SerializeField]
        AudioClip[] harvest;  //通常:0  列(横):1  行(縦):2
        [Header("咀嚼")]
        [SerializeField]
        AudioClip eat;

        //---------------------------------
        //シュートモード
        //---------------------------------
        [Space(20)]
        [Header("シュートモード")]
        [Header("スタート歩き")]
        [SerializeField]
        AudioClip startWalk;
        [Header("引っ張り")]
        [SerializeField]
        AudioClip pull;
        [Header("投擲")]
        [SerializeField]
        AudioClip bloackThrow;
        [Header("跳ね返り")]
        [SerializeField]
        AudioClip rebound;
        [Header("ブロック接触")]
        [SerializeField]
        AudioClip connect;
        [Header("接触削除開始")]
        [SerializeField]
        AudioClip connectDelete;
        [Header("自由落下開始")]
        [SerializeField]
        AudioClip freeFall;
        [Header("収穫")]
        [SerializeField]
        AudioClip harvestShoot;
        [Header("スペシャルタップ")]
        [SerializeField]
        AudioClip specialTap;
        [Header("スペシャル歩き")]
        [SerializeField]
        AudioClip specialWalk;
        [Header("スペシャル収穫")]
        [SerializeField]
        AudioClip specialHarvest;
        [Header("フィーバー開始")]
        [SerializeField]
        AudioClip feverStart;
        [Header("フィーバーBGM")]
        [SerializeField]
        AudioClip feverBGM;
        [Header("フィーバー収穫")]
        [SerializeField]
        AudioClip feverHarvest;
        [Header("nextブロック交換")]
        [SerializeField]
        AudioClip blockCange;

        AudioSource audio_SE;  //SE_AudioSource    
        AudioSource audio_BGM; //BGM_AudioSource
        [System.NonSerialized]
        public int bgmIndex;

        public static SoundManager instance = null;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                audio_SE = GetComponent<AudioSource>();
                audio_BGM = transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        //=============================================
        //BGM
        //=============================================
        //BGM開始
        public void BGM_Start(int seIndex)
        {
            bgmIndex = seIndex;
            audio_BGM.clip = bgm[seIndex];
            audio_BGM.Play();
            if (EnvironmentalSetting.bgm)
            {
                float volume = (seIndex == 0) ? 0.2f : 0.5f;
                audio_BGM.volume = 0.0f;
                StartCoroutine(BGM_Volume_Fade(volume));
            }
        }
        //BGM再開
        public void BGM_Restart()
        {
            if (EnvironmentalSetting.bgm)
            {
                float volume = (bgmIndex == 0) ? 0.2f : 0.5f;
                audio_BGM.volume = 0.0f;
                StartCoroutine(BGM_Volume_Fade(volume));
            }
        }
        //BGM音量フェード
        public IEnumerator BGM_Volume_Fade(float volume)
        {
            float oneFlameTime = 0.02f;
            if (audio_BGM.volume < volume)
            {
                //フェードイン
                while (EnvironmentalSetting.bgm)
                {
                    audio_BGM.volume += oneFlameTime;
                    yield return new WaitForSecondsRealtime(oneFlameTime);
                    if (audio_BGM.volume >= volume - oneFlameTime)
                    {
                        audio_BGM.volume = volume;
                        break;
                    }
                }
            }
            else
            {
                //フェードアウト
                while (EnvironmentalSetting.bgm)
                {
                    audio_BGM.volume -= oneFlameTime;
                    yield return new WaitForSecondsRealtime(oneFlameTime);
                    if (audio_BGM.volume <= volume + oneFlameTime)
                    {
                        audio_BGM.volume = volume;
                        break;
                    }
                }
            }
        }
        //BGM音量設定
        public void BGM_Volume(float volume)
        { audio_BGM.volume = volume; }


        //=============================================
        //共通
        //=============================================

        //SEを止める
        public void SE_Stop()
        { audio_SE.Stop(); }

        //SE音量設定
        public void SE_Volume(float volume)
        { audio_SE.volume = volume; }

        //Yes
        public void YesTapSE()
        { audio_SE.PlayOneShot(yesTap); }

        //No
        public void NoTapSE()
        { audio_SE.PlayOneShot(noTap); }

        //ゲームオーバー
        public void GameOverSE(int seIndex)
        { audio_SE.PlayOneShot(gameOver[seIndex]); }

        //ゲームクリア
        public void GameClearSE()
        { audio_SE.PlayOneShot(gameClear); }


        //=============================================
        //パズルモード
        //=============================================

        //パネル移動
        public void PanelChangeSE()
        { audio_SE.PlayOneShot(panelChange); }

        //パネル揃った
        public void HarvestSE(int seIndex)
        { audio_SE.PlayOneShot(harvest[seIndex]); }

        //食べる
        public void EatSE()
        { audio_SE.PlayOneShot(eat); }


        //=============================================
        //シュートモード
        //=============================================

        //スタート
        public void StartWalkSE_Shoot()
        {
            audio_SE.clip = startWalk;
            audio_SE.Play();
        }

        //引っ張り
        public void PullSE_Shoot()
        { audio_SE.PlayOneShot(pull); }

        //投擲
        public void ThrowSE_Shoot()
        { audio_SE.PlayOneShot(bloackThrow); }

        //跳ね返り
        public void ReboundSE_Shoot()
        { audio_SE.PlayOneShot(rebound); }

        //ブロック接触
        public void ConnectSE_Shoot()
        { audio_SE.PlayOneShot(connect); }

        //接触削除開始
        public void ConnectDeleteSE_Shoot()
        { audio_SE.PlayOneShot(connectDelete); }

        //自由落下開始
        public void FeeFallSE_Shoot()
        { audio_SE.PlayOneShot(freeFall); }

        //収穫
        public void HarvestSE_Shoot_Shoot()
        { audio_SE.PlayOneShot(harvestShoot); }

        //スペシャルタップ
        public void SpecialTapSE_Shoot()
        { audio_SE.PlayOneShot(specialTap); }

        //スペシャル歩く
        public void SpecialWalkSE_Shoot()
        {
            audio_SE.clip = specialWalk;
            audio_SE.Play();
        }

        //スペシャル収穫
        public void SpecialHarvestSE_Shoot()
        { audio_SE.PlayOneShot(specialHarvest); }

        //フィーバー開始
        public void FeverStartSE_Shoot()
        { audio_SE.PlayOneShot(feverStart); }

        //フィーバーBGM
        public IEnumerator FeverBGM_Shoot()
        {
            yield return StartCoroutine(BGM_Volume_Fade(0.0f));
            audio_BGM.clip = feverBGM;
            audio_BGM.Play();
            yield return StartCoroutine(BGM_Volume_Fade(0.5f));
        }

        //フィーバー収穫
        public void FeverHarvestSE_Shoot()
        { audio_SE.PlayOneShot(feverHarvest); }

        //nextブロック交換
        public void BlockCangeSE_Shoot()
        { audio_SE.PlayOneShot(blockCange); }
    }
}
