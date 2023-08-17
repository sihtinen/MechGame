using System.Collections;
using System.Collections.Generic;
using Tensori.SaveSystem;
using UnityEngine;

[System.Serializable]
public abstract class SavePreProcessor : ScriptableObject
{
    public abstract void PreProcess(SaveData saveData);
}
