#region Disclaimer/Info
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Subtext.Framework.Components;
using Subtext.Framework.Data;
using Subtext.Framework.Web;

namespace Subtext.Framework.Syndication
{
    /// <summary>
    /// RssCommentHandler is a proposed extention to the CommentApi. This is still beta/etc.
    /// The Main Rss feed now contains an element for each entry, which will generate a rss feed 
    /// containing the comments for each post.
    /// </summary>
    public class RssCommentHandler : EntryCollectionHandler<FeedbackItem>
    {
        protected Entry ParentEntry;
        protected ICollection<FeedbackItem> Comments;
        ICollection<FeedbackItem> comments;

        public RssCommentHandler(ISubtextContext subtextContext) : base(subtextContext) 
        {
        }

        /// <summary>
        /// Gets the feed entries.
        /// </summary>
        /// <returns></returns>
        protected override ICollection<FeedbackItem> GetFeedEntries()
        {
            if (ParentEntry == null)
            {
                ParentEntry = Cacher.GetEntryFromRequest(false, SubtextContext);
            }

            if (ParentEntry == null)
            {
                // bad news... we couldn't find the entry the request is looking for - return 404.
                HttpHelper.SetFileNotFoundResponse();
            }

            if (ParentEntry != null && Comments == null)
            {
                Comments = Cacher.GetFeedback(ParentEntry, true, SubtextContext);
            }

            return Comments;
        }

        protected virtual CommentRssWriter GetCommentWriter(ICollection<FeedbackItem> comments, Entry entry)
        {
            return new CommentRssWriter(HttpContext.Response.Output, comments, entry, SubtextContext);
        }

        /// <summary>
        /// Builds the feed using delta encoding if it's true.
        /// </summary>
        /// <returns></returns>
        protected override CachedFeed BuildFeed()
        {
            CachedFeed feed;

            comments = GetFeedEntries();
            if (comments == null)
                comments = new List<FeedbackItem>();

            feed = new CachedFeed();
            CommentRssWriter crw = GetCommentWriter(comments, ParentEntry);
            if (comments.Count > 0)
            {
                feed.LastModified = ConvertLastUpdatedDate(comments.Last().DateCreated);
            }
            else
            {
                feed.LastModified = this.ParentEntry.DateCreated;
            }
            feed.Xml = crw.Xml;
            return feed;
        }

        protected override bool IsLocalCacheOK()
        {
            string dt = LastModifiedHeader;
            if (dt != null)
            {
                comments = GetFeedEntries();

                if (comments != null && comments.Count > 0)
                {
                    return DateTime.Compare(DateTime.Parse(dt), ConvertLastUpdatedDate(comments.Last().DateCreated)) == 0;
                }
            }
            return false;
        }

        protected override BaseSyndicationWriter SyndicationWriter
        {
            get
            {
                return new CommentRssWriter(HttpContext.Response.Output, comments, ParentEntry, SubtextContext);
            }
        }

        /// <summary>
        /// Returns true if the feed is the main feed.  False for category feeds and comment feeds.
        /// </summary>
        protected override bool IsMainfeed
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the item created date.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override DateTime GetItemCreatedDate(FeedbackItem item)
        {
            return item.DateCreated;
        }
    }
}

