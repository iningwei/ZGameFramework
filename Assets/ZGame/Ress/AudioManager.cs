using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;
using static UnityEngine.GraphicsBuffer;

namespace ZGame.Ress
{
    public class AudioManager : Singleton<AudioManager>
    {
        GameObject gameObj;
        AudioSource bgmAudioSource;


        Dictionary<int, AudioSource> soundAudioSourceDic = new Dictionary<int, AudioSource>();

        Dictionary<string, AudioClip> soundClipDic = new Dictionary<string, AudioClip>();
        bool isSoundLimitPlaying(string name, int limitCount = 1)
        {
            int count = 0;
            foreach (var item in soundAudioSourceDic)
            {
                if (item.Value.isPlaying && item.Value.clip.name == name)
                {
                    count++;
                }
            }
            if (count >= limitCount)
            {
                return true;
            }
            return false;
        }
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
                soundAudioSourceDic[target].volume = Storage.GetSoundValue();
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
            bgmAudioSource.volume = Storage.GetSoundValue();

            EventDispatcher.Instance.AddListener(EventID.OnSoundSliderValueChange, onSoundChanged);
        }

        private void onSoundChanged(string evtId, object[] paras)
        {
            float soundValue = float.Parse(paras[0].ToString());
            bgmAudioSource.volume = soundValue;

            foreach (var item in soundAudioSourceDic)
            {
                item.Value.volume = soundValue;
            }
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
            PlayerPrefs.Save();
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
            PlayerPrefs.Save();
        }


        public void ClearBGM()
        {
            if (bgmAudioSource.clip != null)
            {
                if (bgmAudioSource.isPlaying)
                {
                    bgmAudioSource.Stop();
                }
                var res = ABManager.Instance.GetCachedRes(ABType.Audio, bgmAudioSource.clip.name);
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
                var res = ABManager.Instance.GetCachedRes(ABType.Audio, bgmAudioSource.clip.name);
                if (res != null)
                {
                    res.Destroy();
                }

                bgmAudioSource.clip = null;
            }



            ABManager.Instance.LoadAudioClip(name, (clip) =>
            {
                bgmAudioSource.clip = clip;
                bgmAudioSource.Play();
                bgmAudioSource.loop = isLoop;
            }, true);
        }



        public void StopBGM()
        {
            if (bgmAudioSource.clip != null && bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }
        }

        public int PlaySound(string name, float volume, bool isLoop = false)
        {
            if (IsSoundEnabled == false)
            {
                return 0;
            }
            if (name == "0")
            {
                return 0;
            }
            //限制同时一种声音播放个数
            if (isSoundLimitPlaying(name, 2))
            {
                return 0;
            }

            var audioSourceId = getFreeSoundAudioSourceID();
            var audioSource = soundAudioSourceDic[audioSourceId];
            audioSource.loop = isLoop;

            if (soundClipDic.ContainsKey(name))
            {
                audioSource.clip = soundClipDic[name];
            }
            else
            {
                //////var clip = Resources.Load("Audio/" + name, typeof(AudioClip)) as AudioClip;
                //////audioSource.clip = clip; 
                //////soundClipDic.Add(name, clip);

                //
                ABManager.Instance.LoadAudioClip(name, (clip) =>
                {
                    audioSource.clip = clip;
                    soundClipDic.Add(name, clip);
                }, true);
            }

            //设置音量
            audioSource.volume = volume;
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