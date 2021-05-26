using DataCatalog.Data.Model;
using System.Collections.Generic;
using System.Net;

namespace DataCatalog.Api.Data.Response
{
    public class CategoryResponse : BaseResponse
    {
        public Category Category { get; protected set; }
        public IEnumerable<Category> Categories { get; protected set; }

        public CategoryResponse(bool success, string message, Category category) :base(success, message)
        {
            Category = category;
        }

        public CategoryResponse(bool success, string message, IEnumerable<Category> categories) : base(success, message)
        {
            Categories = categories;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="category">Category.</param>
        /// <returns>Response.</returns>
        public CategoryResponse(Category category) : this(true, string.Empty, category)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="categories">Categories.</param>
        /// <returns>Response.</returns>
        public CategoryResponse(IEnumerable<Category> categories) : this(true, string.Empty, categories)
        { }

        /// <summary>
        /// Creates an error response.
        /// </summary>
        /// <param name="httpStatusCode">The http status code</param>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public CategoryResponse(HttpStatusCode httpStatusCode, string message) : base(false, message, httpStatusCode)
        { }
    }
}
