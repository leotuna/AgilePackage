using Microsoft.AspNetCore.Mvc;

namespace AgilePackage.Web.App.Extensions
{
    public static class ToastExtensions
    {
        public static void ToastSuccess(this Controller controller, string message)
        {
            controller.TempData["ToastSuccess"] = message;
        }

        public static void ToastError(this Controller controller, string message)
        {
            controller.TempData["ToastError"] = message;
        }
    }
}
