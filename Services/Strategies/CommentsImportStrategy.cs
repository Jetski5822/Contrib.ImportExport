using System.Collections.Generic;
using System.Linq;
using Contrib.ImportExport.Extensions;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Orchard;
using Orchard.Comments.Models;
using Orchard.Comments.Services;
using Orchard.ContentManagement;

namespace Contrib.ImportExport.Services.Strategies {
    public class CommentsImportStrategy : IMultipleImportStrategy {
        private readonly ICommentService _commentService;
        private readonly IDataCleaner _dataCleaner;
        private readonly IOrchardServices _orchardServices;

        public CommentsImportStrategy(ICommentService commentService,
            IDataCleaner dataCleaner,
            IOrchardServices orchardServices) {
            _commentService = commentService;
            _dataCleaner = dataCleaner;
            _orchardServices = orchardServices;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is Comments;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            Comments commentsToImport = (Comments)objectToImport;

            foreach (var commentToImport in commentsToImport.CommentList) {
                var author = (commentToImport.UserName ?? string.Empty).Truncate(255);
                var dateCreated = commentToImport.DateCreated;

                var comment = _commentService.GetCommentsForCommentedContent(parentContent.Id)
                    .Where(o => o.Author == author)
                    .List()
                    .FirstOrDefault(o => o.Record.CommentDateUtc.HasValue && o.Record.CommentDateUtc.Value.Equals(dateCreated));

                if (comment != null)
                    return comment.ContentItem;
                else
                    comment = _orchardServices.ContentManager.New<CommentPart>("Comment");

                comment.Author = author;
                comment.CommentText = (_dataCleaner.Clean(commentToImport.Content.Value, importSettings) ?? string.Empty).Truncate(10000);
                comment.Email = (commentToImport.UserEmail ?? string.Empty).Truncate(255);
                comment.SiteName = (commentToImport.UserURL ?? string.Empty).Truncate(255);
                comment.CommentedOn = parentContent.Id;
                comment.CommentDateUtc = dateCreated;
                comment.UserName = (commentToImport.UserName ?? "Anonymous").Truncate(255);

                if (parentContent.As<CommentsPart>().Record.CommentPartRecords == null)
                    parentContent.As<CommentsPart>().Record.CommentPartRecords = new List<CommentPartRecord>();

                _orchardServices.ContentManager.Create(comment);

                if (commentToImport.Approved)
                    _commentService.ApproveComment(comment.Id);
            }

            return null;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
        }
    }
}