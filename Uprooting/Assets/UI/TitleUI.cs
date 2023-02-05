using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TitleUI : MonoBehaviour
{
    [SerializeField]
    private Sprite playButtonEatenSprite;

    public void Start()
    {
        var ui = GetComponent<UIDocument>();
        var play = ui.rootVisualElement.Q<Button>("play");
        play.clicked += () =>
        {
            play.style.backgroundImage = new StyleBackground(playButtonEatenSprite);
            SceneManager.LoadScene(1);
        };
    }
}
