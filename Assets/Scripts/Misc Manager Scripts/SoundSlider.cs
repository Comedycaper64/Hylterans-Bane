using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] private SliderStruct.SliderType sliderType;

    public static event EventHandler<SliderStruct> OnAnySliderChanged;

    private void Start() 
    {
        switch (sliderType)
        {
            case SliderStruct.SliderType.Master:
                GetComponent<Slider>().value = SoundManager.Instance.GetMasterVolume();
                break;
            case SliderStruct.SliderType.Music:
                GetComponent<Slider>().value = SoundManager.Instance.GetJustMusicValue();
                break;
            case SliderStruct.SliderType.SFX:
                GetComponent<Slider>().value = SoundManager.Instance.GetJustSFXValue();
                break;    
        }

    }

    public void OnSliderChanged()
    {
        OnAnySliderChanged?.Invoke(this, new SliderStruct(sliderType, GetComponent<Slider>().value));
    }
}
