using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Vuforia;


public class root : MonoBehaviour, ITrackableEventHandler
{

  private TrackableBehaviour mTrackableBehaviour;
  public TextMesh text_mesh;
  public GameObject heart;
  public Transform myModelPrefab;
  // Use this for initialization
  void Start ()
  {
    mTrackableBehaviour = GetComponent<TrackableBehaviour>();
    if (mTrackableBehaviour) {
      mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }
  }
  // Update is called once per frame
  void Update ()
  {
  }
  public void OnTrackableStateChanged(
    TrackableBehaviour.Status previousStatus,
    TrackableBehaviour.Status newStatus)
  { 
    if (newStatus == TrackableBehaviour.Status.DETECTED ||
        newStatus == TrackableBehaviour.Status.TRACKED ||
        newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
    {
      OnTrackingFound();
    }
  }
  Boolean first = true; 
  private void OnTrackingFound()
  {
    if(first){
      first = false;
      CreateQuote("iPhone 6S 64GB LTE");
    }
    if (myModelPrefab != null)
    {

      Transform myModelTrf = GameObject.Instantiate(myModelPrefab) as Transform;
      myModelTrf.parent = mTrackableBehaviour.transform;
      myModelTrf.localPosition = new Vector3(0f, 0f, 0f);
      myModelTrf.localRotation = Quaternion.identity;
      myModelTrf.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);
      myModelTrf.gameObject.active = true;
      //text_mesh.text="HELLOOO";
      //heart.GetComponent<Renderer>().material()
    }
  }
  public void resetGame(){
    text_mesh.text="Score";
    first= true;
  }
  /* Create a Root Object to store the returned json data in */
  [System.Serializable]
  public class Quotes
  {
    public Quote[] values;
  }

  [System.Serializable]
  public class Quote
  {
    public string package_name;
    public string sum_assured;
    public int base_premium;
    public string suggested_premium;
    public string created_at;
    public string quote_package_id;
    public QuoteModule module;
  }

  [System.Serializable]
  public class QuoteModule
  {
    public string type;
    public string make;
    public string model;
  }

  [Serializable]
  public class Param
  {
    public Param (string _key, string _value) {
      key = _key;
      value = _value;
    }
    public string key;
    public string value;
  }

    public string api_key = "sandbox_YzAzMmE5ODItNzM0Yi00YzY3LTlhMWMtNTY3ZWIxZTExNjkzLnlyRGgybFFZVlpvbkNlbFBHcmpvU1RYNHJJRDMzclZQ";

  // private void Start()
  // {
  //   // CreateQuote("iPhone 6S 64GB LTE");
  // }
  public void createPolicy(){
 List<Param> parameters = new List<Param>();
    //String[][] idList = new String[3][3]();
    String id = "\"id\":";
    String hey = "{\"type\": \"id\",\"number\": \"6801015800084\",\"country\": \"ZA\"}";
    parameters.Add(new Param(id,hey));

    StartCoroutine(CallAPICoroutine("https://sandbox.root.co.za/v1/insurance/quotes", parameters));

  }
  public void CreateQuote(string gadget) {
    List<Param> parameters = new List<Param>();
    parameters.Add(new Param("type", "root_gadgets"));
    parameters.Add(new Param("model_name", gadget));

    StartCoroutine(CallAPICoroutine("https://sandbox.root.co.za/v1/insurance/quotes", parameters));
  }

  IEnumerator CallAPICoroutine(String url, List<Param> parameters)
  {

    string auth = api_key + ":";
    auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
    auth = "Basic " + auth;

    WWWForm form = new WWWForm();

    foreach (var param in parameters) {
      form.AddField(param.key, param.value);
    }

    UnityWebRequest www = UnityWebRequest.Post(url, form);
    www.SetRequestHeader("AUTHORIZATION", auth);
    yield return www.Send();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.downloadHandler.text);
    }
    else
    {
      Quotes json = JsonUtility.FromJson<Quotes>("{\"values\":" + www.downloadHandler.text + "}");
      String text = "Make: " + json.values[0].module.make + "\nPremium: R" + (json.values[0].base_premium / 100);
      String packageId = json.values[0].quote_package_id;

      
      Debug.Log("Form upload complete!");
      Debug.Log(text);
      text_mesh.text = text;
    }
    yield return true;
  }
}
