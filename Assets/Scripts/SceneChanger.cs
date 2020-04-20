using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int targetSceneIndex = 0;

    public void ChangeScene()
    {
        FindObjectOfType<TransitionController>().TransitionOut(() => SceneManager.LoadScene(targetSceneIndex));
    }
}
