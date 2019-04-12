// https://en.wikibooks.org/wiki/C_Sharp_Programming/Namespaces
namespace Serial
{
    public interface Serializable
    {
        SerialDataStore GetCurrentState();

        void SetState(SerialDataStore state);
    }

    public interface SerialDataStore
    {

    }
}