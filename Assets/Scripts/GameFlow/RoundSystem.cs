using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;


public class RoundSystem : MonoBehaviour
{
    [SerializeField] private float _timer = 5f;
    [SerializeField] private float _timerMax = 5f;

    public event EventHandler OnNextRound;

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0) 
        {
            _timer = _timerMax;
            NextRound();
        }
    }

    private void NextRound()
    {
        //print("Next round");
        if (OnNextRound != null) OnNextRound(this, EventArgs.Empty);
    }
}
