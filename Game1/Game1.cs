using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Game1 : Game
    {
        //to load content to project, open cmd and enter mgcb-editor
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        List<Texture2D> hangmanTex = new List<Texture2D>();
        Rectangle hangmanRectangle = new Rectangle(new Point(74, 74), new Point(453));
        int hangmanState;

        KeyboardState keyState;
        KeyboardState oldKeyState;
        List<Keys> keysPressed = new List<Keys>();

        string correctWord;
        string displayedWord;
        string displayedWord2;

        SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            hangmanState = 0;
            correctWord = "hello";
            displayedWord += new string('_', correctWord.Length);
            displayedWord2 = string.Join(" ", displayedWord.Reverse()); //turns word into _ _ _ _
        }

        protected override void Initialize()
        {
            Window.TextInput += TextInputHandler;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/square"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-1"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-2"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-3"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-4"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-5"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-6"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-7"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-8"));
            hangmanTex.Add(this.Content.Load<Texture2D>("hangman/hangman-9"));

            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            keyState = Keyboard.GetState();

            if(GetKeyDown(Keys.Space))
            {
                AdvanceHangman();
            }

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = keyState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            _spriteBatch.Draw(hangmanTex[hangmanState], hangmanRectangle, Color.White);

            if(hangmanState >= 9)
            {
                DrawText("You lose!", new Vector2(260.0f, 30.0f), Color.Black, 24.0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary> Returns true the first frame the key is pressed </summary>
        bool GetKeyDown(Keys key)
        {
            if (oldKeyState.IsKeyUp(key) && keyState.IsKeyDown(key))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary> Returns true the first frame the key is released </summary>
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

        void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;

            if(!keysPressed.Contains(pressedKey))
            {
                keysPressed.Add(pressedKey);

                if(correctWord.Contains(character))
                {
                    StringBuilder sb = new StringBuilder(displayedWord);
                    for (int i = 0; i < correctWord.Length; i++)
                    {
                        if (correctWord.Substring(i, 1) == character.ToString())
                        {
                            sb[i] = character;
                        }
                    }
                    displayedWord = sb.ToString();
                }
                else
                {
                    AdvanceHangman();
                }
            }
        }

        void DrawText(string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(font, text, position, color);
        }
        void DrawText(string text, Vector2 position, Color color, Vector2 scale)
        {
            _spriteBatch.DrawString(font, text, position, color, 0.0f, new Vector2(0), scale, new SpriteEffects(), 0.0f);
        }
        void DrawText(string text, Vector2 position, Color color, float size)
        {
            _spriteBatch.DrawString(font, text, position, color, 0.0f, new Vector2(0), size / 48, new SpriteEffects(), 0.0f);
        }
        void DrawText(string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.DrawString(font, text, position, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
