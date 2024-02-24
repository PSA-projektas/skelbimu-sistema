namespace Skelbimu_sistema.Models
{
	public class Advert
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public bool Suspended { get; set; }
		public int NumberOfReports { get; set; }

		public int User { get; set; }
	}

	public class MockAdvertRepo
	{
		public static List<Advert> GetAdverts()
		{
			// Generate mock user data
			var adverts = new List<Advert>
				{
					new Advert { Id = 1, Title = "Kompiuteris", Suspended = false, NumberOfReports = 0, User = 2 },
					new Advert { Id = 2, Title = "Planšetė", Suspended = false, NumberOfReports = 2, User = 2 },
					new Advert { Id = 3, Title = "Blogas skelbimas", Suspended = true, NumberOfReports = 7, User = 3 },
					new Advert { Id = 4, Title = "Blogas skelbimas 2", Suspended = true, NumberOfReports = 6, User = 4 },
                    // Add more mock users as needed
                };

			return adverts;
		}
	}
}
