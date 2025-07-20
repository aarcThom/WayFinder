using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.Settings;

namespace WayFinder.RevitObjects
{
    public class WallSign
    {

        private bool _active; // is bound model currently active

        public bool Active { get => _active; set => _active = value; }

        public WallSign(bool active)
        {
            _active = active;
        }


        public void SetActiveState (bool state)
        {
            _active = state;
            TaskDialog.Show("Test", $"This sign's active state is {_active}!");
        }



    }
}
