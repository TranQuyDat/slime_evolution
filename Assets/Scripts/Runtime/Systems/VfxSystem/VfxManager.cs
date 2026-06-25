using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


class vfxManager : MonoBehaviour
{
    public static vfxManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        
    }

    public void PlayFX()
    {
    }

}