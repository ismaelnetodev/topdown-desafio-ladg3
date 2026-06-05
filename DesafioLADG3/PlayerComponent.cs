using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioLADG3
{
    internal class PlayerComponent : DrawableGameComponent
    {

        private Vector2 position;
        private float speed;
        private Texture2D currentTexture;
        private string texturePath;
        private SpriteBatch spriteBatch;

        public PlayerComponent(Game game, SpriteBatch spriteBatch, Vector2 initialPosition) : base(game)
        {
            this.texturePath = "player/idle/idle_0";
            this.position = initialPosition;
            this.spriteBatch = spriteBatch;

            this.speed = 160f;
        }

        protected override void LoadContent()
        {
            currentTexture = Game.Content.Load<Texture2D>(texturePath);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (currentTexture == null)
                return; 

            spriteBatch.Draw(currentTexture, position, Color.White);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                direction.X += 1;
            if (keyboardState.IsKeyDown(Keys.W))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S))
                direction.Y += 1;

            if (direction != Vector2.Zero)
                direction.Normalize();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            position += direction * speed * deltaTime;

            base.Update(gameTime);
        }
    }
}
