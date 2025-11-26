namespace kurssienhallinta.tests;
using kurssienhallinta.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

public class HomeControllersTest
{
    [Fact]
    public void HomeControllerTest()
    {
        var logger = NullLogger<HomeController>.Instance;
        var controller = new HomeController(logger);
        var result = controller.Index();
        Assert.IsType<ViewResult>(result);
    }
}
