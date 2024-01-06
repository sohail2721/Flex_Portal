using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bismillah.Shared
{
    public class ButtonHandling
    {
        public bool ButtonDisabled { get; set; } = false;
        public string ButtonText { get; set; } = "";
        public ButtonHandling(string Text)
        {
            ButtonText = Text;
        }
        public void EnableButton(string Text)
        {
            ButtonDisabled = false;
            ButtonText = Text;
        }
        public void DisableButton(string Text)
        {
            ButtonDisabled = true;
            ButtonText = Text;
        }
    }
}