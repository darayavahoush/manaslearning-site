using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace manaslearning.UI.Attributes
{
    /// <summary>
    /// Disables the default Model State Validation provided by the .NET
    /// </summary>
    public class SuppressModelStateInvalidFilterAttribute : Attribute, IActionModelConvention
    {
        private const string FilterTypeName = "ModelStateInvalidFilterFactory";

        public void Apply(ActionModel action)
        {
            for (var i = 0; i < action.Filters.Count; i++)
            {
                if (action.Filters[i].GetType().Name == FilterTypeName)
                {
                    action.Filters.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
