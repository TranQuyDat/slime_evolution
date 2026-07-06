using System.Collections.Generic;
using UnityEngine;

class SlimeSpawnManager : MonoBehaviour
{
    [SerializeField] private SlimeDatabase _slimeDatabase; 
    private ObjectPoolSystem _objectPoolSys;
    private IMutableSpawnSource<int> _bag;
    public SlimeDatabase SlimeDatabase => _slimeDatabase;
    private Camera _camera;
    private int _currentDeckIndex;

    private void Start()
    {
        _camera = Camera.main;
        _objectPoolSys = ObjectPoolSystem.Instance;
        _bag = new ShuffleBagSpawnSource<int>(DeckEasy());
    }
#region Decks
    private int[] DeckEasy()
    => new [] 
    {
        0,0,0,0,0,0,0,
        1,1,1,1,
        2,2
    };
    private int[] DeckNomal()
    => new[]
    {
        0,0,0,0,0,0,
        1,1,1,1,1,
        2,2,2,
        3,
    };
    private int[] DeckHard()
    => new[]
    {
        0,0,0,0,0,
        1,1,1,1,1,
        2,2,2,
        3,3,
        4,
    };
#endregion

    public Slime Spawn()
    {
        Slime slimePrefab =  _slimeDatabase.SlimePrefab;
        GameObject obj = _objectPoolSys.Order(slimePrefab.gameObject,slimePrefab.PoolKey); 
        obj.transform.position = _camera.ViewportToWorldPoint(new Vector2(0.5f,0.8f));
        obj.transform.rotation = Quaternion.identity;

        Slime newSlime = obj.GetComponent<Slime>();
        int id = _bag.GetNext();
        SlimeData data = _slimeDatabase.SlimeDatas[id];
        newSlime.Init(data);
        newSlime.Freeze();
        return newSlime;
    }

    public void SwapDeck(int highestSlimeLevel)
    {
        int deckIndex = Mathf.Clamp(highestSlimeLevel / 4, 0, 2);

        if (deckIndex == _currentDeckIndex)
            return;

        _currentDeckIndex = deckIndex;

        int[] deck = deckIndex switch
        {
            0 => DeckEasy(),
            1 => DeckNomal(),
            _ => DeckHard()
        };
        _bag.SetItems(deck);
        print($"Swap to Deck : {deckIndex}");
    }

    public SlimeData PreviewNextSlime()
    {
        int id = _bag.PeekNext();
        SlimeData data = _slimeDatabase.SlimeDatas[id];
        return data;
    }
    
    public void Reset()
    {
        _bag.SetItems(DeckEasy());
        _bag.Reset();
    }

}