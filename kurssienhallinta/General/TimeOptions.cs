using Microsoft.AspNetCore.Mvc.Rendering;

namespace kurssienhallinta.General
{
    public static class TimeOptions
    {
        public static IEnumerable<SelectListItem> FullHourSlots()
        {
            return Enumerable.Range(8, 9)
                .Select(h => new SelectListItem
                {
                    Value = $"{h}:00:00",
                    Text = $"{h}:00"
                });
        }
    }
}