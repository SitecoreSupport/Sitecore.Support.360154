using System;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.XA.Foundation.IoC;
using Sitecore.XA.Foundation.SitecoreExtensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;

namespace Sitecore.Support.XA.Foundation.Multisite.EventHandlers
{
    public class SiteCacheClearer : Sitecore.XA.Foundation.Multisite.EventHandlers.SiteCacheClearer
    {
        public new void OnItemMoved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)args, nameof(args));
            Item resultItem = Event.ExtractParameter(args, 0) as Item;
            ID oldParentId = Event.ExtractParameter(args, 1) as ID;
            OnItemMoved(sender, resultItem, oldParentId);
        }

        public new void OnItemMovedRemote(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));
            Assert.ArgumentNotNull((object)args, nameof(args));
            ItemMovedRemoteEventArgs movedRemoteEventArgs = args as ItemMovedRemoteEventArgs;
            if (movedRemoteEventArgs == null)
                return;
            OnItemMoved(sender, movedRemoteEventArgs.Item, movedRemoteEventArgs.OldParentId);
        }

        protected new void OnItemMoved(object sender, Item resultItem, ID oldParentId)
        {
            if (resultItem == null || StartPathNotChanged(resultItem, GetOldPath(oldParentId, resultItem)) || JobsHelper.IsPublishing())
                return;
            ClearCache(sender);
        }

        protected new string GetOldPath(ID oldParentId, Item resultItem)
        {
            Item obj = ServiceLocator.Current.Resolve<IContentRepository>().GetItem(oldParentId);
            if (obj == null)
                return string.Empty;
            if (resultItem == null)
                return obj.Paths.Path;
            return string.Format("{0}/{1}", (object)obj.Paths.Path, (object)resultItem.Name);
        }
    }
}