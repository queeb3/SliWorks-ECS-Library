namespace SliLib.ECS;

public class TagsArray // TODO this entire feature lmao
{
    public string[] Tokens { get; private set; }

    public TagsArray(int cap)
    {
        Tokens = new string[cap];
    }
}
