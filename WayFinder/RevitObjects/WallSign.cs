using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayFinder.RevitObjects
{
    public class WallSign : IDisposable
    {

        private readonly IDisposable _subscription;

        public WallSign(AppSettings.ModelSettings settings)
        {
            _subscription = settings.FocusedDebugState.Subscribe(isActive =>
            {
                TaskDialog.Show("cool", "The active state changed!");
            });
        }


        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
