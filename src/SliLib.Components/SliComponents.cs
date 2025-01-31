namespace SliLib.SliComponents
{
    namespace SliLib.SliComponents.Rendering
    {
        public struct Grid(int height, int length) // kinda just a canvas sorta thing
        {
            public int Height = height;
            public int Length = length;
        }

        public struct Sprite1D // ascii console games
        {
            public char Spr;
            public int RenderLevel; // priority for what gets rendered over another
        }

        public struct Sprite2D
        {
            // gotta figure out how to handle 2d image imports in vulkan using silk.net
        }

        public struct Tile1D // ascii
        {
            public Sprite1D Sprite;
        }

        public struct Tile2D
        {
            public Sprite2D Sprite;
        }

        public struct TileMap1D(int height, int length)
        {
            public Grid Grid = new(height, length);
            public Tile1D[,] Tiles = new Tile1D[height, length];
        }

        public struct TileMap2D(int height, int length)
        {
            public Grid Grid = new(height, length);
            public Tile2D[,] Tiles = new Tile2D[height, length];
        }
    }

    namespace SliLib.SliComponents.Transforms
    {
        public struct Position(float x, float y, float z = 0)
        {
            public float X = x;
            public float Y = y;
            public float Z = z;
        }

        public struct Rotation(float x = 0, float y = 0, float z = 90)
        {
            public float X = x;
            public float Y = y;
            public float Z = z;
        }

        public struct Scale(float x = 1, float y = 1, float z = 1)
        {
            public float X = x;
            public float Y = y;
            public float Z = z;
        }

        public struct Velocity(float x = 0, float y = 0, float z = 0)
        {
            public float X = x;
            public float Y = y;
            public float Z = z;
        }

        public struct Transform // 3d, yea i aint got a clue how to render 3d in vulkan yet so just placeholder
        {
            public Position Posistion;
            public Velocity Velocity;
            public Rotation Rotation;
            public Scale Scale;
        }

        public enum Direction2D
        {
            Left,
            Right,
            Front,
            Back
        }

        public struct Transform2D
        {
            public Position Position;
            public Velocity? Velocity; // not always needed for non moving entities
            public Scale Scale;
            Direction2D Facing;
        }
    }

    namespace SliLib.SliComponents.Collision
    {
        using SliComponents.Transforms;

        public struct Collider1D
        {
            public Transform2D Transform;
        }

        public struct Collider2D
        {
            public Transform2D Transform;
        }

        public struct Collider
        {
            public Transform Transform;
        }

    }
}
