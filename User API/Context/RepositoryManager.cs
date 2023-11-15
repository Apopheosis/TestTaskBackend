using Repository.Interfaces;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repContext;

    private RepositoryManager(RepositoryContext repContext)
    {
        _repContext = repContext;
    }

    public void Save()
    {
        _repContext.SaveChanges();
    }
}