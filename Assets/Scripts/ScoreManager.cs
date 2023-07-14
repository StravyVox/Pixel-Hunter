using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<int, int> _scores;

    private void Start()
    {
        _scores = new Dictionary<int, int>();
    }
    public int GetScore(int id)
    {
        return _scores[id];
    }
    public List<int> GetPlayers()
    {
        return _scores.Keys.ToList<int>();
    }
    public void AddScore(int id)
    {
        if(_scores.ContainsKey(id))
        _scores[id] += 1;
    }
    public void AddObject(int id)
    {
        if (!_scores.ContainsKey(id))
        {
            _scores.Add(id, 0);
        }
    }
    public void AddObjects(List<ulong> ids)
    {
        foreach(ulong id in ids)
        {
            this.AddObject((int)id);
        }
    }
}
