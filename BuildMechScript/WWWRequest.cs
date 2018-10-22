using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WWWRequest : MonoBehaviour
{

    public GameObject text;
    public InputField userNameInput;
    public InputField passwordINput;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateText()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:1337/asd"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                text.GetComponent<Text>().text = www.downloadHandler.text;
                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void SignUp()
    {
        StartCoroutine(SignUpEnum());
    }

    IEnumerator SignUpEnum()
    {
        WWWForm form = new WWWForm();
        string userName = userNameInput.text;
        string password = passwordINput.text;
        form.AddField("userName", userName);
        form.AddField("password", password);
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:1337/signup", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                text.GetComponent<Text>().text = www.downloadHandler.text;
            }
        }
    }
}
