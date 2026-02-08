using DI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MainApp
{
    public class PopUpService : MonoBehaviour, IService
    {
        private List<IPopUp> popUps = new List<IPopUp>();

        private Dictionary<IPopUp, List<Cart>> popUpKeyCartValue = new Dictionary<IPopUp, List<Cart>>();

        private void Awake()
        {
            popUps = FindObjectsOfType<MonoBehaviour>().OfType<IPopUp>().ToList();
            foreach (var popUp in popUps)
            {
                popUpKeyCartValue.Add(popUp, new List<Cart>());
            }
        }

        public void AddCart(Cart cart, bool isPremium)
        {
            if (isPremium)
            {
                var premPopUp = typeof(PremiumPopUp) as IPopUp;
                popUpKeyCartValue[premPopUp].Add(cart);
                cart.SetOnButtonClickCallback(premPopUp.Open);
            }
            else
            {
                var viewImagePopUp = typeof(ViewImagePopUp) as IPopUp;
                popUpKeyCartValue[viewImagePopUp].Add(cart);
                cart.SetOnButtonClickCallback(viewImagePopUp.Open);
            }
        }
    }
}
