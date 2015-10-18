using CMClient.Settings;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;

namespace CMClient.ViewComponents
{
    public class CustomerMatchesViewComponent : ViewComponent
    {
        private readonly ServiceSetting _serviceSetting;

        public CustomerMatchesViewComponent(IOptions<ServiceSetting> serviceSetting)
        {
            _serviceSetting = serviceSetting.Options;
        }

        public IViewComponentResult Invoke(int id)
        {
            return View<ServiceSetting>(_serviceSetting);
        }
    }
}
