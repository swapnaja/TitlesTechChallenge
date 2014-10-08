using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TitlesTechChallenge.Models;
using PagedList;

namespace TitlesTechChallenge.Controllers
{
    public class TitlesController : Controller
    {
        private TitlesEntities db = new TitlesEntities();

        // GET: Titles
        public async Task<ActionResult> Index(string sortOrder,string currentFilter, string searchString, int? page)
        {
            try
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Year" ? "year_desc" : "Year";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                IEnumerable<Title> titles = await db.Titles.ToListAsync();

                if (!String.IsNullOrEmpty(searchString))
                {
                    titles = titles.Where(t => t.TitleName.ToUpper().Contains(searchString.ToUpper().Trim()));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        titles = titles.OrderByDescending(t => t.TitleName);
                        break;
                    case "Year":
                        titles = titles.OrderBy(t => t.ReleaseYear);
                        break;
                    case "year_desc":
                        titles = titles.OrderByDescending(t => t.ReleaseYear);
                        break;
                    default:
                        titles = titles.OrderBy(t => t.TitleName);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(titles.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                return View("_Error");
            }
        }

        // GET: Titles/Details/5
        [Route("Movie_Details-{id}")]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Title title = await db.Titles.FindAsync(id);
                if (title == null)
                {
                    return HttpNotFound();
                }
                return View(title);
            }
            catch (Exception ex)
            {
                return View("_Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
