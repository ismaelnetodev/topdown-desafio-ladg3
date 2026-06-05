using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
namespace DesafioLADG3
{
    public class Game1 : Core
    {

        private Texture2D _player;

        public Game1() : base("Desafio LADG 3", 1280, 720, false)
        {
           
        }

        protected override void Initialize()
        { 
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _player = Content.Load<Texture2D>("player/spritesheets");
            base.LoadContent();
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(
                texture: _player,
                position: new Vector2(100, 100),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: new Vector2(5.5f, 5.5f),
                effects: SpriteEffects.None,
                layerDepth: 0f
            );

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
