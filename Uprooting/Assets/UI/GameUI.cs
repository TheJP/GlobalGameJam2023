using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    private Label actions;
    private VisualElement movement;

    public void Start()
    {
        var ui = GetComponent<UIDocument>();
        actions = ui.rootVisualElement.Q<Label>("actions");
        movement = ui.rootVisualElement.Q("movement");
    }

    public void Update()
    {
        actions.text = $"{TurnSystemController.Instance.CurrentAPLeft}";
        var movementHeight = (float)TurnSystemController.Instance.CurrentMovementLeft /
            (float)TurnSystemController.Instance.PlayerMovementPerTurn;
        movement.style.height = new StyleLength(Length.Percent(movementHeight * 100f));
    }
}