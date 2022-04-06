using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


/*
Оценка приложения в приложении
https://developer.android.com/guide/playcore/in-app-review/unity

    На русском
    https://developers.google.com/games/services/integration
*/

#if (FALSE)
using UnityEngine.SocialPlatforms;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using DG.Tweening;
using Google.Play.Review;

public static class GglAccount
{
    public static bool isLogin = false;

    public static void Init(System.Action onDone)
    {
/*
#if (UNITY_EDITOR)
        NetCore.inst.startInit = true;
        return;
#endif
*/
        try
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            //.EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running.
            //.WithInvitationDelegate(< callback method >)
            // registers a callback for turn based match notifications received while the
            // game is not running.
            //.WithMatchDelegate(< callback method >)
            // requests the email address of the player be available.
            // Will bring up a prompt for consent.
            //.RequestEmail()
            // requests a server auth code be generated so it can be passed to an
            //  associated back end server application and exchanged for an OAuth token.
            //.RequestServerAuthCode(false)
            // requests an ID token be generated.  This OAuth token can be used to
            //  identify the player to other services such as Firebase.
            .RequestIdToken()
            .Build();


            PlayGamesPlatform.InitializeInstance(config);


            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;

            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();


            /*
            PlayGamesPlatform.Instance.Authenticate((result) =>
            {
                helloMSG.text = "КОКОКОКВО: " + result.ToString();
                // handle results
            });
            */

            //PlayGamesPlatform.Instance.
            LogIn(onDone);

        }
        catch (System.Exception e)
        {
            onDone?.Invoke();
            //DOTween.Sequence().AppendInterval(0.01f).OnComplete(() => onDone?.Invoke());
        }
    }

    /*
    public static void RateUS()
    {
        var reviewManager = new ReviewManager();

        // start preloading the review prompt in the background
        var playReviewInfoAsyncOperation = reviewManager.RequestReviewFlow();

        // define a callback after the preloading is done
        playReviewInfoAsyncOperation.Completed += playReviewInfoAsync => {
            if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
            {
                // display the review prompt
                var playReviewInfo = playReviewInfoAsync.GetResult();
                reviewManager.LaunchReviewFlow(playReviewInfo);
            }
            else
            {
                Debug.LogError(playReviewInfoAsync.Error);
                // handle error when loading review prompt
            }
        };
    }
    */

    public static IEnumerator RateUS()
    {
        ReviewManager _reviewManager;
        _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        Debug.LogError("1");
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(requestFlowOperation.Error);
            yield break;
        }
        var _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        Debug.LogError("2");
        yield return launchFlowOperation;

        Debug.LogError("3");
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError(launchFlowOperation.Error);
            yield break;
        }
    }


    public static void LogOut()
    {
        if (isLogin)
        {
            PlayGamesPlatform.Instance.SignOut();
            Debug.Log("GglAccount> LogOut");
        }
    }
    

    public static void LogIn(System.Action onDone)
    {
        // authenticate user:
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
        {
            /*
            helloMSG.text = "pp: " + result.ToString()
            + " tkn:" + PlayGamesPlatform.Instance.GetIdToken()
            + " ID:" + PlayGamesPlatform.Instance.GetUserId();
            */
//#if !(UNITY_EDITOR)
            string kToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            Debug.Log("TOKEN: " + kToken);

            GS.GOOGLE_ID = PlayGamesPlatform.Instance.GetUserId();
            GS.GOOGLE_DISPLAY_NAME = PlayGamesPlatform.Instance.GetUserDisplayName();
            Debug.Log("GS.GOOGLE_ID: " + GS.GOOGLE_ID);
            //#endif

            isLogin = false;
            if (kToken != null && kToken.Length != 0)
            {
                isLogin = true;
            }

            // Просто гугл ID пользователя
            // ((PlayGamesLocalUser)Social.localUser).id;
            //onDone?.Invoke();
            //NetCore.inst.startInit = true;
            NetCore.inst.AppendTask(onDone);
            //DOTween.Sequence().AppendInterval(0.01f).AppendCallback(() => onDone?.Invoke());
        });
    }

}

#else
public static class GglAccount
{
    public static void Init(System.Action onDone) {onDone?.Invoke();}
    public static void LogOut(){}
    public static void LogIn(System.Action onDone) {onDone?.Invoke();}
}
#endif