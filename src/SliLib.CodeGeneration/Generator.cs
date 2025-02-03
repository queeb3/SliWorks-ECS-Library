namespace SliLib.CodeGeneration;


public static class SoAGenerator
{
    /*
        take in struct

        read attributes
            sort attributes by meaning (global, vectorizable)

        store property variables
        store property names
        store struct name

        generate SoA struct source
            generate vector instance for vectorizables
            generate properties for globals
            generate method for adding original struct as parameter
            generate method for removing
            generate method for accessing properties at an index
            return generated code

        create new source file taking in the generated SoA code
    */
}
