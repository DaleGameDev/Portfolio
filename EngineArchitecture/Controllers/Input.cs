using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace TheLastImperial.Controllers
{
    //This class implements the input code from the tutorial
    class Input
    {
        // Current and previous keyboard states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }

        // List of keys to check for
        public HashSet<Keys> KeyList;
        public HashSet<MouseButton> ButtonList;

        //Keyboard event handlers
        //key is down
        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };
        //key was up and is now down
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };
        //key was down and is now up
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };

        //Mouse event handlers
        public event EventHandler<MouseEventArgs> OnMouseButtonDown = delegate { };

        public Input()
        {
            CurrentKeyboardState = Keyboard.GetState();
            PrevKeyboardState = CurrentKeyboardState;

            CurrentMouseState = Mouse.GetState();
            PrevMouseState = CurrentMouseState;

            KeyList = new HashSet<Keys>();
            ButtonList = new HashSet<MouseButton>();
        }

        public void AddButton(MouseButton button)
        {
            ButtonList.Add(button);
        }

        public void AddKey(Keys key)
        {
            KeyList.Add(key);
        }

        public void Update()
        {
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            PrevMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            FireKeyboardEvents();
            FireMouseEvents();
        }

        private void FireKeyboardEvents()
        {
            // Check through each key in the key list
            foreach (Keys key in KeyList)
            {
                // Is the key currently down?
                if (CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyDown event
                    if (OnKeyDown != null)
                        OnKeyDown(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }

                // Has the key been released? (Was down and is now up)
                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key))
                {
                    // Fire the OnKeyUp event
                    if (OnKeyUp != null)
                        OnKeyUp(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }

                // Is the key pressed (was up and is now down)
                if (PrevKeyboardState.IsKeyUp(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyPressed event
                    if (OnKeyPressed != null)
                        OnKeyPressed(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
            }

        }

        private void FireMouseEvents()
        {
            // Check through each key in the key list
            foreach (MouseButton button in ButtonList)
            {
                if (button == MouseButton.LEFT)
                {
                    // Is the left mouse button currently down?
                    if (CurrentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Fire the OnMouseButtonDown event
                        if (OnMouseButtonDown != null)
                            OnMouseButtonDown(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }
                }
            }

        }


    }

    public class KeyboardEventArgs : EventArgs
    {
        public KeyboardEventArgs(Keys key, KeyboardState currentKeyboardState, KeyboardState prevKeyboardState)
        {
            CurrentState = currentKeyboardState;
            PrevState = prevKeyboardState;
            Key = key;
        }

        public readonly KeyboardState CurrentState;
        public readonly KeyboardState PrevState;
        public readonly Keys Key;
    }


    public enum MouseButton
    {
        NONE = 0x00,
        LEFT = 0x01,
        RIGHT = 0x02,
        MIDDLE = 0x04,
        XBUTTON1 = 0x08,
        XBUTTON2 = 0x10,
    }

    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(MouseButton button, MouseState currentState, MouseState prevState)
        {
            CurrentState = currentState;
            PrevState = prevState;
            Button = button;
        }

        public readonly MouseState CurrentState;
        public readonly MouseState PrevState;
        public readonly MouseButton Button;
    }


    public delegate void GameAction(eButtonState buttonState, Vector2 amount);

    public class CommandManager
    {
        private Input m_Input;

        private Dictionary<Keys, GameAction> m_KeyBindings = new Dictionary<Keys, GameAction>();
        private Dictionary<MouseButton, GameAction> m_MouseButtonBindings = new Dictionary<MouseButton, GameAction>();

        public CommandManager()
        {
            m_Input = new Input();

            // Register events with the input listener
            m_Input.OnKeyDown += this.OnKeyDown;
            m_Input.OnKeyPressed += this.OnKeyPressed;
            m_Input.OnKeyUp += this.OnKeyUp;
            m_Input.OnMouseButtonDown += this.OnMouseButtonDown;
        }

        public void Update()
        {
            // Update polling input listener, everything else is handled by events
            m_Input.Update();
        }

        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            { 
                action(eButtonState.DOWN, new Vector2(1.0f));
            }

        }

        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.UP, new Vector2(1.0f));
            }
        }

        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.PRESSED, new Vector2(1.0f));
            }
        }

        //
        public void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            GameAction action = m_MouseButtonBindings[e.Button];

            if (action != null)
            {
                action(eButtonState.DOWN, new Vector2(e.CurrentState.X, e.CurrentState.Y));
            }
        }

        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddKey(key);

            // Add the binding to the command map
            m_KeyBindings.Add(key, action);
        }

        public void AddMouseBinding(MouseButton button, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddButton(button);

            // Add the binding to the command map
            m_MouseButtonBindings.Add(button, action);
        }

    }

    public enum eButtonState
    {
        NONE = 0,
        DOWN,
        UP,
        PRESSED,
    }



}
