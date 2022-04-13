using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid() 
    {
        id = System.Guid.NewGuid().ToString();
    }

    private SpriteRenderer visual;
    private ParticleSystem collectParticle;
    private bool collected = false;

    private void Awake() 
    {
        visual = this.GetComponentInChildren<SpriteRenderer>();
        collectParticle = this.GetComponentInChildren<ParticleSystem>();
        collectParticle.Stop();
    }


    //
    //
    // LOAD SAVED DATA

    public void LoadData(GameData data) 
    {
        data.coinsCollected.TryGetValue(id, out collected);
        if (collected) 
        {
            visual.gameObject.SetActive(false);
        }
    }


    //
    //
    // SAVE UPDATED DATA

    public void SaveData(GameData data) 
    {
        if (data.coinsCollected.ContainsKey(id))
        {
            data.coinsCollected.Remove(id);
        }
        data.coinsCollected.Add(id, collected);
    }


    //
    //
    // COIN COLLECTED

    private void OnTriggerEnter2D() 
    {
        if (!collected) 
        {
            collectParticle.Play();
            CollectCoin();
        }
    }


    //
    //
    // COIN REMOVED FROM SCENE

    private void CollectCoin() 
    {
        collected = true;
        visual.gameObject.SetActive(false);
        GameEventsManager.instance.CoinCollected();
    }

}
