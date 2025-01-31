using UnityEngine;
using UnityEngine.InputSystem;

namespace Donutask.Wordfall
{
    public class ExitSubMenu : MonoBehaviour
    {
        [SerializeField] InputAction exitAction;
        bool loading;

        private void Start()
        {
            exitAction.Enable();
            exitAction.performed += ExitAction_performed;
        }

        private void ExitAction_performed(InputAction.CallbackContext obj)
        {
            if (loading)
            {
                return;
            }
            exitAction.Disable();
            SceneLoadingTransition.Instance.LoadScene("Game", UnityEngine.UI.Slider.Direction.BottomToTop);
            loading = true;
        }
    }
}
