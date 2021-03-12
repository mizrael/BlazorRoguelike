using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRoguelike.Core.Utils;

namespace BlazorRoguelike.Core.GameServices
{
    public enum MouseButtons
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    public struct ButtonState
    {
        public ButtonState(bool clicked, bool wasClicked)
        {
            IsClicked = clicked;
            WasClicked = wasClicked;
        }

        public bool IsClicked { get; }
        public bool WasClicked { get; }

        public enum States
        {
            Up = 0,
            Down = 1
        }

        public static readonly ButtonState None = new ButtonState(false, false);
    }

    public enum Keys
    {
        Up = 38,
        Left = 37,
        Down = 40,
        Right = 39,
        Space = 32,
        Enter = 13,
        Esc = 27,
        LeftCtrl = 17,
        LeftAlt = 18,
    }

    public class MouseState
    {
        private readonly IDictionary<MouseButtons, ButtonState> _buttonStates;
        private int _x;
        private int _y;

        public MouseState()
        {
            _buttonStates = EnumUtils.GetAllValues<MouseButtons>()
               .ToDictionary(v => v, v => ButtonState.None);
        }

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void SetButtonState(MouseButtons button, ButtonState.States state)
        {
            var oldState = _buttonStates[button];
            _buttonStates[button] = new ButtonState(state == ButtonState.States.Down, oldState.IsClicked);
            
            OnButtonStateChanged?.Invoke(button, _buttonStates[button], oldState);
        }

        public ButtonState GetButtonState(MouseButtons button) => _buttonStates[button];

        public int X => _x;
        public int Y => _y;

        public event OnButtonStateChangedHandler OnButtonStateChanged;
        public delegate void OnButtonStateChangedHandler(MouseButtons buttons, ButtonState newState, ButtonState oldState);
    }

    public class KeyboardState
    {
        private readonly IDictionary<Keys, ButtonState> _keyboardStates;

        public KeyboardState()
        {
            _keyboardStates = EnumUtils.GetAllValues<Keys>()
                                       .ToDictionary(v => v, v => ButtonState.None);
        }

        public void SetKeyState(Keys key, ButtonState.States state)
        {
            var oldState = _keyboardStates[key];
            _keyboardStates[key] = new ButtonState(state == ButtonState.States.Down, oldState.IsClicked);
        }

        public ButtonState GetKeyState(Keys key) => _keyboardStates[key];
    }

    public class InputService : IGameService
    {
        public ValueTask Step() => ValueTask.CompletedTask;

        public MouseState Mouse { get; } = new();
        public KeyboardState Keyboard { get; } = new();
    }

}