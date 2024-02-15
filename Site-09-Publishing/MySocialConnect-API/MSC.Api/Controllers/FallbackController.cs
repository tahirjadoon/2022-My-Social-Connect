using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace MSC.Api.Controllers;
public class FallbackController : Controller
{
    public ActionResult Index()
    {
        //get the index.html from wwwwroot folder
        var indexHtml = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

        return PhysicalFile(indexHtml, "text/HTML");
    }
}
