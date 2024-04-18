using System.ComponentModel.DataAnnotations;

namespace Skelbimu_sistema.ViewModels
{
	public class ReportCreationRequest
	{
		public Product Product { get; set; }
		public Report Report { get; set; }
	}
}
