using MatchThreeEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] Button toggleSoundButton;
    [SerializeField] Image disabledSoundImage;
    [SerializeField] Image enabledSoundImage;
    [SerializeField] Button toggleMusicButton;
    [SerializeField] Image disabledMusicImage;
    [SerializeField] Image enabledMusicImage;
    [SerializeField] Button closeButton;

    private void Start() {
        toggleSoundButton.onClick.AddListener(() => {
            
        });
    }

}
