using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class Audio : MonoBehaviour

    
{

    [SerializeField]
    private Slider m_MusicVolumeSlider;

    [SerializeField]
    private Slider m_EffectsVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {

        
        //playerprefas를 이용해 저장된 값을 불러옴
        m_MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        

        m_EffectsVolumeSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 0.5f);
        

        //변경될때 값을 자동 저장
       m_MusicVolumeSlider.onValueChanged.AddListener(OnMusicVolume);
       m_EffectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolume);
    }

    public void OnMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnEffectsVolume(float value)
    {
        PlayerPrefs.SetFloat("EffectsVolume", value);
        AudioManager.Instance.SetEffectsVolume(value);
    }
}
