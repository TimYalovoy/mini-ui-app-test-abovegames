using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cart : MonoBehaviour
{
    private Button _bttn;
    private Image _image;
    private Image _premiumBadge;

    private Action _onButtonClickCallback;
    private UnityAction _onBttnClkCallbackWrapper;

    void Awake()
    {
        _bttn = GetComponent<Button>();
        _image = _bttn.GetComponent<Image>();
        _premiumBadge = transform.GetChild(1).GetComponent<Image>();
        _premiumBadge.enabled = false;
    }

    private void OnEnable()
    {
        _bttn.onClick.AddListener(_onBttnClkCallbackWrapper);
    }

    private void OnDisable()
    {
        _bttn.onClick.RemoveListener(_onBttnClkCallbackWrapper);
    }

    public void SetOnButtonClickCallback(Action onButtonClickCallback)
    {
        _onButtonClickCallback = onButtonClickCallback;
        _onBttnClkCallbackWrapper = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), _onButtonClickCallback.Target, _onButtonClickCallback.Method);
    }

    private void OnButtonClick()
    {

    }

    public void RequestContent()
    {

    }

    public void ReceiveContent(Sprite sprite, bool isPremium)
    {
        _image.sprite = sprite;
        _premiumBadge.enabled = isPremium;
    }

    private void OnBecameVisible()
    {
        RequestContent();
    }
}
