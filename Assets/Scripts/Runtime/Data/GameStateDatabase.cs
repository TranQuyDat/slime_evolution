using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="GameStateDatabase",menuName ="Data/GameStateDatabase")]
class GameStateDatabase:ScriptableObject
{
    [SerializeField]private GameObject[] _uis;
    public GameObject[] Uis => _uis;

    void Reset()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/UI");
        _uis = new GameObject[prefabs.Length];
        foreach(GameObject go in prefabs)
        {
            if(Enum.TryParse(go.name,true,out StateType type))
            {
                _uis[(int)type] = go;
            }
        }

    }
}
