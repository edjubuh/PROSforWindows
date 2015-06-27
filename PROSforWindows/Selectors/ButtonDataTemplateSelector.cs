using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PROSforWindows.Selectors
{
    public class ButtonDataTemplateSelector : DataTemplateSelector
    {
        // Based off of https://msdn.microsoft.com/en-us/library/system.windows.controls.itemscontrol.itemtemplateselector.aspx
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if(element != null && item != null && item is Models.Button)
            {
                if (((Models.Button)item).Buttons != null && ((Models.Button)item).Buttons.Count > 0)
                    return element.FindResource("Button_WithButtons") as DataTemplate;
                else
                    return element.FindResource("Button_WithoutButtons") as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
