using System;
using System.Collections.Generic;
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
            List<Direction> directions = new List<Direction> {Direction.Above, Direction.Right, Direction.Left};
            int usedVerticeCount;
            int[] result = BlockMesh.GetCulledIndices(directions, out usedVerticeCount);
            
            Assert.AreEqual(result, new []
            {
                2, 3, 6, 6, 3, 7, //Above
                4, 5, 6, 6, 7, 4, //Right
                0, 2, 1, 0, 3, 2 //Left
            });
            
            Assert.AreEqual(usedVerticeCount, 18);
        }

        [Test]
        public void ReplaceBlock_Test()
        {
            Chunk chunk = new Chunk();
            Vector3 coordinate = new Vector3(20, 15, 15);
            
            Assert.AreEqual(chunk.VerifyCoordinate(coordinate), false);
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
            
            Assert.AreEqual(result, new Direction[] {Direction.Left, Direction.Front, Direction.Behind});
        }
        
        [Test] //Testing the corner of the chunk in this case
        public void SidesExposedToAir_Test_Corner()
        {
            Chunk chunk = new Chunk();
            chunk.SetBlock(new Vector3(0, 0, 0), BlockType.Dirt); //base

            var result = chunk.SidesExposedToAir(new Vector3(0, 0, 0));
            
            Assert.AreEqual(result, new Direction[] {Direction.Above, Direction.Right, Direction.Front});
        }

        [Test]
        public void GetAllVertices_Offset_Test()
        {
            //offset 5 blocks to the right, 1 up, 1 in.
            var result = BlockMesh.GetAllVertices(new Vector3(5, 1, 1)); 
            
            Assert.AreEqual(result, new Vector3[]
            {
                new Vector3(4.5f, 0.5f,  0.5f), //Left Bottom Back
                new Vector3(5.5f, 0.5f,  0.5f), //Right Bottom Back
                new Vector3(5.5f, 1.5f,  0.5f), //Right Top Back
                new Vector3(4.5f, 1.5f,  0.5f), //Left Top Back - One face is completed!
                new Vector3(4.5f, 0.5f,  1.5f), //Left Bottom Front
                new Vector3(5.5f, 0.5f,  1.5f), //Right Bottom Front
                new Vector3(5.5f, 1.5f,  1.5f), //Right Top Front
                new Vector3(4.5f, 1.5f,  1.5f), //Left Top Front
            });
        }
        
        [Test]
        public void GetVerticeCount_Test()
        {
            Chunk chunk = new Chunk();
            chunk.FillUpToY(1, BlockType.Dirt);
            
            Assert.AreEqual(chunk.GetVerticeCount(), 9216); //16 blocks * 16 blocks * 36 indices per block.
        }
    }
}