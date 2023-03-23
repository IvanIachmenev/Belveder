using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belveder
{
    internal class FindFloorLevel
    {
        public static Dictionary<double, double> GetAllFloor(Document document)
        {
            FilteredElementCollector newFloorFilter = new FilteredElementCollector(document);
            ICollection<Element> allFloors = newFloorFilter.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType().ToElements();
            SortedSet<double> valueFloor = new SortedSet<double>();
            string str = "";
            foreach (var floor in allFloors)
            {
                foreach (Parameter parameter in floor.Parameters)
                {
                    if (parameter.Definition.Name == "Отметка верха")
                    {
                        var temp = Convert.ToDouble(parameter.AsValueString());
                        valueFloor.Add(temp);
                    }
                }
            }

            Dictionary<double, double> dictValueFloor = new Dictionary<double, double>();
            int cnt = 0;
            foreach (var value in valueFloor)
            {
                dictValueFloor.Add(value, value);
                //  str += Convert.ToString(value) + " ";
            }

            //TaskDialog.Show("ASD", str);

            return dictValueFloor;
        }
    }
}
