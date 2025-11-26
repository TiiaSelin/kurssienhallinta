namespace kurssienhallinta.tests;
using kurssienhallinta.Controllers;
using kurssienhallinta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class RoomsControllersTest
{
    [Fact]
    public void ListRooms()
    {
        // Initialize in-memory database and define mock data.
        var context = DbContextFactory.CreateInMemoryDbContext();
        context.Rooms.Add(new Room{Name = "Mush room", Capacity = 50, Room_code = "mr_0"});
        context.Rooms.Add(new Room{Name = "Kapselihotelli", Capacity = 1, Room_code = "ch_0"});
        context.Rooms.Add(new Room{Name = "Discord kokoushuone", Capacity = 6, Room_code = "dk_0"});
        context.SaveChanges();

        // Create controller instance with context defined above and mock logger.
        var logger = NullLogger<RoomsController>.Instance;
        var controller = new RoomsController(context, logger);

        // Define and test that the view works.
        var result = controller.List_rooms() as ViewResult;
        Assert.IsType<ViewResult>(result);

        // Test model contains the correct information.
        var model = Assert.IsAssignableFrom<IEnumerable<Room>>(result.Model);
        Assert.Equal(3, model.Count());
        Assert.Contains(model, room => room.Name == "Mush room");
        Assert.Contains(model, room => room.Capacity == 1);
        Assert.Contains(model, room => room.Room_code == "dk_0");
    }
}
