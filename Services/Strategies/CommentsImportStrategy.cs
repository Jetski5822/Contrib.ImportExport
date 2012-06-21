using System.Collections.Generic;
using System.Linq;
using Contrib.ImportExport.Extensions;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Orchard.Comments.Models;
using Orchard.Comments.Services;
using Orchard.ContentManagement;

namespace Contrib.ImportExport.Services.Strategies {
    public class CommentsImportStrategy : IMultipleImportStrategy {
        private readonly ICommentService _commentService;
        private readonly IDataCleaner _dataCleaner;

        public CommentsImportStrategy(ICommentService commentService,
            IDataCleaner dataCleaner) {
            _commentService = commentService;
            _dataCleaner = dataCleaner;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is Comments;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            Comments commentsToImport = (Comments)objectToImport;

            foreach (var commentToImport in commentsToImport.CommentList) {
                var author = (commentToImport.UserName ?? string.Empty).Truncate(255);
                var dateCreated = commentToImport.DateCreated;

                var existingComment = _commentService.GetCommentsForCommentedContent(parentContent.Id)
                    .Where(o => o.Author == author)
                    .List()
                    .Where(o => o.Record.CommentDateUtc.HasValue && o.Record.CommentDateUtc.Value.Equals(dateCreated))
                    .FirstOrDefault();

                if (existingComment != null)
                    return existingComment.ContentItem;

                var context = new CreateCommentContext {
                    Author = author,
                    CommentText = (_dataCleaner.Clean(commentToImport.Content.Value, importSettings) ?? string.Empty).Truncate(10000),
                    Email = (commentToImport.UserEmail ?? string.Empty).Truncate(255),
                    SiteName = (commentToImport.UserURL ?? string.Empty).Truncate(255),
                    CommentedOn = parentContent.Id,
                };

                if (parentContent.As<CommentsPart>().Record.CommentPartRecords == null)
                    parentContent.As<CommentsPart>().Record.CommentPartRecords = new List<CommentPartRecord>();

                var comment = _commentService.CreateComment(context, true);

                comment.Record.CommentDateUtc = dateCreated;
                comment.Record.UserName = (commentToImport.UserName ?? "Anonymous").Truncate(255);

                if (commentToImport.Approved)
                    _commentService.ApproveComment(comment.ContentItem.Id);
            }

            return null;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
        }
    }
}