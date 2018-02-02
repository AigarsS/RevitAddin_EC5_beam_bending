using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes;
using Autodesk.Revit.UI.ExtensibleStorage.Framework.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Documentation.Attributes;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes.Schema("CalculationParam", "791386dd-bba5-4b64-9ee6-c637490ec969")]
    public class CalculationParameter : Autodesk.Revit.DB.ExtensibleStorage.Framework.SchemaClass
    {
        [SchemaProperty(Unit = UnitType.UT_Number, DisplayUnit = DisplayUnitType.DUT_GENERAL)]
        [UnitTextBox(
            Description = "@g{M}",
            ValidateMinimumValue = true,
            MinimumValue = 0,
            AttributeUnit = DisplayUnitType.DUT_GENERAL,
            Category = "Data")]
        [ValueWithName(Name = "@g{M}")]
        public Double gammaM { get; set; }



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
