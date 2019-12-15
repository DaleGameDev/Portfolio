using System;
using Microsoft.Xna.Framework;
using MonoGame.UI.Forms;

namespace TheLastImperial.Controllers
{
    //using a downloaded library monogame.ui.forms
    class UI : ControlManager
    {
        public UI(Game game) : base(game)
        {
   
        }
        private float time;
        private float fireCooldown;
        private float weaponsCooldown;
        private float shieldCooldown;

        public Button btn;
        public Button btn0;
        public Button btn1;
        public Button restart;
        public Button Start;
        public TextArea txt;

        //Creating the buttons 
        public override void InitializeComponent()
        {
            txt = new TextArea()
            {
                TextColor = Color.White,
                Location = new Vector2(20, 400),
                Text = Entity.PlayerInfo.playerInfo.Level.ToString() + "\n" +
                Entity.PlayerInfo.playerInfo.Power.ToString() + "\n" +
                Entity.PlayerInfo.playerInfo.Distance.ToString() + "\n" +
                Entity.PlayerInfo.playerInfo.Material.ToString()
                
            };



            restart = new Button()
            {
                Text = "Restart",
                Size = new Vector2(90, 45),
                Location = new Vector2(20, 40),
                BackgroundColor = Color.Black
            };
            restart.Clicked += Restart_clicked;

            btn = new Button()
            {
                Text = "Shields",
                Size = new Vector2(90, 45),
                Location = new Vector2(20, 40),
                BackgroundColor = Color.Black

            };
            btn.Clicked += Btn_clicked;
            btn0 = new Button()
            {
                Text = "Weapons",
                Size = new Vector2(90, 45),
                Location = new Vector2(20, 80),
                BackgroundColor = Color.Black

            };
            btn0.Clicked += Btn0_clicked;
            btn1 = new Button()
            {
                Text = "Fire",
                Size = new Vector2(90, 45),
                Location = new Vector2(20, 120),
                BackgroundColor = Color.Black

            };
            btn1.Clicked += Btn1_clicked;

            Start = new Button()
            {
                Text = "Start",
                Size = new Vector2(90, 45),
                Location = new Vector2(20, 120),
                BackgroundColor = Color.Black
            };
            Start.Clicked += Start_Clicked;
            Controls.Add(Start);

           
        }

        //methods that respond to button clicked events 

        private void Start_Clicked(object sender, EventArgs e)
        {
            GameSceneUI();
            OnStart();
        }

        private void Restart_clicked(object sender,EventArgs e)
        {
            GameSceneUI();
            btn.Text = "Shields";
            btn0.Text = "Weapons";
            OnRestart();
        }

       
        private void Btn_clicked(object sender,EventArgs e)
        {
            if (shieldCooldown <= 0.0f)
            {
                Button btn = sender as Button;
                if(btn.Text != "Active")
                {
                    btn.Text = "Active";
                }
                else
                {
                    btn.Text = "Shields";
                }
                
                btn.BackgroundColor = Color.Red;
                OnShield();
                shieldCooldown = Entity.PlayerInfo.playerInfo.Cooldown;
            } 
        }

        private void Btn0_clicked(object sender, EventArgs e)
        {
            if (weaponsCooldown <= 0.0f)
            {
                Button btn0 = sender as Button;
                if (btn0.Text != "Active")
                {
                    btn0.Text = "Active";
                }else
                {
                    btn0.Text = "Weapons";
                }
                btn0.BackgroundColor = Color.Red;
                OnWeapons();
                weaponsCooldown = Entity.PlayerInfo.playerInfo.Cooldown;
            }
        }

        private void Btn1_clicked(object sender, EventArgs e)
        {
            if (fireCooldown <= 0.0f)
            {
                Button btn = sender as Button;
                btn1.Text = "Fire";
                btn1.BackgroundColor = Color.Red;
                OnFire();
                fireCooldown = Entity.PlayerInfo.playerInfo.Cooldown;
            }
        }

        //called every frame to update buttons
        public void UpdateButtons(GameTime gameTime)
        {
            time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fireCooldown -= time;
            weaponsCooldown -= time;
            shieldCooldown -= time;
            if (fireCooldown <= 0.0f) { btn1.BackgroundColor = Color.Black; }
            if(weaponsCooldown <= 0.0f) { btn0.BackgroundColor = Color.Black; }
            if(shieldCooldown <= 0.0f) { btn.BackgroundColor = Color.Black; }
            txt.Text = Entity.PlayerInfo.playerInfo.Level.ToString() + "\n" +
                Entity.PlayerInfo.playerInfo.Power.ToString() + "\n" +
                Entity.PlayerInfo.playerInfo.Distance.ToString() + "\n" +
                Entity.PlayerInfo.playerInfo.Material.ToString();
        }

        //organizing the buttons removing and adding 
        public void GameOver()
        {
            Controls.Remove(btn);
            Controls.Remove(btn0);
            Controls.Remove(btn1);
            Controls.Add(restart);

        }
        private void GameSceneUI()
        {
            Controls.Clear();
            Controls.Add(btn);
            Controls.Add(btn0);
            Controls.Add(btn1);
            Controls.Add(txt);
        }

        //Events from various buttons 
        public event EventHandler<EventArgs> ShieldClicked;

        protected virtual void OnShield()
        {
            ShieldClicked(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> WeaponsClicked;

        protected virtual void OnWeapons()
        {
            WeaponsClicked(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> FireClicked;

        protected virtual void OnFire()
        {
            FireClicked(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> StartClicked;

        protected virtual void OnStart()
        {
            StartClicked(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> RestartClicked;

        protected virtual void OnRestart()
        {
            RestartClicked(this, EventArgs.Empty);
        }

    }



    public enum UIButtonState
    {
        None,
        Clicked,
        UnClicked
    }

    
}
