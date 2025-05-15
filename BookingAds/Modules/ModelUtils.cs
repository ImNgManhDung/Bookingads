using System;
using System.Linq;
using System.Web.Mvc;

namespace BookingAds.Modules
{
    public static class ModelUtils
    {
        public static void RemoveError(this ModelStateDictionary modelState, params string[] excludeProperty)
        {
            if (modelState == null)
            {
                return;
            }

            if (excludeProperty == null)
            {
                excludeProperty = Array.Empty<string>();
            }

            foreach (var key in modelState.Keys)
            {
                if (!excludeProperty.Any(o => o == key))
                {
                    modelState[key].Errors.Clear();
                }
            }
        }

    }
}