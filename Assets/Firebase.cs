using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using System.Net.Http;

public class Firebase : MonoBehaviour
{
    public Text twitchToken;
    // Start is called before the first frame update
    void Start()
    {
        Application.OpenURL("https://us-central1-valerian-games-dev.cloudfunctions.net/oAuthRedirectUnity");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void enterTwitchToken()
    {
        
        string url = "https://us-central1-valerian-games-dev.cloudfunctions.net/tokenUnity?code=";

        url += twitchToken.text;
        Debug.Log("posting : "+url);
        await uploadAsync(url);
        //StartCoroutine(Upload(url));
    }

    /* IEnumerator Upload(string url)
     {
         Debug.Log("working");
         List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
         formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
         formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

         UnityWebRequest www = UnityWebRequest.Post(url,formData);
         yield return www.SendWebRequest();

         if (www.isNetworkError || www.isHttpError)
         {
             Debug.Log(www.error);
         }
         else
         {
             Debug.Log("form data : "+formData);
         }
     }*/
    private static readonly HttpClient client = new HttpClient();
    async System.Threading.Tasks.Task uploadAsync(string url)
    {
        Debug.Log("working");
        var response = await client.PostAsync(url,"");
        var responseString = await response.Content.ReadAsStringAsync();
        Debug.Log(responseString);
    }

}
