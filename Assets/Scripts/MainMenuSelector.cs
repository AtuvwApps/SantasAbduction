using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class MainMenuSelector : MonoBehaviour
{
    private int audio; //0 = muted, 1 = playing
    public Camera camera;
    public Button muteButton;
    public Sprite soundOn;
    public Sprite soundOff;

    // Start is called before the first frame update
    void Start()
    {
        audio = PlayerPrefs.GetInt("audio", 1);
        //If muted from last time set muted symbol to button
        if(audio == 0)
        {
            camera.GetComponent<AudioListener>().enabled = false;
            AudioListener.volume = 0;
            muteButton.GetComponent<Image>().sprite = soundOff;
        }

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }

    public void Play()
    {
        SceneManager.LoadScene(31);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void Mute()
    {
        if(audio == 0)
        {
            camera.GetComponent<AudioListener>().enabled = true;
            AudioListener.volume = 1;
            audio = 1;
            muteButton.GetComponent<Image>().sprite = soundOn;
        }
        else
        {
            camera.GetComponent<AudioListener>().enabled = false;
            AudioListener.volume = 0;
            audio = 0;
            muteButton.GetComponent<Image>().sprite = soundOff;
        }

        PlayerPrefs.SetInt("audio", audio);
    }
}
