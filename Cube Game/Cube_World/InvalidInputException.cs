using System;

namespace Cube_Game
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException()
        {
        }
        
        public InvalidInputException(string message) : base(message)
        {
        }
    }
}