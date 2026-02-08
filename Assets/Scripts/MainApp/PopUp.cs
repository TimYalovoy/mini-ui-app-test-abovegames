using DI;
using UnityEngine;
using UnityEngine.UI;

namespace MainApp
{
    public abstract class PopUp : MonoBehaviour, IPopUp
    {
        [SerializeField] private Button closeButton;
        private PopUpService _popUpService;

        protected virtual void Awake()
        {
            closeButton.onClick.AddListener(Close);
        }

        public virtual void Inject(IService service)
        {
            if (service is PopUpService popUpService)
            {
                _popUpService = popUpService;
            }
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
