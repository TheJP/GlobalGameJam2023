using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TitleUI : MonoBehaviour
{
    public void Start()
    {
        var ui = GetComponent<UIDocument>();
        var play = ui.rootVisualElement.Q<Button>("play");
        play.clicked += () => SceneManager.LoadScene(1);
    }
}
