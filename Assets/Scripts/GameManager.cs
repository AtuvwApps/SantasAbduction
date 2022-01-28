using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class GameManager : MonoBehaviour
{
    private int currentSceneIndex;
    public GameObject pauseMenuUI;
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject skipPanel;
    public GameObject preview;
    public Text levelNumber;
    public Camera camera;
    private int audio; //0 = muted, 1 = playing
    public Button muteButton;
    public Sprite soundOn;
    public Sprite soundOff;
    private RewardedAd rewardedAd;
    private InterstitialAd interstitial;
    //Used to keep track of which panel opened Skip, for backward navigation
    private int skipPanelInitiator;
    private bool rewarded;
    public GameObject errorMessage;

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
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        rewarded = false;
        String index = currentSceneIndex.ToString();
        levelNumber.text = "Level "+index;

        CreateAndLoadRewardedAd();

        if(currentSceneIndex % 2 == 0)
        {
            CreateAndLoadInterstitialAd();
        }

        //Freeze time until player clicks to start game
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(rewarded)
        {
            if(currentSceneIndex == 30)
            {
                stageComplete();
            }
            else{
                winGame();
                int currentUnlock = PlayerPrefs.GetInt("levelReached");
                int nextUnlock = currentSceneIndex + 1;
                if(nextUnlock > currentUnlock)
                {
                    PlayerPrefs.SetInt("levelReached", nextUnlock);
                }
                rewarded = false;
            }
        }
    }

    public void previewClicked()
    {
        //Unfreeze time and start the game
        preview.SetActive(false);
        Time.timeScale = 1;
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        pausePanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void winGame()
    {
        if(currentSceneIndex % 2 == 0)
        {
            showInterstitial();
        }

        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        pausePanel.SetActive(false);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
        skipPanel.SetActive(false);
    }

    public void stageComplete()
    {
        SceneManager.LoadScene(32);
    }

    public void loseGame()
    {
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    public void resumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void returnToHome()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void restartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void nextLevel()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(currentSceneIndex+1);
    }

    public void muteAudio()
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

    public void skipLevel()
    {
        if(pausePanel.activeSelf)
        {
            skipPanelInitiator = 0;
        }
        else if(losePanel.activeSelf)
        {
            skipPanelInitiator = 1;
        }
        skipPanel.SetActive(true);
        pausePanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void cancelSkip()
    {
        skipPanel.SetActive(false);
        if(skipPanelInitiator == 0)
        {
            pausePanel.SetActive(true);
        }
        else if(skipPanelInitiator == 1)
        {
            losePanel.SetActive(true);
        }
    }


    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        this.CreateAndLoadRewardedAd();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        rewarded = true;
    }

    public void showAd()
    {
        if(CheckConnection())
        {
            if (this.rewardedAd.IsLoaded()) 
            {
                this.rewardedAd.Show();
            }
            else
            {
                CreateAndLoadRewardedAd();
            }
            errorMessage.SetActive(false);
        }
        else
        {
            errorMessage.SetActive(true);
        }
    }

    public bool CheckConnection()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void CreateAndLoadRewardedAd()
    {
        string adUnitId;
        #if UNITY_ANDROID
            //My adUnit: adUnitId = "[adUnitId]";
            //Test adUnit: adUnitId = "ca-app-pub-3940256099942544/5224354917";
            adUnitId = "[adUnitId]";
        #elif UNITY_IPHONE
            //My adUnit: adUnitId = "[adUnitId]";
            //Test adUnit: adUnitId = "ca-app-pub-3940256099942544/1712485313";
            adUnitId = "[adUnitId]";
        #else
            adUnitId = "unexpected_platform";
        #endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void CreateAndLoadInterstitialAd()
    {
        string adUnitId;
        #if UNITY_ANDROID
            //My adUnit: "[adUnitId]"
            //Test adUnit: "ca-app-pub-3940256099942544/1033173712"
            adUnitId = "[adUnitId]";
        #elif UNITY_IPHONE
            //My adUnit: "[adUnitId]"
            //Test adUnit: "ca-app-pub-3940256099942544/4411468910"
            adUnitId = "[adUnitId]";
        #else
            adUnitId = "unexpected_platform";
        #endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);


        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    public void showInterstitial()
    {
        if (this.interstitial.IsLoaded()) {
            this.interstitial.Show();
        }
    }
}
