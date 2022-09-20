using ChessAI.ChessPieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;

namespace ChessAI
{
    public class Game1 : Game
    {
        private Texture2D chessBoardTexture;
        private Dictionary<string, Texture2D> chessPieceTextures;

        private Team Turn { get; set; } = Team.White;
        private Task<ChessMove?> getBestMove;

        private readonly IChessPiece[] chessBoard = new IChessPiece[78];

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly ChessAI chessAIBlack = new ChessAI(Team.Black);

        private SpriteFont _font;
        public static int optimisim = 0;
        public static int pesimism = 0;
        public static int movesToCalculate = 0;

        private bool disable = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GCSettings.LatencyMode = GCLatencyMode.Batch;

            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.PreferredBackBufferWidth = 1200;

            _graphics.ApplyChanges();

            ChessboardHelper.IntializeChessBoard(in chessBoard);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            chessBoardTexture = Content.Load<Texture2D>("chessboard");

            chessPieceTextures = new Dictionary<string, Texture2D>
            {
                { "bb", Content.Load<Texture2D>("bb") },
                { "bk", Content.Load<Texture2D>("bk") },
                { "bn", Content.Load<Texture2D>("bn") },
                { "bp", Content.Load<Texture2D>("bp") },
                { "bq", Content.Load<Texture2D>("bq") },
                { "br", Content.Load<Texture2D>("br") },
                { "wb", Content.Load<Texture2D>("wb") },
                { "wk", Content.Load<Texture2D>("wk") },
                { "wn", Content.Load<Texture2D>("wn") },
                { "wp", Content.Load<Texture2D>("wp") },
                { "wq", Content.Load<Texture2D>("wq") },
                { "wr", Content.Load<Texture2D>("wr") }
            };

            _font = Content.Load<SpriteFont>("File");

            // TODO: use this.Content to load your game content here
        }

        private bool pressed = false;

        public static (IChessPiece, Vector2, Vector2) MovingChessPiece = (new King(Team.Black), new Vector2(-150, -150), new Vector2(-150, -150));

        public double Timer = 0.0;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region Mouse

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && pressed != true && disable == false)
            {
                pressed = true;

                int index = 0;
                foreach (IChessPiece chessPiece in chessBoard)
                {
                    ChessPosition chessPosition = new ChessPosition(index);
                    int x = chessPosition.X * 150;
                    int y = chessPosition.Y * 150;

                    if (
                        chessPiece != null &&
                        x > mouseState.X - 150 &&
                        x < mouseState.X &&
                        y > mouseState.Y - 150 &&
                        y < mouseState.Y &&
                        chessPiece.Team == Turn
                    )
                    {
                        MovingChessPiece = (chessPiece, new Vector2(x, y), new Vector2(x, y));
                    }
                    index++;
                }
            }

            if (pressed && MovingChessPiece.Item2.X != -150)
            {
                MovingChessPiece.Item2.X = MovingChessPiece.Item3.X + (mouseState.X - 75) - MovingChessPiece.Item3.X;
                MovingChessPiece.Item2.Y = MovingChessPiece.Item3.Y + (mouseState.Y - 75) - MovingChessPiece.Item3.Y;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released && pressed)
            {
                pressed = false;

                if (MovingChessPiece.Item2.X >= -75 && MovingChessPiece.Item2.Y >= -75)
                {
                    ChessMove chessMove = new ChessMove(
                        new ChessPosition(Convert.ToInt32(MovingChessPiece.Item3.X / 150), Convert.ToInt32(MovingChessPiece.Item3.Y / 150)),
                        new ChessPosition(Convert.ToInt32(MovingChessPiece.Item2.X / 150), Convert.ToInt32(MovingChessPiece.Item2.Y / 150))
                    );

                    if (MovingChessPiece.Item1.CanMove(in chessBoard, chessMove) &&
                        !IChessPiece.AttacksKing(in chessBoard, chessMove)
                        && MovingChessPiece.Item1.Team == Turn
                    )
                    {
                        MovingChessPiece.Item1.Move(in chessBoard, chessMove);
                        Turn = Turn == Team.White ? Team.Black : Team.White;
                    }

                    MovingChessPiece.Item2.X = -150;
                    MovingChessPiece.Item2.Y = -150;
                    MovingChessPiece.Item3.X = -150;
                    MovingChessPiece.Item3.Y = -150;
                }
            }

            #endregion Mouse

            if (Turn == Team.Black && Timer <= gameTime.TotalGameTime.TotalMilliseconds)
            {
                disable = true;
                if (getBestMove == null)
                {
                    getBestMove = Task.Run(() => chessAIBlack.GetBestMove(chessBoard, Turn, 5));
                }
                if (getBestMove != null && getBestMove.IsCompleted)
                {
                    ChessMove? nullableBestMove = getBestMove.Result;
                    if (nullableBestMove.HasValue)
                    {
                        ChessMove bestMove = nullableBestMove.Value;
                        int fromPosition = bestMove.From.ToIndex();
                        if (
                        chessBoard[fromPosition] != null &&
                        chessBoard[fromPosition].CanMove(chessBoard, bestMove)
                        && !IChessPiece.AttacksKing(chessBoard, bestMove)
                        )
                        {
                            chessBoard[fromPosition].Move(chessBoard, bestMove);
                            Turn = Turn == Team.White ? Team.Black : Team.White;
                            getBestMove = null;
                        }
                    }

                    disable = false;
                    getBestMove = null;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //_spriteBatch.Begin(0, null, null, null, null, null, scaleMatrix);
            _spriteBatch.Begin();

            _spriteBatch.Draw(chessBoardTexture, new Vector2(0, 0), Color.White);

            int index = 0;
            foreach (IChessPiece chessPiece in chessBoard)
            {
                if (chessPiece != null)
                {
                    ChessPosition chessPosition = new ChessPosition(index);
                    _spriteBatch.Draw(
                        chessPieceTextures[chessPiece.Texture],
                        new Vector2(
                            chessPosition.X * 150,
                            chessPosition.Y * 150
                        ),
                        Color.White
                    );
                }
                index++;
            }

            _spriteBatch.Draw(
                chessPieceTextures[MovingChessPiece.Item1.Texture],
                new Vector2(
                    MovingChessPiece.Item2.X,
                    MovingChessPiece.Item2.Y
                )
                , new Color(255, 255, 255, 0.8f));

            _spriteBatch.DrawString(_font, "Optimism: " + optimisim, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Pesimism: " + pesimism, new Vector2(0, 32), Color.White);
            _spriteBatch.DrawString(_font, "MovesToCalculate: " + movesToCalculate, new Vector2(0, 64), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}