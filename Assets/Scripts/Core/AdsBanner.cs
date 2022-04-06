using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (GGL_BANNER)
using GoogleMobileAds.Api;

public class AdsBanner : MonoBehaviour
{
    BannerView bannerView;

    public void Start()
    {
#if (ZX_RELEASE)
        RequestBanner();
#endif
    }

    private void RequestBanner()
    {
#if (UNITY_ANDROID)
        string adUnitId = "ca-app-pub-2640647163440835/5096694305";
#elif (UNITY_IOS)
        string adUnitId = "ca-app-pub-2640647163440835/6620654605";
#endif
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);
        
        AdRequest adRequest = new AdRequest.Builder().Build();
        bannerView.LoadAd(adRequest);

        bannerView.OnAdLoaded += HandleAdLoaded;

        StartCoroutine(ReloadBanner());
    }

    public void HandleAdLoaded(object sender, System.EventArgs args)
    {
        bannerView.Show();
    }

    IEnumerator ReloadBanner()
    {
        yield return new WaitForSeconds(60);

        RequestBanner();
    }

    private void OnDestroy()
    {
        bannerView.Hide();
        bannerView?.Destroy();
    }
}
#elif (IRON_BANNER)


public class AdsBanner : MonoBehaviour
{
    public void Start ()
    {
#if (ZX_RELEASE)
        RequestBanner ();
#endif
    }

    private void RequestBanner ()
    {
        Debug.Log ("IRON_BANNER> Init");
        IronSource.Agent.loadBanner (IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);

        IronSourceEvents.onBannerAdLoadedEvent += () => {
            Debug.Log ("IRON_BANNER> Load done");
            IronSource.Agent.displayBanner ();
        };
        
        IronSourceEvents.onBannerAdLoadFailedEvent += x=> {
            Debug.LogError (x.getDescription ());
        };
        /*
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
        */
        //IronSource.Agent.displayBanner ();

        /*
        if (bannerView != null)
        {
            bannerView.Destroy ();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth (AdSize.FullWidth);

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView (adUnitId, adaptiveSize, AdPosition.Bottom);

        AdRequest adRequest = new AdRequest.Builder ().Build ();
        bannerView.LoadAd (adRequest);

        bannerView.OnAdLoaded += HandleAdLoaded;
        */
        StartCoroutine (ReloadBanner ());
    }

    public void HandleAdLoaded (object sender, System.EventArgs args)
    {
        //bannerView.Show ();
    }

    IEnumerator ReloadBanner ()
    {
        yield return new WaitForSeconds (60);

        //RequestBanner ();
    }

    private void OnDestroy ()
    {
    }
}

#else
public class AdsBanner : MonoBehaviour
{
    public void HandleAdLoaded(object sender, System.EventArgs args){}
}
#endif