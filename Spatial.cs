using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using System.Text;
namespace PEngine
{
    class Spatial
    {
        int cellsize = 20;

        public int size;
        Dictionary<Vector2, List<Particle>> Cells = new Dictionary<Vector2, List<Particle>>();


        public Spatial(int windowsize)
        {

            this.size = windowsize / cellsize;
            
            for (int i = 0; i <= size; i++)
            {
                for (int j = 0; j <= size; j++)
                {
                    Cells[new Vector2(i, j)] = new List<Particle>();
                }

            }



        }
        public void Add(Particle particle)
        {

            var start = particle._pos / cellsize;
            var end = (particle._pos + new Vector2(Particle.rad, Particle.rad)) / cellsize;
            for (var i = (int)start.X; i <= (int)end.X; i++)
            {
                for (var j = (int)start.Y; j <= (int)end.Y; j++)
                {
                    try
                    {
                        Cells[new Vector2(i, j)].Add(particle);
                    }
                    catch
                    {

                    }
                    
                    }



            }

        }
        public List<Particle> Retrieve(Vector2 cell)
        {

            return Cells[cell];

        }

    }
}
