using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LobbyPrefabs", menuName = "PrefabManagers/Lobby Prefabs")]
public class LobbyPrefabManager : ScriptableObject
{
    //Lobby prefabs must have a room type object
    [SerializeField] List<GameObject> lobbyPrefabs;

    private Dictionary<eLobbyType, GameObject> _lobbyPrefabs;

    public GameObject GetPrefab(eLobbyType target)
    {
        if (target == eLobbyType.Void || !_lobbyPrefabs.ContainsKey(target))
            return _lobbyPrefabs[eLobbyType.Default];

        return _lobbyPrefabs[target];
    }



    private void OnValidate()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        _lobbyPrefabs = new Dictionary<eLobbyType, GameObject>(lobbyPrefabs.Count);

        foreach (GameObject obj in lobbyPrefabs)
        {
            Lobby lobby = obj.GetComponent<Lobby>();
            if(lobby == null)
            {
                Debug.Log("The lobby prfab " + obj.name + " needs to have a Lobby Script");
                continue;
            }

            _lobbyPrefabs.Add(lobby.LobbyType, obj);
        }
    }

}
