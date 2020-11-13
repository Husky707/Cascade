using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CascadeEventData
{
    public CascadeEventData(int count = 0, int value = 0, int score = 0)
    {
        _count = count;
        _value = value;
        _score = score;
    }

    public int Count => _count;
    private int _count = 0;
    public int Value => _value;
    private int _value = 0;
    public int Score => _score;
    private int _score = 0;

    public eCascadeIntensity Intensity => _intensity;
    private eCascadeIntensity _intensity = eCascadeIntensity.Noone;

    public void IncreaseCascade(EffectedTileData data)
    {
        _count += data.cascadeCountIncreased;
        _value += data.cascadeValueAdded;
    }

    public void SetScore(int score)
    {
        _score = Score;
    }

    public void SetIntinsity(eCascadeIntensity intensity)
    {
        _intensity = intensity;
    }

}
