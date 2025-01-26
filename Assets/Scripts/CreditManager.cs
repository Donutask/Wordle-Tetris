using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Donutask.Wordfall
{
    public class CreditManager : MonoBehaviour
    {
        [SerializeField] InputAction exitAction;

        private void Start()
        {
            exitAction.Enable();
            exitAction.performed += ExitAction_performed;
        }

        private void ExitAction_performed(InputAction.CallbackContext obj)
        {
            exitAction.Disable();
            SceneManager.LoadSceneAsync("Game");
        }
    }
}
