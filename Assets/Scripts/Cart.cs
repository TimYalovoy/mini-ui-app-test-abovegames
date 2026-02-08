using DI;
using MainApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cart : MonoBehaviour,
    IClient
{
    [SerializeField] private Button _bttn;
    [SerializeField] private Image _image;
    [SerializeField] private Image _premiumBadge;

    private PopUpService _popUpService;

    private Action _onButtonClickCallback;
    private UnityAction _onBttnClkCallbackWrapper;

    void Awake()
    {
        _premiumBadge.enabled = false;
    }

    public void Inject(IService service)
    {
        if (service is PopUpService popUpService)
        {
            _popUpService = popUpService;
            _popUpService.AddCart(this, _premiumBadge.enabled);
        }
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
        _onButtonClickCallback.Invoke();
    }

    public void ReceiveContent(Sprite sprite, bool isPremium)
    {
        _image.sprite = sprite;
        _premiumBadge.enabled = isPremium;
    }

    private void OnBecameVisible()
    {
        //RequestContent();
    }
}
