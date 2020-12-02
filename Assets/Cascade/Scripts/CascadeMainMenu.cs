using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;


public class CascadeMainMenu : MonoBehaviour
{
    [SerializeField] Button playButton = null;
    [SerializeField] eRoomType _defaultPlay = eRoomType.Clasic1v1;
    PlayerController Player = null;

    private void OnEnable()
    {
        OnPlayerUpdate(ClientScene.localPlayer);
        PlayerAnouncer.PlayerUpdated += OnPlayerUpdate;
        playButton.onClick.AddListener(PlayDefault);
    }

    private void OnDisable()
    {
        PlayerAnouncer.PlayerUpdated -= OnPlayerUpdate;
        playButton.onClick.RemoveListener(PlayDefault);
    }

    private void OnPlayerUpdate(NetworkIdentity identity)
    {
        if (identity == null)
            return;

        Player = identity.gameObject.GetComponent<PlayerController>();
    }

    #region Commands
    public void PlayDefault()
    {

        Player?.RequestPlay(_defaultPlay);
    }



    #endregion
}
