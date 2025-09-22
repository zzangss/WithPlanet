using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_MusicAudioSource; // 소리를 재생하는 스피커

    [SerializeField]
    private AudioSource m_EffectsAudioSource; // 버튼 클릭 효과음 스피커

    [SerializeField]
    private AudioClip m_BgMusic; // 배경음악 오디오 파일

    [SerializeField]
    private AudioClip m_ClickSound; // 클릭 효과음 오디오 파일

    private static AudioManager m_Instance;

    //싱글톤 패턴
    public static AudioManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<AudioManager>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    m_Instance = obj.AddComponent<AudioManager>();
                }
            }
            return m_Instance;
        }
    }
    private void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject); //씬이 바뀌어도 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); //중복된 오브젝트가 있으면 파괴
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        PlayBgMusic();
    }


    //배경음악 재생

    public void PlayBgMusic()
    {
        //저장된 값을 설정
        m_MusicAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        //배경음악 재생
        m_MusicAudioSource.clip = m_BgMusic;
        //반복
        m_MusicAudioSource.loop = true;
        //재생
        m_MusicAudioSource.Play();
    }

    //버튼 클릭 효과음 재생
    public void PlayButtonClickSound()
    {
        float volume=PlayerPrefs.GetFloat("EffectsVolume",0.5f);
        m_EffectsAudioSource.PlayOneShot(m_ClickSound,volume);

    }

    // 실시간 음악 볼륨 조절
    public void SetMusicVolume(float volume)
    {
        m_MusicAudioSource.volume = volume;
    }

    //효과음들 추가

    //1. 단발성 효과음 재생(UI 클릭, 피격, 아이템 획득)
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        m_EffectsAudioSource.PlayOneShot(clip);
    }

    //2. 반복 효과음 재생 (환경음
}
