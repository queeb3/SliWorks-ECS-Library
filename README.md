# SliLib ECS Framework

## Overview

Hey there! I’m the developer of SliLib, and this project started as a crazy idea that got out of hand—in the best way possible.
I wanted something that would make my game dev life easier, and over time it turned into this full-blown ECS framework.
It’s not just some random system; it’s my solution to managing game logic and data in a clean, modular way.

SliLib is my first major coding project, and it’s been one hell of a ride. I didn’t start with the goal of making an ECS.
I just wanted a reusable framework for my games, but then I fell into the rabbit hole of learning low-level stuff like architecture, data structures, and how game engines tick.
Now, here we are—a working ECS that’s mine. It’s simple, flexible, and honestly, it just works how I want it to.

## Key Features

- **Modular ECS Architecture**: Entities, components, and systems are completely separate, so you’re not stuck with spaghetti code when you make changes.
- **Archetype System**: Think of this as grouping entities by what they have in common. It keeps things fast and organized.
- **Component Management**: Components are easy to add, update, and remove. No bloat, just straightforward handling.
- **Entity Lifecycle Management**: Central APIs for creating, deleting, and tagging entities. It’s smooth and saves you from manual headaches.
- **Configurable Core**: You can tweak the system to fit your game without breaking a sweat.

## Directory Structure

### Core API

- **Config.cs**:

  1. This file manages global settings for the ECS, making it the backbone of system-wide configurations.
  2. It provides centralized control over parameters like tick rates and debug options.
  3. It ensures developers can easily tweak runtime behavior without diving deep into the code.

- **Nexus.cs**:

  1. The Nexus is the heart of the ECS, coordinating entities, components, and systems.
  2. It handles system lifecycle management, ensuring smooth execution order.
  3. It centralizes the creation and registration of entities, improving performance and organization.

- **Time.cs**:
  1. This file manages global time and tick-based updates, providing consistent logic execution.
  2. I designed it to ensure smooth delta-time calculations across all systems.
  3. It supports adjustable tick rates, making it flexible for various game genres.

### Data Management

#### Archetypes

- **ArchettypeRegistry.cs**:

  1. This registry tracks all archetypes in the ECS, ensuring efficient grouping.
  2. It handles dynamic archetype creation and removal, allowing for flexibility.
  3. It ensures fast lookups, which is vital for performance.

- **Archetype.cs**:

  1. Archetypes define the shared structure of entities with similar components.
  2. They optimize memory allocation by grouping entities efficiently.
  3. They’re key to enabling performant queries.

- **BoxedEntity.cs**:

  1. BoxedEntity encapsulates entity data for safety and flexibility.
  2. It abstracts away direct references, reducing potential bugs.
  3. It’s also designed to simplify serialization and deserialization.

- **Query.cs**:
  1. The Query class allows developers to select entities based on their components.
  2. It optimizes searches by leveraging archetypes.
  3. I designed it to support chaining for advanced query building.

#### Components

- **ComponentArray.cs**:

  1. This class stores and manages collections of components efficiently.
  2. It’s built to minimize memory fragmentation through preallocation.
  3. It supports dynamic resizing for component addition.

- **ComponentData.cs**:
  1. Represents the raw structure of individual components.
  2. Encodes metadata to integrate smoothly with the ECS.
  3. Allows extension for system interoperability.

#### Entities

- **EntityArray.cs**:

  1. Handles collections of entities for quick iteration.
  2. Supports dynamic addition and removal without impacting performance.
  3. Works seamlessly with the archetype system for faster lookups.

- **EntityManager.cs**:

  1. The EntityManager oversees the creation, destruction, and management of entities.
  2. Provides APIs to add, remove, and query components efficiently.
  3. It’s designed to handle large numbers of entities without performance drops.

- **TagsArray.cs**:
  1. Implements a tagging system for entities, making filtering simple.
  2. Optimized for fast lookups, even with large tag sets.
  3. Supports runtime addition and removal of tags.

#### Systems

- **BaseSystem.cs**:

  1. A base class for all ECS systems, providing a consistent update loop.
  2. Simplifies custom system creation by enforcing best practices.
  3. Supports extensibility for complex behaviors.

- **SystemsList.cs**:
  1. Organizes active systems and their execution order.
  2. Supports prioritization and dependency resolution.
  3. Provides hooks for system registration and lifecycle management.

### Chunks

- **Chunk.cs**:

  1. Represents chunks of grouped entity data for efficient processing.
  2. Supports serialization for debugging and persistence.
  3. Facilitates bulk operations on related entities.

- **ChunkCode.cs**:

  1. Contains utility functions for processing chunks.
  2. Handles splitting and merging operations for performance.
  3. Includes tools for debugging chunk data.

- **ChunkMask.cs**:
  1. Implements a bitmasking system for chunk filtering.
  2. Ensures operations are limited to relevant data.
  3. Works with archetypes to boost query performance.

## Getting Started

1. Clone my repository:
   ```bash
   git clone <repository_url>
   ```
2. Open the solution in your IDE (I recommend Visual Studio).
3. Explore the `Core API`, `Data Management`, and `Chunks` folders to understand the framework.
4. Create your own systems and components to extend it for your game.

## Example Usage

```csharp
// What is required for everything below to work
using SliLib;
using SliLib.Query;
using SliLib.Archetypes;
using SliLib.Systems;

// Example pipeline for setting up usage through nexus singleton
class Program
{
    static void Main()
    {
        Nexus.Entity.Reg<Health>(1).Reg<Damage>(2);
        Nexus.Systems.Add([new SHp()]);
        Nexus.Config.TickIntervals = 1f;
        Nexus.Config.FrameRateCap = 1f;
        Nexus.Self.Run();
    }
}

// Showcase of how to factory generate entities
static class EntityCreator
{
    public static int Hp()
    {
        return Nexus.Entity
            .StageComp<Health>(new(100f, 1f))
            .CreateEnt();
    }
    public static int HpDmg()
    {
        return Nexus.Entity
            .StageComp<Health>(new(100f, 1f))
            .StageComp<Damage>(new(5f))
            .CreateEnt();
    }
}

// Basic component examples
struct Health(float max, float regenTimer = 3f)
{
    public float Max = max;
    public float Current = max;
    public float RegenWait = regenTimer;
}

struct Damage(float amount)
{
    public float Amount = amount;
}

// System creation and usage
class SHp : BaseSystem
{
    public override int Priority { get; } = 5;
    public override Query Query { get; } = new Query().With<Health>();

    public float timer = 0f;

    public override void Update()
    {
        // query.Execute((arch, index) => Regen(arch, index));
    }

    public override void TickUpdate()
    {
        timer += Time.TickInterval;

        Query.Execute((arch, index) => Regen(arch, index));
    }

    void Regen(Archetype arch, int index)
    {
        ref Health hp = ref arch.GetComponent<Health>(index);

        if (Time.IsTickFrame && timer % hp.RegenWait == 1)
        {
            hp.Current = Math.Min(hp.Max, hp.Current + 0.5f);
            Console.WriteLine($"Archetype {arch.Id} Entity {index}: Health {hp.Current}/{hp.Max}");
        }
    }
}

```

## Future Improvements

- Add multi-threading support for better scalability.
- Implement advanced profiling and debugging tools.
- Optimize queries with indexing and caching.
- Expand documentation and include tutorials.

## License

This project is licensed under the AGPL License.

---

Let me know what you think or if you'd like to collaborate!
