using System.Threading.Tasks;

namespace WebApi.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}