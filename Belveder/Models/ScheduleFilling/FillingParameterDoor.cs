using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belveder
{
    internal class FillingParameterDoor
    {
        public static void GetAllDoors(Document document, Dictionary<double, double> valueFloors)
        {
            FilteredElementCollector newDoorFilter = new FilteredElementCollector(document);
            ICollection<Element> allDoors = newDoorFilter.OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().ToElements();

            foreach (var door in allDoors)
            {
                string str = "";
                foreach (Parameter parameter in door.Parameters)
                {
                    if (parameter.Definition.Name == "Уровень")
                    {
                        string tmp = "";
                        str = parameter.AsValueString();
                        for (var i = 0; i < str.Length; i++)
                        {
                            if (str[i] != '(')
                            {
                                if (Char.IsDigit(str[i]))
                                {
                                    tmp += str[i];
                                }
                                if (str[i] == '-')
                                {
                                    tmp += str[i];
                                }
                            }
                            else
                            {

                                str = tmp;
                                break;
                            }
                        }
                        str = tmp;
                    }
                }
                foreach (Parameter parameter in door.Parameters)
                {
                    if (parameter.Definition.Name == "Уровень_текст")
                    {
                        using (Transaction transaction = new Transaction(document, "Set value parm"))
                        {
                            transaction.Start("set");

                            parameter.Set(str);

                            transaction.Commit();
                        }
                        break;
                    }
                }
                foreach (Parameter parameter in door.Parameters)
                {
                    if (parameter.Definition.Name == "Уровень_число")
                    {
                        using (Transaction transaction = new Transaction(document, "Set value parm"))
                        {
                            try
                            {
                                transaction.Start("set");
                                parameter.Set(valueFloors[Convert.ToDouble(str)]);
                                str = "";
                                transaction.Commit();
                            }
                            catch
                            {
                                str = "";
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
