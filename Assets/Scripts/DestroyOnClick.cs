﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
    public GameObject target;

    public void DestroyTarget()
    {
        Destroy(target);
    }
}
