using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GETRequest02 : MonoBehaviour {



    public LocationServices location;
    string lat, lon;
    string apiKey = "AIzaSyCZ4vWIwiGKRIyJ4Ne1dCUeFE94QY4QSRA";
    public bool inPark;
    string testLat = "40.782779"; //Central Park //"40.726295":// Tompkins Square //"40.731043"; //Washington Square
    string testLon = "-73.965901"; // Central Park //"-73.981767";//Tompkins Square //"-73.997544";// Washington Square
    string url, UItext;
    public string place;
    public Text textDisplay;
    public GameObject plantMaker;
    public List<string> placesVisited = new List<string>();
    private string currentPlace;


    void Start()
    {

        lat = location.lat.ToString();
        lon = location.lon.ToString();
        //lat = testLat;
        //lon = testLon;
        plantMaker.SetActive(false);

        url = string.Concat("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=", lat, ",", lon, "&radius=50&type=park&key=" , apiKey);
        StartCoroutine(GetText());

    }

    IEnumerator GetText()
    {

        UnityWebRequest request = UnityWebRequest.Get(url);
        Debug.Log(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            // Show results as text
            Debug.Log(request.downloadHandler.text);
            var result = JSON.Parse(request.downloadHandler.text);
            Debug.Log(result);

            place = result["results"][0]["name"].Value;
            Debug.Log(place);

            //here we look for a game object by its name and
            // use the SendMAessage method to run a public method/function in it
            //more on Send Messsage
            // https://docs.unity3d.com/ScriptReference/Component.SendMessage.html

        }
    }


    void Update()
    {
        if(string.IsNullOrEmpty(place))
        {
            UItext = "Try visiting your local park!";
            plantMaker.SetActive(false);

        }
        else if (!string.IsNullOrEmpty(place))
        {
            plantMaker.SetActive(true);
            bool placeVisited = false;


            if (placesVisited.Count == 0)
            {
                placesVisited.Add(place);
            }


            else
            {
                foreach (string previousPlace in placesVisited) 
                {
                    if (previousPlace == place)
                    {
                        placeVisited = true;
                        break;
                    }
                }

            }

            if(placeVisited)
            {
                UItext = "Welcome to " + place + "! Try planting something.";
            } else
            {
                UItext = "Welcome to " + place + "! Try planting something.";

            }

        }

        textDisplay.text = UItext;

    }
}