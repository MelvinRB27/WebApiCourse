using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiCourse.Utils
{
    public class SwaggerGroupVersion: IControllerModelConvention
    {
        public void Apply(ControllerModel controllerModel)
        {
            var namespaceController = controllerModel.ControllerType.Namespace; //Controllers.v1
            var versionAPI = namespaceController.Split('.').Last().ToLower();//1
            controllerModel.ApiExplorer.GroupName = versionAPI;
        }
    }
}
