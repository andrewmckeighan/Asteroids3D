using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

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
        public static Vector3 campos;
        public static Matrix world = Matrix.CreateTranslation(new Vector3(10, 0, 0));//Where the vertices are in relationship to the whole world.
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);//Puts coordinates into view space. Where vertices are in relation to the viewer.
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 1000f);//Where on the screen vertices should appear. Camera control.
        
        Effect effect;
        private Model playerModel;
        private Texture2D playerTexture;
        private Model skyboxModel;
        private Texture2D skyboxTexture;
        private Vector3 playerPosition = new Vector3(0,0,0);
        private Quaternion playerRotation = Quaternion.Identity;
        float gameSpeed = 1.0f;


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
            Services.AddService<Space>(new Space());
            new Cell(this, new Vector3(-2, 0, -5), 2, new Vector3(0.2f, 0, 0), new Vector3( 0.3f, 0.5f, 0.5f));
            new Cell(this, new Vector3(2, 0, -5), 3, new Vector3(-0.2f, 0, 0), new Vector3( -0.5f, -0.6f, 0.2f));
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

            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 80.0f * gameSpeed;
            
            ProcessKeyboard(gameTime, ref playerPosition);
            
            MoveForward(ref playerPosition, playerRotation, moveSpeed);
            UpdateCamera();
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
            DrawSkybox(skyboxModel, view, projection);
            // TODO: Add your drawing code here
            DrawPlayerModel(playerModel, view, projection);
            base.Draw(gameTime);
        }

        private void DrawPlayerModel(Model model, Matrix view, Matrix projection)
        {
            Matrix worldMatrix = Matrix.CreateScale(0.01f, 0.01f, 0.01f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateRotationZ(MathHelper.Pi/2) * Matrix.CreateFromQuaternion(playerRotation) * Matrix.CreateTranslation(playerPosition);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.Texture = playerTexture;
                    effect.TextureEnabled = true;
                }

                mesh.Draw();
            }
        }

        private void DrawSkybox(Model model,  Matrix view, Matrix projection)
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

        //help from http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Flight_kinematics.php
        private void ProcessKeyboard(GameTime gameTime, ref Vector3 playerPosition)
        {
            float leftRightRot = 0;
            float upDownRot = 0;

            float turningSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2000.0f;
            turningSpeed *= 1.6f * gameSpeed;
            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.A))
                leftRightRot += turningSpeed;
            if (keys.IsKeyDown(Keys.D))
                leftRightRot -= turningSpeed;
         
            if (keys.IsKeyDown(Keys.W))
                upDownRot += turningSpeed;
            if (keys.IsKeyDown(Keys.S))
                upDownRot -= turningSpeed;

            

            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot);
            playerRotation *= additionalRot;
        }
        private void MoveForward(ref Vector3 position, Quaternion rotationQuat, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), rotationQuat);
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.LeftShift))
                position -= addVector * speed;
            if (keys.IsKeyDown(Keys.LeftControl))
                position += addVector * speed;


        }
        private void UpdateCamera()
        {
            campos = new Vector3(0, 0.02f, -0.2f);
            campos = Vector3.Transform(campos, Matrix.CreateFromQuaternion(playerRotation));
            campos += playerPosition;

            Vector3 camup = new Vector3(0, 1, 0);
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(playerRotation));

            view = Matrix.CreateLookAt(campos, playerPosition, camup);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);
        }

    }
}
