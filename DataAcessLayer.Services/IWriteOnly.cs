
namespace DataAccessLayer.Services
{
    public interface IWriteOnly
    {
        void Submit(long value);
        void Rebind(long value);
    }
}
