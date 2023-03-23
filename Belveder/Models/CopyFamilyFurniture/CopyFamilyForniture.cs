using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using static UIFramework.Widget.CustomControls.NativeMethods;

namespace Belveder
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CopyFamilyForniture : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            //Define a reference Object to accept the pick result

            try
            {
                Reference pickedref = null;

                //Pick a group
                Selection sel = uiapp.ActiveUIDocument.Selection;
                GroupPickFilter selFilter = new GroupPickFilter();
                pickedref = sel.PickObject(ObjectType.Element, selFilter, "Please select a group");
                Element elem = doc.GetElement(pickedref);
                Group group = elem as Group;

                XYZ origin = GetElementCenter(group);
                Room room = GetRoomOfGroup(doc, origin);
                XYZ sourceCenter = GetRoomCenter(room);

                RoomPickFilter roomPickFilter = new RoomPickFilter();
                IList<Reference> rooms = sel.PickObjects(ObjectType.Element, roomPickFilter, "Select target rooms for duplicate furniture group");

                //Place the group
                Transaction trans = new Transaction(doc);
                trans.Start("Lab");
                PlaceFurnitureInRooms(doc, rooms, sourceCenter, group.GroupType, origin);
                trans.Commit();


            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public XYZ GetElementCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            return (bounding.Max + bounding.Min) * 0.5;
        }

        public XYZ GetRoomCenter(Room room)
        {
            XYZ boundCenter = GetElementCenter(room);
            LocationPoint locationPoint = (LocationPoint)room.Location;
            return new XYZ(boundCenter.X, boundCenter.Y, locationPoint.Point.Z);
        }

        Room GetRoomOfGroup(Document document, XYZ point)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfCategory(BuiltInCategory.OST_Rooms);

            Room room = null;
            foreach (Element element in collector)
            {
                room = element as Room;
                if (room != null)
                {
                    if (room.IsPointInRoom(point))
                    {
                        break;
                    }
                }
            }
            return room;
        }

        public void PlaceFurnitureInRooms(Document document, IList<Reference> rooms, XYZ sourceCenter, GroupType groupType, XYZ groupOrigin)
        {
            XYZ offset = groupOrigin - sourceCenter;
            XYZ offsetXY = new XYZ(offset.X, offset.Y, 0);

            foreach (Reference room in rooms)
            {
                Room roomTarget = document.GetElement(room) as Room;
                if (roomTarget != null)
                {
                    XYZ roomCenter = GetRoomCenter(roomTarget);
                    Group group = document.Create.PlaceGroup(roomCenter + offsetXY, groupType);
                }
            }
        }
    }
}
