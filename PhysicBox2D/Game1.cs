using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Definitions;
using Genbox.VelcroPhysics.Collision.Shapes;
using System.Diagnostics;
using Genbox.VelcroPhysics.Utilities;
using System;

namespace PhysicBox2D
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // tạo world để cập nhật toàn bộ body
        private World world;

        private Fixture boxFixture;

        // rigid body
        private Body rigidBody;

        // create shape to demonstrate physic
        private Texture2D rectangleTexture;
        private Rectangle rectangle;

        private float DEGREES_TO_RADIANS = 0.0174532925199432957f;

        private KeyboardState lastKey;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });
            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // tạo gravity cho world;
            Vector2 gravity = new Vector2(0, 400);
            world = new World(gravity);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(10);

            // Định nghĩa body
            // khởi tạo body def
            BodyDef rectangleBodyDef = new BodyDef();

            // vị trí của body def
            // convert Pixel to box2d's world space
            rectangleBodyDef.Position = ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            // mặc định của body def là Static, vì vậy cần set lại
            rectangleBodyDef.Type = BodyType.Dynamic;

            // tạo rigid body từ world để tracking body
            rigidBody = world.CreateBody(rectangleBodyDef);

            // tạo shape cho body
            PolygonShape polygonShape = new PolygonShape(1);

            polygonShape.SetAsBox(ConvertUnits.ToSimUnits(20 / 2), ConvertUnits.ToSimUnits(10 / 2));

            // tạo fixture và attach shape vào body
            // fixture nghĩa là tạo shape, vì có nhiều dạng shape khác nhau nên sẽ có các class để tạo shape
            // nếu không tạo fixture thì sẽ ko có physic.
            boxFixture = rigidBody.CreateFixture(polygonShape);

            // create shape
            rectangle = new Rectangle(new Point(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), new Point(20, 10));
            //rectangleTexture = Content.Load<Texture2D>("Trees");

            createKinematic();
            createStaticBody();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            reset();

            // lấy vị trí của rigid body
            rectangle.X = (int)ConvertUnits.ToDisplayUnits(rigidBody.Position.X) - 20 / 2;
            rectangle.Y = (int)ConvertUnits.ToDisplayUnits(rigidBody.Position.Y) - 10 / 2;

            Debug.WriteLine(rigidBody.Position.ToString());

            // Cập nhật world
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            //_spriteBatch.Draw(rectangleTexture, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), null, Color.White, MathHelper.ToRadians((float)gameTime.TotalGameTime.TotalSeconds * 30 % 360), new Vector2(rectangleTexture.Width / 2, rectangleTexture.Height / 2), .2f, SpriteEffects.None, 0);

            // https://learn.microsoft.com/en-us/previous-versions/windows/xna/bb203919(v=xnagamestudio.10)#sprite-origin
            // If you draw a 50×50 pixel sprite at location (400,200) without specifying an origin,
            // the upper left of the sprite will be on pixel (400,200)
            // If you use the center of the 50×50 sprite as the origin (25,25), to draw the sprite in the same position
            // you MUST add the origin coordinates to the position.
            // In this case, the position is (425,225) and the origin is (25,25)

            rectangle.X = (int) Math.Floor(rectangle.X + 10f);
            rectangle.Y = (int) Math.Floor(rectangle.Y + 5f);
            var rotate = MathHelper.ToRadians(180f % 360) ;
            _spriteBatch.Draw(rectangleTexture, rectangle, null, Color.White, rigidBody.Rotation % MathHelper.TwoPi /* rigidBody.Rotation % MathHelper.TwoPi */,
                         new Vector2(0.5f, 0.5f)
                          , SpriteEffects.None, 0);
            _spriteBatch.Draw(groundTexture, groundForDraw, Color.White);

            platformForDraw.X = (int) Math.Floor(platformForDraw.X + 10f);
            platformForDraw.Y = (int) Math.Floor(platformForDraw.Y + 10f);
            _spriteBatch.Draw(platformTexture, platformForDraw, null, Color.White,
                    platformRigidBody.Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
            _spriteBatch.End();

            platformForDraw.X = (int) Math.Floor(platformForDraw.X - 10f);
            platformForDraw.Y = (int) Math.Floor(platformForDraw.Y - 10f);
            rectangle.X = (int) Math.Floor(rectangle.X - 1f);
            rectangle.Y = (int) Math.Floor(rectangle.Y - 1f);

            base.Draw(gameTime);
        }

        #region GROUND
        private Body groundRigidBody;
        private Rectangle groundForDraw;
        private Texture2D groundTexture;
        private void createStaticBody()
        {
            groundTexture = new Texture2D(GraphicsDevice, 1, 1);
            groundTexture.SetData(new[] { Color.Red });

            BodyDef ground = new BodyDef();
            ground.Position = ConvertUnits.ToSimUnits(0, _graphics.PreferredBackBufferHeight - 30);

            groundRigidBody = world.CreateBody(ground);

            EdgeShape shape = new EdgeShape(Vector2.Zero, ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth, 0));
            //shape.SetTwoSided(Vector2.Zero, ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth, 0));

            // attach shape
            groundRigidBody.CreateFixture(shape);

            // restitution (sự hồi phục của box)
            boxFixture.Restitution = 0.2f;
            boxFixture.Friction = 1f;

            groundForDraw = new Rectangle(ConvertUnits.ToDisplayUnits(ground.Position).ToPoint(), new Point(_graphics.PreferredBackBufferWidth, 5));

        }
        #endregion

        #region PLATFORM
        private Body platformRigidBody;
        private Rectangle platformForDraw;
        private Texture2D platformTexture;

        /// <summary>
        /// Kinematic là vật có thể move nhưng ko chịu tác động bởi thứ khác. Nhưng nó có thể tác động tới dynamic
        /// </summary>
        private void createKinematic()
        {
            Vector2 position = new Vector2(_graphics.PreferredBackBufferWidth / 2 - (30 * (2 - 2)), _graphics.PreferredBackBufferHeight / 2 + 75);
            int width = 20;
            int height = 20;

            platformTexture = new Texture2D(GraphicsDevice, 1, 1);
            platformTexture.SetData(new[] { Color.Blue });

            BodyDef platform = new BodyDef();
            platform.Position = ConvertUnits.ToSimUnits(position.X, position.Y);
            //platform.Angle = DEGREES_TO_RADIANS;
            // set type
            platform.Type = BodyType.Kinematic;

            platformRigidBody = world.CreateBody(platform);
            platformRigidBody.AngularVelocity = 360 * DEGREES_TO_RADIANS;
            //platformRigidBody.Rotation = DEGREES_TO_RADIANS;

            PolygonShape shape = new PolygonShape(1);
            shape.SetAsBox(ConvertUnits.ToSimUnits(width / 2), ConvertUnits.ToSimUnits(height / 2));

            // attach shape
            platformRigidBody.CreateFixture(shape);


            // restitution (sự hồi phục của box)
            boxFixture.Restitution = 0.75f;

            // render từ world's  box2d sang screen
            var point = position.ToPoint();
            point.X -= width / 2;
            point.Y -= height / 2;
            platformForDraw = new Rectangle(point, new Point(width, height));
        }

        #endregion


        private void reset()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space) && lastKey.IsKeyUp(Keys.Space))
            {
                rectangle.X = _graphics.PreferredBackBufferWidth / 2;
                rectangle.Y = _graphics.PreferredBackBufferHeight / 2;
                rigidBody.Position = ConvertUnits.ToSimUnits(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
                rigidBody.AngularVelocity = 0;
                rigidBody.Rotation = 0;
            }
            lastKey = keyboardState;
        }

        
        /// <summary>
        /// Phép transform xoay quanh tọa độ góc. Hiện ko dùng
        /// </summary>
        /// <param name="worldSpace"></param>
        /// <param name="rotate"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public Vector2 RotatePoint(Vector2 worldSpace, float rotate, Vector2 origin)
        {
            // chuyển về local -> xoay -> translate về world
            var wm = Matrix.CreateTranslation(-worldSpace.X - origin.X, -worldSpace.Y - origin.Y,0)//set the reference point to world reference taking origin into account
                     * Matrix.CreateRotationZ(rotate) //rotate
                     * Matrix.CreateTranslation(worldSpace.X, worldSpace.Y, 0); //translate back

            var rp = Vector2.Transform(worldSpace, wm);
            return rp;
        }
    }
}