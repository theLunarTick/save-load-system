using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserNameText : MonoBehaviour, IDataPersistence
{

    public TMP_InputField UserNameInputField;

    private string userName = "Todd";

    private TextMeshProUGUI userNameText;

    private GameData Ddata = new GameData();



    private void Awake() 
    {
        userNameText = this.GetComponent<TextMeshProUGUI>();
    }

    public void LoadData(GameData data) 
    {
        this.userName = data.userName;
    }

    public void SaveData(GameData data) 
    {
        data.userName = this.userName;
    }

    private void Start() 
    {
        // subscribe to events
        GameEventsManager.instance.onButtonClicked += UpdateForm;
    }

    private void OnDestroy() 
    {
        // unsubscribe from events
        GameEventsManager.instance.onButtonClicked -= UpdateForm;
    }

    private void UpdateForm() 
    {
        Ddata.userName = this.userName;
        Debug.Log("It worked");
    }

    private void Update() 
    {
        userNameText.text = "" + userName;
    }
}
