using System;
using System.Web.UI.WebControls;
using Subtext.Framework.Configuration;

namespace Subtext.Web.UI.WebControls
{
	/// <summary>
	/// Use this to link to the Atom feed and do not specify the NavigateUrl property. 
	/// It'll fill this in for you.
	/// </summary>
	public class AtomHyperLink : HyperLink
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RssHyperLink"/> class.
		/// </summary>
		public AtomHyperLink() : base()
		{
		}

		/// <summary>
		/// Overrides the NavigateUrl property to point too the RSS feed 
		/// and raises the <see cref="E:System.Web.UI.Control.PreRender"/>
		/// event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			NavigateUrl = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}Atom.aspx", Config.CurrentBlog.RootUrl);
			base.OnPreRender (e);
		}
	}
}
