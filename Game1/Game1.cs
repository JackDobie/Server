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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Cyan);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            foreach (Texture2D tex in hangman)
            {
                _spriteBatch.Draw(tex, position: Vector2.Zero, color: Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
