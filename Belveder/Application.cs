using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Belveder
{
    internal class Application : IExternalApplication
    {

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = AddRibbonPanel(application);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;


            if (panel.AddItem(new PushButtonData("ScheduleFilling", "Заполнение параметров", thisAssemblyPath, "Belveder.ScheduleFilling"))
                is PushButton buttonScheduleFilling)
            {
                buttonScheduleFilling.ToolTip = "Заполняет у элемента параметры: \"Уровень_текст\", \"Уровень_число\" ";

                Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "icons", "green.png"));
                BitmapImage bitmap = new BitmapImage(uri);
                buttonScheduleFilling.LargeImage = bitmap;
            }

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public RibbonPanel AddRibbonPanel(UIControlledApplication application)
        {
            string tabName = "BelvederDev";
            RibbonPanel ribbonPanel = null;

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                application.CreateRibbonPanel(tabName, "Belveder");
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<RibbonPanel> panels = application.GetRibbonPanels(tabName);
            foreach(var panel in panels.Where(p => p.Name == "Belveder"))
            {
                ribbonPanel = panel;
            }

            return ribbonPanel;
        }
    }
}
