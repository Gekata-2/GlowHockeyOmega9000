using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class LevelStateHandler : MonoBehaviour
{
    public Action onLevelComplete;
    private readonly List<TriggerZone> _triggerZones = new();

    private void Start()
    {
        var zones = GameObject.FindGameObjectsWithTag("TriggerZone");
        foreach (var zone in zones)
            if (zone.TryGetComponent(out TriggerZone triggerZone))
                _triggerZones.Add(triggerZone);
        foreach (var triggerZone in _triggerZones)
        {
            triggerZone.onZoneComplete += OnZoneComplete;
        }
    }

    private void OnZoneComplete(object sender, TriggerZone.ZoneCompleteEventArgs e)
    {
        if (e.IsLast)
        {
            Debug.Log("Level Complete!");
            onLevelComplete?.Invoke();
            Time.timeScale = 0f;
        }
    }
}