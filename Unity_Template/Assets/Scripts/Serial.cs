// https://en.wikibooks.org/wiki/C_Sharp_Programming/Namespaces
namespace Serial
{
    public interface ISerializable
    {
        /*
         * GetCurrentState
         * Return ISerialDataStore of object's current state
         */
        ISerialDataStore GetCurrentState();

        /*
         * SetState
         * Params:
         *  - ISerialDataStore state - goal state the object should change to
         */
        void SetState(ISerialDataStore state);
    }

    public interface ISerialDataStore
    {
        // Empty for now, only used as an identifier 
    }
}