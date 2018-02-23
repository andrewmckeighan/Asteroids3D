using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        private Matrix world = Matrix.CreateTranslation(new Vector3(10, 0, 0));//Where the vertices are in relationship to the whole world.
        private Matrix view = Matrix.CreateLookAt(new Vector3(-10, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);//Puts coordinates into view space. Where vertices are in relation to the viewer.
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);//Where on the screen vertices should appear. Camera control.
        
        Effect effect;
        private Model playerModel;
        private Texture2D playerTexture;
        private Model skyboxModel;
        private Texture2D skyboxTexture;
        private Vector3 playerPosition;
        private Quaternion playerRotation;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //effect = Content.Load<Effect>("effects");
            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            skyboxModel = Content.Load<Model>("skybox");
            skyboxTexture = Content.Load<Texture2D>("skyboxTexture");
            playerTexture = Content.Load<Texture2D>("shipTexture");
            playerModel = Content.Load<Model>("spaceship");
            
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Crimson);
            DrawSkybox(skyboxModel, Matrix.CreateTranslation(new Vector3(0, 0, 0)), view, projection);
            // TODO: Add your drawing code here
            DrawPlayerModel(playerModel, world, view, projection);
            base.Draw(gameTime);
        }

        private void DrawPlayerModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.Texture = playerTexture;
                    effect.TextureEnabled = true;
                }

                mesh.Draw();
            }
        }

        private void DrawSkybox(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateScale(80.0f)*world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.Texture = skyboxTexture;
                    effect.TextureEnabled = true;
                }

                mesh.Draw();
            }
        }
        
        

    }
}
