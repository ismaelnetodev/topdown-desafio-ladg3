using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace DesafioLADG3
{
    public class Game1 : Core
    {

        private AnimatedSprite _player;
        private AnimatedSprite _enemy;

        private Vector2 _playerPosition;
        private const float MOVEMENT_SPEED = 5.0f;

        private Vector2 _enemyPosition;
        private Vector2 _enemyVelocity;

        public Game1() : base("Desafio LADG 3", 1280, 720, false)
        {
           
        }

        protected override void Initialize()
        {
            base.Initialize();
            _enemyPosition = new Vector2(_player.Width + 10, 0);
            AssignRandomEnemyVelocity();
        }

        protected override void LoadContent()
        {
            TextureAtlas playerTextureAtlas = TextureAtlas.FromFile(Content, "player/player_definition.xml");
            TextureAtlas enemyTextureAtlas = TextureAtlas.FromFile(Content, "enemy/enemy_definition.xml");

            _player = playerTextureAtlas.CreateAnimatedSprite("player_walk_down");
            _player.Scale = new Vector2(2.5f, 2.5f);

            _enemy = enemyTextureAtlas.CreateAnimatedSprite("enemy");
            _enemy.Scale = new Vector2(3.0f, 3.0f);


            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);
            _enemy.Update(gameTime);

            CheckForKeyboardInput();
            CheckForGamePadInput();

            Rectangle screenBounds = new Rectangle(
                0,
                0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            );

            Circle playerBounds = new Circle(
                (int)(_playerPosition.X + (_player.Width * 0.5f)),
                (int)(_playerPosition.Y + (_player.Height * 0.5f)),
                (int)(_player.Width * 0.5f)
            );

            if (playerBounds.Left < screenBounds.Left)
                _playerPosition.X = screenBounds.Left;
            else if (playerBounds.Right > screenBounds.Right)
                _playerPosition.X = screenBounds.Right - _player.Width;

            if (playerBounds.Top < screenBounds.Top)
                _playerPosition.Y = screenBounds.Top;
            else if (playerBounds.Bottom > screenBounds.Bottom)
                _playerPosition.Y = screenBounds.Bottom - _player.Height;

            Vector2 newEnemyPosition = _enemyPosition + _enemyVelocity;

            Circle enemyBounds = new Circle(
                (int)(newEnemyPosition.X + (_enemy.Width * 0.5f)),
                (int)(newEnemyPosition.Y + (_enemy.Height * 0.5f)),
                (int)(_enemy.Width * 0.5f)
            );

            Vector2 normal = Vector2.Zero;

            if (enemyBounds.Left < screenBounds.Left)
            {
                normal.X = Vector2.UnitX.X;
                newEnemyPosition.X = screenBounds.Left;
            }
            else if (enemyBounds.Right > screenBounds.Right)
            {
                normal.X = -Vector2.UnitX.X;
                newEnemyPosition.X = screenBounds.Right - _enemy.Width;
            }

            if (enemyBounds.Top < screenBounds.Top)
            {
                normal.Y = Vector2.UnitY.Y;
                newEnemyPosition.Y = screenBounds.Top;
            }
            else if (enemyBounds.Bottom > screenBounds.Bottom)
            {
                normal.Y = -Vector2.UnitY.Y;
                _enemyVelocity.Y = screenBounds.Bottom - _enemy.Height;
            }

            if (normal != Vector2.Zero)
            {
                normal.Normalize();
                _enemyVelocity = Vector2.Reflect(_enemyVelocity, normal);
            }

            _enemyPosition = newEnemyPosition;

            if (playerBounds.Intersects(enemyBounds))
            {
                int TotalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_enemy.Width;
                int TotalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_enemy.Height;

                int column = Random.Shared.Next(0, TotalColumns);
                int row = Random.Shared.Next(0, TotalRows);

                _enemyPosition = new Vector2(column * _enemy.Width, row * _enemy.Height);

                AssignRandomEnemyVelocity();
            }

            base.Update(gameTime);
        }

        private void AssignRandomEnemyVelocity()
        {
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);
            
            _enemyVelocity = direction * MOVEMENT_SPEED;
        }

        private void CheckForKeyboardInput()
        {
            float speed = MOVEMENT_SPEED;
            if (Input.Keyboard.IsKeyDown(Keys.Space)) speed *= 1.5f;

            if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
                _playerPosition.Y -= speed;

            if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
                _playerPosition.Y += speed;

            if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
                _playerPosition.X -= speed;

            if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
                _playerPosition.X += speed;
        }

        private void CheckForGamePadInput()
        {
            GamePadInfo gamePadOne = Input.GamePads[(int)PlayerIndex.One];

            float speed = MOVEMENT_SPEED;

            if (gamePadOne.IsButtonDown(Buttons.A))
            {
                speed *= 1.5f;
                gamePadOne.SetVibration(1.0f, TimeSpan.FromSeconds(1));
            }
            else
            {
                gamePadOne.StopVibration();
            }

            if (gamePadOne.LeftThumbStick != Vector2.Zero)
            {
                _playerPosition.X += gamePadOne.LeftThumbStick.X * speed;
                _playerPosition.Y -= gamePadOne.LeftThumbStick.Y * speed; 
            }
            else
            {
                if (gamePadOne.IsButtonDown(Buttons.DPadUp))
                    _playerPosition.Y -= speed;

                if (gamePadOne.IsButtonDown(Buttons.DPadDown))
                    _playerPosition.Y += speed;
                
                if (gamePadOne.IsButtonDown(Buttons.DPadLeft))
                    _playerPosition.X -= speed;

                if (gamePadOne.IsButtonDown(Buttons.DPadRight))
                    _playerPosition.X += speed;
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _player.Draw(SpriteBatch, _playerPosition);
            _enemy.Draw(SpriteBatch, _enemyPosition);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
