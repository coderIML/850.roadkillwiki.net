﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Roadkill.Core.Mvc.ViewModels;

namespace Roadkill.Core.Plugins.SpecialPages.BuiltIn
{
	public class RandomPage : SpecialPagePlugin
	{
		private static Random _random = new Random();

		public override string Name
		{
			get { return "Random"; }
		}

		public RandomPage() { }
		public RandomPage(Random random)
		{
			_random = random;
		}

		public override ActionResult GetResult()
		{
			RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
			List<PageViewModel> pages = PageService.AllPages().ToList();

			if (pages.Count == 0)
			{
				routeValueDictionary.Add("controller", "Home");
				routeValueDictionary.Add("action", "Index");		
			}
			else
			{
				int randomIndex = _random.Next(0, pages.Count -1);
				PageViewModel randomPage = pages[randomIndex];

				routeValueDictionary.Add("controller", "Wiki");
				routeValueDictionary.Add("action", "Index");
				routeValueDictionary.Add("id", randomPage.Id);
			}

			return new RedirectToRouteResult(routeValueDictionary);
		}
	}
}
