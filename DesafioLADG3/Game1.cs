using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
namespace DesafioLADG3
{
    public class Game1 : Core
    {

        private AnimatedSprite _player;
        private AnimatedSprite _enemy;

        public Game1() : base("Desafio LADG 3", 1280, 720, false)
        {
           
        }

        protected override void Initialize()
        { 
            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextureAtlas playerTextureAtlas = TextureAtlas.FromFile(Content, "player/player_definition.xml");
            TextureAtlas enemyTextureAtlas = TextureAtlas.FromFile(Content, "enemy/enemy_definition.xml");

            _player = playerTextureAtlas.CreateAnimatedSprite("player");
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _player.Draw(SpriteBatch, Vector2.Zero);
            _enemy.Draw(SpriteBatch, new Vector2(_player.Width + 10, 0));

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
