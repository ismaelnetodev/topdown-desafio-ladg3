using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Gum.GueDeriving;
using MonoGameGum;

namespace DesafioLADG3
{
    public class Game1 : Core
    {

        private AnimatedSprite _player;
        private TextureAtlas _playerAtlas;
        private AnimatedSprite _enemy;

        private Vector2 _playerPosition;
        private const float MOVEMENT_SPEED = 5.0f;

        private Vector2 _enemyPosition;
        private Vector2 _enemyVelocity;

        private const int PLAYER_MAX_HEALTH = 5;
        private int _playerHealth = PLAYER_MAX_HEALTH;

        private static readonly TimeSpan DAMAGE_COOLDOWN_DURATION = TimeSpan.FromSeconds(1);
        private TimeSpan _damageCooldown = TimeSpan.Zero;

        private Panel _hudPanel;
        private RectangleRuntime _healthBarBackground;
        private RectangleRuntime _healthBarFill;
        private TextRuntime _healthText;

        private const float HEALTH_BAR_WIDTH = 200f;
        private const float HEALTH_BAR_HEIGHT = 24f;

        public Game1() : base("Desafio LADG 3", 1280, 720, false)
        {
           
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitizalizeGum();
            CreateHud();
            _enemyPosition = new Vector2(_player.Width + 10, 0);
            AssignRandomEnemyVelocity();
        }

        private void InitizalizeGum()
        {
            GumService.Default.Initialize(this, DefaultVisualsVersion.V3);
            GumService.Default.ContentLoader.XnaContentManager = Core.Content;
        }

        private void CreateHud()
        {
            _hudPanel = new Panel();
            _hudPanel.Dock(Dock.Fill);
            _hudPanel.AddToRoot();

            _healthBarBackground = new RectangleRuntime();
            _healthBarBackground.X = 20;
            _healthBarBackground.Y = 20;
            _healthBarBackground.Width = HEALTH_BAR_WIDTH;
            _healthBarBackground.Height = HEALTH_BAR_HEIGHT;
            _healthBarBackground.FillColor = Color.DarkRed;
            _hudPanel.AddChild(_healthBarBackground);

            _healthBarFill = new RectangleRuntime();
            _healthBarFill.X = 20;
            _healthBarFill.Y = 20;
            _healthBarFill.Width = HEALTH_BAR_WIDTH;
            _healthBarFill.Height = HEALTH_BAR_HEIGHT;
            _healthBarFill.FillColor = Color.Red;
            _hudPanel.AddChild(_healthBarFill);

            _healthText = new TextRuntime();
            _healthText.X = 24;
            _healthText.Y = 22;
            _healthText.Text = $"HP: {_playerHealth}/{PLAYER_MAX_HEALTH}";
            _hudPanel.AddChild(_healthText);

            UpdateHealthUi();
        }

        private void UpdateHealthUi()
        {
            float healthPercent = _playerHealth / (float)PLAYER_MAX_HEALTH;

            _healthBarFill.Width = HEALTH_BAR_WIDTH * healthPercent;
            _healthText.Text = $"HP: {_playerHealth}/{PLAYER_MAX_HEALTH}";
        }

        protected override void LoadContent()
        {
            //TextureAtlas playerTextureAtlas = TextureAtlas.FromFile(Content, "player/player_definition.xml");
            _playerAtlas = TextureAtlas.FromFile(Content, "player/player_definition.xml");
            TextureAtlas enemyTextureAtlas = TextureAtlas.FromFile(Content, "enemy/enemy_definition.xml");

            _player = _playerAtlas.CreateAnimatedSprite("walk_down");
            _player.Scale = new Vector2(2.5f, 2.5f);

            _enemy = enemyTextureAtlas.CreateAnimatedSprite("enemy");
            _enemy.Scale = new Vector2(3.0f, 3.0f);


            base.LoadContent();
        }

        private void SetPlayerAnimation(string name, bool flipH)
        {
            if (_player.Animation == _playerAtlas.GetAnimation(name)) return;
            _player.Animation = _playerAtlas.GetAnimation(name);

            _player.Effects = flipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);
            _enemy.Update(gameTime);

            if (_damageCooldown > TimeSpan.Zero) 
                _damageCooldown -= gameTime.ElapsedGameTime;

            CheckForKeyboardInput();
            CheckForGamePadInput();

            Rectangle screenBounds = new Rectangle(
                0,
                0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            );

            Circle playerBounds = GetPlayerBounds();

            if (playerBounds.Left < screenBounds.Left)
                _playerPosition.X = screenBounds.Left;
            else if (playerBounds.Right > screenBounds.Right)
                _playerPosition.X = screenBounds.Right - _player.Width;

            if (playerBounds.Top < screenBounds.Top)
                _playerPosition.Y = screenBounds.Top;
            else if (playerBounds.Bottom > screenBounds.Bottom)
                _playerPosition.Y = screenBounds.Bottom - _player.Height;

            playerBounds = GetPlayerBounds();

            Vector2 newEnemyPosition = _enemyPosition + _enemyVelocity;

            Circle enemyBounds = GetEnemyBounds(newEnemyPosition);

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
                newEnemyPosition.Y = screenBounds.Bottom - _enemy.Height;
            }

            if (normal != Vector2.Zero)
            {
                normal.Normalize();
                _enemyVelocity = Vector2.Reflect(_enemyVelocity, normal);
            }

            _enemyPosition = newEnemyPosition;

            enemyBounds = GetEnemyBounds(_enemyPosition);

            if (playerBounds.Intersects(enemyBounds) && _damageCooldown <= TimeSpan.Zero)
            {
                _playerHealth--;

                if (_playerHealth < 0)
                    _playerHealth = 0;

                UpdateHealthUi();

                _damageCooldown = DAMAGE_COOLDOWN_DURATION;

                RespawnEnemy(screenBounds);
            }

            GumService.Default.Update(gameTime);

            base.Update(gameTime);
        }

        private void RespawnEnemy(Rectangle screenBounds)
        {
            int totalColumns = screenBounds.Width / (int)_enemy.Width;
            int totalRows = screenBounds.Height / (int)_enemy.Height;

            int column = Random.Shared.Next(0, totalColumns);
            int row = Random.Shared.Next(0, totalRows);

            _enemyPosition = new Vector2(column * _enemy.Width, row * _enemy.Height);

            AssignRandomEnemyVelocity();
        }

        private Circle GetPlayerBounds()
        {
            return new Circle(
                (int)(_playerPosition.X + (_player.Width * 0.5f)),
                (int)(_playerPosition.Y + (_player.Height * 0.5f)),
                (int)(_player.Width * 0.5f)
            );
        }

        private Circle GetEnemyBounds(Vector2 position)
        {
            return new Circle(
                (int)(position.X + (_enemy.Width * 0.5f)),
                (int)(position.Y + (_enemy.Height * 0.5f)),
                (int)(_enemy.Width * 0.5f)
            );
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

            var dir = Vector2.Zero;

            if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up)) dir.Y -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down)) dir.Y += 1;
            if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left)) dir.X -= 1;
            if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right)) dir.X += 1;

            if (dir != Vector2.Zero)
            {
                _playerPosition += Vector2.Normalize(dir) * speed;

                var (animName, flip) = (dir.X, dir.Y) switch
                {
                    (0, 1) => ("walk_down", false),
                    (0, -1) => ("walk_up", false),
                    (-1, 0) => ("walk_left", false),
                    (1, 0) => ("walk_left", true),
                    (-1, 1) => ("walk_diag_down_left", false),
                    (1, 1) => ("walk_diag_down_left", true),
                    (-1, -1) => ("walk_diag_up_left", false),
                    (1, -1) => ("walk_diag_up_left", true),
                    _ => ("walk_down", false)
                };

                SetPlayerAnimation(animName, flip);
            }
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

            GumService.Default.Draw();

            base.Draw(gameTime);
        }
    }
}
