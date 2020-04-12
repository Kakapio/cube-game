namespace Cube_Game
{
    public class Program
    {
        static void Main()
        {
            using (Game game = new Game(1280, 720, "Kakapo Terrain Generator"))
            {
                game.Run();
            }
        }
    }
}
