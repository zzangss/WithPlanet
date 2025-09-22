using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
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

    [Header("오디오 소스 (스피커)")]
    [SerializeField]
    private AudioSource m_MusicSource; // 소리를 재생하는 스피커

    [SerializeField]
    private AudioSource m_SfxSource; // 버튼 클릭 효과음 스피커

    [SerializeField] 
    private AudioSource m_LoopingSfxSource;

    // 3D 사운드 재생 시 사용할 볼륨 값을 저장하기 위한 변수
    private float m_CurrentEffectsVolume = 1.0f;

    [SerializeField]
    private AudioClip m_BgMusic; // 배경음악 오디오 파일

    [SerializeField]
    private AudioClip m_ClickSound; // 클릭 효과음 오디오 파일

    private static AudioManager m_Instance;


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
        PlayBgMusic();// 게임 시작 시 배경음악 자동 재생
    }

    // --- 외부(Audio.cs)에서 볼륨을 설정하는 함수들 ---
    public void SetMusicVolume(float volume)
    {
        m_MusicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetEffectsVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        m_SfxSource.volume = clampedVolume;
        m_LoopingSfxSource.volume = clampedVolume;
        m_CurrentEffectsVolume = clampedVolume;
    }


    //배경음악 재생
    public void PlayBgMusic()
    {
        if (m_BgMusic == null) return;

        // 배경음악 클립 설정
        m_MusicSource.clip = m_BgMusic;
        // 반복 설정
        m_MusicSource.loop = true;
        // 재생
        m_MusicSource.Play();
    }

    // 일반 버튼 클릭 효과음 재생
    public void PlayButtonClickSound()
    {
        PlayLoopingSfx(m_ClickSound);
        
    }
    /*
    // 실시간 음악 볼륨 조절
    public void SetMusicVolume(float volume)
    {
        m_MusicAudioSource.volume = volume;
    }
    */

    //효과음들 추가

    //1. 단발성 효과음 재생(UI 클릭, 피격, 아이템 획득)
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        m_SfxSource.PlayOneShot(clip);
    }

    //2. 반복 효과음 재생 (환경음_
    public void PlayLoopingSfx(AudioClip clip)
    {
        if (clip == null || (m_LoopingSfxSource.isPlaying && m_LoopingSfxSource.clip == clip)) return;
        m_LoopingSfxSource.clip = clip;
        m_LoopingSfxSource.loop = true;
        m_LoopingSfxSource.Play();
    }

    public void StopLoopingSfx()
    {
        m_LoopingSfxSource.Stop();
    }

    // 3D 위치 지정 효과음 재생 (NPC 대화, 폭발음 등)
    public void PlaySfxAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, m_CurrentEffectsVolume);
    }

}
