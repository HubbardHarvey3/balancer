namespace Balancer.Components.Services
{
    internal class DialogService
    {
        public Task<bool> ShowConfirmationDialog(
            string title,
            string message,
            string confirmButton = "Yes",
            string cancelButton = "No"
        )
        {
            return Application.Current.MainPage.DisplayAlert(title, message, confirmButton, cancelButton);
        }
    }
}
