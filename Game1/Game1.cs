using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        //to load content to project, open cmd and enter mgcb-editor
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        List<Texture2D> hangman = new List<Texture2D>();
        Rectangle hangmanRectangle = new Rectangle(new Point(74, 74), new Point(453));
        int hangmanState;

        KeyboardState keyState;
        KeyboardState oldKeyState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            hangmanState = 0;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            hangman.Add(this.Content.Load<Texture2D>("hangman/square"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-1"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-2"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-3"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-4"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-5"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-6"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-7"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-8"));
            hangman.Add(this.Content.Load<Texture2D>("hangman/hangman-9"));
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            keyState = Keyboard.GetState();

            if(GetKeyDown(Keys.Space))
            {
                AdvanceHangman();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = keyState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            _spriteBatch.Draw(hangman[hangmanState], hangmanRectangle, Color.White);

            //foreach (Texture2D tex in hangman)
            //{
            //    _spriteBatch.Draw(tex,hangmanRectangle,Color.White);
            //}

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //returns true the first frame the key is pressed
        bool GetKeyDown(Keys key)
        {
            if (oldKeyState.IsKeyUp(key) && keyState.IsKeyDown(key))
            {
                return true;
            }
            else
                return false;
        }

        //returns true the first frame the key is released
        bool GetKeyUp(Keys key)
        {
            if (oldKeyState.IsKeyDown(key) && keyState.IsKeyUp(key))
            {
                return true;
            }
            else
                return false;
        }

        void AdvanceHangman()
        {
            if(hangmanState < 9)
            {
                hangmanState++;
            }
        }
    }
}
