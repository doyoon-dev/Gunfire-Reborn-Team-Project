using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    #region Variable
    const float m_defaultVelue = -20f;//0 = 100%, -40 = 0%
    public bool endloop = false;
    AudioMixer audioMixer;
    AudioSource[] m_audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> m_audioClips = new Dictionary<string, AudioClip>();
    #endregion
    public float GetAudioGropValue(string _grop)
    {
        audioMixer.GetFloat(_grop, out var l_value);
        if (l_value == -80) return -40;
        else return l_value;
    }

    public void SetAudioGropValue(string _grop, float _value)
    {
        if (_value <= -40f) _value = -80;

        audioMixer.SetFloat(_grop, _value);
    }
    public void ssss()
    {
       Debug.LogError(GetAudioGropValue("Master"));
    }
    public void InitAudioMixer()//오디오 그룹의 모든 볼륨을 디폴트 값으로 초기화
    {
        SetAudioGropValue(Define.Sound.Master.ToString(), m_defaultVelue);
        SetAudioGropValue(Define.Sound.BGM.ToString(), m_defaultVelue);
        SetAudioGropValue(Define.Sound.UI.ToString(), m_defaultVelue);
        SetAudioGropValue(Define.Sound.Effect.ToString(), m_defaultVelue);
    }

    public void InitAudioMixer(float master, float bgm, float ui, float effect)//오디오 그룹의 모든 볼륨을 디폴트 값으로 초기화
    {
        SetAudioGropValue(Define.Sound.Master.ToString(), master);
        SetAudioGropValue(Define.Sound.BGM.ToString(), bgm);
        SetAudioGropValue(Define.Sound.UI.ToString(), ui);
        SetAudioGropValue(Define.Sound.Effect.ToString(), effect);
    }

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);
            audioMixer = //Resources.Load<AudioMixer>($"Sounds/{Define.Path.SoundMixer}");
                Managers.Resource.Load<AudioMixer>($"Sounds/{Define.Path.SoundMixer}");

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound)); // "Bgm", "Effect"
            for (int i = 1; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                m_audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;

            }
            m_audioSources[(int)Define.Sound.BGM].loop = true; // bgm 재생기는 무한 반복 재생
        }

    }

    public void Play(AudioClip audioClip, bool isloop, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null) return;

        if (type == Define.Sound.BGM) // BGM 배경음악 재생
        {
            AudioSource audioSource = m_audioSources[(int)Define.Sound.BGM];
            if (audioSource.isPlaying) audioSource.Stop();

            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[(int)Define.Sound.BGM];
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else // Effect 효과음 재생
        {
            AudioSource audioSource = m_audioSources[(int)type];
            if (endloop)
            {
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
                endloop = false;
            }
            if (isloop)
            {
                audioSource.loop = true;
                audioSource.clip = audioClip;
                audioSource.Play();
                endloop = true;
            }
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[(int)type];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);

        }
    }

    public void Play(string path, bool isloop, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, isloop, type, pitch);
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // Sound 폴더 안에 저장될 수 있도록

        AudioClip audioClip = null;

        if (type == Define.Sound.BGM) // BGM 배경음악 클립 붙이기
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else // Effect 효과음 클립 붙이기
        {
            if (m_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                m_audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null) Debug.LogError($"AudioClip Missing ! {path}");
        return audioClip;
    }

    public void Clear()
    {
        // 재생기 전부 재생 스탑, 음반 빼기
        foreach (AudioSource audioSource in m_audioSources)
        {
            if (audioSource == null) continue;
            audioSource.clip = null;
            audioSource.Stop();
        }
        // 효과음 Dictionary 비우기
        m_audioClips.Clear();
    }
}