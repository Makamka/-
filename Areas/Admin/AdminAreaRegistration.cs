using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ECommerceSolution.Areas.Admin
{
    public class AdminAreaRegistration : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            // Логіка застосування конфігурації для контролера
        }
    }
}
