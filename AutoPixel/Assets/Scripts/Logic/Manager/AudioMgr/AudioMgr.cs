using System.Collections;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

namespace Logic.Manager.AudioMgr
{
    [ManagerDefine(15, true)]
    public sealed class AudioMgr : Manager<AudioMgr>, IManager
    {
        private Dictionary<int, AudioClip> m_audioClips;
        private Stack<AudioSource> m_sources;
        private Dictionary<int, AudioSource> m_playingAudio;
        private AudioSource m_curBg;

        public override void OnAwake()
        {
            m_audioClips = new Dictionary<int, AudioClip>();
            m_sources = new Stack<AudioSource>();
            m_playingAudio = new Dictionary<int, AudioSource>();
        }

        public void PlayBackBg()
        {
            if(m_curBg.time > 30)
            {
                m_curBg.time -= 30;
            }
            else
            {
                m_curBg.time = 0;
            }
        }

        public void SetBgVolume(float volume)
        {
            if(m_curBg)
            {
                m_curBg.volume = volume;
            }
        }

        public void PlayBG(int audio, float volume = 1f)
        {
            if (!m_curBg)
            {
                m_curBg = Alloc();
                m_curBg.loop = true;
            }

            if (m_audioClips.TryGetValue(audio, out var audioClip))
            {

                m_curBg.clip = audioClip;
                m_curBg.volume = volume;
                m_curBg.time = 0;
                m_curBg.Play();
                
            }
        }
        
        public void Play(int audio, bool loop = false, float volume = 1f, float time = 0f)
        {
            if (m_audioClips.TryGetValue(audio, out var audioClip))
            {
                if (Get(audio, out var audioSource))
                {
                    audioSource.time = 0;
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = audioClip;
                    audioSource.loop = loop;
                    audioSource.volume = volume;
                    audioSource.time = time;
                    audioSource.Play();
                    m_playingAudio.Add(audio, audioSource);
                }
            }
        }

        private bool Get(int audio, out AudioSource audioSource)
        {
            // 如果正在播放 把在播放的source返回
            if (m_playingAudio.TryGetValue(audio, out audioSource))
            {
                return true;
            }

            //否则返回空source
            if (m_sources.Count > 0)
            {
                audioSource = m_sources.Pop();
            }
            else
            {
                audioSource = Alloc();
            }
            return false;
        }

        private AudioSource Alloc()
        {
            var source = GameRoot.m_instance.gameObject.AddComponent<AudioSource>();
            return source;
        }

        private void Return(AudioSource audioSource)
        {
            audioSource.Stop();
            m_sources.Push(audioSource);
            //m_playingAudio.Remove(audioSource);
        }

        public override IEnumerator PreInit()
        {
            var audios = Resources.LoadAll<AudioClip>("Audio/");
            foreach (var audioClip in audios)
            {
                audioClip.LoadAudioData();
                m_audioClips.Add(int.Parse(audioClip.name), audioClip);
            }
            yield return null;
        }
    }

    public class AudioDefine
    {
        public const int AudioDefineBase = 1000000;
        public const int CommonClick = 1000001;
        public const int Jump = 1000002;
        public const int Control = 1000003;
        public const int Death = 1000004;
        public const int StoneBreak = 1000005;
        public const int Bird = 1000006;
        public const int Collide = 1000007;
        public const int Rotate = 1000008;
        public const int Wind = 1000009;
        public const int MainSceneBgm = 1000014;
        public const int FillLight = 1000015;
    }
}