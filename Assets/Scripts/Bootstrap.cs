using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public int defaultSceneIndex = 0;

    private void Start()
    {
        SceneManager.LoadScene(defaultSceneIndex);
    }
}
