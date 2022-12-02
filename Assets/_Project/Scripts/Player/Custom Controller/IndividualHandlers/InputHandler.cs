using UnityEngine;
using UnityEngine.InputSystem;

namespace SilverWing.Player.Controller
{
    public class InputHandler
    {
        [SerializeField]
        public PlayerInputs _controls { get; private set; }
        public FrameInput _input { get; private set; }

        public float x;
        public bool jumpPressed, jumpReleased;
        public bool _lastPressed { get; private set; } = false;
        public bool _lastReleased { get; private set; } = true;
        InputAction _jump;
        InputAction _attack;

        public InputHandler(PlayerInputs inputs)
        {
            _controls = inputs;
            _controls.Enable();
            _jump = _controls.PlayerMovement.Jump;
            _attack = _controls.PlayerMovement.WingBounce;

            _controls.PlayerMovement.HorizontalMovement.performed += ctx => UpdateDirection(ctx.ReadValue<float>());
        }

        public void DisableControls() => _controls.Disable();
        public void EnableControls() => _controls.Enable();

        private void UpdateDirection(float direction) => x = direction;

        public bool GetAttackInput() => _attack.WasPerformedThisFrame();

        public FrameInput GatherInput() {
            _input = new FrameInput
            {
                JumpPressed = _jump.WasPressedThisFrame(),
                JumpReleased = _jump.WasReleasedThisFrame(),
                X = x
            };
            
            return _input; 
        }
    }
}
