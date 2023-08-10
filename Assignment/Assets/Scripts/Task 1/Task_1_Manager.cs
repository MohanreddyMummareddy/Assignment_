using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;
using System;

public class Task_1_Manager : MonoBehaviour
{
    public static Task_1_Manager Instance { get; private set; }

    public TMPro.TMP_Dropdown dropdown;

    public GameObject popupPanel;

    [SerializeField] private GameObject listItem;
    [SerializeField] private Transform scrollViewContent;


    [SerializeField] private JSONResponse response;

    private List<ClientData> clientsData;

    private const string jsonURL = "https://qa2.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data";

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI addressText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener( delegate { FilterClients(dropdown.value); } );

        StartCoroutine(FetchAndProcessJSON());
    }

    private IEnumerator FetchAndProcessJSON()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching JSON data: " + request.error);
            }
            else
            {
                string jsonText = request.downloadHandler.text;
                Debug.Log(jsonText);
                var jsonResponse = JsonConvert.DeserializeObject<JSONResponse>(jsonText);
                ProcessJSONResponse(jsonResponse);
            }
        }
    }

    private void ProcessJSONResponse(JSONResponse jsonResponse)
    {
        response = jsonResponse;
        foreach (ClientData client in jsonResponse.clients)
        {
            Debug.Log("Client Label: " + client.label);
            Debug.Log("Is Manager: " + client.isManager);
            Debug.Log("Client ID: " + client.id);

            if (jsonResponse.data.ContainsKey(client.id))
            {
                var clientDetails = jsonResponse.data[client.id];

                Debug.Log("Client Name: " + clientDetails.name);
                Debug.Log("Client Points: " + clientDetails.points);
                Debug.Log("Client Address: " + clientDetails.address);
            }

            var obj = GameObject.Instantiate(listItem,scrollViewContent);
            obj.GetComponent<ListItem>().clientData = client;
            obj.GetComponent<ListItem>().clientDetails =  jsonResponse.data.ContainsKey(client.id) ? jsonResponse.data[client.id] : null;
            obj.SetActive(true);
        }
    }

    public void ShowClientPopup(ClientDetails client)
    {
        nameText.text = string.Format("{0}\t:\t\t{1}", "Name", client.name);
        pointsText.text = string.Format("{0}\t:\t\t{1}", "Points", client.points);
        addressText.text = string.Format("{0}\t:\t\t{1}", "Address", client.address);

        // Animate popup appearance
        popupPanel.SetActive(true);
        popupPanel.transform.localScale = Vector3.zero;
        popupPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        Invoke("ClosePopup", 3f);
    }

    public void ClosePopup()
    {
        // Animate popup disappearance
        popupPanel.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => popupPanel.SetActive(false));
    }

    // All Clients, Managers only, Non managers
    public void FilterClients(int filterIndex)
    {
        // Update client list UI based on the selected filter

        foreach (var tmp in scrollViewContent.GetComponentsInChildren<ListItem>(true))
        {
            if(filterIndex == 0)
            {
                tmp.gameObject.SetActive(true);
            }
            else
            {
                if(filterIndex == 1)
                {
                    if (!tmp.clientData.isManager)
                    {
                        tmp.gameObject.SetActive(false);
                    }
                    else
                    {
                        tmp.gameObject.SetActive(true);
                    }
                }
                else if (filterIndex == 2)
                {
                    if (tmp.clientData.isManager)
                    {
                        tmp.gameObject.SetActive(false);
                    }
                    else
                    {
                        tmp.gameObject.SetActive(true);
                    }
                }
                }
            }
        }
    }

[System.Serializable]
public class ClientData
{
    public bool isManager;
    public int id;
    public string label;
}

[System.Serializable]
public class ClientDetails
{
    public string address;
    public string name;
    public int points;
}

[System.Serializable]
public class JSONResponse
{
    public List<ClientData> clients;
    public Dictionary<int,ClientDetails> data;
    public string label;
}
