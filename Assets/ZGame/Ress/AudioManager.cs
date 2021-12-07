using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    public class AudioManager : Singleton<AudioManager>
    {
        GameObject gameObj;
        AudioSource bgmAudioSource;


        Dictionary<int, AudioSource> soundAudioSourceDic = new Dictionary<int, AudioSource>();
        int getFreeSoundAudioSourceID()
        {
            int target = 0;
            int count = soundAudioSourceDic.Count;

            foreach (var item in soundAudioSourceDic)
            {
                if (item.Value.isPlaying == false)
                {
                    target = item.Key;
                    break;
                }
            }


            if (target == 0)
            {
                target = count + 1;
                soundAudioSourceDic[target] = gameObj.AddComponent<AudioSource>();
                soundAudioSourceDic[target].playOnAwake = false;
            }
            return target;
        }

        public AudioManager()
        {
            gameObj = new GameObject();
            gameObj.name = "AudioRoot";
            Object.DontDestroyOnLoad(gameObj);

            gameObj.GetOrAddComponent<AudioListener>();
            bgmAudioSource = gameObj.GetOrAddComponent<AudioSource>();
            bgmAudioSource.playOnAwake = false;
        }


        public bool IsBGMEnabled
        {
            get { return PlayerPrefs.GetInt("IsBGMEnabled", 1) == 1; }
        }



        public void SetBGMEnableStatus(bool isEnable)
        {
            if (isEnable)
            {
                PlayerPrefs.SetInt("IsBGMEnabled", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsBGMEnabled", 0);
            }
        }

        public bool IsSoundEnabled
        {
            get { return PlayerPrefs.GetInt("IsSoundEnabled", 1) == 1; }
        }

        public void SetSoundEnableStatus(bool isEnable)
        {
            if (isEnable)
            {
                PlayerPrefs.SetInt("IsSoundEnabled", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsSoundEnabled", 0);
            }
        }


        public void ClearBGM()
        {
            if (bgmAudioSource.clip != null)
            {
                if (bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Stop();
                }
                var res = ABManager.Instance.GetRes(ABType.Audio, bgmAudioSource.clip.name);
                if (res != null)
                {
                    res.Destroy();
                }
                bgmAudioSource.clip = null;
            }
        }

        public float GetCurBGMClipLength()
        {
            float length = 0f;
            if (bgmAudioSource.clip != null)
            {
                length = bgmAudioSource.clip.length;
            }

            return length;
        }

        public void ResumeBGM()
        {
            if (IsBGMEnabled == false)
            {
                return;
            }
            if (bgmAudioSource.clip == null || (bgmAudioSource.isPlaying == false && bgmAudioSource.loop == false))
            {
                return;
            }
            bgmAudioSource.Play();
        }

        public void PlayBGM(string name, bool isLoop)
        {
            if (IsBGMEnabled == false)
            {
                return;
            }
            //clip里面正是目标音乐
            if (bgmAudioSource.clip != null && bgmAudioSource.clip.name == name)
            {
                if (bgmAudioSource.isPlaying == false)
                {

                    bgmAudioSource.Play();
                }

                if (bgmAudioSource.loop != isLoop)
                {
                    bgmAudioSource.loop = isLoop;
                }
                return;
            }

            //clip里不是目标背景音乐，先卸载之
            if (bgmAudioSource.clip != null && bgmAudioSource.clip.name != name)
            {
                if (bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Stop();
                }
                var res = ABManager.Instance.GetRes(ABType.Audio, bgmAudioSource.clip.name);
                if (res != null)
                {
                    res.Destroy();
                }

                bgmAudioSource.clip = null;
            }



            AudioClip clip = ABManager.Instance.LoadAudioClip(name);
            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
            bgmAudioSource.loop = isLoop;

        }



        public void StopBGM()
        {
            if (bgmAudioSource.clip != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }
        }

        public int PlaySound(string name, bool isLoop = false)
        {
            if (IsSoundEnabled == false)
            {
                return 0;
            }

            var audioSourceId = getFreeSoundAudioSourceID();
            var audioSource = soundAudioSourceDic[audioSourceId];
            audioSource.loop = isLoop;
            var clip = ABManager.Instance.LoadAudioClip(name);
            audioSource.clip = clip;
            audioSource.Play();
            return audioSourceId;
        }

        public void StopSound(int id)
        {
             
            if (soundAudioSourceDic.ContainsKey(id))
            {
                 
                soundAudioSourceDic[id].Stop();
            }
        }

    }
}