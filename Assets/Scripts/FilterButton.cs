using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [Space]
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;
    [Space]
    [SerializeField] private Image selectedImg;

    private int id;

    public int ID => id;

    private Action<int> _onButtonClicked = (id) => { };
    public event Action<int> OnButtonClicked
    {
        add => _onButtonClicked += value;
        remove => _onButtonClicked -= value;
    }

    private void Start()
    {
        button.onClick.AddListener(ButtonClicked);
    }

    public void SetData(FilterButtonEssentialData data)
    {
        id = (int)data.SortType;
        label.text = data.Label;
    }

    public void SetState(bool isActive)
    {
        selectedImg.gameObject.SetActive(isActive);
        label.color = isActive ? selectedColor : defaultColor;
    }

    private void ButtonClicked()
    {
        _onButtonClicked.Invoke(id);
        SetState(true);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
