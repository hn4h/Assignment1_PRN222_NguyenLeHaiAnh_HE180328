using System.Collections.Generic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;

namespace NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Repositories
{ 
    public interface IAccountRepository
    {
        IEnumerable<SystemAccount> GetAll(int? role = null);
        SystemAccount? Get(short id);
        SystemAccount? GetByEmail(string email);
        void Add(SystemAccount acc);
        void Update(SystemAccount acc);
        void Delete(SystemAccount acc);
        public short GetNextId();
    }
}