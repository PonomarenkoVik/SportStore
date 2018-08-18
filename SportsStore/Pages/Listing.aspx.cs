﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SportsStore.Models;
using SportsStore.Models.Repository;
using SportsStore.Pages.Helpers;
using System.Web.Routing;

namespace SportsStore.Pages
{
    public partial class Listing : System.Web.UI.Page
    {
        private readonly Repository _repo = new Repository();
        private int _pageSize = 4;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                int selectedProductId;
                if (int.TryParse(Request.Form["add"], out selectedProductId))
                {
                    Product selectedProduct = _repo.Products
                        .Where(p => p.ProductId == selectedProductId).FirstOrDefault();
                    if (selectedProduct != null)
                    {
                        SessionHelper.GetCart(Session).AddItem(selectedProduct, 1);
                        SessionHelper.Set(Session, SessionKey.RETURN_URL,
                            Request.RawUrl);
                        Response.Redirect(RouteTable.Routes
                            .GetVirtualPath(null, "cart", null).VirtualPath);
                    }
                }
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            return FilterProducts()
                .OrderBy(p => p.ProductId)
                .Skip((CurrentPage - 1) * _pageSize)
                .Take(_pageSize);
        }

        protected int CurrentPage
        {
            get
            {
                int page = GetPageFromRequest();
                return page > MaxPage ? MaxPage : page;
            }
        }

        protected int MaxPage
        {
            get
            {
                int prodCount = FilterProducts().Count();
                return (int)Math.Ceiling((decimal)prodCount / _pageSize);
            }
        }

        private IEnumerable<Product> FilterProducts()
        {
            IEnumerable<Product> products = _repo.Products;
            string currentCategory = (string)RouteData.Values["category"] ??
                                     Request.QueryString["category"];
            return currentCategory == null ? products
                : products.Where(p => p.Category == currentCategory);
        }
        private int GetPageFromRequest()
        {
            int page;
            string reqValue = (string)RouteData.Values["page"] ??
                              Request.QueryString["page"];
            return reqValue != null && int.TryParse(reqValue, out page) ? page : 1;
        }
    }
}