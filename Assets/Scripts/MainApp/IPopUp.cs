using DI;

namespace MainApp
{
    public interface IPopUp : IClient
    {
        void Open();
        void Close();
    }
}
