﻿using Microsoft.Xna.Framework;
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
        Rectangle hangmanRectangle = new Rectangle(new Point(74, 74), new Point(453));
        int hangmanState;

        List<Keys> keysPressed = new List<Keys>();

        bool typingEnabled;

        string correctWord;
        string displayedWord;

        SpriteFont font;
        enum TextAlignment { Centre = 1, Left, Right }

        enum GameScreen { WaitForConnect = 1, WaitForStart, Main, EndScreen }
        GameScreen currentScreen;
        enum PlayerType { Chooser = 1, Guesser }
        PlayerType currentPlayerType;

        Rectangle fullScreenRect;

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
            SwitchScreen(GameScreen.WaitForConnect);
            connected = false;
            typingEnabled = false;
            fullScreenRect = new Rectangle(new Point(0), new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

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
        }

        protected override void Update(GameTime gameTime)
        {
            if (!connected)
                Exit();

            switch (currentScreen)
            {
                case GameScreen.WaitForConnect:
                    break;
                case GameScreen.WaitForStart:
                    break;
                case GameScreen.Main:
                    if(hangmanState >= 9)
                    {
                        typingEnabled = false;
                        ResultPacket(false);
                        SwitchScreen(GameScreen.EndScreen);
                    }
                    break;
                default:
                    break;
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
                    DrawTextAligned("Waiting for second player to connect.", fullScreenRect, TextAlignment.Centre, Color.Black);
                    break;
                case GameScreen.WaitForStart:
                    if(currentPlayerType == PlayerType.Guesser)
                        DrawTextAligned("Waiting for the chooser to pick a word.", fullScreenRect, TextAlignment.Centre, Color.Black);
                    else
                        DrawTextAligned("Enter a word: " + correctWord, fullScreenRect, TextAlignment.Centre, Color.Black);
                    break;
                case GameScreen.Main:
                    _spriteBatch.Draw(hangmanTex[hangmanState < 9 ? hangmanState : 9], hangmanRectangle, Color.White);
                    DrawTextAligned(displayedWord, new Rectangle(35, 650, 530, 65), TextAlignment.Centre, Color.Black);
                    break;
                default:
                    DrawTextAligned("Game screen not set up: " + currentScreen.ToString(), fullScreenRect, TextAlignment.Centre, Color.Black);
                    break;
            }

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
            DrawTextAligned(displayedText, fullScreenRect, TextAlignment.Centre, Color.Black);
            Thread.Sleep(2000);
            Exit();
            //display text
            //sleep for a few seconds
            //exit game
        }

        void AdvanceHangman()
        {
            hangmanState++;
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
                    else
                    {
                        if (char.IsLetter(character))
                        {
                            correctWord += character;
                        }
                    }
                }

                if (char.IsLetter(character))
                {
                    if (currentScreen == GameScreen.Main && currentPlayerType == PlayerType.Guesser)
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
                                UpdateDisplayedWordPacket();
                                UpdateHangmanStatePacket();
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
                                StopGame("Other player has disconnected.");
                                break;
                            case PacketType.GameSetWord:
                                GameSetWordPacket wordPacket = (GameSetWordPacket)packet;
                                correctWord = wordPacket.correctWord;
                                displayedWord = "";
                                displayedWord += new string('_', correctWord.Length);
                                displayedWord = string.Join(" ", displayedWord.Reverse());
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
                                    if (hangmanState >= 9)
                                        SwitchScreen(GameScreen.EndScreen);
                                }
                                break;
                            case PacketType.GameResult:
                                GameResultPacket resultPacket = (GameResultPacket)packet;
                                if (resultPacket.win)
                                {
                                    StopGame(currentPlayerType == PlayerType.Guesser ? "You win!" : "The guesser won!");
                                }
                                else
                                {
                                    StopGame(currentPlayerType == PlayerType.Guesser ? "You lost! The correct word was " + correctWord : "The guesser lost! The correct word was " + correctWord);
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
