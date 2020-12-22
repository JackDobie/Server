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
using System.Text.RegularExpressions;
using System.Threading;

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
        Rectangle hangmanRectangle = new Rectangle(new Point(74), new Point(453));
        int hangmanState;

        List<Keys> keysPressed = new List<Keys>();

        bool typingEnabled;

        string correctWord;
        string displayedWord;

        SpriteFont font;
        SpriteFont font2;
        enum TextAlignment { Centre = 1, Left, Right }

        enum GameScreen { WaitForConnect = 1, WaitForStart, Main, Stop }
        GameScreen currentScreen;
        enum PlayerType { Chooser = 1, Guesser }
        PlayerType currentPlayerType;

        Rectangle fullScreenRect;

        bool stopGame;
        string stopGameMessage;
        bool canExit;

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
            //correctWord = "hello";
            //displayedWord += new string('_', correctWord.Length);
            //displayedWord = string.Join(" ", displayedWord.Reverse()); //turns word into _ _ _ _
            SwitchScreen(GameScreen.WaitForConnect);
            connected = false;
            typingEnabled = false;
            fullScreenRect = new Rectangle(new Point(0), new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            stopGame = false;
            stopGameMessage = "";
            canExit = false;

            if (Connect("127.0.0.1", 4444))
            {
                connectedPlayers.Add(userID);
                ConnectPacket();
                connected = true;

                new Thread(ProcessServerResponse).Start();
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
            font2 = Content.Load<SpriteFont>("Font2");
        }

        protected override void Update(GameTime gameTime)
        {
            if (!connected || canExit)
                Exit();

            if(currentScreen == GameScreen.Main)
            {
                if(hangmanState >= 9)
                {
                    typingEnabled = false;
                    ResultPacket(false);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            switch(currentScreen)
            {
                case GameScreen.WaitForConnect:
                    DrawTextAligned(font, "Waiting for second player to connect.", fullScreenRect, TextAlignment.Centre, Color.Black, 24);
                    break;
                case GameScreen.WaitForStart:
                    if(currentPlayerType == PlayerType.Guesser)
                        DrawTextAligned(font, "Waiting for the chooser to pick a word.", fullScreenRect, TextAlignment.Centre, Color.Black, 24);
                    else
                        DrawTextAligned(font, "Enter a word:\n" + correctWord, fullScreenRect, TextAlignment.Centre, Color.Black, 24);
                    break;
                case GameScreen.Main:
                    _spriteBatch.Draw(hangmanTex[hangmanState < 9 ? hangmanState : 9], hangmanRectangle, Color.White);
                    DrawTextAligned(font2, displayedWord, new Rectangle(35, 650, 530, 65), TextAlignment.Centre, Color.Black, 24);
                    if(currentPlayerType == PlayerType.Chooser)
                    {
                        DrawTextAligned(font, "The other player is now guessing.\nThe correct word is '" + correctWord + "'", new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, 70), TextAlignment.Centre, Color.Black, 18);
                    }
                    else if (currentPlayerType == PlayerType.Guesser)
                    {
                        DrawTextAligned(font, "You are the guesser", new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, 70), TextAlignment.Centre, Color.Black, 24);
                    }
                    break;
            }

            if (stopGame)
                StopGame(stopGameMessage);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void SwitchScreen(GameScreen screen)
        {
            currentScreen = screen;

            switch (screen)
            {
                case GameScreen.WaitForConnect:
                    break;
                case GameScreen.WaitForStart:
                    correctWord = "";
                    if (currentPlayerType == PlayerType.Chooser)
                    {
                        typingEnabled = true;
                    }
                    break;
                case GameScreen.Main:
                    hangmanState = 0;
                    if (currentPlayerType == PlayerType.Guesser)
                    {
                        typingEnabled = true;
                    }
                    break;
            }
        }
        void StopGame(string displayedText)
        {
            SwitchScreen(GameScreen.Stop);
            DrawTextAligned(font, displayedText, fullScreenRect, TextAlignment.Centre, Color.Black, 24);
            Thread thread = new Thread(() => { canExit = WaitForSeconds(2); });
            thread.Start();
        }

        void AdvanceHangman()
        {
            hangmanState++;
            UpdateHangmanStatePacket();
        }

        void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;

            if (typingEnabled)
            {
                if (currentScreen == GameScreen.WaitForStart && currentPlayerType == PlayerType.Chooser)
                {
                    if (pressedKey == Keys.Enter)
                    {
                        if (!string.IsNullOrWhiteSpace(correctWord))
                        {
                            typingEnabled = false;
                            SetWordPacket(correctWord);
                        }
                    }
                    if(pressedKey == Keys.Space)
                    {
                        correctWord += " ";
                    }

                    if (char.IsLetter(character))
                    {
                        correctWord += character;
                    }
                }

                if (currentScreen == GameScreen.Main && currentPlayerType == PlayerType.Guesser)
                {
                    if (!keysPressed.Contains(pressedKey))
                    {
                        keysPressed.Add(pressedKey);

                        if(pressedKey == Keys.Space)
                        {
                            if (correctWord.Contains(' '))
                            {
                                StringBuilder sb = new StringBuilder(displayedWord);
                                for (int i = 0; i < correctWord.Length; i++)
                                {
                                    if (correctWord.Substring(i, 1) == " ")
                                    {
                                        sb[i] = ' ';
                                    }
                                }
                                displayedWord = sb.ToString();
                                if (displayedWord == correctWord)
                                {
                                    ResultPacket(true);
                                }
                                else
                                {
                                    UpdateDisplayedWordPacket();
                                    UpdateHangmanStatePacket();
                                }
                            }
                        }
                        else if (char.IsLetter(character))
                        {
                            if (correctWord.Contains(character))
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
                                if (displayedWord == correctWord)
                                {
                                    ResultPacket(true);
                                }
                                else
                                {
                                    UpdateDisplayedWordPacket();
                                }
                            }
                            else
                            {
                                AdvanceHangman();
                            }
                        }
                    }
                }
            }
        }

        void DrawTextAligned(SpriteFont _font, string text, Rectangle bounds, TextAlignment align, Color colour, float scale)
        {
            Vector2 size = _font.MeasureString(text);
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
            _spriteBatch.DrawString(_font, text, pos, colour, 0, origin, scale / 24, SpriteEffects.None, 0);
        }

        Vector2 GetRectCentre(Rectangle rect)
        {
            return new Vector2(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        }

        bool WaitForSeconds(float time)
        {
            Thread.Sleep((int)(time * 1000));
            return true;
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
            DisconnectPacket();

            base.OnExiting(sender, args);
        }

        void ProcessServerResponse()
        {
            int byteNum;
            try
            {
                while (connected)
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
                                SwitchScreen(GameScreen.WaitForStart);
                                break;
                            case PacketType.GameDisconnect:
                                stopGame = true;
                                stopGameMessage = "The other player has disconnected";
                                break;
                            case PacketType.GameSetWord:
                                GameSetWordPacket wordPacket = (GameSetWordPacket)packet;
                                correctWord = wordPacket.correctWord;
                                displayedWord = "";
                                displayedWord += new string('_', correctWord.Length);
                                //displayedWord = string.Join(" ", displayedWord.Reverse());
                                SwitchScreen(GameScreen.Main);
                                break;
                            case PacketType.GameUpdateDisplayedWord:
                                if (currentPlayerType == PlayerType.Chooser)
                                {
                                    GameUpdateWordPacket updateWordPacket = (GameUpdateWordPacket)packet;
                                    displayedWord = updateWordPacket.displayedWord;
                                }
                                break;
                            case PacketType.GameUpdateHangmanState:
                                if (currentPlayerType == PlayerType.Chooser)
                                {
                                    GameUpdateHangmanPacket updateHangmanPacket = (GameUpdateHangmanPacket)packet;
                                    hangmanState = updateHangmanPacket.hangmanState;
                                }
                                break;
                            case PacketType.GameResult:
                                GameResultPacket resultPacket = (GameResultPacket)packet;
                                if (resultPacket.win)
                                {
                                    stopGame = true;
                                    stopGameMessage = currentPlayerType == PlayerType.Guesser ? "You win!" : "The guesser won!";
                                }
                                else
                                {
                                    stopGame = true;
                                    stopGameMessage = currentPlayerType == PlayerType.Guesser ? "You lost!\nThe correct word was " + correctWord : "The guesser lost!\nThe correct word was " + correctWord;
                                }
                                break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //ooga booga
                connected = false;
            }
        }

        public void ConnectPacket()
        {
            int rand = new Random().Next(1, 3);
            GameConnectPacket packet = new GameConnectPacket(userID, connectedPlayers, GameConnectPacket.PlayerType.Null);
            SendPacket(packet);
        }
        public void DisconnectPacket()
        {
            GameDisconnectPacket packet = new GameDisconnectPacket(userID, connectedPlayers);
            SendPacket(packet);
            Thread.Sleep(10);
            Disconnect();
        }
        public void ResultPacket(bool win)
        {
            GameResultPacket packet = new GameResultPacket(win);
            SendPacket(packet);
        }
        public void UpdateDisplayedWordPacket()
        {
            GameUpdateWordPacket packet = new GameUpdateWordPacket(displayedWord);
            SendPacket(packet);
        }
        public void UpdateHangmanStatePacket()
        {
            GameUpdateHangmanPacket packet = new GameUpdateHangmanPacket(hangmanState);
            SendPacket(packet);
        }
        public void SetWordPacket(string _correctWord)
        {
            GameSetWordPacket packet = new GameSetWordPacket(_correctWord);
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
