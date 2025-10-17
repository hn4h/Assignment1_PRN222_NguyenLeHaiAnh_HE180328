using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic { 
    public interface ICategoryService
    {
        IEnumerable<Category> GetAll(bool? active = null);
        Category? Get(short id);
        void Add(Category cat);
        (bool Success, string Message) Update(Category cat);
        bool Delete(short id);
    }
}
