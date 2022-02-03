using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface AvailabilityNotificator
{
    public UnityEvent<bool> AvailabilityEvent { get; }
}