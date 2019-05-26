using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public Dropdown SelectedClass;

    public void Exit()
    {
        Application.Quit();
    }
}
