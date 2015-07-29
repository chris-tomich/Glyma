using System;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace SevenSigma.Logging.ApplicationPages
{
	public partial class LoggingServiceTestPage : LayoutsPageBase
	{
		protected void LogButton_Click(object sender, EventArgs e)
		{
			if(!string.IsNullOrEmpty(MessageTextBox.Text))
			{
				LoggingService.WriteTrace(LoggingService.Categories.ApplicationPages, TraceSeverity.Medium, MessageTextBox.Text);
				//LoggingService.WriteEvent(LoggingService.Categories.ApplicationPages, EventSeverity.Information, MessageTextBox.Text);
			}
		}
	}
}
