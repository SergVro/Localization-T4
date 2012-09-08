


using EPiServer.Framework.Localization;
namespace Playground.Resources
{
	public static class @Resources
	{
		///<summary>
		/// <para>en: English</para>
		///</summary>
		public static string @en{ get { return LocalizationService.Current.GetString("/en"); } }
		///<summary>
		/// <para>en: hour</para>
		///</summary>
		public static string @hour{ get { return LocalizationService.Current.GetString("/hour"); } }
		///<summary>
		/// <para>en: January</para>
		///</summary>
		public static string @january{ get { return LocalizationService.Current.GetString("/january"); } }
		///<summary>
		/// <para>en: July</para>
		///</summary>
		public static string @july{ get { return LocalizationService.Current.GetString("/july"); } }
		///<summary>
		/// <para>en: June</para>
		///</summary>
		public static string @june{ get { return LocalizationService.Current.GetString("/june"); } }
		///<summary>
		/// <para>en: March</para>
		///</summary>
		public static string @march{ get { return LocalizationService.Current.GetString("/march"); } }
		///<summary>
		/// <para>en: May</para>
		///</summary>
		public static string @may{ get { return LocalizationService.Current.GetString("/may"); } }
		///<summary>
		/// <para>en: minute</para>
		///</summary>
		public static string @minute{ get { return LocalizationService.Current.GetString("/minute"); } }
		///<summary>
		/// <para>en: Monday</para>
		///</summary>
		public static string @monday{ get { return LocalizationService.Current.GetString("/monday"); } }
		///<summary>
		/// <para>en: month</para>
		///</summary>
		public static string @month{ get { return LocalizationService.Current.GetString("/month"); } }
		///<summary>
		/// <para>en: November</para>
		///</summary>
		public static string @november{ get { return LocalizationService.Current.GetString("/november"); } }
		///<summary>
		/// <para>en: October</para>
		///</summary>
		public static string @october{ get { return LocalizationService.Current.GetString("/october"); } }
		///<summary>
		/// <para>en: Tuesday</para>
		///</summary>
		public static string @tuesday{ get { return LocalizationService.Current.GetString("/tuesday"); } }
		public static class @exceptionmanager
		{
			public static class @errorform
			{
				///<summary>
				/// <para>en: If you have found anything that doesn’t work, let us know so we can correct the error that may be causing the problem (don’t forget to describe what you did to generate the error).</para>
				///</summary>
				public static string @description{ get { return LocalizationService.Current.GetString("/exceptionmanager/errorform/description"); } }
				///<summary>
				/// <para>en: Your message has now been sent. We will be examining your error description and taking action to correct it.</para>
				///</summary>
				public static string @messagesentdescription{ get { return LocalizationService.Current.GetString("/exceptionmanager/errorform/messagesentdescription"); } }
				///<summary>
				/// <para>en: Message Sent</para>
				///</summary>
				public static string @messagesenttitle{ get { return LocalizationService.Current.GetString("/exceptionmanager/errorform/messagesenttitle"); } }
				///<summary>
				/// <para>en: Send Message</para>
				///</summary>
				public static string @sendbutton{ get { return LocalizationService.Current.GetString("/exceptionmanager/errorform/sendbutton"); } }
			}
			public static class @exceptions
			{
				public static class @exception
				{
					public static class @EPiServer_Core_AccessDeniedException
					{
						///<summary>
						/// <para>en: You do not have sufficient rights to view this page.</para>
						///</summary>
						public static string @description{ get { return LocalizationService.Current.GetString("/exceptionmanager/exceptions/exception[@type='EPiServer.Core.AccessDeniedException']/description"); } }
						///<summary>
						/// <para>en: Access denied</para>
						///</summary>
						public static string @title{ get { return LocalizationService.Current.GetString("/exceptionmanager/exceptions/exception[@type='EPiServer.Core.AccessDeniedException']/title"); } }
					}
					public static class @EPiServer_Core_PageNotFoundException
					{
						///<summary>
						/// <para>en: The link you gave does not work, either because the page it points to has been deleted or moved. If you clicked on a link, please inform the site’s webmaster that the link is faulty.</para>
						///</summary>
						public static string @description{ get { return LocalizationService.Current.GetString("/exceptionmanager/exceptions/exception[@type='EPiServer.Core.PageNotFoundException']/description"); } }
						///<summary>
						/// <para>en: Page does not exist</para>
						///</summary>
						public static string @title{ get { return LocalizationService.Current.GetString("/exceptionmanager/exceptions/exception[@type='EPiServer.Core.PageNotFoundException']/title"); } }
					}
				}
			}
			public static class @httperrors
			{
				public static class @status
				{
					public static class @_404
					{
						///<summary>
						/// <para>en: The link you gave does not work, either because the page it points to has been deleted or moved. If you clicked on a link, please inform the site’s webmaster that the link is faulty.</para>
						///</summary>
						public static string @description{ get { return LocalizationService.Current.GetString("/exceptionmanager/httperrors/status[@code='404']/description"); } }
						///<summary>
						/// <para>en: Incorrect link</para>
						///</summary>
						public static string @title{ get { return LocalizationService.Current.GetString("/exceptionmanager/httperrors/status[@code='404']/title"); } }
					}
					public static class @_500
					{
						///<summary>
						/// <para>en: The link you specified does not work. This may either be the result of temporary maintenance or an incorrect link.</para>
						///</summary>
						public static string @description{ get { return LocalizationService.Current.GetString("/exceptionmanager/httperrors/status[@code='500']/description"); } }
						///<summary>
						/// <para>en: Page could not be loaded</para>
						///</summary>
						public static string @title{ get { return LocalizationService.Current.GetString("/exceptionmanager/httperrors/status[@code='500']/title"); } }
					}
				}
			}
		}
	}
}
// Code generation time (msec):18
