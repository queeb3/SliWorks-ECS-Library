/*
Psuedo Code:
 - Entity
Create entity
generate unique id
add components to entity ** see Components
allocate entity to unique mask ** see ChunkMask
allocate entity to archetype of matching mask ** see Archetype
set component values
set initial version
return id of entity

 - Component
create new struct
define its properties
register component
generate id
save type
calculate size
determine mask bit position
gather meta info
store meta data into ComponentInfo
store ComponentInfo at bit position
return ComponentInfo

 - Archetype
generated on mask creation ** see ChunkMask
generate id
generate component SoA from mask
allocate chunk for entities and components
set initial version
determine current size
determine position
save instance to ArchetypeInfo
store Archetype at new position
return ArchetypeInfo

 - System
define new system with BaseSystem
add required properties and methods
generate Query of components ** see Query
create logic (Start|Update|TickUpdate)
set any requirements to run (events|conditionals|chains|priority)
register new instance of system


 - ChunkMask
generate empty mask
check ChunkCodes from composition of components upon entity creation
pull or push to a cache of already generated masks
store if not been created before
return ChunkMask

 - Query
input component composition
request mask of composotion
if mask doesnt exist do nothing
else request archetypes containing mask (default)
if specified conditions request archetypes matching conditions
store new query for later requests
update query whenever a version change occurs in is archetypes



Component Info:
 - ChunkCode
 - ID
 - Type
 - Size (bytes)

Entity Info:
 - ID
 - Location (where and which archetype)
 - Version (multithreading)

Archetype Info:
 - ChunkMask (composition)
 - ID
 - Index
 - Version (multithreading)
*/

struct ComponentSoA : Component
{
    public Vector<int> EntityValues = new(4);
    public static bool GlobalValue = false;
    public bool IndividualComponentGlobalValue = true;

    public void SomeAction(int index)
    {
        ref entityX = EntityValues[index];

        entityX = DoSomething;
    }
}

class ComponentArray
{
    private Component component;

    public ref Component Ref() => ref component;
}

struct Entity
{
    public int Id;
    public int Version;
    public ref Archetype CurrentLocation;
}

class EntityRegistry
{
    private Entity[] entities;
    public int Count;
    public int Capacity;
    private int nextId;

    public int Generate() { }
    public void Remove(int id) { }
    public ref Entity Ref(int id) => ref entities[id];
}

class Archetype
{
    public ComponentMask Mask;
    public Dictionary<Type, ComponentArray> Components;
    public HashSet<int> Entities;
    public int Version;

    public ref T GetT<T>() where T : Component => ref (T)Components[typeof(T)].Ref();
}

class ArchetypeRegistry
{
    private Dictionary<ComponentMask, Archetype> archetypes;

    public Archetype Get(ComponentMask mask) => archetypes[mask];
    public Archetype GetOrCreate(ComponentMask mask) { }
}

class Query
{
    private List<Archetype> archetypes;

    public bool Validate(params Archetype[] archs) { }
    public bool Sync(params Archetype[] archs) { }
    public void Process(Action<int, Archeype> action) { }

    public static Query Inc<T>() where T : Component { }
    public static Query Exc<T>() where T : Component { }
}

class System : BaseSystem
{
    public int Priority;
    public int[] WaitForSystemIds;
    Query Q = Query.Inc<Component1>().Inc<Component2>().Inc<Component3>().Exc<Component4>();

    public override Update() => Q.Process((id, arch) => Logic(id, arch));
    public void Logic(int id, Archetype arch) { }
}
