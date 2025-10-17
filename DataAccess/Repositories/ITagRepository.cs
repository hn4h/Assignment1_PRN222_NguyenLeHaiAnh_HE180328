using System.Collections.Generic;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;

namespace NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetAll();
        Tag? Get(int id);
        void Add(Tag tag);
        void Update(Tag tag);
        void Delete(Tag tag);
        IEnumerable<Tag> Search(string? tagName);
        IEnumerable<NewsArticle> GetArticlesByTag(int tagId);
    }
}
