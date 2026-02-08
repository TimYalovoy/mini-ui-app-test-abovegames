using System;
using System.Collections.Generic;
using UnityEngine;

public class FilterController : MonoBehaviour
{
    [SerializeField] private List<FilterButtonEssentialData> filters = new List<FilterButtonEssentialData>();
    [Space]
    [SerializeField] private FilterButton bttnPrefab;
    [SerializeField] private RectTransform buttonsHolder;

    private List<FilterButton> buttons = new List<FilterButton>();

    private Action<int> _onFilterSelected = (id) => { };
    public event Action<int> OnFilterSelected
    {
        add => _onFilterSelected += value;
        remove => _onFilterSelected -= value;
    }

    private void Start()
    {
        buttons.Clear();
        buttons = new List<FilterButton>(filters.Count);
        foreach (var filter in filters)
        {
            var filterButton = Instantiate<FilterButton>(bttnPrefab, buttonsHolder, false);
            filterButton.gameObject.name = filter.Label;
            filterButton.SetData(filter);
            filterButton.SetState(false);
            buttons.Add(filterButton);
        }

        buttons[0].SetState(true);

        foreach (var bttn in buttons)
        {
            bttn.OnButtonClicked += OnFilterButtonClicked;
        }
    }

    private void OnFilterButtonClicked(int id)
    {
        foreach (var bttn in buttons)
        {
            if (bttn.ID == id)
            {
                _onFilterSelected.Invoke(id);
                continue;
            }

            bttn.SetState(false);
        }
    }

    private void OnDestroy()
    {
        foreach (var bttn in buttons)
        {
            bttn.OnButtonClicked -= OnFilterButtonClicked;
        }
    }
}
