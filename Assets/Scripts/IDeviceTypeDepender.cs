using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDeviceType
{
    UnityEditor = 0,
    Phone = 1,
    Tablet = 2
}

public interface IDeviceTypeDepender
{
    void Set(EDeviceType deviceType);
}
