using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    //notify
    //public abstract void OnNotify(string value);
}

public abstract class Subject : MonoBehaviour
{
    /*
    //list semua observer
    private List<Observer> _observers = new List<Observer>();

    //register semua observer
    public void RegisterObserver(Observer observer)
    {
        _observers.Add(observer);
    }

    //notify
    public void Notify(string value)
    {
        foreach(var observer in _observers)
        {
            observer.OnNotify(value);
        }
    }
    */
}
