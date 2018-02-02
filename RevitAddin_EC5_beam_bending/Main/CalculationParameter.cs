using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes.Schema("CalculationParam", "791386dd-bba5-4b64-9ee6-c637490ec969")]
    public class CalculationParameter : Autodesk.Revit.DB.ExtensibleStorage.Framework.SchemaClass
    {
        public CalculationParameter()
        {
        }

        public CalculationParameter(Document document)
        {
        }

        public CalculationParameter(Entity entity, Document document)
            : base(entity, document)
        {
        }
    }
}
