using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IDiffRepository
    {
        Diff GetDiff(int id);
        void PutLeft(int id, Byte[] data);
        void PutRight(int id, Byte[] data);
    }
}
