using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace RevitAddin_EC5_beam_bending
{
    /// <summary>
    /// Represents labels.
    /// </summary>
    [Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes.Schema("Label", "d35e83c7-10ea-46fb-81de-50e833218469")]
    public class Label : Autodesk.Revit.DB.ExtensibleStorage.Framework.SchemaClass
    {
        public Label()
        {
        }

        public Label(Document document)
        {
        }

        public Label(Entity entity, Document document)
            : base(entity, document)
        {
        }
    }
}
