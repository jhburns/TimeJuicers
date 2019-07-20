namespace Serial
{
    public interface ISerializable
    {
        // It is recommended to put a '//IM' comment next to each field in a class extending this
        // that isn't tracked by the serialization. This tell us that other fields are
        // saved by the respective ISerialDataStore, or are purposely not saved.

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
        // Only used as an identifier

        // Recommended to call the class implementing this 'Save[Class Name implementing ISerializable]' 
        // And the class should be 'internal'

    }
}