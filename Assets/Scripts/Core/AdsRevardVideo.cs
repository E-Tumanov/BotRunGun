using System;
using UnityEngine;
using UnityEngine.UI;


#if (IRON_REWARD)

public class AdsRevardVideo : MonoBehaviour
{
    public static AdsRevardVideo inst;
    public event System.Action OnEarnedReward = delegate { };
    public event System.Action OnForceCloseReward = delegate { };

    bool isLoaded;

    private void Awake()
    {
        inst = this;
    }

    void Start ()
    {
        //IronSource.Agent.init("db616649", IronSourceAdUnits.REWARDED_VIDEO);

        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
    }


    public bool IsLoadVideo ()
    {
        return isLoaded;
    }

    private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
    {
        Debug.LogWarning("IRON_INTERSTITIAL> Ad Show Failed");
        OnForceCloseReward?.Invoke();
    }

    private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
    {
        Debug.LogWarning("IRON_INTERSTITIAL> Досмотрел");
        OnEarnedReward?.Invoke();
    }

    private void RewardedVideoAdEndedEvent()
    {
    }

    private void RewardedVideoAdStartedEvent()
    {
    }

    private void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        isLoaded = available;
    }

    private void RewardedVideoAdClosedEvent()
    {
        Debug.LogWarning("IRON_INTERSTITIAL> Force closed");
        OnForceCloseReward?.Invoke();
    }


    private void RewardedVideoAdClickedEvent(IronSourcePlacement obj)
    {
    }

    private void RewardedVideoAdOpenedEvent()
    {    
    }

    public void PlayVideo ()
    {
        if (!isLoaded)
        {
            
            //DefaultInterstitial
            Debug.LogWarning ("IRON_REWARD> MISS SHOW");
            OnForceCloseReward?.Invoke ();
            return;
        }

        isLoaded = false;
        IronSource.Agent.showRewardedVideo ();
    }

    void OnApplicationPause (bool isPaused)
    {
        IronSource.Agent.onApplicationPause (isPaused);
    }
}

#elif (GGL_REWARD)

using GoogleMobileAds.Api;

/// <summary>
/// Манагер рекламы
/// </summary>
public class AdsRevardVideo : MonoBehaviour
{
    public event System.Action OnEarnedReward = delegate { };
    public event System.Action OnForceCloseReward = delegate { };

    private RewardedAd rewardedAd;


    private void Awake()
    {
        MobileAds.Initialize(initStatus => {

            Debug.Log("......... OK ADS init");
            LoadVideo();
        });
    }

    
    public void LoadVideo()
    {
        if (rewardedAd != null)
        {
            rewardedAd.OnAdLoaded -= OnAdLoaded;
            rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;
            rewardedAd.OnAdClosed -= OnAdClosed;
        }

        //  Сразу грузим след. ролик
#if (UNITY_ANDROID)
        string adUnitId = "ca-app-pub-2640647163440835/7576348940";
#elif (UNITY_IOS)
        string adUnitId = "ca-app-pub-2640647163440835/4513366013";
#endif 

        rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += OnAdLoaded;
        rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
        rewardedAd.OnAdClosed += OnAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);

        //  Сбросим старых подписчиков. Ролик уже другой. Значит старое не нужно
        OnEarnedReward = delegate { };
        OnForceCloseReward = delegate { };
    }


    public bool IsLoadVideo()
    {
        if (rewardedAd == null)
            return false;

        return rewardedAd.IsLoaded();
    }

    public void PlayVideo()
    {
        rewardedAd.Show();
    }

    private void OnAdClosed(object sender, System.EventArgs e)
    {
        OnForceCloseReward();
        OnEarnedReward = delegate { };
        OnForceCloseReward = delegate { };
        LoadVideo();
    }

    private void OnUserEarnedReward(object sender, Reward e)
    {
        OnEarnedReward();
        OnEarnedReward = delegate { };
        OnForceCloseReward = delegate { };
        LoadVideo();
    }

    private void OnAdLoaded(object sender, System.EventArgs e)
    {
        //rewardedAd.Show();
    }

}
#else

public class AdsRevardVideo : MonoBehaviour
{
    public event System.Action OnEarnedReward = delegate { };
    public event System.Action OnForceCloseReward = delegate { };

    public void PlayVideo()
    {
        OnEarnedReward?.Invoke();
    }

    public bool IsLoadVideo()
    {
        return true;
    }

    public void LoadRewardVideo() { }
}
#endif


public static partial class ADS
{
    public static bool IsAvaliableReward()
    {
        var inst = GameObject.FindObjectOfType<AdsRevardVideo>();
        if (inst == null)
            return false;
        return inst.IsLoadVideo();
    }

    public static void OpenReward(System.Action onReward, System.Action onClose)
    {
        var inst = GameObject.FindObjectOfType<AdsRevardVideo>();
        if (inst != null && inst.IsLoadVideo())
        {
            inst.OnEarnedReward += onReward;
            inst.OnForceCloseReward += onClose;
            inst.PlayVideo();
        }
        else
        {
            onClose?.Invoke();
        }
    }
}


