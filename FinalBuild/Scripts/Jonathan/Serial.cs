// https://en.wikibooks.org/wiki/C_Sharp_Programming/Namespaces

/* 
 * Full Name: Jonathan Burns
 * Student ID: 2288851
 * Chapman email: jburns@chapman.edu/
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - Serial: a namespace that is imported by any serial scipt
 *  - ISerializable: an object to be serialized 
 *  - ISerialDataStore: the data storage object, respective to each class
 */

namespace Serial
{
    public interface ISerializable
    {
        //     It is recommended to put a '//IM' comment next to each field in a class
        // that isn't tracked by the serialization. This tell us that other fields are
        // saved by the respective ISerialDataStore, or are purposly not saved.

        /*
         * GetCurrentState - called by StateController each frame, unless rewinding
         * Return: ISerialDataStore of object's current state
         */
        ISerialDataStore GetCurrentState();

        /*
         * SetState - triggers an object to try and restore its past state
         * Params:
         *  - ISerialDataStore state - goal state the object should change to
         */
        void SetState(ISerialDataStore state);
    }

    /// Used by any class that is ISerializable to store it's associated variables
    public interface ISerialDataStore
    {
        // Empty for now, only used as an identifier

        // Recommended to call the class implementing this 'Save[Class Name implementing ISerializable]' 
        // And should be internal, for safety
        
        // Example: 

        /*
            internal class SaveCamera : ISerialDataStore
            {
                public float positionX { get; private set; }
                public float positionY { get; private set; }

                public SaveCamera(float posX, float posY)
                {
                    positionX = posX;
                    positionY = posY;
                }
            }
        */
    }
}