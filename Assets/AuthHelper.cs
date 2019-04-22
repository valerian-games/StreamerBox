using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthHelper : MonoBehaviour {

    private readonly string baseTokenUrl = "https://us-central1-valerian-games-dev.cloudfunctions.net/tokenUnity?code=";
    private readonly string baseRedirectUrl = "https://us-central1-valerian-games-dev.cloudfunctions.net/oAuthRedirectUnity";

    public Text twitchToken;

    Firebase.Auth.FirebaseAuth mAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;

    void Start () {
        Application.OpenURL (baseRedirectUrl);
    }

    void Update () {

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

            var json = JsonUtility.FromJson<Token>(result);

            Debug.Log(json.authToken);

            mAuth.SignInWithCustomTokenAsync(json.authToken).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SignInWithCustomTokenAsync was canceled.");
                    return;
                }

                if (task.IsFaulted) {
                    Debug.LogError("SignInWithCustomTokenAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;

                Debug.LogFormat (
                    "User signed in successfully: {0} ({1})",
                    newUser.DisplayName, 
                    newUser.UserId
                );
            });

        } else {
            Debug.Log(www.error);
        }
    }

    private class Token {
        public string authToken { get; set; }
    }
}