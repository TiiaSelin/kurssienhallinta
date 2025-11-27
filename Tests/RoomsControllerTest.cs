namespace kurssienhallinta.tests;
using kurssienhallinta.Controllers;
using kurssienhallinta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class RoomsControllersTest
{
    [Fact]
    public void Test_List_rooms()
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
    [Fact]
    public void Test_Add_room_success()
    {
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<RoomsController>.Instance;
        var controller = new RoomsController(context, logger);

        var room = new Room
        {
            Name = "Sauna",
            Capacity = 30,
            Room_code = "sa_0"
        };

        var result = controller.Add_room(room);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List_rooms", redirect.ActionName);
        Assert.Equal(1, context.Rooms.Count());
    }
    [Fact]
    public void Test_Add_room_failure()
    {
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<RoomsController>.Instance;
        var controller = new RoomsController(context, logger);

        // Failed validation must be defined manually in unit test.
        controller.ModelState.AddModelError("Name", "Required");

        var room = new Room
        {   
            // No name, fails as a result.
            Capacity = -1,
            Room_code = "N/A"
        };

        var result = controller.Add_room(room);
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<Room>(viewResult.Model);
        Assert.Equal(room.Room_code, returnedModel.Room_code);
        Assert.Empty(context.Rooms);
    }
    [Fact]
    public void Test_Edit_room_get()
    {
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<RoomsController>.Instance;
        var controller = new RoomsController(context, logger);

        context.Rooms.Add(new Room{Id = 1, Name = "Tyhjiö", Capacity = 0, Room_code = "ty_0"});
        context.SaveChanges();

        var result = controller.Edit_room(1);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Room>(viewResult.Model);

        Assert.Equal("Tyhjiö", model.Name);
        Assert.Equal(0, model.Capacity);
        Assert.Equal("ty_0", model.Room_code);
    }
    [Fact]
    public void Test_Edit_room_post()
    {
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<RoomsController>.Instance;
        var controller = new RoomsController(context, logger);

        var room_to_edit = new Room
        {
            Id = 1,
            Name = "Tyhjiö",
            Room_code = "ty_0",
            Capacity = 0
        };

        context.Rooms.Add(room_to_edit);
        context.SaveChanges();

        room_to_edit.Name = "Ei enää tyhjiö.";
        room_to_edit.Capacity = 50;

        var result = controller.Edit_room(room_to_edit);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List_rooms", redirect.ActionName);

        var room_in_db = context.Rooms.First(room => room.Id == 1);
        Assert.Equal("Ei enää tyhjiö.", room_in_db.Name);
        Assert.Equal(50, room_in_db.Capacity);
        Assert.Equal("ty_0", room_in_db.Room_code);
    }
    [Fact]
    public void Test_Delete_room()
    {
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<RoomsController>.Instance;
        var controller = new RoomsController(context, logger);

        context.Rooms.Add(new Room{Id = 1, Name = "Poistettava tila.", Capacity = 0, Room_code = "pt_0"});
        context.Rooms.Add(new Room{Id = 2, Name = "Rusty Bucket Bay", Capacity = 40, Room_code = "rb_0"});
        context.Rooms.Add(new Room{Id = 3, Name = "Ananaskissa's medieval castle", Capacity = 300, Room_code = "am_0"});
        context.Rooms.Add(new Room{Id = 4, Name = "Velukotivirta's glade of harmony", Capacity = 20, Room_code = "vg_0"});
        context.Rooms.Add(new Room{Id = 5, Name = "Emetsuz's BMW garage", Capacity = 5, Room_code = "eb_0"});
        context.SaveChanges();

        var result = controller.Delete_room(1, confirm: true);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List_rooms", redirect.ActionName);

        Assert.Equal(4, context.Rooms.Count());
        var room_in_db = context.Rooms.First(room => room.Id == 2);
        Assert.Equal("Rusty Bucket Bay", room_in_db.Name);
    }
}
