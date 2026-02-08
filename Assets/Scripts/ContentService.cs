using DI;
using MainApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentService : MonoBehaviour,
    IModelDepend,
    IClient
{
    [SerializeField] private Cart cartPrefab;
    [SerializeField] private RectTransform content;

    private List<Cart> carts = new List<Cart>();

    private Model _model;
    private ServerService _serverService;

    public void Inject(IService service)
    {
        if (service is ServerService serverService)
        {
            _serverService = serverService;
        }
    }

    public void SetModel(Model model)
    {
        _model = model;
    }

    private void Start()
    {
        carts.Clear();
        for (int i = 0; i < _model.AllPics; i++)
        {
            var cart = Instantiate<Cart>(cartPrefab, content, false);
            carts.Add(cart);
        }
    }
}
