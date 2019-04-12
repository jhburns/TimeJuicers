// https://en.wikibooks.org/wiki/C_Sharp_Programming/Namespaces
namespace Serial
{
    public interface ISerializable
    {
        ISerialDataStore GetCurrentState();

        void SetState(ISerialDataStore state);
    }

    public interface ISerialDataStore
    {

    }
}