using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using UnityEngine.UI;
using System;
using System.Threading;

public class AdsManager : MonoBehaviour, IRewardedVideoAdListener
{
	public static bool Loaded = false;
    private static Level _level;


    public bool GiveRewardFromAnotherThread = false;
    public bool LoseAnywayFromanoterThread = false;
    private void Update()
    {
        if (GiveRewardFromAnotherThread)
        {
            _level.ContinueLevel();
            GiveRewardFromAnotherThread = false;
        }
        if (LoseAnywayFromanoterThread)
        {
            _level.LoseLevel();
            LoseAnywayFromanoterThread = false;

        }
    }
    private void Start()
	{
        Appodeal.SetTesting(false);
        Initialize();
    }

    public void Initialize()
    {
        int adTypes = AppodealAdType.Banner | AppodealAdType.RewardedVideo;
        AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
        Appodeal.Initialize("b1d0b07f7a7064603209c04f04e58042457628b4b42b4091", adTypes);
        //StartCoroutine("WaitAndPrint");

        AppodealCallbacks.Banner.OnLoaded += OnBannerLoaded;
        AppodealCallbacks.Banner.OnExpired += OnBannerExpired;
        AppodealCallbacks.Banner.OnShowFailed += OnBannerShowFailed;
        AppodealCallbacks.Banner.OnFailedToLoad += OnBannerFailed;
        AppodealCallbacks.Banner.OnShown += OnBannerShown;
        //AppodealCallbacks.RewardedVideo.OnShown += OnRewardedVideoShown;
        Appodeal.SetRewardedVideoCallbacks(this);
        Appodeal.SetLogLevel(AppodealLogLevel.Verbose);
    }


    private void OnBannerFailed(object sender, EventArgs e)
    {
        Debug.Log("LoadFailed");
        //Appodeal.Hide(AppodealAdType.Banner);
        //BannerShowing = false;
    }

    private void OnBannerShown(object sender, EventArgs e)
    {
        Debug.Log("Shown");
        //Thread.Sleep(7);
        //Appodeal.Hide(AppodealAdType.Banner);

    }

    private void OnBannerShowFailed(object sender, EventArgs e)
    {

        Debug.Log("ShowFailed");
        //Appodeal.Hide(AppodealAdType.Banner);
    }

    private void OnBannerExpired(object sender, EventArgs e)
    {
        Debug.Log("Expired");
       // Appodeal.Hide(AppodealAdType.Banner);
    }

    private void OnBannerLoaded(object sender, BannerLoadedEventArgs e)
    {
        Debug.Log("LoadedBanner");
        if (Appodeal.IsLoaded(AppodealAdType.Banner) && Appodeal.CanShow(AppodealAdType.Banner))
        {
            Appodeal.Show(AppodealShowStyle.BannerBottom);
        }
    }

    public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) 
	{

        Debug.Log("Initialization Finished");
    }

    public static bool ShowRewardAD(Level level)
    {
        _level = level;

        if (Appodeal.CanShow(AppodealAdType.RewardedVideo))
        {
            Debug.Log("Vid CanShow");
            Appodeal.Show(AppodealShowStyle.RewardedVideo);
            return true;
        }
     
        return false;
    }
    public void OnRewardedVideoLoaded(bool isPrecache)
    {
        Debug.Log("Loaded");
        Loaded = true;
    }

    public void OnRewardedVideoFailedToLoad()
    {
        Debug.Log("FailLoad");
        Loaded = false;
    }

    public void OnRewardedVideoShowFailed()
    {
        Debug.Log("FailShown");
        LoseAnywayFromanoterThread = true;
    }

    public void OnRewardedVideoShown()
    {
        Debug.Log("Shown");
    }

    public void OnRewardedVideoFinished(double amount, string currency)
    {
        Debug.Log("FinishVideo");
        GiveRewardFromAnotherThread = true;
    }

    public void OnRewardedVideoClosed(bool finished)
    {
        Debug.Log("VideoClose");
        if (finished)
            GiveRewardFromAnotherThread = true;
        else
            LoseAnywayFromanoterThread = true;
    }

    public void OnRewardedVideoExpired()
    {
        Loaded = false;
    }

    public void OnRewardedVideoClicked()
    {
        Debug.Log("VideoClicked");
        GiveRewardFromAnotherThread = true;
    }
}
