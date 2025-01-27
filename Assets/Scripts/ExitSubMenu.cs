using UnityEngine;
using UnityEngine.SceneManagement;
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
            SceneManager.LoadSceneAsync("Game");
            loading = true;
        }
    }
}
