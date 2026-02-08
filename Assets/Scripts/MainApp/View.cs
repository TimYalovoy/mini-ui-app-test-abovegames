using UnityEngine;

namespace MainApp
{
    public class View : MonoBehaviour
    {
        [SerializeField] private FilterController filterController;
        [SerializeField] private ContentService contentService;
        [SerializeField] private PopUpService popUpService;
    }
}
