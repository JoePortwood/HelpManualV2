using HelpManual.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpManual.Data
{
    public class DbInitializer
    {
        public static void Initialize(HelpManualDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any objects.
            if (!context.ControlTypes.Any())
            {
                var controlTypes = new ControlType[]
                {
                    new ControlType{Control="p",Name="Paragraph"},
                    new ControlType{Control="img",Name="Image"},
                    new ControlType{Control="select",Name="Drop Down"},
                    new ControlType{Control="h1",Name="Title"},
                };
                foreach (ControlType ct in controlTypes)
                {
                    context.ControlTypes.Add(ct);
                }
                context.SaveChanges();
            }

            if (!context.ObjectTypes.Any())
            {
                var objectTypes = new ObjectType[]
                {
                    new ObjectType{Name="Entry",Data="Entry",ControlTypeId=1},
                    new ObjectType{Name="Drop",Data="Drop",Options="Yes,No",ControlTypeId=3},
                    new ObjectType{Name="Title",Data="Title",ControlTypeId=4},
                };
                foreach (ObjectType c in objectTypes)
                {
                    context.ObjectTypes.Add(c);
                }
                context.SaveChanges();
            }


            if (!context.FormObject.Any())
            {
                var formObjects = new FormObject[]
                {
                    new FormObject{ObjectTypeId=1,Order=1,PageNo=1},
                    new FormObject{ObjectTypeId=2,Order=2,PageNo=1},
                    new FormObject{ObjectTypeId=3,Order=3,PageNo=1},
                };
                foreach (FormObject e in formObjects)
                {
                    context.FormObject.Add(e);
                }
                context.SaveChanges();
            }
        }
    }
}
