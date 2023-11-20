using Assets.Scripts.Wears;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public static Prefabs prefabs;

    public List<Wear> torso;
    public List<Wear> legs;
    public List<Wear> boots;
    public List<Wear> hats;
    public List<Wear> wrists;
    public List<Wear> glasses;
    public List<Wear> masks;
    public List<Wear> scarf;

    private void Awake()
    {
        if (prefabs != null)
            Destroy(prefabs);
        else
            prefabs = this;

        DontDestroyOnLoad(this);
    }
}
