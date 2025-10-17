using System.Collections.Generic;
using System.Linq;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Models;
using NguyenLeHaiAnh_HE180328_Assignment1.DataAccess.Repositories;

namespace NguyenLeHaiAnh_HE180328_Assignment1.BusinessLogic
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;

        public TagService(ITagRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Tag> GetAll()
            => _repo.GetAll();

        public Tag? Get(int id)
            => _repo.Get(id);

        // ✅ Thêm mới tag
        public void Add(Tag tag)
        {
            //if (string.IsNullOrWhiteSpace(tag.TagName))
            //    return;

            //// Nếu đã tồn tại tag trùng tên thì bỏ qua (hoặc bạn có thể kiểm tra riêng ở Controller)
            //var exists = _repo.GetAll().Any(t => t.TagName.Equals(tag.TagName, System.StringComparison.OrdinalIgnoreCase));
            //if (exists) return;

            _repo.Add(tag);
        }

        public (bool Success, string Message) Update(Tag tag)
        {
            var existing = _repo.Get(tag.TagId);
            if (existing == null)
                return (false, "Tag not found.");

            // Nếu tag có bài viết liên kết thì không cho cập nhật (tùy yêu cầu nghiệp vụ)
            if (existing.NewsArticles != null && existing.NewsArticles.Any())
            {
                var articleCount = existing.NewsArticles.Count;
                return (false, $"Cannot update tag. This tag has {articleCount} article(s) associated with it.");
            }

            // Ensure the tag name is not empty or whitespace
            if (string.IsNullOrWhiteSpace(tag.TagName))
                return (false, "Tag name cannot be empty or whitespace.");

            // Check for duplicate tag names (case-insensitive)
            var duplicateExists = _repo.GetAll()
                .Any(t => t.TagId != tag.TagId && t.TagName.Equals(tag.TagName, System.StringComparison.OrdinalIgnoreCase));
            if (duplicateExists)
                return (false, "A tag with the same name already exists.");

            _repo.Update(tag);
            return (true, "Tag updated successfully!");
        }

        public bool Delete(int id)
        {
            var tag = _repo.Get(id);
            if (tag == null) return false;

            if (tag.NewsArticles is not null && tag.NewsArticles.Any())
                return false;

            _repo.Delete(tag);
            return true;
        }

        public IEnumerable<Tag> Search(string? tagName)
        {
            return _repo.Search(tagName);
        }

        public IEnumerable<NewsArticle> GetArticlesByTag(int tagId)
        {
            return _repo.GetArticlesByTag(tagId);
        }
    }
}
