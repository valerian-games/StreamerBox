using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthHelper : MonoBehaviour {

    private readonly string baseTokenUrl = "https://us-central1-valerian-games-dev.cloudfunctions.net/tokenUnity?code=";
    private readonly string baseRedirectUrl = "https://us-central1-valerian-games-dev.cloudfunctions.net/oAuthRedirectUnity";

    public Text twitchToken;

    Firebase.Auth.FirebaseAuth mAuth;

    void Start () {
        mAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }
    
    public void checkSignIn()
    {
        string authToken = PlayerPrefs.GetString("authToken", "");
        MainmenuManager menuMananger = GameObject.Find("Scripts").GetComponent<MainmenuManager>();
        if (authToken.Equals(""))
        {
            Debug.Log("null");
            menuMananger.setArea(1);
            Application.OpenURL(baseRedirectUrl);
        }
        else
        {
            Debug.Log("notnull");
            menuMananger.setArea(2);
            signIn(authToken);
        }
            
    }

    public void enterTwitchToken() {
        string url = baseTokenUrl + twitchToken.text;
        Debug.Log("loading");

        getToken(url);
    }

    void getToken(string URL) {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        var packetToSend    = JsonUtility.ToJson(string.Empty);
        byte[] postData     = System.Text.Encoding.UTF8.GetBytes(packetToSend);
        WWW www             = new WWW(URL, postData, headers);

        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www) {
        yield return www;

        if (www.error == null) {
            var result = www.text;

            Token json = Token.getTokenFromJson(result);

            signIn(json.authToken);
            
        } else {
            Debug.Log(www.error);
        }
    }

    void signIn(string authToken)
    {
        mAuth.SignInWithCustomTokenAsync(authToken).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCustomTokenAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCustomTokenAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;

            Debug.LogFormat(
                "User signed in successfully: {0} ({1})",
                newUser.DisplayName,
                newUser.UserId
            );

            PlayerPrefs.SetString("authToken", authToken);
        });
    }
    
    private class Token {
        public string authToken;

        public static Token getTokenFromJson(string jsonStr)
        {
            return JsonUtility.FromJson<Token>(jsonStr);
        }
    }

}