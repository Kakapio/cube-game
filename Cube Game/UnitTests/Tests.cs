using System;
using System.Collections.Generic;
using System.Linq;
using Cube_Game;
using NUnit.Framework;
using OpenTK;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void GetCulledIndices_Test()
        {
            List<Direction> directions = new List<Direction> {Direction.Above, Direction.East, Direction.West};
            var result = BlockMesh.GetCulledIndices(directions);
            
            Assert.AreEqual(result, new int[]
            {
                0, 1, 2, 2, 3, 0,
                4, 5, 6, 6, 7, 4,
                8, 9, 10, 10, 11, 8
            });
        }

        [Test]
        public void ReplaceBlock_Test()
        {
            Chunk chunk = new Chunk();
            Vector3 coordinate = new Vector3(20, 15, 15);
            
            Assert.AreEqual(false, chunk.VerifyCoordinate(coordinate));
        }

        [Test]
        public void SidesExposedToAir_Test_Middle()
        {
            Chunk chunk = new Chunk();
            chunk.SetBlock(new Vector3(5, 5, 5), BlockType.Dirt); //base
            chunk.SetBlock(new Vector3(6, 5, 5), BlockType.Dirt); //right blocked
            chunk.SetBlock(new Vector3(5, 4, 5), BlockType.Dirt); //below blocked
            chunk.SetBlock(new Vector3(5, 6, 5), BlockType.Dirt); //above blocked

            var result = chunk.SidesExposedToAir(new Vector3(5, 5, 5));
            
            Assert.AreEqual(new Direction[] {Direction.West, Direction.North, Direction.South}, result);
        }
        
        [Test] //Testing the corner of the chunk in this case
        public void SidesExposedToAir_Test_Corner()
        {
            Chunk chunk = new Chunk();
            chunk.SetBlock(new Vector3(0, 0, 0), BlockType.Dirt); //base

            var result = chunk.SidesExposedToAir(new Vector3(0, 0, 0)).ToArray();
            
            Assert.AreEqual(new Direction[] {Direction.Above, Direction.East, Direction.North, Direction.Below, Direction.West, Direction.South}, result);
        }
        
        [Test] //Testing the corner of the chunk in this case
        public void SidesExposedToAir_Block_Surrounded_By_Dirt_Test()
        {
            Chunk chunk = new Chunk();
            chunk.FillUpToY(64, BlockType.Dirt);

            var result = chunk.SidesExposedToAir(new Vector3(8, 8, 8));
            
            Assert.AreEqual(new Direction[] {}, result);
        }

        [Test]
        public void GetAllVertices_Offset_Test()
        {
            //offset 5 blocks to the right, 1 up, 1 in.
            var result = BlockMesh.GetAllVertices(new Vector3(5, 1, 1)); 
            
            Assert.AreEqual(new Vector3[]
            {
                new Vector3(4.5f, 0.5f,  0.5f), //Left Bottom Back
                new Vector3(5.5f, 0.5f,  0.5f), //Right Bottom Back
                new Vector3(5.5f, 1.5f,  0.5f), //Right Top Back
                new Vector3(4.5f, 1.5f,  0.5f), //Left Top Back - One face is completed!
                new Vector3(4.5f, 0.5f,  1.5f), //Left Bottom Front
                new Vector3(5.5f, 0.5f,  1.5f), //Right Bottom Front
                new Vector3(5.5f, 1.5f,  1.5f), //Right Top Front
                new Vector3(4.5f, 1.5f,  1.5f), //Left Top Front
            },result);
        }

        [Test]
        public void GetCulledVertices_Test()
        {
            Chunk chunk = new Chunk();
            
            //Covering left and right of a block.
            chunk.SetBlock(new Vector3(5, 5, 5), BlockType.Dirt);   
            chunk.SetBlock(new Vector3(4, 5, 5), BlockType.Dirt);   
            chunk.SetBlock(new Vector3(6, 5, 5), BlockType.Dirt);

            int usedVertCount = 0;
            var result = BlockMesh.GetCulledVertices(chunk.SidesExposedToAir(new Vector3(5, 5, 5)), out usedVertCount,
                new Vector3(0, 0, 0)).ToList();
            
            Assert.AreEqual(new Vector3[]
            {
                new Vector3(-0.5f, 0.5f,  0.5f), //Left Top Front 7
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front 6
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back 2
                new Vector3(-0.5f, 0.5f,  -0.5f), //Left Top Back 3
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back 0
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back 1
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front 5
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front 4
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back 0
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back 1
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back 2
                new Vector3(-0.5f, 0.5f,  -0.5f), //Left Top Back 3
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front 4
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front 5
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front 6
                new Vector3(-0.5f, 0.5f,  0.5f) //Left Top Front 7
            }, result);
            
            Assert.AreEqual(16, usedVertCount);
        }
    }
}