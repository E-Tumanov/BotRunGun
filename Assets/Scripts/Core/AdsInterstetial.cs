using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if (IRON_PAGE)

public class AdsInterstetial : MonoBehaviour
{
    public event System.Action OnEarnedReward = delegate { };

    public static AdsInterstetial inst;
    
    bool isLoaded = false;

    private void Awake()
    {
        inst = this;       
    }

    void Start()
    {
        //IronSource.Agent.init("db616649", IronSourceAdUnits.INTERSTITIAL);

        IronSourceEvents.onInterstitialAdReadyEvent += AdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += AdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += AdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += AdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += AdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += AdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += AdClosedEvent;

        //LoadVideo();
    }

    private void AdClosedEvent()
    {
        Debug.Log("IRON_INTERSTITIAL> InterstitialAdClosedEvent... OK");
        OnEarnedReward?.Invoke();
    }

    private void AdOpenedEvent()
    {
    }

    private void AdClickedEvent()
    {
    }

    private void AdShowFailedEvent(IronSourceError obj)
    {
        OnEarnedReward?.Invoke();
    }

    private void AdShowSucceededEvent()
    {
        Debug.Log("IRON_INTERSTITIAL> InterstitialAdShowSucceededEvent... OK");
        
        OnEarnedReward?.Invoke();
        IronSource.Agent.loadInterstitial ();
    }

    private void AdLoadFailedEvent(IronSourceError obj)
    {
        Debug.Log ($"IRON_INTERSTITIAL> Error load. {obj.getErrorCode()} {obj.getDescription()}");
    }

    private void AdReadyEvent()
    {
        isLoaded = true;
    }


    public bool IsLoadVideo()
    {
        if (!isLoaded) // по требованию
        {
            Debug.LogWarning("IRON_INTERSTITIAL> NOT LOADED");
            IronSource.Agent.loadInterstitial ();
            return false;
        }
        return isLoaded;
    }

    public void PlayVideo()
    {
        if (!isLoaded)
        {
            Debug.LogWarning("IRON_ADS> run play but not loaded");
            return;
        }

        IronSource.Agent.showInterstitial();
    }
    /*
    public void LoadVideo()
    {
        isLoaded = false;
        IronSource.Agent.loadInterstitial();
    }*/

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}

#elif (GGL_PAGE)
using GoogleMobileAds.Api;

public class AdsInterstetial : MonoBehaviour
{
    private InterstitialAd interstitial;
    public event System.Action OnEarnedReward = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine (LoadVideo ());
    }

    /*
    public bool IsLoadVideo()
    {
        if (interstitial == null)
            return false;

        return interstitial.IsLoaded();
    }
    */

    public void PlayVideo()
    {
        if (interstitial != null && interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            OnEarnedReward ();
            OnEarnedReward = delegate { };

            //  корутина не закрылась
            if (isFreeForDownloadNew)
                StartCoroutine (LoadVideo());
        }
    }

    public IEnumerator LoadVideo ()
    {
        isFreeForDownloadNew = true;

#if (UNITY_ANDROID)
        string adUnitId = "ca-app-pub-2640647163440835/1708782899";
#elif (UNITY_IOS)
        string adUnitId = "ca-app-pub-2640647163440835/6401162759";
#endif      

        if (interstitial != null)
            interstitial.Destroy();

        this.interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();

        interstitial.OnAdClosed += Interstitial_OnAdClosed;
        interstitial.OnAdFailedToLoad += Interstitial_OnAdClosed;

        this.interstitial.LoadAd(request);

        yield return new WaitForSeconds (60);
        isFreeForDownloadNew = true;
    }


    bool isFreeForDownloadNew = false;

    private void Interstitial_OnAdClosed(object sender, System.EventArgs e)
    {
        isFreeForDownloadNew = true;
        OnEarnedReward ();
        OnEarnedReward = delegate { };
    }

    /*
    bool doneView = false;
    private void FixedUpdate()
    {
        // Это затем, что.... чет непонятно, просто не трогаю. Тут события "досмотрел" нету
        if (doneView)
        {
            OnEarnedReward();
            LoadVideo();
            doneView = false;
            OnEarnedReward = delegate { };
        }
    }*/
}
#else

public class AdsInterstetial : MonoBehaviour
{
    public event System.Action OnEarnedReward = delegate { };

    public static AdsInterstetial inst;

    private void Awake()
    {
        inst = this;
    }

    public bool IsLoadVideo()
    {
        return true;
    }

    public void PlayVideo()
    {
        OnEarnedReward?.Invoke();
    }

    public void LoadVideo()
    {
    }
}
#endif


public static partial class ADS
{
    public static void OpenInter(System.Action proc)
    {
        var inst = GameObject.FindObjectOfType<AdsInterstetial>();
        if (inst != null)
        {
            inst.OnEarnedReward += proc;
            inst.PlayVideo();
        }
        else
        {
            proc();
        }
    }
}