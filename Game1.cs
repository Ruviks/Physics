using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PEngine;
namespace PEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    
    public class Game1 : Game
    {
       static string text = System.IO.File.ReadAllText(@"speed.txt");
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static int N = 2000;
        public int maxspeed = int.Parse(text), minspeed = -int.Parse(text);
        public Texture2D particle;
        public static float Radius;
        public static List<Particle> Plist = new List<Particle>();
        public static float width,height;
        public static float _d = 1;
        public Game1()
        { 
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            width = graphics.PreferredBackBufferWidth;
            height = graphics.PreferredBackBufferHeight;


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
            
            var rand = new Random();
            // TODO: Add your initialization logic here
           
            this.IsMouseVisible = true;
            for (int j = 0; j <= N ; j++)
                {
               
                Plist.Add(new Particle(grid: new Particle[10, 10], velocity: new Vector2(rand.Next(minspeed, maxspeed), rand.Next(minspeed, maxspeed)), position: new Vector2(rand.Next(0, graphics.PreferredBackBufferWidth), rand.Next(0, graphics.PreferredBackBufferHeight))));
             
            }
            



            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            particle = Content.Load<Texture2D>("dot");
            Radius = particle.Height / 2;
            Particle.rad = Radius;
            
            // TODO: use this.Content to load your game content here
        }
        public static void ResolveCollision(  Particle A, Particle B)
        {
          
           

            float overlap =(float)0.5*( Vector2.Distance(A._pos, B._pos) - 2 * Radius) ;

            B._pos.X -= overlap * (B._pos.X - A._pos.X) / Vector2.Distance(A._pos, B._pos);
            B._pos.Y -= overlap * (B._pos.Y - A._pos.Y) / Vector2.Distance(A._pos, B._pos);
            A._pos.X += overlap * (B._pos.X - A._pos.X) / Vector2.Distance(A._pos, B._pos);
            A._pos.Y += overlap * (B._pos.Y - A._pos.Y) / Vector2.Distance(A._pos, B._pos);
            Vector2 normal = (B._pos - A._pos);
            normal.Normalize();

            float kx = (A._vel.X - B._vel.X);
            float ky = (A._vel.Y - B._vel.Y);
            float p =  (normal.X * kx + normal.Y* ky) ;
            A._vel.X = A._vel.X - p * normal.X;
            A._vel.Y = A._vel.Y - p * normal.Y;
            B._vel.X = B._vel.X + p * normal.X;
            B._vel.Y = B._vel.Y + p * normal.Y;
        }
        public static  Dictionary<float,int> Graph(List<Particle> particles)
        {
            Dictionary<float, int>  nv = new Dictionary<float, int>();
            //    float minv = particles.Min(particle => particle._vel.Length());
            float minv = 0;
            float maxv = particles.Max(particle => particle._vel.Length());
            float d = _d;
            int count=0;
            while (minv < maxv)
            {
                foreach (Particle particle in particles)
                {
                    if (particle._vel.Length() >= (minv) && particle._vel.Length() <= (minv+d)) count++;
                }
                nv.Add(minv, count);
                count = 0;
                minv += d;
            }
            return nv;
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        /// 


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

          var  grid = new Spatial((int)width);
            for (int i = 0; i <= N; i++) grid.Add(Plist[i]);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


         
            int MaxX =
       graphics.PreferredBackBufferWidth - particle.Width;
            int MinX = 0;
            int MaxY =
             graphics.PreferredBackBufferHeight - particle.Height;
            int MinY = 0;

            // TODO: Add your update logic here
            //main collision
            /*    for (int i = 0; i < N; i++)
              {
                   for (int j = i+1; j < N; j++)
                       if (Vector2.Distance(Plist[i]._pos, Plist[j]._pos) <= 2 * Radius) ResolveCollision(Plist[i], Plist[j]);

               } */
            for (int i = 0; i <= grid.size; i++)
            {
                for (int j = 0; j <= grid.size; j++)
                {
                    var list = grid.Retrieve(new Vector2(i, j));
                    for(int k=0;k<list.Count;k++)
                    {
                        for (int l = k+1; l < list.Count; l++)
                        {
                            if (Vector2.Distance(list[k]._pos, list[l]._pos) <= 2 * Radius) ResolveCollision(list[k], list[l]);
                        }
                    }

                }
            }


            //wall collision

            for (int i = 0; i <= N; i++)
                 {
                     if (Plist[i]._pos.X > MaxX)
                     {
                         Plist[i]._vel.X *= -1;
                         Plist[i]._pos.X = MaxX;
                     }

                     else if (Plist[i]._pos.X < MinX)
                     {
                         Plist[i]._vel.X *= -1;
                         Plist[i]._pos.X = MinX;
                     }

                     if (Plist[i]._pos.Y > MaxY)
                     {
                         Plist[i]._vel.Y *= -1;
                         Plist[i]._pos.Y = MaxY;
                     }

                     else if (Plist[i]._pos.Y < MinY)
                     {
                         Plist[i]._vel.Y *= -1;
                        Plist[i]._pos.Y = MinY;
                     }
                 }

            //Integration
            for (int i = 0; i <= N; i++)
            {
             
                Plist[i]._pos += Vector2.Multiply(Plist[i]._vel, Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds));
                
            }



         



            base.Update(gameTime);
         
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        { 
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
          for (int i = 0; i <= N; i++)
#pragma warning disable CS0618 // Type or member is obsolete
                spriteBatch.Draw(particle, position:Plist[i]._pos);

            float c = 0;
            using (StreamWriter file = new StreamWriter("file.txt"))
                foreach (var x in Graph(Plist))
            {
                if (x.Value != 0)
                {
                        file.WriteLine("{0},{1}", x.Key, (float)x.Value);

                        Texture2D rect = new Texture2D(graphics.GraphicsDevice, (int)_d, x.Value);

                    Vector2 coor = new Vector2(c, height - x.Value);
                    Color[] data = new Color[(int)_d * x.Value];
                    for (int i = 0; i < data.Length; ++i) data[i] = Color.Blue;
                    rect.SetData(data);
                    spriteBatch.Draw(rect, coor, Color.White);
                    c = c + _d;
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
            int d = 100;
            while (d <= width)
            {
                spriteBatch.DrawLine(0, d, width, d, Color.White);
                spriteBatch.DrawLine(d, 0, d, width, Color.White);
                d = d + 100;
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }

    public class Particle
    {
        public Particle[,] _grid;
       
        public Vector2 _vel;
        public Vector2 _pos;
        public static float rad;
        public Particle(Vector2 velocity, Vector2 position,Particle[,] grid)
        {
            _grid = grid;
            _vel = velocity;
            _pos = position;
        }

     
    }
  
  


}
