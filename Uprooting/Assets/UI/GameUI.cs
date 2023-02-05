using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    private Label actions;
    private VisualElement movement;
    private ProgressBar enemyProgress;

    public void Start()
    {
        var ui = GetComponent<UIDocument>();
        var noOxygen = ui.rootVisualElement.Q("no-oxygen");
        actions = ui.rootVisualElement.Q<Label>("actions");
        movement = ui.rootVisualElement.Q("movement");
        var nextRound = ui.rootVisualElement.Q<Button>("next-round");
        var enemyBox = ui.rootVisualElement.Q("enemy-box");
        enemyProgress = ui.rootVisualElement.Q<ProgressBar>("enemy-progress");

        FindObjectOfType<MoleOxygen>().HasOxygenChanged +=
            mole => noOxygen.style.display = mole.HasOxygen ? DisplayStyle.None : DisplayStyle.Flex;
        nextRound.RegisterCallback<ClickEvent>(e => TurnSystemController.Instance.EndPlayerTurn());

        TurnSystemController.Instance.OnPlayerTurnEnd += (o, e) => enemyBox.style.display = DisplayStyle.Flex;
        TurnSystemController.Instance.OnEnemyTurnEnd += (o, e) => enemyBox.style.display = DisplayStyle.None;
    }

    public void Update()
    {
        actions.text = $"{TurnSystemController.Instance.CurrentAPLeft}";
        var movementHeight = (float)TurnSystemController.Instance.CurrentMovementLeft /
            (float)TurnSystemController.Instance.PlayerMovementPerTurn;
        movement.style.height = new StyleLength(Length.Percent(movementHeight * 100f));

        enemyProgress.value = EnemyController.Instance.Progress;
    }
}
