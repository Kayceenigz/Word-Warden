using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Animator ani;

   public void startButton()
    {
        Debug.Log("Error");
        StartCoroutine(LevelScene());
    }

    IEnumerator LevelScene()
    {
        ani.SetTrigger("Start");

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Level");
    }
}
