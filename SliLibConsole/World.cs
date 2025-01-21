using SliLib.Systems;
using SliLib.Query;
using SliLib.Archetypes;

enum ZLevel
{
    Ravine = -1, // impassible
    Water = 0, // passible under conditions
    Ground = 1, // passable
    Mountain = 2, // impassible
    Sky = 3, // for flying creatures
}

struct Grid(int length, int height) // max bounds of a 2d array
{
    public int Length = length;
    public int Height = height;
}

struct Coordinates(int x, int y, ZLevel z = ZLevel.Ground) // position on a grid [x,y,z]
{
    public int X = x;
    public int Y = y;
    public ZLevel Z = z; // sky, ground, mountain, cave, water levels default 1 for on ground
}

struct RenderLevel(int level) // what gets priority in being rendered over another
{
    public int Level = level;
}

struct Collider2D(int boundsX, int boundsY) // collision detection and prevent certain entities from overlapping
{
    public Coordinates Bounds = new(boundsX, boundsY);
    public bool Stackable = false; // if another collider can be on top even if the other is not stackable
    public bool Trigger = false;
}

struct Sprite(char body, int level = 0) // rendering object
{
    public char Body = body;
    public RenderLevel Priority = new(level); // generally always higher than 0
}

struct Tile(Sprite sprite, int x, int y, ZLevel z = ZLevel.Ground) // rendered world object where things can be located on top of
{
    public Sprite Sprite = sprite; // generally want ground to be 0 or -1
    public Coordinates Position = new(x, y, z);
}

struct TileMap(int length, int height, Tile[,]? tiles = null) // a 2d array of tiles a.k.a a mini map
{
    public Grid Grid = new(length, height);
    public Tile[,] TMap = tiles ?? (new Tile[height, length]);
}

struct Map(int length, int height, TileMap[,]? tMap = null) // chunks of tile maps aligned to a grid
{
    public Grid Grid = new(length, height);
    public TileMap[,] TMap = tMap ?? (new TileMap[height, length]);
}

class SWorld : BaseSystem
{
    public override int Priority { get; } = 20;
    public override Query Query { get; } = new Query()
                                                .With<Sprite>()
                                                .With<Collider2D>();

    Map? Map = null;

    public override void Update()
    {
        return;
    }

    public override void TickUpdate()
    {
        CheckCollision();
        UpdateEntityPos();
        Render();
    }

    void Render()
    {
        Map ??= GenerateMap(8, 8);
    }

    void UpdateEntityPos()
    {

    }

    void CheckCollision()
    {

    }

    Map GenerateMap(int length, int height)
    {

    }

    TileMap GenerateTMap()
    {

    }

    Tile Tile_Ground(int x, int y, char obj)
    {
        return new(x, y, obj, 0, ZLevel.Ground);
    }
}

class MapChunks
{
    static int[] c0 = new int[]
    {
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,
0,0,0,0,0,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    };
    static int[] c1 = new int[]
    {
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    };
    static int[] c2 = new int[]
    {
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0,
1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0,
1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,
1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,
1,1,1,1,1,1,1,1,1,0,0,0,1,1,0,0,0,0,0,0,
    };
    static int[] c3 = new int[]
    {
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,
0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,
0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,
0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,1,1,1,0,0,
0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,0,0,
1,1,0,0,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,0,
1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,0,0,
1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,0,0,
1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,0,0,
1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,0,0,0,
1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,
    };
    static int[] c4 = new int[]
    {
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,
0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,
    };
    static int[] c5 = new int[]
    {
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,
1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,
0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,
1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,
1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,
1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,
    };
    static int[] c6 = new int[]
    {
1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,
1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,
1,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    };
    static int[] c7 = new int[]
    {
0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
    };
    static int[] c8 = new int[]
    {
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,
0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,0,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,
    };
    static int[] c9 = new int[]
    {
1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    };
    static int[] c10 = new int[]
    {
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    };
    static int[] c11 = new int[]
    {
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
    };
    static int[] c12 = new int[]
    {
0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,
0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,
0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    };
    static int[] c13 = new int[]
    {
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,
1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,
1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    };
    static int[] c14 = new int[]
    {
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,
1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    };
    static int[] c15 = new int[]
    {
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
1,1,1,1,1,1,1,1,1,0,1,1,1,1,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,0,0,0,1,1,0,0,0,0,0,0,0,
1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,
1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    };
}
/*
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1, 1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0, 1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,
0,0,0,0,0,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0, 0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,
0,0,0,0,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0, 0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,
0,0,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0,0,0, 0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0, 0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,1,1,1,0,0,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,0, 0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,1,1,1,0,0,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1, 1,1,0,0,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,0,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,0,0,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,0,0,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1, 1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,0,0,
0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1, 1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,0,0,1,1,0,0,0,0,0,0, 1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,

0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0, 0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0, 0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0, 0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0, 1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1, 1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1, 1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1, 1,1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,

0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 1,1,1,1,1,1,1,1,0,0,0,0,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0, 0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,1,1,1,1,0,0,0,1,1,1,1,1,0,0,0,0, 0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0, 0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0, 0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0, 0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,
0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,

0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,0,1,1,1,1,0,0,0,0,0,0,
0,0,0,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,0,0,0,1,1,0,0,0,0,0,0,0,
0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1, 1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1, 1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1, 1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,0, 0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1, 1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1, 1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
*/
