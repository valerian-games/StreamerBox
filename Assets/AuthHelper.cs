using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthHelper : MonoBehaviour {

    private readonly string baseTokenUrl = "https://us-central1-valerian-games-dev.cloudfunctions.net/tokenUnity?code=";
    private readonly string baseRedirectUrl = "https://us-central1-valerian-games-dev.cloudfunctions.net/oAuthRedirectUnity";

    // mainmenu variables
    private string twitchToken;
    private string email;
    private string password;
    private string newPassword;
    public Text info;
    MainmenuManager menuMananger;
    Firebase.Auth.FirebaseAuth mAuth;

    void Start () {
        DontDestroyOnLoad(this.gameObject);
        mAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        menuMananger = GameObject.Find("Scripts").GetComponent<MainmenuManager>();
        checkCredential();
    }

    void checkCredential()
    {
        string email;
        string password;
        email = PlayerPrefs.GetString("Email", "");
        password = PlayerPrefs.GetString("Password", "");
        if (!(email.Equals("") || password.Equals("")))
        {
            trySignIn(email,password);
            menuMananger.setArea(3);
        }
    }
    
    public void signInWithEmail()
    {
        if (email.Equals("") || password.Equals(""))
        {
            info.text = "Email or password empty!";
            return;
        }

        // buraya email kaydetme ekle sonradan

        trySignIn(email, password);

        menuMananger.setArea(3);        

    }

    public void trySignIn(string email,string password)
    {
       
        mAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    
    public void openTwitchSignIn()
    {
        menuMananger.setArea(1);
        Application.OpenURL(baseRedirectUrl);
    }
    
    public void enterTwitchToken()
    {
        string URL = baseTokenUrl + twitchToken;
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

    public void openTutorialScene()
    {
        SceneManager.LoadScene(1);
    }
    
    public void setTwitchToken(string twitchToken)
    {
        this.twitchToken = twitchToken;
    }

    public void setEmail(string email)
    {
        this.email = email;
    }

    public void setPassword(string password)
    {
        this.password = password;
    }

    private class Token {
        public string authToken;

        public static Token getTokenFromJson(string jsonStr)
        {
            return JsonUtility.FromJson<Token>(jsonStr);
        }
    }

}