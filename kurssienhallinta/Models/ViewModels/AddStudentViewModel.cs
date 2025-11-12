namespace kurssienhallinta.Models.ViewModels
{
    public class AddStudentViewModel
    {
        public Opiskelija Opiskelija { get; set; } = new();
        public List<Opiskelija> Opiskelijat { get; set; } = new();
    }

}

