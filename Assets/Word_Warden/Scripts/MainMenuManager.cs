using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Animator ani;

    bool muted = false;
    [SerializeField] Image SoundOnIcon;
    [SerializeField] Image SoundOffIcon;

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

    //public void OnVolumeButton()
    //{
    //    if (muted == false)
    //    {
    //        muted = true;
    //        AudioListener.pause = true;
    //    }
    //    else
    //    {
    //        muted = false;
    //        AudioListener.pause = false;
    //    }
    //}
}
