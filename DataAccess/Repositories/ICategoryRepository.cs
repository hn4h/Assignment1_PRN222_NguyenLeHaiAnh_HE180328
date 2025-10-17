using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;

namespace NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll(bool? active = null);
        Category? Get(short id);
        void Add(Category cat);
        void Update(Category cat);
        void Delete(Category cat);
        bool HasNews(short categoryId);



    }
}
