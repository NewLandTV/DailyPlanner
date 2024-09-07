using UnityEngine;

public class RightButton : MonoBehaviour
{
    private enum Visible
    {
        Show = 0,
        Hide = 1
    }

    [SerializeField]
    private KeyCode showKeyCode = KeyCode.Delete;
    [SerializeField]
    private KeyCode hideKeyCode = KeyCode.End;

    private bool visible;

    private Animator animator;

    private void Awake() => Initialize();

    private void Update() => GetInput();

    private void Initialize() => animator = GetComponent<Animator>();

    private void GetInput()
    {
        bool pressedShowKey = Input.GetKeyDown(showKeyCode);
        bool pressedHideKey = Input.GetKeyDown(hideKeyCode);

        if (pressedShowKey && !visible)
        {
            visible = true;

            SetAnimatorTrigger(Visible.Show);
        }

        if (pressedHideKey && visible)
        {
            visible = false;

            SetAnimatorTrigger(Visible.Hide);
        }
    }

    private void SetAnimatorTrigger(Visible visible)
    {
        string name = visible == Visible.Show ? "Show" : "Hide";

        animator.SetTrigger(name);
    }
}
