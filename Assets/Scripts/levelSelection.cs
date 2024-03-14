using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelSelection : MonoBehaviour
{
  

    public void changeScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }


}
