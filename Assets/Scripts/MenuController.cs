using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Transform mainCamera;
    public Transform menuCameraLocation;
    public Transform helpCameraLocation;
    public float cameraMoveTime = 0.8f;
    public AnimationCurve moveCurve;

    public GameObject[] helpPanels;

    bool moving = false;

    int helpPanelIndex = 0;

    private void Awake()
    {
        UpdatePanels();
    }

    public void PrevPanel()
    {
        helpPanelIndex--;
        if (helpPanelIndex < 0) helpPanelIndex = helpPanels.Length - 1;

        UpdatePanels();
    }

    public void NextPanel()
    {
        helpPanelIndex++;
        if (helpPanelIndex > helpPanels.Length - 1) helpPanelIndex = 0;

        UpdatePanels();
    }

    void UpdatePanels()
    {
        for (int i = 0; i < helpPanels.Length; i++)
        {
            if (helpPanelIndex == i) helpPanels[i].SetActive(true);
            else helpPanels[i].SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int index)
    {
        FindObjectOfType<TransitionController>().TransitionOut(() => SceneManager.LoadScene(index));
    }

    public void HelpScreen()
    {
        if (!moving) StartCoroutine(MoveCameraCoroutine(menuCameraLocation.position, helpCameraLocation.position));
    }

    public void MenuScreen()
    {
        if (!moving) StartCoroutine(MoveCameraCoroutine(helpCameraLocation.position, menuCameraLocation.position));
    }

    IEnumerator MoveCameraCoroutine(Vector3 from, Vector3 to)
    {
        moving = true;

        for (float t = 0; t < 1f; t += Time.deltaTime / cameraMoveTime)
        {
            mainCamera.transform.position = Vector3.Lerp(from, to, moveCurve.Evaluate(t));
            yield return null;
        }
        mainCamera.transform.position = to;

        moving = false;
    }
}
