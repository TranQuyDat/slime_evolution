using UnityEngine;
using System;

[CreateAssetMenu(fileName ="SlimeDatabase",menuName ="Data/SlimeDatabase")]
class SlimeDatabase : ScriptableObject
{
    public enum SlimeType
    {
        Rat = 0, Cat = 1, Rooster = 2, Dog = 3, Monkey = 4, Snake = 5, 
        Goat = 6, Pig = 7, Horse = 8, Tiger = 9, Buffalo = 10, Dragon = 11
    }
    [SerializeField]private SlimeData[] _slimeDatas ;
    [SerializeField]private Slime _slimePrefab;
    
    public SlimeData[] SlimeDatas =>_slimeDatas;
    public Slime SlimePrefab => _slimePrefab;
    void Reset()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("SlimeSprites/Slime12");
        float[] scales = { 0.4f, 0.6f, 0.8f, 1.0f, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f, 2.2f, 2.5f, 3.0f };

        _slimeDatas = new SlimeData[12];
        foreach(Sprite s in allSprites)
        {
            if(Enum.TryParse(s.name,true,out SlimeType slimeType))
            {
                int index = (int) slimeType;

                SlimeData data = new SlimeData(index,s,scales[index]);
                _slimeDatas[index] = data;
            }
        }


    }
}