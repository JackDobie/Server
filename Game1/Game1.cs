using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Game1
{
    public class Game1 : Game
    {
        TcpClient tcpClient;
        NetworkStream stream;
        BinaryFormatter formatter;
        BinaryReader reader;
        BinaryWriter writer;
        int userID = new Random().Next(10000);

        bool connected;
        List<int> connectedPlayers = new List<int>();

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

        SpriteFont font;
        enum TextAlignment { Centre = 1, Left, Right }

        enum GameScreen { WaitForStart = 1, ChooserMain, GuesserMain, EndScreen }
        GameScreen currentScreen;
        enum PlayerType { Chooser = 1, Guesser }
        PlayerType currentPlayerType;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Start();
        }

        void Start()
        {
            hangmanState = 0;
            correctWord = "hello";
            displayedWord += new string('_', correctWord.Length);
            displayedWord = string.Join(" ", displayedWord.Reverse()); //turns word into _ _ _ _
            currentScreen = GameScreen.WaitForStart;
            connected = false;

            if (Connect("127.0.0.1", 4444))
            {
                connectedPlayers.Add(userID);
                ConnectPacket();
                connected = true;
            }
            else
            {
                Exit();
            }
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

            if (connected)
                ProcessServerResponse();

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = keyState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            if(currentScreen == GameScreen.WaitForStart)
            {
                DrawTextAligned("Waiting for second player", new Rectangle(new Point(0), new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)), TextAlignment.Centre, Color.Black);
            }
            else if(currentScreen != GameScreen.EndScreen)
            {
                _spriteBatch.Draw(hangmanTex[hangmanState < 9 ? hangmanState : 9], hangmanRectangle, Color.White);

                DrawTextAligned(displayedWord, new Rectangle(35, 650, 530, 65), TextAlignment.Centre, Color.Black);

                if (hangmanState >= 9)
                {
                    DrawText(currentPlayerType == PlayerType.Chooser ? "You lose!" : "The guesser lost!", new Vector2(260.0f, 30.0f), Color.Black, 24);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void StartGame()
        {

        }
        void StopGame()
        {

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
            hangmanState++;
        }

        void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;

            if(currentScreen == GameScreen.GuesserMain)
            {
                if (!keysPressed.Contains(pressedKey))
                {
                    keysPressed.Add(pressedKey);

                    if (correctWord.Contains(character))
                    {
                        StringBuilder sb = new StringBuilder(displayedWord);
                        for (int i = 0; i < correctWord.Length; i++)
                        {
                            if (correctWord.Substring(i, 1) == character.ToString())
                            {
                                sb[i + i] = character;
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
        }

        /// <summary> Draws text to the screen. Must be called in Draw(), as it uses the spritebatch </summary>
        void DrawText(string text, Vector2 position, Color colour)
        {
            _spriteBatch.DrawString(font, text, position, colour);
        }
        void DrawText(string text, Vector2 position, Color colour, Vector2 scale)
        {
            _spriteBatch.DrawString(font, text, position, colour, 0.0f, new Vector2(0), scale, SpriteEffects.None, 0.0f);
        }
        /// <summary> The scale is (size / 48) as the font size is 48. If the font size changes, the function must be changed </summary>
        void DrawText(string text, Vector2 position, Color colour, float size)
        {
            _spriteBatch.DrawString(font, text, position, colour, 0.0f, new Vector2(0), size / 48, SpriteEffects.None, 0.0f);
        }
        void DrawText(string text, Vector2 position, Color colour, float rotation, Vector2 origin, float size, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.DrawString(font, text, position, colour, rotation, origin, size / 48, effects, layerDepth);
        }
        void DrawTextAligned(string text, Rectangle bounds, TextAlignment align, Color colour)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = GetRectCentre(bounds);
            Vector2 origin = size * 0.5f;

            switch (align)
            {
                case TextAlignment.Left:
                    origin.X += (bounds.Width / 2) - (size.X / 2);
                    break;
                case TextAlignment.Right:
                    origin.X -= (bounds.Width / 2) - (size.X / 2);
                    break;
            }
            _spriteBatch.DrawString(font, text, pos, colour, 0, origin, 1, SpriteEffects.None, 0);
        }

        Vector2 GetRectCentre(Rectangle rect)
        {
            return new Vector2(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        bool Connect(string ipAddress, int port)
        {
            try
            {
                connected = true;
                tcpClient = new TcpClient(ipAddress, port);
                stream = tcpClient.GetStream();
                formatter = new BinaryFormatter();
                reader = new BinaryReader(stream, Encoding.UTF8);
                writer = new BinaryWriter(stream, Encoding.UTF8);
                return true;
            }
            catch
            {
                connected = false;
                return false;
            }
        }
        public bool Disconnect()
        {
            try
            {
                connected = false;
                reader.Close();
                writer.Close();
                stream.Close();
                tcpClient.Close();
                return true;
            }
            catch
            {
                connected = true;
                return false;
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Disconnect();

            base.OnExiting(sender, args);
        }

        void ProcessServerResponse()
        {
            int byteNum;
            try
            {
                if ((byteNum = reader.ReadInt32()) != 0)
                {
                    byte[] buffer = reader.ReadBytes(byteNum);
                    MemoryStream memstream = new MemoryStream(buffer);

                    Packet packet = formatter.Deserialize(memstream) as Packet;

                    switch (packet.packetType)
                    {
                        case PacketType.GameConnect:
                            GameConnectPacket connectPacket = (GameConnectPacket)packet;
                            connectedPlayers.Add(connectPacket.ID);
                            currentPlayerType = connectPacket.playerType == GameConnectPacket.PlayerType.Chooser ? PlayerType.Chooser : PlayerType.Guesser;
                            StartGame();
                            break;
                        case PacketType.GameDisconnect:
                            StopGame();
                            break;
                        case PacketType.GameSetWord:
                            GameSetWordPacket wordPacket = (GameSetWordPacket)packet;
                            correctWord = wordPacket.correctWord;
                            displayedWord = "";
                            displayedWord += new string('_', correctWord.Length);
                            displayedWord = string.Join(" ", displayedWord.Reverse());
                            break;
                        case PacketType.GameEnterCharacter:
                            if(currentPlayerType == PlayerType.Chooser)
                            {
                                //do stuff
                            }
                            break;
                        case PacketType.GameResult:
                            GameResultPacket resultPacket = (GameResultPacket)packet;
                            if(resultPacket.win)
                            {
                                //do stuff
                            }
                            else
                            {
                                //do stuff
                            }
                            break;
                    }
                }
            }
            catch
            {
                //ooga booga
            }
        }

        public void ConnectPacket()
        {
            int rand = new Random().Next(1, 3);
            currentPlayerType = rand == 1 ? PlayerType.Chooser : PlayerType.Guesser;
            GameConnectPacket packet = new GameConnectPacket(userID, connectedPlayers, rand == 1 ? GameConnectPacket.PlayerType.Chooser : GameConnectPacket.PlayerType.Guesser);
            SendPacket(packet);
        }
        public void DisconnectPacket()
        {
            GameDisconnectPacket packet = new GameDisconnectPacket(userID, connectedPlayers);
            SendPacket(packet);
        }

        void SendPacket(Packet packet)
        {
            MemoryStream memstream = new MemoryStream();
            formatter.Serialize(memstream, packet);
            byte[] buffer = memstream.GetBuffer();
            writer.Write(buffer.Length);
            writer.Write(buffer);
            writer.Flush();
        }
    }
}
