using DataAccessLayer.Entities;

namespace DataAccessLayer.Services
{
    public interface IWriteOnly
    {
        void Submit(ref long value);
        void Submit(WriteValue writeValue);
        void Rebind(long value);
    }
}
