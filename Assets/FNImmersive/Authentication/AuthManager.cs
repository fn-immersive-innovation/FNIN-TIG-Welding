using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Events;
using System.Text;
using FNImmersiveInnovation;


[System.Serializable]
public class CompaniesObject
{
    public string success;
    public CompanyObject[] companies;
}

[System.Serializable]
public class CompanyObject
{
    public string CompanyName;
    public int CompanyID;
}

public class AuthManager : MonoBehaviour {

    [SerializeField] private bool m_isTest = true;

    [Header("Settings Inputs")]
    [SerializeField] private string m_BaseAPIPath = "";
    [SerializeField] private Notification m_Notification;

    [Header("Login Inputs")]
    [SerializeField] private TMP_InputField m_LoginEmail;
    [SerializeField] private TMP_InputField m_LoginPassword;

    [Header("Registration Inputs")]
    [SerializeField] private TMP_InputField m_RegFirstName;
    [SerializeField] private TMP_InputField m_RegLastName;
    [SerializeField] private TMP_InputField m_RegEmail;
    [SerializeField] private TMP_InputField m_RegPassword;
    [SerializeField] private TMP_InputField m_RegConfirmPassword;

    [Header("Events")]
    public UnityEvent onAuthenticated;
    public UnityEvent onDeAuthenticated;

    private string _loginTokenDetails = "_fnii_";
    private string m_UserToken = "";

    public string getUserToken
    {
        get { return m_UserToken; }
    }
    public string getAPIURL
    {
        get { return m_BaseAPIPath; }
    }

    public Notification getNotificationManager
    {
        get { return m_Notification; }
    }

    public class LoginObject
    {
        public string email;
        public string password;
        public int companyId;
    }

    public class LoginResponse
    {
        public string error;
        public string success;
        public string token;
    }

    public void StartAuth()
    {
        if (PlayerPrefs.HasKey(_loginTokenDetails) && !m_isTest)
        {
            m_UserToken = PlayerPrefs.GetString(_loginTokenDetails);
            onAuthenticated.Invoke();
        }
        else
        {
            //GetRegisteredCompanies();
            onDeAuthenticated.Invoke();
        }
    }

    private void OnEnable()
    {
        

    }

    public void GetRegisteredCompanies()
    {
        StartCoroutine(GetRequest("/v1/companies/all", (string response) => {

            CompaniesObject res = JsonUtility.FromJson<CompaniesObject>(response);

            if (res.companies != null)
            {
                // Iterate through each company and print details
                foreach (var company in res.companies)
                {
                    
                }
            }
            else
            {
                Debug.LogWarning("No companies found in the response.");
            }

        }));
    }

    public void Login()
    {
        // Create a form object to send data
        LoginObject form = new LoginObject();

        //Add fields
        form.email = m_LoginEmail.text;
        form.password = m_LoginPassword.text;
        form.companyId = 13;

        StartCoroutine(PostRequest("/v1/users/login", form, LoginPostCompleted));
    }

    private void LoginPostCompleted(string response)
    {
        // Process the response or perform other actions here
        Debug.Log("Received response: " + response);

        LoginResponse res = JsonUtility.FromJson<LoginResponse>(response);

        //arowosegbe.hammed.olawale @gmail.com

        if (!string.IsNullOrEmpty(res.error))
        {
            m_Notification.ShowNotification(res.error, false);
            return;
        }

        m_Notification.ShowNotification(res.success);

        PlayerPrefs.SetString(_loginTokenDetails, res.token);

        onAuthenticated.Invoke();
    }

    public class RegistrationObject
    {
        public string email;
        public string password;
        public string fname;
        public string lname;
    }

    public void Register()
    {
        //Validate password
        if(m_RegPassword.text != m_RegConfirmPassword.text)
        {
            Debug.Log("Incorrect password");
            m_Notification.ShowNotification("password and confirm password not identical", false);
            return;
        }

        // Create a form object to send data
        RegistrationObject form = new RegistrationObject();

        //Add fields
        form.email = m_RegEmail.text;
        form.password = m_RegPassword.text;
        form.fname = m_RegFirstName.text;
        form.lname = m_RegLastName.text;

        StartCoroutine(PostRequest("/v1/users/register", form, RegistrationPostCompleted));
    }

    private void RegistrationPostCompleted(string response)
    {
        // Process the response or perform other actions here
        Debug.Log("Received response: " + response);

        LoginResponse res = JsonUtility.FromJson<LoginResponse>(response);

        //arowosegbe.hammed.olawale @gmail.com

        if (!string.IsNullOrEmpty(res.error))
        {
            m_Notification.ShowNotification(res.error, false);
            return;
        }

        m_Notification.ShowNotification(res.success);

        onAuthenticated.Invoke();
    }

    IEnumerator ValidateWebRequest(UnityWebRequest webRequest, UnityAction<string> callback)
    {
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", getUserToken);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(webRequest.error);

            callback.Invoke(webRequest.downloadHandler.text);
        }
        else
        {
            string json = webRequest.downloadHandler.text;

            callback.Invoke(json);

        }
    }
    IEnumerator PostRequest<T>(string path, T data, UnityAction<string> callback)
    {
        string jsonPayload = JsonUtility.ToJson(data);

        // Create a UnityWebRequest object
        UnityWebRequest www = UnityWebRequest.PostWwwForm(m_BaseAPIPath + path, "");

        www.SetRequestHeader("Content-Type", "application/json");

        // Attach the JSON payload to the request body
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.uploadHandler.contentType = "application/json";


        // Send the request
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Request successful, retrieve the response
            Debug.Log("POST request successful!");
            callback.Invoke(www.downloadHandler.text);
        }
        else
        {
            // Request failed
            Debug.Log("POST request error: " + www.error);
            callback.Invoke(www.downloadHandler.text);
        }
    }

    IEnumerator GetRequest(string url, UnityAction<string> callback)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(m_BaseAPIPath + url);

        yield return ValidateWebRequest(webRequest, callback);
    }


}
