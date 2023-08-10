using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public ClientData clientData;
    public ClientDetails clientDetails;

    [SerializeField] private TextMeshProUGUI labelName;
    [SerializeField] private TextMeshProUGUI points;

    private void OnEnable()
    {
        if(clientData != null)
        labelName.text = clientData.label;
        else
            labelName.text = "";

        if (clientDetails != null)
            points.text = clientDetails.points.ToString();
        else 
            points.text = "";
    }

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { if(clientDetails != null) Task_1_Manager.Instance.ShowClientPopup(clientDetails); });
    }
}
